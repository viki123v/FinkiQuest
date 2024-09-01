using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code;

public partial class DeathScreen : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	
	public void OnVisibilityChanged()
	{
		GD.Print(Main.Game.Score + "As'das'd'asd'as'das'das'");
		GetNode<Label>("Container/NumContainers/GradeNum").Text = Main.Game.Grade.ToString();
		GetNode<Label>("Container/NumContainers/ScoreNum").Text = Main.Game.Score.ToString();
		GetNode<Label>("Container/NumContainers/WaveCountNum").Text = Main.Game.WaveCount.ToString();
	}
}