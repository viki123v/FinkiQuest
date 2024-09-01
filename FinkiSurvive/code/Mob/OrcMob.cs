using Godot;
using System;
using System.Collections.Generic;

namespace FinkiAdventureQuest.FinkiSurvive.code {
	
public partial class OrcMob : Mob
	{
		public static int BaseHp = 60;
		[Export]
		public int AppearsAtLevel = 1;
		[Export]
		private float _hpScaleFactor = 0.3f;

		
		public override void _Ready()
		{
			
			MaxHealth = BaseHp;
			if(Main.Game.WaveCount > AppearsAtLevel)
				ScaleHp();
			Speed = rng.Next(120, 130);
			base._Ready();
			AttackSpeedTimer.WaitTime = 0.1f;
			AddChild(AttackSpeedTimer);
			
		}

		public override PackedScene DropCoin()
		{
			return GD.Load<PackedScene>(ProjectPaths.CoinScenesPath + "bronze_coin.tscn");
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
			return new Random().Next(4,6);
		}
		

		public override void ScaleHp()
		{
			MaxHealth = GetScaledHp(_hpScaleFactor,MobType.Orc);
		}
	}
}

