using Godot;
using System;

public partial class Shield : Area2D
{
	int speed = 400;
	private PackedScene _explosionPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/explosion.tscn");


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Name = "Meteor";

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position -= new Vector2((float)(speed * delta), 0);
	}

	private void _on_area_entered(Area2D area){
		if(area is Laser){
			area.QueueFree();
		}
	}
}
