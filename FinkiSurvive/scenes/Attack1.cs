using Godot;
using System;

namespace FinkiAdventureQuest.FinkiSurvive.FinkiQuest.scenes
{
public partial class Attack1 : BaseAttack
{

	[Signal]
	public delegate void AtkSpeedEventHandler();
	
	[Signal]
	public delegate void HealPlayerEventHandler(float healAmount);


	public float healAmount = 2f;
	public override void _Ready()
	{
		var player = GetNode<player>("/root/Level/Player");

		Connect(nameof(HealPlayer), new Callable(player, nameof(player.HealPlayer)));
	}

	public override float GetAttackSpeed()
	{
		return 0.1f;
	}

	public override float GetDamage()
	{
		return Rng.Next(5, 10);
	}

	public override float GetAttackRange()
	{
		return 100;
	}

	public override void ScaleHp()
	{
		throw new NotImplementedException();
	}

	public override string GetContainerName()
	{
		return "Attack1";
	}

	public override string GetIconPath()
	{
		return "res://FinkiSurvive/assets/fx/slash/image/skash_00007.png"; 
	}

	public override int AvailableAtWave()
	{
		return 1;
	}

	public override void Ability()
	{
		EmitSignal(nameof(HealPlayer), healAmount);
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
}
}
