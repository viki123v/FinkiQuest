using Godot;
using System;
using FinkiAdventureQuest.FinkiSurvive.code;
using System.Linq;
using Godot.Collections;

namespace FinkiAdventureQuest.MainScene
{
public partial class ChooseGame : Control
{
	private Label _label;
	private Button _graduateButton;
	
	private static Dictionary<GameNames, int> _gameNameToGrade = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().Paused = false;
		_label = GetNode<Label>("Container/Passed");
		_graduateButton = GetNode<Button>("Container/Graduate");
		_graduateButton.Disabled = true;
		_graduateButton.Pressed += () =>
		{
			GetTree().ChangeSceneToFile("res://Credits/credits.tscn");
		};

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		_label.Text = "Exams passed: " + GetPassed() + "/3";
		if (GetPassed() == 1)
		{
			_graduateButton.Disabled = false;
			GetNode<Button>("/root/MainMenu/VBoxContainer/credits");
		}
		
	}

	public static void AddGradeEntry(GameNames name, int grade)
	{
		_gameNameToGrade[name] = grade;
	}
	
	public static int GetPassed(){
		return _gameNameToGrade.Values.Count(value => value > 5);
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
