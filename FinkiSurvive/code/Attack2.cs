using Godot;
using System;

namespace FinkiAdventureQuest.FinkiSurvive.code;

[GlobalClass]
public partial class Attack2 : BaseAttack
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
		return AttackSpeed;
	}

	public override int GetDamage()
	{
		return Rng.Next(Damage, Damage + 50);
	}

	public override float GetAttackRange()
	{
		return AttackRange;
	}

	protected override void ScaleHp()
	{
		throw new NotImplementedException();
	}

	public override string GetContainerName()
	{
		return "Attack2";
	}

	public override string GetIconPath()
	{
		return "res://FinkiSurvive/assets/fx/slash6/image/slash6_00005.png";
	}

	public override int GetAvailableAtWave()
	{
		return AvailableAtWave;
	}

	protected override void Ability()
	{
	}
}