using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Godot;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections.randomGenerationStrategies;

public partial class FinkiTetrisPlayingGame : Node2D
{
	private static int Step = 32;
	private static int BlockSize = 32; 
	
	private Vector2I _gameWindowSize = new Vector2I(650, 704);
	private Vector2I _defaultGameSize = new Vector2I(1152, 648);

	private IPickers<int> _rotationStepPicker; 
	private IPickers<Texture2D> _colorPicker;
	private IPickers<PackedScene> _blockPicker;

	private BlockWrapper _nextBlock = null; 
	private Queue<BlockWrapper> _falling = new Queue<BlockWrapper>();
	private Timer _spawnTimer;
	private ColorRect _nextBlockWindow;
	private float _middle;

	
	public override void _Ready()
	{
		//NOTE: default window size (1152,648) 
		DisplayServer.WindowSetSize(_gameWindowSize);
		
		_colorPicker = DefaultConstructionMethods.CraeteDefaultColorPicker();
		_blockPicker = DefaultConstructionMethods.CreateDefaultBlockTypePicker();
		_rotationStepPicker = DefaultConstructionMethods.CreateDefaultRotationStepPicker();
		_spawnTimer = GetNode<Timer>("SpawnTimer");
		_nextBlockWindow = GetNode<ColorRect>("NextBlockBorder"); 
		_middle = GetNode<ColorRect>("SpawingRectangle").Size.X / 2;

		_nextBlock = InsertBlock();
		_nextBlockWindow.AddChild(_nextBlock.Block);

		
		InsertBlock();
	}

	public void OnSpawnTimerEnd()
	{
		if(_nextBlockWindow.GetChildren().Count > 0)
			_nextBlockWindow.RemoveChild(_nextBlock!.Block);
		
		_nextBlock!.Rotate(_rotationStepPicker.Generate());
		_nextBlock.SetStartPosition(new Vector2(_middle,0));
		
		AddChild(_nextBlock!.Block);
		_falling.Enqueue(_nextBlock);
		
		_nextBlock = InsertBlock();
		_nextBlockWindow.AddChild(_nextBlock!.Block);
	}

	private void MoveBlock(CharacterBody2D body2D, Vector2 move, bool canMove)
	{
		GD.Print(canMove);
		if(canMove)
			body2D.MoveAndCollide(move);
	}

	public void OnMoveTimer()
	{
		if(_falling.Count == 0) return; 
		
		foreach (var wrapperNode in _falling)
		{
			MoveBlock((CharacterBody2D)wrapperNode.Block, new Vector2(0,Step), true);
		}
	}

	private BlockWrapper InsertBlock()
	{
		var colorPick = _colorPicker.Generate(); 
		var block = (Node2D) _blockPicker.Generate().Instantiate();

		foreach (var node in block.GetChildren())
			if (node is TextureRect)
				((TextureRect)node).Texture = colorPick;
		
		return new BlockWrapper(block); 
	}

	public override void _Input(InputEvent @event)
	{
		if (_falling.Count == 0 ) return; 
		
		var first = _falling.Peek();
		var firstBlock = first.Block as CharacterBody2D;
		
		if (@event.IsActionPressed("Tetris_MoveBlockLeft"))
		{
			MoveBlock(firstBlock, new Vector2(-Step,0), first.CanLeft);
		}else if (@event.IsActionPressed("Tetris_MoveBlockRight"))
		{
			MoveBlock(firstBlock, new Vector2(Step,0), first.CanRight);
			
		}else if (@event.IsActionPressed("Tetris_MoveBlockDown"))
		{
			MoveBlock(firstBlock,new Vector2(0,Step), first.CanDown);
		}else if (@event.IsActionPressed("Tetris_RotateBlock"))
		{
			_falling.Peek().Rotate();
		}
	}

	public void OnLeftBorderCollisision(Node2D _)
	{
		_falling.Peek().CanLeft = false;
	}
	
	public void OnLeftBorderLeft(Node2D _)
	{
		_falling.Peek().CanLeft = true;
	}
	
	public void OnRightBorder(Node2D _)
	{
		_falling.Peek().CanRight = false;
	}

	public void OnRightBorderLeft(Node2D _)
	{
		_falling.Peek().CanRight = true;
	}
	
	public void OnBlockBottomCollision(Node2D _)
	{
		_falling.Dequeue(); 
	}
}
