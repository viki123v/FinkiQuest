using Godot;
using System;
using FinkiAdventureQuest.FinkiSurvive.code;
using System.Linq;
using FinkiAdventureQuest.FinkiSurvive.code.Util;
using Godot.Collections;
using CollectionExtensions = System.Collections.Generic.CollectionExtensions;

namespace FinkiAdventureQuest.MainScene
{
public partial class ChooseGame : Control
{
	private Label _label;
	private Button _graduateButton;
	
	private static Dictionary<GameNames, int> _gameNameToGrade = new();
	
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
	
	public override void _Process(double delta)
	{
		var passed = GetPassed();
		_label.Text = "Exams passed: " + passed + "/3";
		if (passed == 3)
		{
			_graduateButton.Disabled = false;
		}
		
	}

	public static void AddGradeEntry(GameNames name, int grade)
	{
		if (CollectionExtensions.TryAdd(_gameNameToGrade, name, grade)) return; // ako imat vekje entry vrakjat true
		
		if (grade > _gameNameToGrade[name])
		{
			_gameNameToGrade[name] = grade;
		}
	}
	
	public static int GetPassed(){
		return _gameNameToGrade.Values.Count(value => value > 5);
	}

	public void FinkiSurvive()
	{
		GetTree().ChangeSceneToFile("res://FinkiSurvive/scenes/how_to_play.tscn");
	}

	public void ShowFinkiSurviveStats()
	{
		
		var label = GetNode<Label>("FinkiSurviveStats/Label");
		label.Visible = true;
		if (_gameNameToGrade.ContainsKey(GameNames.FinkiSurvive))
		{
			label.Text = "Best Grade: " + _gameNameToGrade[GameNames.FinkiSurvive];
		}
		else
		{
			label.Text = "Not Passed";
		}
		
	}

	public void HideFinkiSurviveStats()
	{
		GetNode<Label>("SpaceAlgorithmsStats/Label").Visible = false;
	}

	public void ShowSpaceAlorithms()
	{
		
		var label = GetNode<Label>("SpaceAlgorithmsStats/Label");
		label.Visible = true;
		if (_gameNameToGrade.ContainsKey(GameNames.SpaceAlgorithms))
		{
			label.Text = "Best Grade: " + _gameNameToGrade[GameNames.SpaceAlgorithms];
		}
		else
		{
			label.Text = "Not Passed";
		}
		
	}

	public void HideSpaceAlorithms()
	{
		GetNode<Label>("FinkiSurviveStats/Label").Visible = false;
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
