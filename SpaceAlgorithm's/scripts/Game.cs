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
    private CanvasLayer _pauseMenu;
    Boss boss;
    private Vector2 _originalCameraPosition;

    private List<string> enemies = new List<string>
    {
        "ARRAY", "LIST", "HASH MAP", "STACK", "QUEUE", "HASH", "TREE",
        "BINARY TREE", "HEAP", "GRAPH", "BUBBLE SORT", "DIJKSTRA",
        "MERGE SORT", "KRUSKAL", "PRIM", "O(N)", "BINARY SEARCH",
        "SORT", "DLL", "SLL", "PRIORITY QUEUE", "QUICK SORT",
        "INSERTION SORT", "BFS", "DFS", "BRUTE FORCE", "SET",
        "INORDER", "PREORDER", "POSTORDER", "GREEDY", "DEQUE",
        "HASH TABLE", "LINKED LIST", "KNAPSACK", "DIVIDE AND CONQUER",
        "O(N*LogN)", "O(1)", "NODE", "LINEAR"
    };

    public override void _Ready()
    {
        _timer = GetNode<Timer>("enemy_timer");
        _screenSize = GetViewport().GetVisibleRect().Size;
        _player = GetNode<Node2D>("player");
        _control = GetNode<Control>("game_ui");
        _label = GetNode<Label>("game_ui/score_label");
        _endTimer = GetNode<Timer>("end_timer");
        _pauseMenu = GetNode<CanvasLayer>("PauseMenu");
        _originalCameraPosition = GetViewport().CanvasTransform.Origin;
		// startBoss();
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("escape")){
            togglePause();
        }

        if (boss != null && IsInstanceValid(boss))
        {
            boss.Position = new Vector2(_screenSize.X - 250, _screenSize.Y / 2);
        }

        _screenSize = GetViewport().GetVisibleRect().Size;
        _control.CustomMinimumSize = _screenSize;
    }

    public void togglePause()
    {
        _pauseMenu.Visible = !_pauseMenu.Visible;
        GetTree().Paused = !GetTree().Paused;
    }

    private void _on_enemy_timer_timeout()
    {
        if(bossStarted || enemies.Count == 0) return;

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

    public void startBoss()
    {
		bossStarted = true;
        boss = (Boss)bossPrefab.Instantiate();
        boss.Position = new Vector2(_screenSize.X - 400, _screenSize.Y / 2);
        this.AddChild(boss);
    }

    private void _on_boss_attack_meteor_timeout()
    {
        Random r = new Random();
        if(bossStarted)
        {
            for (int i = 0; i < 3; i++)
            {
                Meteor meteor = (Meteor)_meteorPrefab.Instantiate();
                meteor.Position = new Vector2(_screenSize.X + 20, r.Next(40, (int)_screenSize.Y - 40));
                AddChild(meteor);
            }
        }
        else
        {
            Meteor meteor = (Meteor)_meteorPrefab.Instantiate();
            meteor.Position = new Vector2(_screenSize.X + 20, r.Next(40, (int)_screenSize.Y - 40));
            AddChild(meteor);
        }
    }

    private void _on_boss_shield_timeout()
    {
        if(bossStarted)
        {
            Shield shield = (Shield)_shieldPrefab.Instantiate();
            shield.Position = new Vector2(_screenSize.X - 400, _screenSize.Y / 2);
            AddChild(shield);
        }
    }

    private void _on_boss_special_timeout()
    {
        if(!bossStarted) return;
        BossSpecial special;

        int halfScreen = (int)(_screenSize.Y / 2);

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

    public void incrementPoints()
    {
        score++;
        updateScore();
    }

    private void updateScore()
    {
        _label.Text = "Points: " + score + "/100";
    }

    private int getGrade()
    {
        int grade;

        if(score >= 90) grade = 10;
        else if(score >= 84) grade = 9;
        else if(score >= 78) grade = 8;
        else if(score >= 72) grade = 7;
        else if(score >= 60) grade = 6;
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

    public void gameWin()
    {
        bossStarted = false;
        score += 60;
        updateScore();
        int grade = getGrade();
        _endTimer.Start();

        StartScreenShake(); // Trigger screen shake effect
    }

    private async void StartScreenShake()
    {
        for (int i = 0; i < 10; i++) // Adjust the loop for duration and intensity
        {
            GetViewport().CanvasTransform = new Transform2D(0, new Vector2((float)GD.RandRange(-5, 5), (float)GD.RandRange(-5, 5)));
            await ToSignal(GetTree().CreateTimer(0.05f), "timeout");
        }
        GetViewport().CanvasTransform = new Transform2D(0, _originalCameraPosition); // Reset camera position
    }
}
