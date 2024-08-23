using Godot;
using System;

public partial class EnemyLaser : Area2D
{
	int speed = 2;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Name = "EnemyLaser";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position -= new Vector2(speed, 0);
	}

	private void _on_area_entered(Area2D area){
		if(area is Laser){
			this.QueueFree();
		}
	}
}
