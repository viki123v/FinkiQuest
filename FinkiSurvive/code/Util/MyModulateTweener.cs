using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code.Util;

[GlobalClass]
public partial class MyModulateTweener : Node2D
{
    [Export] private float startVal = 0;
    [Export] private float endVal = 1;
    [Export] private float durationSecs = 1;
    [Export] private float tickRateSecs = 0.05f;

    private double _modulateAmountPerTick;
    private int ascend = 1;
    
    public override void _Ready()
    {
	    _modulateAmountPerTick = GetModulateAmount(startVal, endVal, durationSecs, tickRateSecs);
	    Timer timer = new Timer();
	    timer.Timeout += ModulateAllChildren;
	    timer.WaitTime = tickRateSecs;
	    GetTree().GetRoot().CallDeferred("add_child",timer);
	    timer.Autostart = true;
    }

    private void ModulateAllChildren()
    {
	    foreach (var node in GetChildren())
	    {
			 ModulateAlpha((CanvasItem)node);
	    }
    }

    public void ModulateAlpha(CanvasItem node)
	{

		var modulate = node.Modulate;
		
		if (modulate.A + _modulateAmountPerTick * ascend >= endVal)
		{
			modulate.A = endVal;
			ascend = -1;

		}
		else if (modulate.A + _modulateAmountPerTick * ascend <= startVal)
		{
			modulate.A = startVal;
			ascend = 1;
		}

		modulate.A += (float) _modulateAmountPerTick * ascend;
		node.Modulate = modulate;
		GD.Print(modulate);
	}
    
    
    
    // X = modulate amount per tick interval
    // startModulateVal + (time / tickrate) * X = endModulateVal
    // n = time / tickrate
    // startModulateVal + nX  = endModulateVal
    // nX = endModulateVal - startModulateVal
    // X = (endModulateVal - startModulateVal) / n
    // X = (endModulateVal - startModulateVal) / (time / tickrate)
    // *** X = ((endModulateVal - startModulateVal) * tickrate) / time ***
    
    public double GetModulateAmount(float startValue, float endValue, float durationSeconds, float tickRateSeconds)
    {
        return ((endValue - startValue) * tickRateSeconds) / durationSeconds;
    }
    
}