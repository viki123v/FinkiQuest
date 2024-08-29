using Godot;
using System;
using System.Collections.Generic;

namespace FinkiAdventureQuest.FinkiSurvive.code {
	
public partial class OrcMob : Mob
	{
		public int BaseHp = 60;
		public int AppearsAtLevel = 1;
		private float _hpScaleFactor = 0.3f;

		
		public override void _Ready()
		{
			
			HpScaling.Add(BaseHp);
			MaxHealth = BaseHp;
			if(Map.WaveCount > AppearsAtLevel)
				ScaleHp();
			Speed = rng.Next(120, 130);
			base._Ready();
			AttackSpeedTimer.WaitTime = 0.1f;
			AddChild(AttackSpeedTimer);
			
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
			MaxHealth = GetScaledHp(_hpScaleFactor);
		}
	}
}

