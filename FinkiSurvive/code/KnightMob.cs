using Godot;
using System;
namespace FinkiAdventureQuest.FinkiSurvive.code
{
	public partial class KnightMob : Mob
	{
		public int BaseHp = 600;
		public int AppearsAtLevel = 4;
		private float _hpScaleFactor = 0.1f;
		public override void _Ready()
		{
			HpScaling.Add(BaseHp);
			MaxHealth = BaseHp;
			if(Map.WaveCount > AppearsAtLevel)
				ScaleHp();
			Speed = rng.Next(60,70);
			base._Ready();
			AttackSpeedTimer.WaitTime = 0.5f;
			AddChild(AttackSpeedTimer);
		}

		public override PackedScene DropCoin()
		{
			return GD.Load<PackedScene>("res://FinkiSurvive/scenes/gold_coin.tscn");
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
			return rng.Next(70,85);
		}

		public override void ScaleHp()
		{
			MaxHealth = GetScaledHp(_hpScaleFactor);
		}
	}
}

