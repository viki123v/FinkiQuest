using System.Linq;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.util;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.Blocks;

public class TBlock : IBlockInformation 
{
    private (Vector2, Vector2)[] _rotationVecs;
    private Vector2[] _blocksDistanceFromPivot; 
    
    public TBlock()
    {
        _rotationVecs= new[]
        {
            (new Vector2(0, -1), new Vector2(1, 0)),
            (new Vector2(1, 0), new Vector2(0, 1)),
            (new Vector2(0, 1), new Vector2(-1, 0)),
            (new Vector2(-1, 0), new Vector2(0, -1))
        };
        _blocksDistanceFromPivot = new[]
        {
            new Vector2(0, 0),
            new Vector2(0, 1),
            new Vector2(1, 1),
            new Vector2(-1, 1)
        };
        
    }

    public (Vector2, Vector2)[] GetRotationVecs()
    {
        return _rotationVecs;
    }

    public Vector2[] GetBlockDistancesFromPivot()
    {
        return _blocksDistanceFromPivot; 
    }
}