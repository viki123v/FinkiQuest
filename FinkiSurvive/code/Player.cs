using Godot;
using System;
using System.Collections.Generic;
using FinkiAdventureQuest.FinkiSurvive.code.AnimationHandler;
using FinkiAdventureQuest.FinkiSurvive.code.Util;

namespace FinkiAdventureQuest.FinkiSurvive.code
{
    public partial class Player : CharacterBody2D
    {
        [Export]
        private float Speed = 170f;
        [Export]
        private int _maxHealth = 300;
        private float _health;
        private int _hpBarSize; // golemina na edno hp bar delce
        private TextureProgressBar _healthBar;
        private MovementState State {get; set;}
		
        private BaseAttack _currentAttack;
        private bool _canAttack;

        private readonly List<string> _attackScenes = new();
        private int _currentAttackIdx;
        private Label _hpLabel;
        private bool _stateValid = true;

        private AnimatedSprite2D _animation;
        [Export]
        public int dashSpeed = 100;
        private bool canDash;

        [Signal]
        public delegate void PlayerMovedEventHandler(Vector2 position);
        [Signal]
        public delegate void PlayerDamagedEventHandler();
        [Signal]
        public delegate void PlayerDiedEventHandler();

        [Signal]
        public delegate void DashTimerStartedEventHandler();

        public override void _Ready()
        {
            _canAttack = true;
            _health = _maxHealth;
            _hpBarSize = _maxHealth / 30; // 15 - br pati so trebit da ta udrat za da umris
            _healthBar = GetNode<TextureProgressBar>("HealthBar");
            _attackScenes.Add("attack1");
            _attackScenes.Add("attack2");
            _attackScenes.Add("attack3");
            _currentAttackIdx = 0;
            _currentAttack = _currentAttack = GD.Load<PackedScene>(ProjectPath.ScenesPath + _attackScenes[_currentAttackIdx] + ".tscn").Instantiate<Attack1>();
            _hpLabel = GetNode<Label>("HealthBar/Label");
            
            _hpLabel.Text = $"{_maxHealth} / {_maxHealth}";
            GetNode<Timer>("AttackSpeed").WaitTime = _currentAttack.GetAttackSpeed();
			
            _animation = GetNode<AnimatedSprite2D>("PlayerImage/PlayerSprite");

            canDash = true;

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
            
            if (Input.IsActionJustPressed("FINKISURVIVE_dash") )
            {
                if (canDash)
                {
                    var map = GetParent<Map>();
                    map.DisablePlayerCollisions();
                    if (direction != Vector2.Zero)
                    {
                        Velocity = direction * dashSpeed;
                    }
                    else
                    {
                        Velocity = Velocity.Normalized();
                    }

                    MoveAndCollide(Velocity);
                    canDash = false;
                    GetNode<Timer>("DashCooldown").Start();
                    EmitSignal(nameof(DashTimerStarted));
                    map.EnablePlayerCollisions();
                }
            }
            
            
            State = MovementState.GetState((GetGlobalMousePosition() - GetGlobalPosition()).Angle(),direction);
            if(!_animation.Animation.ToString().Contains("attack") || !_animation.IsPlaying())
                State.Move(_animation);
            
            
            Velocity = direction * (Speed * (float)delta);
			
            var collision = MoveAndCollide(Velocity);

            if (collision != null)
            {
                if (Map.FrameCount % 10 == 0)
                {
                    Velocity = Velocity.Bounce((collision.GetNormal()).Normalized());
                    MoveAndCollide(Velocity * 2);
                }
                
            }
			
            EmitSignal(nameof(PlayerMoved),Position);
			
            _canAttack = false;
        }
        


        public void OnDashCooldownEnd()
        {
            canDash = true;
        }

        public void HealPlayer(float amount)
        {
            if (_health + amount >= _maxHealth)
            {
                _health = _maxHealth;
            }
            else
            {
                _health += amount;
            }
           
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
            LabelFactory.DisplayDamageLabel(this,damage);
            _health -= damage;
            UpdateHealthBar();

            if (_health <= 0)
            {
                Death();
                EmitSignal(nameof(PlayerDied));
            }
        }

        private void UpdateHealthBar()
        {
            _healthBar.Value = (int)Math.Ceiling(_health / _hpBarSize);
            _hpLabel.Text = $"{_health} / {_maxHealth}";
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