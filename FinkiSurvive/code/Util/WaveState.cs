using System;

namespace FinkiAdventureQuest.FinkiSurvive.code.Util;

public abstract class WaveState
{
    public static Random rng = new();
    public abstract int GetMobSceneIndex();
    public static WaveState GetWaveState(int wave)
    {
        return wave switch
        {
            >= 1 and < 3 => new StartWaveState(),
            >= 3 and < 5 => new ZombieWaveState(),
            >= 5 => new KnightWaveState(),
            _ => null
        };
    }
}