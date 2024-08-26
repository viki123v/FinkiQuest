using System;
using System.Collections.ObjectModel;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections.randomGenerationStrategies;

public class EqualPropabilityPicker : IPickers
{
    private Collection<object> _collection;
    private Random _random = new Random();

    public object Generate()
        => _collection[_random.Next(_collection.Count)]; 

    public void SetValues(Collection<object> values)
        => _collection = values; 
}