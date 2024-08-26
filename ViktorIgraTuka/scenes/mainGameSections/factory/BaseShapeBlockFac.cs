using System;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections.randomGenerationStrategies;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections;

public class BaseShapeBlockFac : IBlockFactory
{
    private Vector2I _spawnPosition;
    private IPickers _colorPicker;

    public static T Load<T>(string path, T type) =>
        ResourceLoader.Load<T>(path); 
    
    public BaseShapeBlockFac(Vector2I spawnPosition)
    {
        _spawnPosition = spawnPosition; 
        _colorPicker = new EqualPropabilityPicker();
        _colorPicker.SetValues(new Collection<object>(ImmutableList<object>.Empty));
    }
    
    private Node2D CreateSkew()
    {
        throw new NotImplementedException(); 
    }

    private Node2D CreateLine()
    {
        throw new NotImplementedException(); 
    }

    private Node2D CreateTShape()
    {
        throw new NotImplementedException(); 
    }

    public Node2D CreateSquare()
    {
        throw new NotImplementedException(); 
    }

    public Node2D CreateBlock()
    {
        throw new NotImplementedException();
    }
}