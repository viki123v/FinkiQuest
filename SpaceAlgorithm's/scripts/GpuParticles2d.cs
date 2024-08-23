using Godot;
using System;

public partial class GpuParticles2d : GpuParticles2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.Emitting = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
