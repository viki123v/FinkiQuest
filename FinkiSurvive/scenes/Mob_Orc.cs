using Godot;
using System;
using System.Collections.Generic;

namespace FinkiAdventureQuest.FinkiSurvive.FinkiQuest.scenes
{
	public partial class Mob_Orc : Mob
	{
		public int BaseHp = 60;
		public int AppearsAtLevel = 1;
		private float _hpScaleFactor = 0.3f;

		
		public override void _Ready()
		{
			HpScaling.Add(BaseHp);
			MaxHealth = BaseHp;
			if(level.WaveCount > AppearsAtLevel)
				ScaleHp();
			Speed = rng.Next(120, 130);
			base._Ready();
			
		}

		public override void Attack()
		{
			
			PackedScene attackScene = GD.Load<PackedScene>(ProjectPath.ScenesPath + "mob_attack.tscn");
			var instance = attackScene.Instantiate<Area2D>();
			var direction = GetDirection();
			var marker = GetNode<Marker2D>("Marker2D");

			var attackPosition = marker.Position;

			instance.Position = attackPosition * direction;
			var cont = GetNode<Node2D>("Attack");
			cont.AddChild(instance);
			
			_animSprite.AnimationFinished += () =>
			{
				foreach (var child in cont.GetChildren())
				{
					child.QueueFree();
				}
			};
			
			_animSprite.Play("attack");
			
		}

		public override void ScaleHp()
		{
			MaxHealth = GetScaledHp(_hpScaleFactor);
		}
	}
}

