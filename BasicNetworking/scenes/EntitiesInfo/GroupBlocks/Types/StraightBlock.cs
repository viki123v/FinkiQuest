using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo.GroupBlocks.Types; 

public class StraightBlock : IGroupTypes
{
    private (Vector2, Vector2)[] _rotationVecs;
    private Vector2[] _blocksDistanceFromPivot;
    private int _currentRotation = 0;
    
    public StraightBlock()
    {
        _rotationVecs= new[]
            {
                (new Vector2(0, -1), new Vector2(1, 0)),
                (new Vector2(1, 0), new Vector2(0, 1))
            }
            ;
        _blocksDistanceFromPivot= new[]
        {
            new Vector2(0, 0),
            new Vector2(-1, 0),
            new Vector2(1, 0)
        };
    }

    public IGroupTypes RandomStart(int val)
    {
        _currentRotation = val % _rotationVecs.Length;
        return this; 
    }

    public (Vector2, Vector2) GetRotationVec()
    {
        var currentRotation = _rotationVecs[_currentRotation];
        _currentRotation=(_currentRotation+1)%_rotationVecs.Length;
        return currentRotation;
    }
    public Vector2[] GetBlockDistancesFromPivot()
    {
        return _blocksDistanceFromPivot; 
    }

    public Vector2 GetNextWindowIdealSpawing()
    {
        _currentRotation = 0;
        return new Vector2(0,2);
    }
}