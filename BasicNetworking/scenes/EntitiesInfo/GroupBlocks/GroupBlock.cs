using System;
using System.Collections.Generic;
using System.Linq;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.util;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo.GroupBlocks;

public class GroupBlock
{
    private IGroupTypes _typeOfGroup;
    public List<Block> _blocks;
    private FinkiTetrisPlayingGame _scene;

    public bool IsStopped { get; set; }
    public Block?[,] SharedBitMap { get; set; }

    public GroupBlock(IGroupTypes typeOfGroup, Vector2 spawnPosition, Texture2D newTexture)
    {
        _typeOfGroup = typeOfGroup;
        _blocks = GenerateBitMapPositions(spawnPosition).Select(bitMapPos => new Block(bitMapPos, this, newTexture))
            .ToList(); 
    }
    
    private List<Vector2> GenerateBitMapPositions(Vector2 spawnPosition)
    {
        var pivotPosition = ScallingUtil.FromDisplayToBitmap(spawnPosition, FinkiTetrisPlayingGame.BlockSizeX,
            FinkiTetrisPlayingGame.BlockSizeY);

        return BlockTransition.FindBlocksBitmapPosition(pivotPosition,
            _typeOfGroup.GetBlockDistancesFromPivot(), _typeOfGroup.GetRotationVec());
    }

    public void AttachAsGroup(FinkiTetrisPlayingGame parent, Vector2 spawnPositionBitMap, int randomRotationStart)
    {
        SharedBitMap = parent.BitMap;
        _scene = parent;

        _typeOfGroup.RandomStart(randomRotationStart);
        var newBitMapPos = GenerateBitMapPositions(spawnPositionBitMap);

        float xNegative = 0;
        float yNegative = 0;

        foreach (var pos in newBitMapPos)
        {
            xNegative = Math.Min(pos.X, xNegative);
            yNegative = Math.Min(pos.Y, yNegative);
        }

        if (xNegative < 0 || yNegative < 0)
        {
            for (int i = 0; i < newBitMapPos.Count; i++)
            {
                newBitMapPos[i] = new Vector2(newBitMapPos[i].X + -1 * xNegative, newBitMapPos[i].Y + -1 * yNegative);
            }
        }

        for (int i = 0; i < _blocks.Count; i++)
        {
            _blocks[i].AttachSelf(parent, newBitMapPos[i]);
        }
    }

    public void AttachAsGroup(Node2D parent)
    {
        foreach (var block in _blocks)
        {
            block.AttachSelf(parent);
        }
    }

    private bool CanMove(List<Vector2> positions, bool isDownMove)
    {
        var rowNum = FinkiTetrisPlayingGame.Dimensions.Item1;
        var columnNum = FinkiTetrisPlayingGame.Dimensions.Item2;

        foreach (var pos in positions)
        {
            var x = (int)pos.X;
            var y = (int)pos.Y;

            if (x >= rowNum && !isDownMove)
            {
                return false;
            }

            if (x >= rowNum && isDownMove)
            {
                IsStopped = true;
                UpdateCount();
                return false;
            }

            if (y < 0 || y >= columnNum)
            {
                return false;
            }

            if (SharedBitMap[x, y] is not null && SharedBitMap[x, y].Parent != this)
            {
                if (isDownMove)
                {
                    IsStopped = true;
                    UpdateCount();
                }

                return false;
            }
        }

        return true;
    }

    public void MoveAsGroup(Vector2 directionVec)
    {
        if (IsStopped || (directionVec.X == 0 && directionVec.Y == 0))
            return;

        Vector2 scalledToBitmap = new Vector2(directionVec.Y, directionVec.X);
        List<Vector2> newPositions = _blocks.Select(pos => pos.BitMapPosition + scalledToBitmap).ToList();

        if (CanMove(newPositions, directionVec.Equals(Vector2.Down)))
        {
            for (int i = 0; i < newPositions.Count; i++)
            {
                _blocks[i].SetPosition(newPositions[i]);
            }
        }
    }

    public void RotateAsGroup()
    {
        if (IsStopped)
            return;

        var currentDir = _typeOfGroup.GetRotationVec();

        List<Vector2> newBitMapPositions =
            BlockTransition.FindBlocksBitmapPosition(_blocks[0].BitMapPosition,
                _typeOfGroup.GetBlockDistancesFromPivot(), currentDir);

        if (CanMove(newBitMapPositions, false))
        {
            for (int i = 0; i < newBitMapPositions.Count; i++)
            {
                _blocks[i].SetPosition(newBitMapPositions[i]);
            }
        }
    }

    private void UpdateCount()
    {
        foreach (var block in _blocks)
        {
            int rowPos = (int)block.BitMapPosition.X;
            _scene.CountByRow[rowPos]++;
        }
    }
}