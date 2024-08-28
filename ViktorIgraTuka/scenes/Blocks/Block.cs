using System.Linq;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.util;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.Blocks;

public class Block
{
    private IBlockInformation _blockInformation; 
    private int _currentDirection;
    private ColorRect[] _blocks;

    public Block(IBlockInformation blockInformation, Vector2 spawnPosition, int currentDirection = 0)
    {
        _blockInformation = blockInformation;
        
         var tmpPivotPoint = BlockTransition.Create(spawnPosition);
        tmpPivotPoint.Position -= (new Vector2(0, FinkiTetrisPlayingGame.BlockSize));

        _currentDirection = currentDirection;

        _blocks = BlockTransition.FindBlocksPosition(tmpPivotPoint.Position, _blockInformation.GetBlockDistancesFromPivot(), _blockInformation.GetRotationVecs()[_currentDirection])
            .Select(pos => BlockTransition.Create(pos)).ToArray();
    }

    public void Attach(Node2D parent)
    {
        foreach (var block in _blocks)
        {
            parent.AddChild(block);
        }
    }

    public void Move(Vector2 directionVec)
    {
        foreach (var block in _blocks)
        {
            block.Position += directionVec;
        }
    }

    public void Rotate()
    {
        _currentDirection = (_currentDirection + 1) % _blockInformation.GetRotationVecs().Length;

        Vector2[] newPositions =
            BlockTransition.FindBlocksPosition(_blocks[0].Position, _blockInformation.GetBlockDistancesFromPivot(), _blockInformation.GetRotationVecs()[_currentDirection]);

        for (int i = 0; i < newPositions.Length; i++)
        {
            _blocks[i].Position = newPositions[i];
        }
    }
    
}