using System;
using System.Collections.ObjectModel;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections.randomGenerationStrategies;

public class EqualPropabilityPicker<T> : IPickers<T>
{
    private Collection<T> _collection;
    private Random _random = new Random();

    public T Generate()
        => _collection[_random.Next(_collection.Count)]; 

    public void SetValues(Collection<T> values)
        => _collection = values; 
}