using System;
using System.Collections.Generic;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo.GroupBlocks;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.GradeStrategies;
using Godot;

public partial class FinkiTetrisPlayingGame : Node2D
{
    public static float BlockSizeX;
    public static float BlockSizeY;
    public static int GAME_END_TIME = 60 + 30;

    public static (int, int) Dimensions = (20, 10);
    
    private bool _isResizing;

    public Label TimeLabel { get; set; }
    private CanvasLayer _pauseMenu;

    private Vector2I _gameWindowSize = new Vector2I(650, 704);
    private Vector2I _defaultGameSize = new Vector2I(1152, 648);

    public Block[,] BitMap = new Block[Dimensions.Item1,Dimensions.Item2];
    public int[] CountByRow = new int[Dimensions.Item1];

    private TimerMenager _timersMenager;
    private NextWindowMenager _nextWindowMenager;
    private IGrader _grader;

    private Queue<GroupBlock> _inFallBlocks = new Queue<GroupBlock>();

    public override void _Ready()
    {
        CountAvailableSpotsForBlcoks();
        GetTree().GetRoot().SizeChanged += CountAvailableSpotsForBlcoks;         
        
        _nextWindowMenager = new NextWindowMenager(this);
        
        TimeLabel = GetNode<Label>("CanvasLayer/Sidepanel/Time");

        _timersMenager = GetNode("AnimationTimer") as TimerMenager;
        _timersMenager.Init(this);
        _timersMenager.TimeFinished += OnTimeFinished;
        
        _grader = new GraderOnTimePassed(_timersMenager.PLAYING_TIME_LEFT);
        _pauseMenu = GetNode<CanvasLayer>("PauseManu");
        
        DrawLines();
    }

    private Line2D CreateLine()
    {
        Line2D newLine = new Line2D();
        newLine.SetDefaultColor(new Color("#3D307A"));
        newLine.Width = 1;
        return newLine;
    }
    
    private void DrawLines()
    {
        var spawning = GetNode<ColorRect>("CanvasLayer/SpawingRectangle");
        var linesHolder = GetNode<Node2D>("CanvasLayer/SpawingRectangle/LinesHolder"); 
        
        float row = 0;

        while (row < spawning.Size.X)
        {
            var newLine = CreateLine();
            
            newLine.AddPoint(new Vector2(row,0));
            newLine.AddPoint(new Vector2(row,spawning.Size.Y));
            
            linesHolder.AddChild(newLine);
            row += BlockSizeX;
        }

        float col = 0;
        while (col < spawning.Size.Y)
        {
            var newLine = CreateLine();
            
            newLine.AddPoint(new Vector2(0,col));
            newLine.AddPoint(new Vector2(spawning.Size.X,col));
            linesHolder.AddChild(newLine);

            col += BlockSizeY;
        }
        
    }

    public void OnTimeFinished()
    {
        _timersMenager.StopMovement();
        FinkiTetrisEndCredits.Grade = _grader.GetGrade(_timersMenager.PLAYING_TIME_LEFT);
        GetTree().ChangeSceneToFile("res://BasicNetworking/scenes/mainGameSections/FinkiTetrisEndCredits.tscn");
    }

    public void OnMoveTimer()
    {
        if (_inFallBlocks.Count == 0)
            return;
        
        foreach (var block in _inFallBlocks)
           block.MoveAsGroup(Vector2.Down);

        if (_inFallBlocks.Peek().IsStopped)
            CheckForFullRows();
    }


    public void OnSpawnTimer()
    {
        try
        {
            _inFallBlocks.Enqueue(_nextWindowMenager.GetNext());
            _timersMenager.ConfigureSpawnTimer();
        }
        catch (Exception e)
        {
            if (e.Message.Equals("Game over"))
            {
                _timersMenager.StopMovement();
                FinkiTetrisEndCredits.Grade = _grader.GetGrade(_timersMenager.PLAYING_TIME_LEFT);
                GetTree().ChangeSceneToFile("res://BasicNetworking/scenes/mainGameSections/FinkiTetrisEndCredits.tscn");
            }
            else
                throw e; 
        }
    }
 
    public void togglePause()
    {
        _pauseMenu.Visible = !_pauseMenu.Visible;
        GetTree().Paused = !GetTree().Paused;
    }

    private void OnResize()
    {
        _isResizing = true;
        _timersMenager.StopMovement();
        CountAvailableSpotsForBlcoks();

        for (int i = 0; i < Dimensions.Item1; i++)
        {
            for (int j = 0; j < Dimensions.Item2; j++)
            {
                BitMap[i,j].ResetDisplayPosition();
            }
        }

        _isResizing = false; 
        _timersMenager.ResumeMovement();
    }

    private void CountAvailableSpotsForBlcoks()
    {
        var spawningSurface = GetNode<ColorRect>("CanvasLayer/SpawingRectangle");
        
        // spawningSurface.Size = new Vector2(viewPortSize.X * 3/4 ,viewPortSize.Y);
        // sidePanel.Size = new Vector2(viewPortSize.X * 1 / 4, viewPortSize.Y); 

        BlockSizeX = spawningSurface.Size.X / Dimensions.Item2;
        BlockSizeY = spawningSurface.Size.Y / Dimensions.Item1;

    }

    public override void _Input(InputEvent @event)
    {
        if (_timersMenager.IsChecking || _inFallBlocks.Count == 0 || _isResizing || _timersMenager.IsChecking || _timersMenager.MoveTimer.IsPaused() )
            return;

        if(@event.IsActionPressed("escape"))
        {
            togglePause(); 
            return; 
        }

        var inFocus = _inFallBlocks.Peek();

        if (@event.IsActionPressed("Tetris_RotateBlock"))
            inFocus.RotateAsGroup();
        else if (@event.IsActionPressed("Tetris_MoveBlockLeft"))
            inFocus.MoveAsGroup(Vector2.Left);
        else if (@event.IsActionPressed("Tetris_MoveBlockRight"))
            inFocus.MoveAsGroup(Vector2.Right);
        else if (@event.IsActionPressed("Tetris_MoveBlockDown"))
            inFocus.MoveAsGroup(Vector2.Down);

        if (inFocus.IsStopped)
            _inFallBlocks.Dequeue();

        CheckForFullRows();
    }

    public void CheckForFullRows()
    {
        bool foundFull = false;
        for (int i = 0; i < CountByRow.Length; i++)
        {
            if (CountByRow[i] == Dimensions.Item2)
            {
                foundFull = true;
                ClearFromRow(i);
                break;
            }
        }

        if (!foundFull && _timersMenager.IsChecking)
        {
            _timersMenager.ResumeMovement();
            _timersMenager.IsChecking = false; 
        }
    }

    private async void ClearFromRow(int fromRow)
    {
        _timersMenager.StopMovement();

        List<Block> forRemove = new List<Block>();

        for (int i = fromRow; i < Dimensions.Item1 && CountByRow[i] == Dimensions.Item2; i++)
        {
            for (int j = 0; j < Dimensions.Item2; j++)
            {
                if (BitMap[i, j] is null) continue;
                forRemove.Add(BitMap[i, j]);
            }
        }

        _timersMenager.StartOpacityAnimation(forRemove);
        await _timersMenager.StartFallDownAnimation(BitMap);
    }
}