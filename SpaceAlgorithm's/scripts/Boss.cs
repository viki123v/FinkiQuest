using Godot;
using System.Threading.Tasks;

public partial class Boss : Area2D
{
    int hp;
    private AnimatedSprite2D sprite;
    private Sprite2D damage_texture;
    private Sprite2D more_damage_texture;
    private Color originalColor;
	private PackedScene _explosionPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/explosion.tscn");
	private PackedScene _bossHitPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/boss_hit.tscn");
	private PackedScene _bossDeathPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/boss_death.tscn");

    public override void _Ready()
    {
        sprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        damage_texture = GetNode<Sprite2D>("damadge_texture");
        more_damage_texture = GetNode<Sprite2D>("more_damadge_texture");
        originalColor = sprite.Modulate; 
        hp = 120;
        sprite.Play(); 
        damage_texture.Visible = false;
        more_damage_texture.Visible = false;

    }

    private void _on_area_entered(Area2D area)
    {
        if (area is Laser)
        {
            hp--;
            if (hp == 0)
            {
                var gameNode = (Game)GetParent();
                gameNode.gameWin();

				
                Node2D explosionInstance = (Node2D) _bossDeathPrefab.Instantiate();
				explosionInstance.Position = Position;
        		this.GetParent().AddChild(explosionInstance);
                QueueFree();
                return;
            }
            else
            {
                ShowHitEffect();
                Node2D explosionInstance = (Node2D) _bossHitPrefab.Instantiate();
				explosionInstance.Position = area.Position;
        		this.GetParent().AddChild(explosionInstance);
            }
            if(hp <= 50){
                damage_texture.Visible = true;
            }
            if(hp <= 30){
                more_damage_texture.Visible = true;
            }
            
        }
    }

    private async void ShowHitEffect()
    {
        // Modulate the sprite to red to indicate a hit
        sprite.Modulate = new Color(1, 0, 0); // Red color

        // Wait for a short duration before resetting the color
        await ToSignal(GetTree().CreateTimer(0.1f), "timeout");

        // Reset the sprite color to its original value
        sprite.Modulate = originalColor;
        
    }
}