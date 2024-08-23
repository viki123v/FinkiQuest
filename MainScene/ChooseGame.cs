using Godot;
using System;
using FinkiAdventureQuest.FinkiSurvive.code;

namespace FinkiAdventureQuest.MainScene
{
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
	public void SpaceAlorithms(){
		GD.Print("Space Algorithm's");
		GetTree().ChangeSceneToFile("res://SpaceAlgorithm's/scenes/start.tscn");
	}
}
	
}
