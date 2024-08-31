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

        private int _currentAttackIdx = 0;
        
        private bool _canAttack;
        private bool _canBounce;
        private bool _canDash;
        private bool _stateValid = true;
        //private int _currentAttackIdx;
        private Label _hpLabel;
        
        private AnimatedSprite2D _animation; 
        [Export] public int dashSpeed = 100;
       

        [Signal] public delegate void PlayerMovedEventHandler(Vector2 position);
        [Signal] public delegate void PlayerDamagedEventHandler();
        [Signal] public delegate void PlayerDiedEventHandler();
        [Signal] public delegate void DashTimerStartedEventHandler();

        [Signal] public delegate void AttackCooldownTimerStartedEventHandler();

        [Signal]
        public delegate void AttackSwitchedEventHandler();

        public override void _Ready()
        {
            _canAttack = true;
            _health = _maxHealth;
            _hpBarSize = _maxHealth / 30; 
            _healthBar = GetNode<TextureProgressBar>("HealthBar");
            _hpLabel = GetNode<Label>("HealthBar/Label");
            _hpLabel.Text = $"{_maxHealth} / {_maxHealth}";
            
            _currentAttackIdx = 0;
            _currentAttack = _currentAttack = GD.Load<PackedScene>(ProjectPath.ScenesPath + BaseAttack.AttackScenes[_currentAttackIdx] + ".tscn").Instantiate<Attack1>();
            
            _animation = GetNode<AnimatedSprite2D>("PlayerImage/PlayerSprite");

            GetNode<Timer>("AttackSpeed").WaitTime = _currentAttack.GetAttackSpeed();
            
            Timer bounceTimer = new Timer();
            
            bounceTimer.Autostart = true;
            bounceTimer.WaitTime = 2f;
            bounceTimer.Timeout += () => { _canBounce = true; };
            AddChild(bounceTimer);

            _canDash = true;

        }
        
        public override void _PhysicsProcess(double delta)
        {
            if(!_stateValid) return;
            _animation.Visible = true;
           
			
            Vector2 direction = Input.GetVector(
                "FINKISURVIVE_player_left", 
                "FINKISURVIVE_player_right", 
                "FINKISURVIVE_player_up", 
                "FINKISURVIVE_player_down").Normalized();

            var walkAudio = GetNode<AudioStreamPlayer2D>("WalkSoundEffect");
            if (direction != Vector2.Zero)
            {
               
                if(!walkAudio.IsPlaying())
                    walkAudio.Play();
            }
            else
            {
                walkAudio.Stop();
            }
            
            if (Input.IsActionPressed("FINKISURVIVE_player_attack"))
            {
                State.Attack(_animation);
                if (_canAttack) Attack();
            }
            
            if (Input.IsActionJustPressed("FINKISURVIVE_dash") )
            {
                if (_canDash)
                {
                    var tween = GetTree().CreateTween();
                    Color modulated = Color.FromHtml("#451c26");
                    modulated.A = 0.3f;
                    tween.TweenProperty(this, "modulate", modulated, 0.1f);
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
                    _canDash = false;
                    GetNode<Timer>("DashCooldown").Start();
                    EmitSignal(nameof(DashTimerStarted));
                    map.EnablePlayerCollisions();
                    
                    tween.TweenProperty(this, "modulate", Modulate, 0.2f);
                }
            }
            
            
            State = MovementState.GetState((GetGlobalMousePosition() - GetGlobalPosition()).Angle(),direction);
            if (!_animation.Animation.ToString().Contains("attack") || !_animation.IsPlaying())
            {
                State.Move(_animation);
            }
              
            
            Velocity = direction * (Speed * (float)delta);
			
            var collision = MoveAndCollide(Velocity);
            EmitSignal(nameof(PlayerMoved),Position);

            if (collision == null) return;
            if (!_canBounce) return;
                
            Velocity = Velocity.Bounce((collision.GetNormal()).Normalized());
            MoveAndCollide(Velocity * 2);
            
        }
        


        public void OnDashCooldownEnd()
        {
            _canDash = true;
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
            GetNextAttack();
			     
            var attackScene = GD.Load<PackedScene>(ProjectPath.ScenesPath + BaseAttack.AttackScenes[_currentAttackIdx] + ".tscn");
            BaseAttack atk = attackScene.Instantiate() as BaseAttack;
            while (atk!.GetAvailableAtWave() > Map.WaveCount)
            {
                GetNextAttack();
                atk = GD.Load<PackedScene>(ProjectPath.ScenesPath + BaseAttack.AttackScenes[_currentAttackIdx] + ".tscn").Instantiate() as BaseAttack;
            }
            
        
            GetNode<TextureProgressBar>("/root/Level/UI/CurrentWeaponCont/Panel/MarginContainer/TextureProgressBar")
                    .TextureProgress = ResourceLoader.Load<Texture2D>(atk!.GetIconPath());
			     
            var timer = GetNode<Timer>("AttackSpeed");
            timer.Stop();
            timer.WaitTime = atk.GetAttackSpeed();
            timer.Start();
            _canAttack = false;
        }
        
        private void GetNextAttack()
        {
            _currentAttackIdx = ++_currentAttackIdx % BaseAttack.AttackScenes.Count;
            
        }

        public override void _Input(InputEvent @event)
        {
            if (!@event.IsActionPressed("FINKISURVIVE_switch_attack")) return;
            
            SwitchAttack();
            EmitSignal(nameof(AttackSwitched));

        }

       

        private void Attack()
        {
            if(!_canAttack) return;

            _canAttack = false;
            
            PackedScene attackScene = GD.Load<PackedScene>(ProjectPath.ScenesPath + BaseAttack.AttackScenes[_currentAttackIdx] + ".tscn");
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
            
            EmitSignal(nameof(AttackCooldownTimerStarted));
           
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