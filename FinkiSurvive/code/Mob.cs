using Godot;
using System;
using System.Collections.Generic;

namespace FinkiAdventureQuest.FinkiSurvive.code;

[GlobalClass]
public abstract partial class Mob : CharacterBody2D
{
    public static int MaxHealth;
    public float Speed = 150;
    public Random rng = new Random();
    public int Health { get; set;}
    public Vector2 playerPos = Vector2.Zero;
    public TextureProgressBar _healthBar;
    public int _hpBarSize;
    public  AnimatedSprite2D _animSprite;
    public int AttackDamage = 2;
    public float SeekForce = 0.05f;
    public Vector2 Acceleration = Vector2.Zero;
		
    protected readonly List<int> HpScaling = new();

    private bool _canMove = true;
    public static bool StateValid = true;

    public int FrameCount = 0;
		
		
    [Signal]
    public delegate void MobDamagedEventHandler(Mob parent);

    [Signal]
    public delegate void MobDiedEventHandler();

    public override void _Ready()
    {
			
        _healthBar = GetNode<TextureProgressBar>("HealthBar");
        Health = MaxHealth;
        _animSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        _hpBarSize = MaxHealth / 5;
			
        _healthBar.GetNode<Label>("Label").Text = Health.ToString();
			
        var playerObj = GetNode<Player>("/root/Level/Player");
        playerObj?.Connect(nameof(Player.PlayerMoved), new Callable(this, nameof(MoveMob)));
			
        _animSprite.Play("walk");

    }

    public void TakeDamage(int amount)
    {
        var hpLabel = _healthBar.GetNode<Label>("Label");
        _healthBar.Value = (int)Math.Floor((float) Health / _hpBarSize);
        Health -= amount;
        int numHp = int.Parse(hpLabel.Text);
        numHp -= amount;
        hpLabel.Text = numHp.ToString();
        _animSprite.Play("hurt");
			
        DisplayDamageTaken(amount);
			
        if (Health <= 0)
        {
            EmitSignal(nameof(MobDamaged), this);
        }
    }

    public void DisplayDamageTaken(int amount)
    {
        var container = GetNode<MarginContainer>("HitLabelsCont");
        var tween = GetTree().CreateTween();
        var label = new Label();
        label.Text = amount.ToString();
        label.AddThemeFontSizeOverride("font_size",50);
        container.AddChild(label);

        var fv = new FontVariation();

        fv.BaseFont = ResourceLoader.Load<FontFile>(ProjectPath.DefaultPath + "tmp_assets/FONTS/videophreak/VIDEOPHREAK.ttf");
        fv.VariationEmbolden = 1.2f;

        label.AddThemeFontOverride("font", fv);
        label.AddThemeColorOverride("font_color", Colors.Crimson);

        //label.Scale = Vector2.Zero;
        var endPos = label.Position;
        Vector2 offset = new Vector2(rng.Next(0,150),rng.Next(0,250));
        endPos += offset;
			
        tween.TweenProperty(label, "scale",Vector2.One, 0.3f);
        tween.TweenProperty(label, "position", endPos, 0.5f); // radi ova imat warinngs vo debug
        tween.TweenProperty(label, "scale",Vector2.Zero, 0.5f);
			
        tween.TweenCallback(Callable.From(label.QueueFree)).SetDelay(0.7f);
			
    }

    public void Death()
    {
        _canMove = false;
        GetNode<CollisionPolygon2D>("MobCollisions/CollisionPolygon2D").CallDeferred("set", "disabled", true);
        GetNode<CollisionPolygon2D>("CollisionPolygon2D").CallDeferred("set", "disabled", true);
        GetNode<TextureProgressBar>("HealthBar").Visible = false;
        var soundEffect = GetNode<AudioStreamPlayer2D>("KillAttackSoundEffect");
        soundEffect.VolumeDb = -20;
        soundEffect.Play();

        EmitSignal(nameof(MobDied));
    }

    public Vector2 GetDirection()
    {
        return (playerPos - Position).Normalized();
    }

    
    public override void _PhysicsProcess(double delta)
    {
        if(!StateValid || !_canMove) return;

        if (!_animSprite.IsPlaying())
        {
            _animSprite.Play("walk");
        }
        
       

        Vector2 desired = (playerPos - Position).Normalized() * (Speed * (float) delta);
        Vector2 steer = desired - Velocity;
        if (steer.Length() > SeekForce)
        {
            steer *= SeekForce;
        }

        Acceleration = steer;

        Velocity += Acceleration;

        if (Velocity.Length() > Speed)
        {
            Velocity *= Speed;
        }
        
			
        var collision = MoveAndCollide(Velocity);

        if (FrameCount++ % 60 == 0)
        {
            _animSprite.FlipH = desired.X < 0;
        }
			
			
        var bounceFactor = 100;

        if (collision != null)
        { 
            var collidedNode = collision.GetCollider() as Node;
            if (collidedNode!.IsInGroup("Player"))
            {
                GetNode<Player>("/root/Level/Player").TakeDamage(AttackDamage);
                Attack();
            }

            var normal = (collision.GetNormal() * bounceFactor).Normalized();
        }
        
    }

    public abstract void Attack();

    public void MoveMob(Vector2 position)
    {
        position.X += rng.Next(-50,50);
        position.Y += rng.Next(-50,50);
        playerPos = position;
    }
		
    protected int GetScaledHp(float hpScaleFactor)
    {
        int hp = HpScaling[^1];
        hp += (int) Math.Ceiling(hp * (hpScaleFactor + Map.WaveCount / 20.0f));
			
        HpScaling.Add(hp);
			
        return hp;
    }
		
    // public int getPrevHp()
    // {
    // 	int hp = BaseHp;
    // 	for (int i = 2; i <= Map.WaveCount; i++)
    // 	{
    // 		hp += (int) Math.Ceiling(hp * (0.3 + (i / 20.0f))); 
    // 	}
    //
    // 	return hp;
    // } // prvo vaka bese GetScaledHp

    public abstract void ScaleHp();
		

}