using System;
using System.Collections;
using System.Linq;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.util;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.Blocks;

public class Block
{
    private IBlockInformation _blockInformation;
    private int _currentDirection;
    private ColorRect[] _blocks;
    private Vector2[] _bitmapPositions;

    private static byte[,] _sharedBitMap = null;


    public Block(IBlockInformation blockInformation, Vector2 spawnPosition, int currentDirection = 0)
    {
        if (_sharedBitMap is null)
            throw new Exception("The bitmap isn't set");

        _blockInformation = blockInformation;

        var pivotPosition = PoistionToBitMapIndex(spawnPosition);
        _currentDirection = currentDirection;

        _bitmapPositions = BlockTransition.FindBlocksPosition(pivotPosition,
            _blockInformation.GetBlockDistancesFromPivot(), _blockInformation.GetRotationVecs()[_currentDirection]);


        var blockSize = FinkiTetrisPlayingGame.BlockSize;
        _blocks = _bitmapPositions.Select(pos =>
        {
            var x = blockSize + pos.X * blockSize;
            var y = blockSize + pos.Y * blockSize;

            return BlockTransition.Create(new Vector2(x, y));
        }).ToArray();
        
        SetPositionsToValue(_bitmapPositions,1);
    }

    public static void SetBitMap((int, int) dimensions)
    {
        _sharedBitMap = new byte[dimensions.Item1, dimensions.Item2];
    }

    public void Attach(Node2D parent)
    {
        foreach (var block in _blocks)
        {
            parent.AddChild(block);
        }
    }

    private Vector2 PoistionToBitMapIndex(Vector2 positon)
    {
        var blockSize = FinkiTetrisPlayingGame.BlockSize;
        positon.X -= blockSize;
        positon.Y -= blockSize;

        int x = (int)positon.X / blockSize;
        int y = (int)positon.Y / blockSize;

        return new Vector2(x, y);
    }

    private void SetPositionsToValue(Vector2[] positions, byte val)
    {
        foreach (var pos in positions)
        {
            var x = (int)pos.X;
            var y = (int)pos.Y;
            _sharedBitMap[x, y] = val;
        }
    }

    private bool CanMove(Vector2[] positions)
    {
        var columnNum = _sharedBitMap.Length / _sharedBitMap.GetLength(0);
        foreach (var pos in positions)
        {
            var x = (int)pos.X;
            var y = (int)pos.Y;
            if (x < 0 || x >= _sharedBitMap.GetLength(0) || y >= columnNum || _sharedBitMap[x, y] == 1)
                return false;
        }

        return true;
    }

    public void Move(Vector2 directionVec)
    {
        Vector2[] newPositions = _bitmapPositions.Select(pos => pos + directionVec).ToArray();
        var blockSize = FinkiTetrisPlayingGame.BlockSize;

        SetPositionsToValue(_bitmapPositions, 0);
        if (CanMove(newPositions))
        {
            _bitmapPositions = newPositions;
            for (int i = 0; i < _blocks.Length; i++)
            {
                _blocks[i].Position = ScaleBitmap(_bitmapPositions[i], blockSize);
            }
        }

        SetPositionsToValue(_bitmapPositions, 1);
    }

    private Vector2 ScaleBitmap(Vector2 bitMapCoordinated, int blockSize)
    {
        var xPos = bitMapCoordinated.X * blockSize + blockSize;
        var yPos = bitMapCoordinated.Y * blockSize + blockSize;
        return new Vector2(xPos, yPos);
    }

    public void Rotate()
    {
        _currentDirection = (_currentDirection + 1) % _blockInformation.GetRotationVecs().Length;

        Vector2[] newPositions =
            BlockTransition.FindBlocksPosition(_bitmapPositions[0], _blockInformation.GetBlockDistancesFromPivot(),
                _blockInformation.GetRotationVecs()[_currentDirection]);
        var blockSize = FinkiTetrisPlayingGame.BlockSize;

        SetPositionsToValue(_bitmapPositions, 0);
        if (CanMove(newPositions))
        {
            _bitmapPositions = newPositions;
            for (int i = 0; i < newPositions.Length; i++)
            {
                _blocks[i].Position = ScaleBitmap(_bitmapPositions[i], blockSize);
            }
        }

        SetPositionsToValue(_bitmapPositions, 1);
    }
}