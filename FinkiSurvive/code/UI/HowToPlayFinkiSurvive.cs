using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code;

public partial class HowToPlayFinkiSurvive : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<Button>("BackButton").Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://MainScene/choose_game.tscn");
		};
		
		GetNode<Button>("PlayButton").Pressed += () =>
		{
			GetTree().ChangeSceneToFile(ProjectPaths.GameScenePath);
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}