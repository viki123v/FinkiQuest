using Godot;
using System;

public partial class Meteor : Area2D
{
	float speed = 2.5f;
	private PackedScene _explosionPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/explosion.tscn");


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Name = "Meteor";

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position -= new Vector2(speed, 0);
	}

	private void _on_area_entered(Area2D area){
		if(area is Laser){
			this.QueueFree();

			Node2D explosionInstance = (Node2D)_explosionPrefab.Instantiate();
			explosionInstance.Position = Position;
        	this.GetParent().AddChild(explosionInstance);

			area.QueueFree();
		}
	}
}
