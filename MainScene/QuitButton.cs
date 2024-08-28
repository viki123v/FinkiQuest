using Godot;
using System;

public partial class QuitButton : Button
{
	// Called when the node enters the scene tree for the first time.
	
	private Label _label;
	public override void _Ready()
	{
		_label = GetNode<Label>("Label");
		_label.Visible = false;
		MouseEntered += () => { _label.Visible = true; };
		MouseExited += () =>
		{
			_label.Visible = false;
			ReleaseFocus();
		};

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
}
