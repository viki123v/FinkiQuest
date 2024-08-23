using Godot;
using System;

namespace FinkiAdventureQuest.FinkiSurvive.code;

[GlobalClass]
public partial class Attack1 : BaseAttack
{

	[Signal]
	public delegate void AtkSpeedEventHandler();
	[Signal]
	public delegate void HealPlayerEventHandler(float healAmount);
	
	
	[Export]
	private float _healAmount = 5f;
	public override void _Ready()
	{
		var player = GetNode<Player>("/root/Level/Player");
		
		Connect(nameof(HealPlayer), new Callable(player, nameof(player.HealPlayer)));
	}

	public override float GetAttackSpeed()
	{
		return AttackSpeed;
	}

	public override int GetDamage()
	{
		return Rng.Next(Damage, Damage + 10);
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
		return "Attack1";
	}

	public override string GetIconPath()
	{
		return "res://FinkiSurvive/assets/fx/slash/image/skash_00007.png"; 
	}

	public override int GetAvailableAtWave()
	{
		return AvailableAtWave;
	}

	protected override void Ability()
	{
		EmitSignal(nameof(HealPlayer), _healAmount);
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
}