using Godot;
using System;

public partial class QuitGamePopup : Control
{
	// Called when the node enters the scene tree for the first time.

	private Label _label; 
	public override void _Ready()
	{
		_label = GetNode<Label>("QuitButton/Label");
		_label.Visible = false;
		GetNode<Button>("QuitButton").MouseEntered += () => { _label.Visible = true; };
		GetNode<Button>("QuitButton").MouseExited += () =>
		{
			_label.Visible = false;
		};
		var dialog = GetNode<ConfirmationDialog>("ConfirmationDialog");
		dialog.Confirmed += () =>
		{
			GetTree().ChangeSceneToFile("res://MainScene/choose_game.tscn");
		};
		
		dialog.Canceled += () =>
		{
			GetTree().Paused = false;
		};
		GetNode<Button>("QuitButton").Pressed += () =>
		{
			dialog.Visible = !dialog.Visible;
			GetTree().Paused = false;

		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
