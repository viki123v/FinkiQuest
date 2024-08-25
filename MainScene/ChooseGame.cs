using Godot;
using System;
using FinkiAdventureQuest.FinkiSurvive.code;

namespace FinkiAdventureQuest.MainScene
{
public partial class ChooseGame : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().Paused = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void FinkiSurvive()
	{
		GetTree().ChangeSceneToFile(ProjectPath.MainScenePath);
	}
	public void SpaceAlorithms(){
		GetTree().ChangeSceneToFile("res://SpaceAlgorithm's/scenes/start.tscn");
	}
	public void _on_back_pressed(){
		GetTree().ChangeSceneToFile("res://MainScene/main_menu.tscn");
	}
}
	
}
