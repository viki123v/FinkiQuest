using Godot;

public partial class FinkiTetris : Node2D
{
	public override void _Ready()
	{
		AddChild(ResourceLoader.Load<PackedScene>("res://ViktorIgraTuka/scenes/mainGameSections/FinkiTetrisExplanation.tscn").Instantiate()); 
	}
}
