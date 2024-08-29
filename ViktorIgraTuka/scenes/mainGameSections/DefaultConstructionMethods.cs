using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections.randomGenerationStrategies;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections;

public class DefaultConstructionMethods
{
    public static string[] ColorPaths =
    {
        "res://ViktorIgraTuka/assets/blockTextures/blueBlock.png",
        "res://ViktorIgraTuka/assets/blockTextures/greenBlock.png",
        "res://ViktorIgraTuka/assets/blockTextures/redBlock.png",
        "res://ViktorIgraTuka/assets/blockTextures/yellowBlock.png"
    }; 
    
    public static IPickers<Texture2D> CraeteDefaultColorPicker()
    {
        IPickers<Texture2D> colorPicker = new EqualPropabilityPicker<Texture2D>();
        colorPicker.SetValues(
            new Collection<Texture2D>(
                ColorPaths.Select(path => ResourceLoader.Load<Texture2D>(path)).ToList()
            )
        );
        return colorPicker; 
    }

  

    public static IPickers<int> CreateDefaultRotationStepPicker()
    {
        IPickers<int> rotationPicker = new EqualPropabilityPicker<int>();
        rotationPicker.SetValues(new Collection<int>(new List<int>{0,1,2,3}));
        return rotationPicker; 
    }
}