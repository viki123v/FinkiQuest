using Godot;
using System;

public partial class win_screen : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (!Visible) return;
		
		GetNode<Control>("Wrap").CustomMinimumSize = GetViewport().GetVisibleRect().Size;
		GetNode<Control>("Wrap/ColorRect").CustomMinimumSize = GetViewport().GetVisibleRect().Size;
	}
}
