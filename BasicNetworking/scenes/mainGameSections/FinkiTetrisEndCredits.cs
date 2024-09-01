using Godot;
using System;
using FinkiAdventureQuest.MainScene;

public partial class FinkiTetrisEndCredits : CanvasLayer
{
	public static int Grade { get; set; }
	
	public override void _Ready()
	{
		var gradeLabel = GetNode<Label>("Grade");
		gradeLabel.Text = $"Your grade: {Grade.ToString()}"; 
		ChooseGame.AddGradeEntry(GameNames.BaseNetworking,Grade);
	}

	public void OnBackClicked()
	{
		GetTree().ChangeSceneToFile("res://MainScene/choose_game.tscn"); 
	}

	public void OnPlayClicked()
	{
		GetTree().ChangeSceneToFile("res://BasicNetworking/scenes/mainGameSections/FinkiTetrisPlayingGame.tscn"); 
	}
}
