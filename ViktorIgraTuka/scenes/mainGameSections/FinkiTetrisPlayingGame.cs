using FinkiAdventureQuest.ViktorIgraTuka.scenes.Blocks;
using Godot;

public partial class FinkiTetrisPlayingGame : Node2D
{
	public static int Step = 32;
	public static int BlockSize = 32; 
	
	private Vector2I _gameWindowSize = new Vector2I(650, 704);
	private Vector2I _defaultGameSize = new Vector2I(1152, 648);
	
	private Block _block; 
	
	public override void _Ready()
	{
		//NOTE: default window size (1152,648) 
		DisplayServer.WindowSetSize(_gameWindowSize);

		_block = new Block(new LBlock(), new Vector2(5 * 32, 5 * 32)); 
		_block.Attach(this);
	}

	public override void _Input(InputEvent @event)
	{
		var inFocus = _block; 
		if(@event.IsActionPressed("Tetris_RotateBlock"))
			inFocus.Rotate();
		else if(@event.IsActionPressed("Tetris_MoveBlockLeft"))
			inFocus.Move(Vector2.Left);
	}
}
