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
            1 => new StartWaveState(),
            > 1 and < 4 => new ZombieWaveState(),
            >= 4 => new KnightWaveState(),
            _ => null
        };
    }
}