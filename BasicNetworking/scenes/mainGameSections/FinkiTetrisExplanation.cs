using Godot;
using System;

public partial class FinkiTetrisExplanation : CanvasLayer
{
	public void OnBackClicked()
	{
		GetTree().ChangeSceneToFile("res://MainScene/choose_game.tscn"); 
	}

	public void OnPlayClicked()
	{
		GetTree().ChangeSceneToFile("res://BasicNetworking/scenes/mainGameSections/FinkiTetrisPlayingGame.tscn"); 
	}
}
