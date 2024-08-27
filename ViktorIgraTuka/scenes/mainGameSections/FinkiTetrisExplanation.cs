using Godot;
using System;

public partial class FinkiTetrisExplanation : Node2D
{
	public void OnBackClicked()
	{
		GetTree().ChangeSceneToFile("res://MainScene/choose_game.tscn"); 
	}

	public void OnPlayClicked()
	{
		GetTree().ChangeSceneToFile("res://ViktorIgraTuka/scenes/mainGameSections/FinkiTetrisPlayingGame.tscn"); 
	}
}
