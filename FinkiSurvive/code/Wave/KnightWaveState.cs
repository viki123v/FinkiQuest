namespace FinkiAdventureQuest.FinkiSurvive.code.WaveStates;

public class KnightWaveState : WaveStates.WaveState
{
    public override int GetMobSceneIndex()
    {
        
        if (rng.NextDouble() < 0.08) // 8% веројатност да се појави KnightMob 
        {
            return 2;
        }
				
        return rng.NextDouble() < 0.65 ? 1 : 0; // 65% веројатност да се појави ZombieMob, 35% OrcMob
        
    }
}