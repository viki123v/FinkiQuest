namespace FinkiAdventureQuest.FinkiSurvive.code.Util;

public class ZombieWaveState : WaveState
{
    public override int GetMobSceneIndex()
    {
        return rng.NextDouble() < 0.31 ? 1 : 0; // 30% шанса да се појави ZombieMob, 70% OrcMob
    }
}