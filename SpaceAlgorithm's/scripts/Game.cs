using Godot;
using System;
using System.Collections.Generic;

public partial class Game : Node2D

{
	private Timer _timer;
	private Vector2 _screenSize;
	int score = 0;

	private bool bossStarted = false;
    private PackedScene _meteorPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/meteor.tscn");
	private PackedScene enemyPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/enemy.tscn");
	private PackedScene bossPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/boss.tscn");
	private PackedScene _shieldPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/shield.tscn");
	private PackedScene _laserPrefab = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/enemy_laser.tscn");
	private PackedScene _bossSpecial = (PackedScene)GD.Load("res://SpaceAlgorithm's/prefabs/enemy_special.tscn");

    private Node2D _player;
    private Label _label;
	private Control _control;
	private Timer _endTimer;
	Boss boss;


	private List<string> enemies = new List<string> 
	{"ARRAY", "LIST", "HASH MAP", "STACK", "QUEUE", "HASH", "TREE",
	 "BINARY TREE", "HEAP", "GRAPH", "BUBBLE SORT", "DIJKSTRA", 
	 "MERGE SORT", "KRUSKAL", "PRIM", "O(N)", "BINARY SEARCH", 
	 "SORT", "DLL", "SLL", "PRIORITY QUEUE", "QUICK SORT", 
	 "INSERTION SORT", "BFS", "DFS", "BRUTE FORCE", "SET", 
	 "INORDER", "PREORDER", "POSTORDER", "GREEDY", "DEQUE",
	 "HASH TABLE", "LINKED LIST", "KNAPSACK", "DIVIDE AND CONQUER",
	 "O(N*LogN)", "O(1)", "NODE", "LINEAR"};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = GetNode<Timer>("enemy_timer");
		_screenSize = GetViewport().GetVisibleRect().Size;
		_player = GetNode<Node2D>("player");
		_control = GetNode<Control>("game_ui");
		_label = GetNode<Label>("game_ui/score_label");
		_endTimer = GetNode<Timer>("end_timer");

		// startBoss();
		// bossStarted = true;
	}
	

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(boss != null) boss.Position = new Vector2(_screenSize.X - 200, _screenSize.Y/2);
		_screenSize = GetViewport().GetVisibleRect().Size;
		_control.CustomMinimumSize = _screenSize;
	}
	private void _on_enemy_timer_timeout()
    {

		Random r = new Random();
		Enemy enemy = (Enemy)enemyPrefab.Instantiate();
		enemy.Position = new Vector2(_screenSize.X + 30, r.Next(40, (int)_screenSize.Y - 40));

		int randomIndex = r.Next(enemies.Count);
		string randomEnemy = enemies[randomIndex];
		enemies.RemoveAt(randomIndex);


		Label enemyLabel = enemy.GetNode<Label>("Label");
		enemyLabel.Text = randomEnemy;

		this.AddChild(enemy);

		if(enemies.Count == 0 && !bossStarted){
			startBoss();
			bossStarted = true;
		}
    }

	public void startBoss(){
		boss = (Boss)bossPrefab.Instantiate();
		boss.Position = new Vector2(_screenSize.X - 200, _screenSize.Y/2);
		this.AddChild(boss);
	}

	private void _on_boss_attack_meteor_timeout(){
		Random r = new Random();
		if(bossStarted){
			Meteor meteor = (Meteor) _meteorPrefab.Instantiate();
			meteor.Position = new Vector2(_screenSize.X + 20, r.Next(40, (int)_screenSize.Y - 40));
			AddChild(meteor);
			meteor.Position = new Vector2(_screenSize.X + 20, r.Next(40, (int)_screenSize.Y - 40));
			AddChild(meteor);

			meteor = (Meteor) _meteorPrefab.Instantiate();
			meteor.Position = new Vector2(_screenSize.X + 20, _player.Position.Y);
			AddChild(meteor);

		}
	}

	private void _on_boss_shield_timeout(){
		if(bossStarted){
			Shield shield = (Shield) _shieldPrefab.Instantiate();
			shield.Position = new Vector2(_screenSize.X - 200, _screenSize.Y/2);
			AddChild(shield);
		}
	}

	private void _on_boss_special_timeout(){
		if(!bossStarted) return;
		BossSpecial special;

		int halfScreen = (int)(_screenSize.Y/2);

		special = (BossSpecial)_bossSpecial.Instantiate();
		special.Position = new Vector2(_screenSize.X + 50, halfScreen);
		AddChild(special);

		special = (BossSpecial)_bossSpecial.Instantiate();
		special.Position = new Vector2(_screenSize.X + 50, halfScreen + halfScreen / 2);
		AddChild(special);

		special = (BossSpecial)_bossSpecial.Instantiate();
		special.Position = new Vector2(_screenSize.X + 50, halfScreen - halfScreen / 2);
		AddChild(special);
	}

	public void incrementPoints(){
		score++;
		updateScore();
	}

	private void updateScore(){
		_label.Text = "Points: " + score + "/100";
	}

	private int getGrade(){
		int grade;

		if(score >= 90) grade = 10;
		else if(score >= 80) grade = 9;
		else if(score >= 70) grade = 8;
		else if(score >= 60) grade = 7;
		else if(score >= 50) grade = 6;
		else grade = 5;

		return grade;
	}
	
	public void _on_end_timer_timeout()
{
    // Load the End scene
    var endScene = (PackedScene)GD.Load("res://SpaceAlgorithm's/scenes/end.tscn");
    End endInstance = (End)endScene.Instantiate();

    // Pass the grade to the End scene
    endInstance.setGrade(getGrade());

    // Change to the End scene
    GetTree().Root.AddChild(endInstance);
    GetTree().CurrentScene.QueueFree();
    GetTree().CurrentScene = endInstance;
}


	public void gameWin(){
		bossStarted = false;
		score+=60;
		updateScore();
		int grade = getGrade();
		_endTimer.Start();
	}
}
