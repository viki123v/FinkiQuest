namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.GradeStrategies;

public class GraderOnTimePassed : IGrader
{
    private float[] _bounds; 
    
    public GraderOnTimePassed(int fullTime )
    {
        _bounds = new float[6];
        float middle = ((float)fullTime) / 2;
        _bounds[0] = middle;

        float deltaIncr = middle / 5;

        for (int i = 1; i < 6; i++)
            _bounds[i] = i * deltaIncr + middle;
    }
    
    public int GetGrade(float finishedOnTime)
    {
        int i;
        float timePassedInSec = _bounds[_bounds.Length - 1] - finishedOnTime; 
        for (i = 0; i < 5 && _bounds[i] < timePassedInSec; i++);
        return i + 5;
    }
}