using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.Misc;

public abstract partial class Coin : Node2D
{
    
    //todo celo vreme da pratit pozicija na igrac
    //todo zvuk za pickup
    //todo timer za da isceznat ako ne gi zemis posle 5 sek
    
    private Vector2 _moveDirection = Vector2.Zero;
    private Vector2 _acceleration = new Vector2(20, 20);
    private int _baseSpeed = 200;
    private int _speed = 0;
    private Node2D player;
	
    private bool _canAttract = false;

    [Signal]
    public delegate void CoinPickedUpEventHandler(int value);
    public override void _Ready()
    {
        player = GetNode<Node2D>("/root/Level/Player");
        GetNode<Area2D>("AttractBox").AreaEntered += area =>
        {
            GD.Print(area.Name);
            _moveDirection = (area.GlobalPosition - GlobalPosition).Normalized();
            _canAttract = true;
        };
		
        GetNode<Area2D>("AttractBox").AreaExited  += area =>
        {
            GD.Print(area.Name);
            _canAttract = false;
        };

        GetNode<Area2D>("PickupBox").AreaEntered += area =>
        {
            GD.Print("Pickup");
            _canAttract = false;

            var tween = GetTree().CreateTween();
			
            tween.TweenProperty(this,"scale", Vector2.Zero, 0.1f);
            tween.TweenCallback(Callable.From(QueueFree));
            
            EmitSignal(nameof(CoinPickedUp), GetValue());
            

        };
        GetNode<AnimatedSprite2D>("AnimatedSprite2D").Play();


    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if(!_canAttract) return;
        Position += _moveDirection * _baseSpeed * (float)delta;
    }

    public abstract int GetValue();
    
    
}