using System;

namespace FinkiAdventureQuest.FinkiSurvive.code.Util;

public class MobSpawner
{
    public static float BaseSpawnRate = 1.3f;
    public static float CalcDecrementValue(int targetWave, float targetSpawnRate)
    {
        float decrement = (BaseSpawnRate - targetSpawnRate) / (targetWave * Map.WaveTime);
        Console.WriteLine("decrement");
        return decrement;
    }

}