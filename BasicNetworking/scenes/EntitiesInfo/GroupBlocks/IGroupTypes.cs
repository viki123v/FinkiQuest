using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo.GroupBlocks;

public interface IGroupTypes
{
    IGroupTypes RandomStart(int val); 
    (Vector2, Vector2) GetRotationVec(); 
    Vector2[] GetBlockDistancesFromPivot();
    Vector2 GetNextWindowIdealSpawing();
}