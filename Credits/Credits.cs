using Godot;
using System;

public partial class Credits : Control
{
	AudioStreamPlayer2D audio;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audio = GetNode<AudioStreamPlayer2D>("Song");
		audio.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
