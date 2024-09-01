using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code;

public partial class DashProgress : TextureProgressBar
{
	private Timer _cooldownTimer;
	private bool _cooldownTimerStarted = false;
	private Timer _valueTimer;
	private bool _valueTimerStarted = false;
    
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_valueTimer = new Timer();
		_valueTimer.WaitTime = 0.01f;
		_valueTimer.OneShot = false;
		_valueTimer.Timeout += UpdateProgressBarValue;
		AddChild(_valueTimer);
		_cooldownTimer = GetNode<Timer>("/root/Level/Player/DashCooldown");
		GetNode<Player>("/root/Level/Player").Connect(nameof(Player.DashTimerStarted), new Callable(this, nameof(UpdateDashIcon)));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_cooldownTimer.TimeLeft <= 0)
		{
			_cooldownTimerStarted = false;
			_valueTimer.Stop();
		}
		
		Value = !_cooldownTimerStarted ? 150 : Value;
		
	}

	private void UpdateProgressBarValue()
	{
		Value++;
	}

	private void UpdateDashIcon()
	{
		_cooldownTimerStarted = true;
		Value = 0;
		_valueTimer.Start();
	}
}