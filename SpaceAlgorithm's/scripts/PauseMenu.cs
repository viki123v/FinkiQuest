using Godot;
using System;

public partial class PauseMenu : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_continue_pressed(){
		var gameNode = (Game) this.GetParent();
		gameNode.togglePause();
	}

	public void _on_exit_pressed(){
		GetTree().ChangeSceneToFile("res://MainScene/choose_game.tscn");
	}
}
