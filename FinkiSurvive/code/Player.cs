using Godot;
using System;
using System.Collections.Generic;
using FinkiAdventureQuest.FinkiSurvive.code.AnimationHandler;

// TODO: HITBOX I HURTBOX IMPL
namespace FinkiAdventureQuest.FinkiSurvive.code
{
    public partial class Player : CharacterBody2D
    {
        private const float Speed = 170f;
        private static int _maxHealth = 300;
        private float _health;
        private int _hpBarSize; // golemina na edno hp bar delce
        private TextureProgressBar _healthBar;
		
        private MovementState State {get; set;}
		
        private BaseAttack _currentAttack;
        private bool _canAttack;

        private readonly List<string> _attackScenes = new();
        private int _currentAttackIdx;
		
        private bool _stateValid = true;

        private AnimatedSprite2D _animation;
		
        [Signal]
        public delegate void PlayerMovedEventHandler(Vector2 position);
        [Signal]
        public delegate void PlayerDamagedEventHandler();
        [Signal]
        public delegate void PlayerDiedEventHandler();

        public override void _Ready()
        {
            _canAttack = true;
            _health = _maxHealth;
            _hpBarSize = _maxHealth / 15; // 15 - br pati so trebit da ta udrat za da umris
            _healthBar = GetNode<TextureProgressBar>("HealthBar");
            _attackScenes.Add("attack1");
            _attackScenes.Add("attack2");
            _attackScenes.Add("attack3");
            _currentAttackIdx = 0;
            _currentAttack = _currentAttack = GD.Load<PackedScene>(ProjectPath.ScenesPath + _attackScenes[_currentAttackIdx] + ".tscn").Instantiate<Attack1>();
			
            GetNode<Timer>("AttackSpeed").WaitTime = _currentAttack.GetAttackSpeed();
			
            _animation = GetNode<AnimatedSprite2D>("PlayerImage/PlayerSprite");
			
        }
        
        public override void _PhysicsProcess(double delta)
        {
            if(!_stateValid) return;
			
			
            _animation.Visible = true;
            var walkAudio = GetNode<AudioStreamPlayer2D>("WalkSoundEffect");
			
            Vector2 direction = Input.GetVector(
                "FINKISURVIVE_player_left", 
                "FINKISURVIVE_player_right", 
                "FINKISURVIVE_player_up", 
                "FINKISURVIVE_player_down").Normalized();
            
            if (Input.IsActionPressed("FINKISURVIVE_player_attack"))
            {
                State.Attack(_animation);
                if (_canAttack) Attack();
            }
            
            
            State = MovementState.GetState((GetGlobalMousePosition() - GetGlobalPosition()).Angle(),direction);
            if(!_animation.Animation.ToString().Contains("attack") || !_animation.IsPlaying())
                State.Move(_animation);
            
            
            Velocity = direction * (Speed * (float)delta);
			
            var collision = MoveAndCollide(Velocity);

            if (collision != null)
            {
                Velocity = Velocity.Bounce((collision.GetNormal()).Normalized());
                MoveAndCollide(Velocity);
            }
			
            EmitSignal(nameof(PlayerMoved),Position);
			
            _canAttack = false;
        }

        public void HealPlayer(float amount)
        {
            _health += amount;
            UpdateHealthBar();
        }
        
        public void SwitchAttack()
        {
            ShiftAttackIdx();
			
            var scene = GD.Load<PackedScene>(ProjectPath.ScenesPath + _attackScenes[_currentAttackIdx] + ".tscn");
            BaseAttack atk = scene.Instantiate() as BaseAttack;
            while (atk!.GetAvailableAtWave() > Map.WaveCount)
            {
                ShiftAttackIdx();
                atk = GD.Load<PackedScene>(ProjectPath.ScenesPath + _attackScenes[_currentAttackIdx] + ".tscn").Instantiate() as BaseAttack;
            }

            GetNode<TextureRect>("/root/Level/UI/CurrentWeaponCont/Panel/TextureRect").Texture =
                ResourceLoader.Load<Texture2D>(atk!.GetIconPath());
			
            var timer = GetNode<Timer>("AttackSpeed");
            timer.Stop();
            timer.WaitTime = atk.GetAttackSpeed();
            timer.Start();
            _canAttack = false;
        }

        private void ShiftAttackIdx()
        {
            if (++_currentAttackIdx > _attackScenes.Count - 1)
            {
                _currentAttackIdx = 0;
            }
			
        }

        public override void _Input(InputEvent @event)
        {
            if (@event.IsActionPressed("FINKISURVIVE_switch_attack"))
            {
                SwitchAttack();
            }
        }

        private void Attack()
        {
            if(!_canAttack) return;

            _canAttack = true;
            PackedScene attackScene = GD.Load<PackedScene>(ProjectPath.ScenesPath + _attackScenes[_currentAttackIdx] + ".tscn");
            _currentAttack = attackScene.Instantiate() as BaseAttack;
            var cont = GetNode<Node2D>("Attacks/" + _currentAttack!.GetContainerName());
            Vector2 mousePosition = GetGlobalMousePosition();
            var point = GetNode<Node2D>("Marker2D");
            Vector2 playerPosition = point.GlobalPosition;

            Vector2 attackPos = point.Position;
            Vector2 direction = (mousePosition - playerPosition).Normalized();
			
            GD.Print(direction);
			

            var attackDistance = _currentAttack!.GetAttackRange();

            _currentAttack.Position = attackPos + direction * attackDistance;
            _currentAttack.Rotation = direction.Angle();
			
			
            var anim = _currentAttack.GetNode<AnimatedSprite2D>("Slash");
            anim.AnimationFinished += () =>
            {
                foreach (var child in cont.GetChildren())
                    child.QueueFree();
            };
            cont.AddChild(_currentAttack);
            anim.Play();
        }

        public void TakeDamage(int damage)
        {
            UpdateHealthBar();
            _health -= damage;

            if (_health <= 0)
            {
                Death();
                EmitSignal(nameof(PlayerDied));
            }
        }

        private void UpdateHealthBar()
        {
            _healthBar.Value = (int) Math.Ceiling((float) _health / _hpBarSize);
        }

        public void Death()
        {
            _canAttack = false;
            _stateValid = false;
            AnimatedSprite2D playerSprite = GetNode<AnimatedSprite2D>("PlayerImage/PlayerSprite");
            playerSprite.Play("death");
        }
        
        private void _on_timer_timeout()
        {
            _canAttack = true;
        }
		
    }

}