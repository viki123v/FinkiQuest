using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.Blocks;

public interface IBlockInformation
{
    (Vector2, Vector2)[] GetRotationVecs(); 
    Vector2[] GetBlockDistancesFromPivot(); 
}