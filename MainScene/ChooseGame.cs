using Godot;
using System;
using FinkiAdventureQuest.FinkiSurvive.code;
using System.Linq;

namespace FinkiAdventureQuest.MainScene
{
public partial class ChooseGame : Control
{
	private Label _label;
	private Button graduate;
	static int[] grades = {5, 5, 5}; 
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().Paused = false;
		_label = GetNode<Label>("Container/passed");
		graduate = GetNode<Button>("Container/Graduate");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_label.Text = "Exams passed: " + getPassed() + "/3";
		// if(getPassed() == 0){
			graduate.Disabled = false;
		// }

		// _label.Text = $"Exams passed: {getPassed()}/3";
		
	}

	public static void setGrade(int gameNum, int grade){
		grades[gameNum-1] = grade;
	}

	public int getPassed(){
		return grades.Count(grade => grade > 5);
	}

	public void FinkiSurvive()
	{
		GetTree().ChangeSceneToFile(ProjectPath.MainScenePath);
	}

	public void FinkiTetris()
	 =>  GetTree().ChangeSceneToFile("res://ViktorIgraTuka/scenes/mainGameSections/FinkiTetrisExplanation.tscn");
	
	
	public void SpaceAlorithms(){
		GetTree().ChangeSceneToFile("res://SpaceAlgorithm's/scenes/start.tscn");
	}
	public void _on_back_pressed(){
		GetTree().ChangeSceneToFile("res://MainScene/main_menu.tscn");
	}
}
	
}
