using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code;

public partial class AttackCooldownProgress : TextureProgressBar
{
	private Timer _cooldownTimer;
	private bool _cooldownTimerStarted = false;
	private Timer _valueTimer;
	private bool _valueTimerStarted = false;

	private Player player;
	public override void _Ready()
	{
		_valueTimer = new Timer();
		_valueTimer.WaitTime = 0.01f;
		_valueTimer.OneShot = false;
		_valueTimer.Timeout += UpdateProgressBarValue;
		AddChild(_valueTimer);
		player = GetNode<Player>("/root/Level/Player");
		_cooldownTimer = player.GetNode<Timer>("AttackSpeed");
		player.Connect(nameof(Player.AttackCooldownTimerStarted), new Callable(this, nameof(ResetIcon)));
		player.Connect(nameof(Player.AttackSwitched), new Callable(this, nameof(UpdateMaxValue)));
		MaxValue = _cooldownTimer.WaitTime * 100;
		Value = MaxValue;
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_cooldownTimer.TimeLeft <= 0)
		{
			_cooldownTimerStarted = false;
			_valueTimer.Stop();
		}
		Value = !_cooldownTimerStarted ? MaxValue : Value;
		
		GD.Print(MaxValue + "<---- max val");
	}

	public void UpdateProgressBarValue()
	{
		Value++;
	}

	public void ResetIcon()
	{
		GD.Print("StartTimer");
		_cooldownTimerStarted = true;
		Value = 0;
		_valueTimer.Start();
	}

	private void UpdateMaxValue()
	{ 
		_cooldownTimer = player.GetNode<Timer>("AttackSpeed");
		MaxValue = _cooldownTimer.WaitTime * 100;
	}
}