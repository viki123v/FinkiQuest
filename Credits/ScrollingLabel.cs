using Godot;
using System;

public partial class ScrollingLabel : Godot.Label
{
	private string creditsText = 
    "\n\nFINKI Quest\n" +
    "\n" +
    "Created by:\n" +
    "Martin Djakov\n" +
    "Stefan Toskovski\n" +
    "Viktor Hristovski\n" +
    "\n" +
	"Mentor\n" +
	"Stefan Andonov\n\n" +
    "Music by\nStefan Toskovski\n\n" +
    "Assets stolen from the internet\n" +
    "\n" +
    "Special thanks to:\n" +
    "YouTube tutorials\n" +
    "ChatGPT\n" +
    "\n" +
    "Any resemblance to professional game development is purely coincidental.";

    
    public override void _Ready()
    {
        ScrollText(creditsText);
    }

    public override void _Input(InputEvent @event)
    {
        
        if(@event.IsActionPressed("ui_cancel") || @event.IsActionPressed("ui_accept"))
        {
            
            GetTree().ChangeSceneToFile("res://MainScene/main_menu.tscn");
        }
    }

    private async void ScrollText(string inputText)
    {
        int visibleCharacters = 0;
        Text = inputText;

        for (int i = 0; i < inputText.Length; i++)
        {
            visibleCharacters++;
            VisibleCharacters = visibleCharacters;
            await ToSignal(GetTree().CreateTimer(0.075f), "timeout");
        }
		
		
		// await ToSignal(GetTree().CreateTimer(4.0f), "timeout");
        // GetTree().ChangeSceneToFile("res://MainScene/main_menu.tscn");
    }
}
