using System;
using System.Collections.Generic;
using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code.Util;

public static class MobSpawner
{
    private const float BaseSpawnRate = 1.3f;

    private static readonly Vector2[] MobSpawnPoints = new Vector2[4];
    private static readonly Random Rng = new();
    private static readonly List<string> MobSceneNames = new(){"mob_orc","mob_zombie","mob_knight"};
    
    public static float CalcDecrementValue(int targetWave, float targetSpawnRate)
    {
        return (BaseSpawnRate - targetSpawnRate) / (targetWave * Map.WaveTime);
    }
    
    public static Vector2 GetMobSpawnPosition(int width, int height)
    {
        
        MobSpawnPoints[0] = new Vector2(Rng.Next(width + 200, width + 300), Rng.Next( height - 150,height)); // gore desno
        MobSpawnPoints[1] = new Vector2(Rng.Next(200, 300), Rng.Next(height - 200,height)); // gore levo
        MobSpawnPoints[2] = new Vector2(Rng.Next(200, 300), Rng.Next(-height, -height + 250)); // dolu levo
        MobSpawnPoints[3] = new Vector2(Rng.Next(200,300), Rng.Next(-height, -height + 250));// dolu desno
			
        return MobSpawnPoints[Rng.Next(MobSpawnPoints.Length)];
    }

    public static PackedScene GetMobScene(int idx)
    {
        return GD.Load<PackedScene>(ProjectPath.ScenesPath + MobSceneNames[idx] +  ".tscn");
    }
    
    

}