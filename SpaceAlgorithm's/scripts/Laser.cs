using Godot;
using System;


public partial class Laser : Area2D
{


	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
		Name = "Laser";
	}
	
	int speed = 25;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += new Vector2(speed, 0);
	}
}