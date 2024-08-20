using Godot;
using System;
using FinkiAdventureQuest.FinkiSurvive.FinkiQuest;

public partial class ChooseGame : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void FinkiSurvive()
	{
		GD.Print("FinkiSurvive");
		GetTree().ChangeSceneToFile(ProjectPath.MainScenePath);
	}
}
