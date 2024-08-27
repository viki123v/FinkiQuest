using System;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections;

public class BlockWrapper
{
    private static float _stepDegree = 90;
    private static int _maxStep = 4;

    public bool CanDown { get; set; } = true;
    public bool CanLeft { get; set; }= true;
    public bool CanRight { get; set; } = true;
    
    
    private int _step =0 ;
    
    public Node2D Block;

    public BlockWrapper(Node2D node2D)
    {
        Block = node2D;
        Rotate();
    }

    public void Rotate()
    {
        Block.RotationDegrees = _step * _stepDegree;
        _step = (_step + 1) % _maxStep; 
    }
    
    public void Rotate(int step)
    {
        _step = step;
        Rotate(); 
    }

    public void SetStartPosition(Vector2 startPosition)
    {
        Block.Position = startPosition;
    }
    
}