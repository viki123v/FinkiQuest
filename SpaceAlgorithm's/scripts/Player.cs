using Godot;
using System;

public partial class Player : Area2D
{
    private Vector2 _screenSize;
	private PackedScene laserPrefab;
    private PackedScene _explosionPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/boss_death.tscn");
	private Timer _restartTimer;

    private bool isHit = false;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _restartTimer = GetNode<Timer>("restart_timer");
        _screenSize = GetViewport().GetVisibleRect().Size;
		laserPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/laser.tscn");
        Position = new Vector2(Position.X, _screenSize.Y / 2);

        Visible = true;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
public float moveSpeed = 500f; // Pixels per second

public override void _Process(double delta)
{
    _screenSize = GetViewport().GetVisibleRect().Size;

    
    Vector2 inputVector = Input.GetVector("space_algorithms_player_left", "space_algorithms_player_right", "space_algorithms_player_up", "space_algorithms_player_down");

    
    Position += inputVector * moveSpeed * (float)delta;

    
    Position = new Vector2(
        Mathf.Clamp(Position.X, 30, _screenSize.X / 2),
        Mathf.Clamp(Position.Y, 30, _screenSize.Y - 30)
    );

    // Handle shooting
    if (Input.IsActionJustPressed("space_algorithms_player_shoot"))
    {
        if(isHit) return;
        Laser laser = (Laser)laserPrefab.Instantiate();
        laser.Position = Position;
        this.GetParent().AddChild(laser);
    }
}


    private void _on_area_entered(Area2D area){
        if(isHit) return;
        if(area is EnemyLaser || area is Enemy || area is Meteor || area is BossSpecial || area is Shield){
            Node2D explosion = (Node2D)_explosionPrefab.Instantiate();
            explosion.Position = Position;
            this.GetParent().AddChild(explosion);
            _restartTimer.Start();
            Visible = false;
            isHit = true;
        }
    }

    private void _on_restart_timer_timeout(){
        GetTree().ChangeSceneToFile("res://SpaceAlgorithm's/scenes/end.tscn");
    }
}
