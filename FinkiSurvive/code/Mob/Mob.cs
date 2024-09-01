using Godot;
using System;
using System.Collections.Generic;
using FinkiAdventureQuest.FinkiSurvive.code.Util;

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

    protected Timer AttackSpeedTimer = new();
    public Timer FlipTimer = new();

    public bool CanFlip = false;
		
    protected readonly List<int> HpScaling = new();
    protected bool CanAttack = false;

    private bool _canMove = true;
    public static bool StateValid = true;
    
    
		
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

        AttackSpeedTimer.Autostart = true;
        AttackSpeedTimer.Timeout += () => { CanAttack = true; };
        
        FlipTimer.Autostart = true;
        FlipTimer.WaitTime = 0.5f;
        FlipTimer.Timeout += () => { CanFlip = true; };
        
        AddChild(FlipTimer);
			
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
			
        LabelFactory.DisplayDamageLabel(this,amount);
        
        if (Health <= 0)
        {
            EmitSignal(nameof(MobDamaged), this);
        }
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

    public abstract PackedScene DropCoin();
    
    
    
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
        

        if (collision != null)
        { 
            var collidedNode = collision.GetCollider() as Node;
            if (collidedNode!.IsInGroup("Player"))
            {
                Attack();

            } else if (collidedNode!.IsInGroup("Mobs"))
            {
                Velocity = Velocity.Bounce((collision.GetNormal()).Normalized());
                MoveAndCollide(Velocity * (float)delta * 20);
            }
        }
        
        
        if (!CanFlip) return;
        
        _animSprite.FlipH = desired.X < 0;
        CanFlip = false;

    }

    public abstract void Attack();
    public abstract void ScaleHp();

    public abstract int GetDamage();
    
    

    public void MoveMob(Vector2 position)
    {
        position.X += rng.Next(-50,50);
        position.Y += rng.Next(-50,50);
        playerPos = position;
    }
		
    protected int GetScaledHp(float hpScaleFactor,MobType type)
    {
        int hp = MobScaleHandler.GetHpEntry(type);
        hp += (int) Math.Ceiling(hp * (hpScaleFactor + Main.Game.WaveCount / 100.0f));
        MobScaleHandler.AddEntry(type, hp);
        GD.Print("HpScalingList: " + HpScaling.ToString());
        return hp;
    }

    public static void AddMobHpEntry()
    {
        
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

   
		

}