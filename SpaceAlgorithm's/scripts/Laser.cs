using Godot;
using System;


public partial class Laser : Area2D
{


	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
		Name = "Laser";
	}
	
	float speed = 1400f;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += new Vector2((float)(speed * delta), 0);
	}
}
