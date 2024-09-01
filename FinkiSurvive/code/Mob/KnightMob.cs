using Godot;
using System;
namespace FinkiAdventureQuest.FinkiSurvive.code
{
	public partial class KnightMob : Mob
	{ 
		public static int BaseHp = 600;
		[Export] private int _appearsAtLevel = 5;
		[Export] private float _hpScaleFactor = 0.1f;
		[Export] private int _minDamage = 50;
		[Export] private float _attackSpeed = 1.0f;
		[Export] private int _minSpeed = 60;
		public override void _Ready()
		{
			MaxHealth = BaseHp;
			if(Main.Game.WaveCount > _appearsAtLevel)
				ScaleHp();
			Speed = rng.Next(_minSpeed,_minSpeed + 10);
			base._Ready();
			AttackSpeedTimer.WaitTime = _attackSpeed;
			AddChild(AttackSpeedTimer);
		}

		public override PackedScene DropCoin()
		{
			return GD.Load<PackedScene>(ProjectPaths.CoinScenesPath + "gold_coin.tscn");
		}

		public override void Attack()
		{
			if (!CanAttack) return;
			
			GetNode<Player>("/root/Level/Player").TakeDamage(GetDamage());
			_animSprite.Play("attack");
			CanAttack = false;
		}

		public override int GetDamage()
		{
			return rng.Next(_minDamage,_minDamage + 10);
		}

		public override void ScaleHp()
		{
			MaxHealth = GetScaledHp(_hpScaleFactor,MobType.Knight);
		}
	}
}

