using Godot;
using System;

public partial class TextureButton : Godot.TextureButton
{
	private int musicBus;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		musicBus = AudioServer.GetBusIndex("Master");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _on_texture_button_pressed(){
		AudioServer.SetBusMute(0, !AudioServer.IsBusMute(musicBus));
	}
}
