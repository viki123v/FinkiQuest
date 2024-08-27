using FinkiAdventureQuest.MainScene;
using Godot;
using System;

public partial class End : CanvasLayer
{
	int grade = 5;
	private Label _label;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_label = GetNode<Label>("Label");

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_label.Text = "Grade: " + grade; 	
	}
	private void _on_play_again_pressed(){
		GetTree().ChangeSceneToFile("res://SpaceAlgorithm's/scenes/game.tscn");

	}
	private void _on_exit_pressed(){
		ChooseGame.setGrade(1, grade);

		GetTree().ChangeSceneToFile("res://MainScene/choose_game.tscn");
	}

	public void setGrade(int grade){
		this.grade = grade;
	}

    public static explicit operator End(Resource v)
    {
        throw new NotImplementedException();
    }

}
