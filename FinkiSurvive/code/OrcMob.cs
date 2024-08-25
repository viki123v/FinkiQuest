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
			
		}

		public override void Attack()
		{
			
			_animSprite.Play("attack");
			
			if (Map.FrameCount % 5 != 0) return;
			GetNode<Player>("/root/Level/Player").TakeDamage(GetDamage());
			
			
			
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

