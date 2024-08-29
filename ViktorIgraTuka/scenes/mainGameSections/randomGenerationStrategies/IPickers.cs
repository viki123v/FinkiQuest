using System;
using System.Collections.ObjectModel;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections.randomGenerationStrategies;

public interface IPickers<T>
{
    T Generate(); 
    void SetValues(Collection<T> values);
}