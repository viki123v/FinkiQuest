using System;
using System.Collections.ObjectModel;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections.randomGenerationStrategies;

public interface IPickers
{
    Object Generate(); 
    void SetValues(Collection<object> values);
}