using Godot;
using System;

[Tool]
public partial class Settings : Control
{
	// Called when the node enters the scene tree for the first time.

	private Label _label;
	public override void _Ready()
	{
		GetNode<MarginContainer>("Menu").Visible = false;
		_label = GetNode<Label>("TextureButton/Label");
		_label.Visible = false;
		GetNode<Button>("TextureButton").MouseEntered += () => { _label.Visible = true; };
		GetNode<Button>("TextureButton").MouseExited += () => {_label.Visible = false;};
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("escape"))
		{
			ShowMenu();
			var blur = GetNode<ColorRect>("/root/Level/UI/ColorRect");
			blur.Visible = !blur.Visible;
		}
	}

	public void OnVolumeSliderValueChanged(float value)
	{
		AudioServer.SetBusVolumeDb(0,value - 50);
	}

	public void OnDisplayModeOptionSelected(int index)
	{
		switch (index)
		{
			case 0:
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
				break;
			case 1:
			
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);
				break;
		}
		
	}

	public void OnResolutionOptionSelected(int index)
	{
		string [] resPair = GetNode<OptionButton>("Menu/VBoxContainer/ResolutionOptions")
			.GetPopup()
			.GetItemText(index)
			.Split('x');
		Vector2I resolution = new Vector2I(int.Parse(resPair[0]), int.Parse(resPair[1]));
		
		
		GetWindow().SetSize(resolution);
		

	}

	public void ShowMenu()
	{
		var menu = GetNode<MarginContainer>("Menu");
		menu.Visible = !menu.Visible;
		GetTree().Paused = !GetTree().Paused;
		GetNode<Button>("TextureButton").ReleaseFocus();
	}
	
	public void OnMuteCheckBoxToggled(bool toggledOn)
	{
		AudioServer.SetBusMute(0,toggledOn);
	}
}
