using Godot;
using System;

public partial class Enemy : Area2D
{
	// Called when the node enters the scene tree for the first time.
    private PackedScene _explosionPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/explosion.tscn");
    private PackedScene _laserPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/enemy_laser.tscn");

	public override void _Ready()
	{
		Name = "Enemy";
	}

	float speed = -1.5f;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += new Vector2(speed, 0);
	}
	private void _on_area_entered(Area2D area){
		if(area is Laser){
			var gameNode = (Game) this.GetParent();
			gameNode.incrementPoints();
			QueueFree();

			Node2D explosionInstance = (Node2D)_explosionPrefab.Instantiate();
			explosionInstance.Position = Position;
        	this.GetParent().AddChild(explosionInstance);

			area.QueueFree();
		}
	}

	private void _on_enemy_laser_timer_timeout(){
		EnemyLaser laser = (EnemyLaser)_laserPrefab.Instantiate();
		laser.Position = Position;
		this.GetParent().AddChild(laser);
	}
}
