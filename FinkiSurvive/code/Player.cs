using Godot;
using System;
using System.Collections.Generic;

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
		
		
		private BaseAttack _currentAttack;
		private bool _canAttack;

		private readonly List<string> _attackScenes = new();
		private int _currentAttackIdx;
		
		private bool _stateValid = true;
		
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
			
		}
		
		//TODO MOB SCALE HEALHT SO WAVE NUM
		

		public override void _PhysicsProcess(double delta)
		{
			if(!_stateValid) return;
			
			AnimatedSprite2D playerSprite = GetNode<AnimatedSprite2D>("PlayerImage/PlayerSprite");
			playerSprite.Visible = true;
			var walkAudio = GetNode<AudioStreamPlayer2D>("WalkSoundEffect");
			
			Vector2 direction = Input.GetVector(
				"FINKISURVIVE_player_left", 
				"FINKISURVIVE_player_right", 
				"FINKISURVIVE_player_up", 
				"FINKISURVIVE_player_down");
			if (direction != Vector2.Zero)
			{
				direction.Normalized();
				
				playerSprite.FlipH = direction.X < 0; 
				
				if(playerSprite.Animation != "attack" || !playerSprite.IsPlaying())
					playerSprite.Play("walk");
				
				if (!walkAudio.Playing)
					walkAudio.Play();
			}
			else
			{
				if(playerSprite.Animation != "attack")
					playerSprite.Play("idle");
				
				walkAudio.Stop();
			}
			
			if (Input.IsActionPressed("FINKISURVIVE_player_attack"))
			{
				playerSprite.Play("attack");
				var mousePos = GetGlobalMousePosition().Normalized();
				var dot = mousePos.Dot(direction);
				var tolerance = 0.1f;
				if(Math.Abs(dot) > (1 - tolerance))
				{
					playerSprite.FlipH = !playerSprite.FlipH;
				}
				
				
				if (_canAttack) Attack();
				
			}
			
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

		public void ShiftAttackIdx()
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
			
			CanAttack();
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
			// playerSprite.AnimationFinished += () =>
			//  {
			//  	playerSprite.Play("idle");
			//  };

		}

		private void CanAttack()
		{
			_canAttack = true;
		}

		private void _on_timer_timeout()
		{
			_canAttack = true;
		}
		
	}

}
