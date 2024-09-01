namespace FinkiAdventureQuest.FinkiSurvive.code.WaveStates;

public class ZombieWaveState : WaveStates.WaveState
{
    public override int GetMobSceneIndex()
    {
        return rng.NextDouble() < 0.31 ? 1 : 0; // 30% шанса да се појави ZombieMob, 70% OrcMob
    }
}