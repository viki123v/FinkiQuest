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
		DisplayServer.WindowSetSize(_gameWindowSize);
		Block.SetBitMap(CountAvailableSpotsForBlcoks());
				
		_block = new Block(new TBlock(), new Vector2(5 * 32, 5 * 32)); 
		_block.Attach(this);

		new Block(new Square(), new Vector2(3 * 32, 4 * 32)).Attach(this); 
	}

	private (int,int) CountAvailableSpotsForBlcoks()
	{
		var spawningSurface = GetNode<ColorRect>("SpawingRectangle");
		
		int x = (int)spawningSurface.Size.X - 2 * BlockSize;
		int y = (int)spawningSurface.Size.Y - BlockSize;
		
		x /= BlockSize;
		y /= BlockSize;
		
		return (x, y); 
	}

	public override void _Input(InputEvent @event)
	{
		var inFocus = _block; 
		
		if(@event.IsActionPressed("Tetris_RotateBlock"))
			inFocus.Rotate();
		else if(@event.IsActionPressed("Tetris_MoveBlockLeft"))
			inFocus.Move(Vector2.Left);
		else if(@event.IsActionPressed("Tetris_MoveBlockRight"))
			inFocus.Move(Vector2.Right);
		else if(@event.IsActionPressed("Tetris_MoveBlockDown"))
			inFocus.Move(Vector2.Down);
	}
}
