using Godot;
using System;

namespace FinkiAdventureQuest.FinkiSurvive.FinkiQuest.scenes
{
	
public partial class Attack3 : BaseAttack
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override float GetAttackSpeed()
	{
		return 0.3f;
	}

	public override float GetDamage()
	{
		return Rng.Next(40,60);
	}

	public override float GetAttackRange()
	{
		return 150;
	}

	public override void ScaleHp()
	{
		throw new NotImplementedException();
	}

	public override string GetContainerName()
	{
		return "Attack3";
	}

	public override string GetIconPath()
	{
		return "res://FinkiSurvive/assets/fx/slash5/image/slash5_1_00006.png";
	}

	public override int AvailableAtWave()
	{
		return 3;
	}

	public override void Ability()
	{
		
	}
}
}

