using Godot;
using System;
using FinkiAdventureQuest.FinkiSurvive.code.Util;

public partial class BellCurve : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	private int ascend = 1;
	private double _modulateAmountPerTick;
	MyModulateTweener tweener = new MyModulateTweener();
	
	public override void _Ready()
	{
		_modulateAmountPerTick = tweener.GetModulateAmount(0.4f, 0.9f, 2, 0.1f); 
		Timer timer = new Timer();
		timer.Timeout += ModulateAlpha;
		timer.WaitTime = 0.1f;
		AddChild(timer);
		timer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	// vrakja so kolkavo delce treba da se modulira od startVal do endVal,za dadeniot duration i tickrate
	
	public void ModulateAlpha()
	{
		
		GD.Print("Textre");

		var modulate = Modulate;
		
		if (modulate.A + _modulateAmountPerTick * ascend > 0.85)
		{
			modulate.A = 0.85f;
			ascend = -1;

		}
		else if (modulate.A + _modulateAmountPerTick * ascend < 0.4)
		{
			modulate.A = 0.4f;
			ascend = 1;
		}

		modulate.A += (float) _modulateAmountPerTick * ascend;
		Modulate = modulate;
		
		GD.Print(Modulate);

	}
}
