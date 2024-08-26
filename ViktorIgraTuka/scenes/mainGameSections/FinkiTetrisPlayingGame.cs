using Godot;
using System;

public partial class FinkiTetrisPlayingGame : Node2D
{
	private Vector2I _gameWindowSize = new Vector2I(650, 704);
	private Vector2I _defaultGameSize = new Vector2I(1152, 648); 
	
	public override void _Ready()
	{
		//NOTE: default window size (1152,648) 
		DisplayServer.WindowSetSize(_gameWindowSize);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
