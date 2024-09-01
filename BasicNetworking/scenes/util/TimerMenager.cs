using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo;
using Timer = Godot.Timer;

public partial class TimerMenager : Node2D
{
	[Signal]
	public delegate void TimeFinishedEventHandler(); 
	
	[Export] private float ANIMATION_DISSAPEAR_MS = 1;
	[Export] private float ANIMATION_MOVE_MS = 0.3f;
	[Export] private float FALL_DOWN_TIMER = 0.1f; 
	[Export] public int PLAYING_TIME_LEFT = FinkiTetrisPlayingGame.GAME_END_TIME;
	[Export] public float MOVE_TIMER_TICK = 0.2f;
	[Export] public float SPAWN_TIME_TIME_MOVEMENT_OFFSET; 
	
	private Timer _timerTracker; 
	private Timer _spawnTimer;
	public Timer MoveTimer;
	private Timer _animationTimer;
	private FinkiTetrisPlayingGame _scene;
	public bool IsChecking { get; set; } 
	

	public void Init(FinkiTetrisPlayingGame parent)
	{
		SPAWN_TIME_TIME_MOVEMENT_OFFSET = 3 * MOVE_TIMER_TICK;
		
		_scene = parent;
		
		_spawnTimer = new Timer();
		MoveTimer = new Timer();
		_animationTimer = new Timer();
		_timerTracker = new Timer(); 

		parent.AddChild(_spawnTimer);
		parent.AddChild(MoveTimer);
		parent.AddChild(_animationTimer);
		parent.AddChild(_timerTracker);

		MoveTimer.Timeout += _scene.OnMoveTimer;
		MoveTimer.Start(MOVE_TIMER_TICK);

		_spawnTimer.Timeout += _scene.OnSpawnTimer; 
		_spawnTimer.Start(SPAWN_TIME_TIME_MOVEMENT_OFFSET);
		
		_timerTracker.Timeout += OnTimeTick; 
		_timerTracker.Start(1);
	}

	public void ConfigureSpawnTimer()
	{
		int timePassed = FinkiTetrisPlayingGame.GAME_END_TIME - PLAYING_TIME_LEFT;
		_spawnTimer.Stop();
		
		if (timePassed < 30)
			_spawnTimer.WaitTime = 9 * MOVE_TIMER_TICK;
		else if (timePassed < 50)
			_spawnTimer.WaitTime = 7 * MOVE_TIMER_TICK;
		else
			_spawnTimer.WaitTime = 5 * MOVE_TIMER_TICK;
		
		_spawnTimer.Start();
	}

	public void ConfigureMoveTimer()
	{
		int timePassed = FinkiTetrisPlayingGame.GAME_END_TIME - PLAYING_TIME_LEFT;
		
		MoveTimer.Stop();
		
		if (timePassed < 30)
			MoveTimer.WaitTime = 1/2 * MOVE_TIMER_TICK;
		else 
			MoveTimer.WaitTime =  MOVE_TIMER_TICK * 1/2 ;
	}

	public void OnTimeTick()
	{
		PLAYING_TIME_LEFT--;
		_scene.TimeLabel.Text = $"Time:{PLAYING_TIME_LEFT /60}:{PLAYING_TIME_LEFT % 60}";

		if (PLAYING_TIME_LEFT == 0)
			EmitSignal(SignalName.TimeFinished); 
	}

	public void StopMovement()
	{
		IsChecking = true;
		_timerTracker.SetPaused(true);
		_spawnTimer.SetPaused(true);
		MoveTimer.SetPaused(true);
	}

	public void ResumeMovement()
	{
		IsChecking = false;
		_timerTracker.SetPaused(false);
		_spawnTimer.SetPaused(false);
		MoveTimer.SetPaused(false);
	}

	
	public async void StartOpacityAnimation(List<Block> forRemove)
	{
		IsChecking = true;
		foreach (var block in forRemove)
		{
			block.Rect.Modulate = new Color(1, 0, 0);
		}
		
		await ToSignal(GetTree().CreateTimer(ANIMATION_DISSAPEAR_MS), "timeout");
		
		foreach (var block in forRemove)
			block.Remove();
	}


	public async Task StartFallDownAnimation(Block[,] blocks)
	{
		IsChecking = true; 
		await ToSignal(GetTree().CreateTimer(ANIMATION_DISSAPEAR_MS), "timeout");

		bool hasSomethingToMove = true;

		while (hasSomethingToMove)
		{
			hasSomethingToMove = false;

			for (int i = FinkiTetrisPlayingGame.Dimensions.Item1 - 1; i > -1; i--)
			{
				for (int j = 0; j < FinkiTetrisPlayingGame.Dimensions.Item2; j++)
				{
					if (blocks[i, j] is not null && blocks[i, j].Parent.IsStopped && blocks[i,j].FallOneTile())
					{
						hasSomethingToMove = true;
					}
				}
			}
			
			if(hasSomethingToMove)
				await ToSignal(GetTree().CreateTimer(FALL_DOWN_TIMER), "timeout");
		}

		await ToSignal(GetTree().CreateTimer(FALL_DOWN_TIMER), "timeout");
		
		_scene.CheckForFullRows();
	}
}
