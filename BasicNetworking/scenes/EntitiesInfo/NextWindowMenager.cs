using System;
using System.Collections.ObjectModel;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo.GroupBlocks;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo.GroupBlocks.Types;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.mainGameSections.randomGenerationStrategies;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.util;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo;

public class NextWindowMenager
{
    private ColorRect _nextGroupWindow;
    private GroupBlock _nextGroup;
    private FinkiTetrisPlayingGame _scene;

    private IPickers<IGroupTypes> _groupTypePicker;
    private IPickers<int> _randomRotationPicker;
    private IPickers<Texture2D> _textureGen; 
    
    public NextWindowMenager(FinkiTetrisPlayingGame parent)
    {
        _nextGroupWindow = parent.GetNode<ColorRect>("CanvasLayer/Sidepanel/Border/NextBlockBorder");
        _scene = parent;

        InitGroupTypePicker();
        InitRandomRotationPicker();
        InitTextureGen();

        GenerateNextGroup();
    }

    private void GenerateNextGroup()
    {
        var typePicked = _groupTypePicker.Generate();

        var idealBitMap = typePicked.GetNextWindowIdealSpawing();
        idealBitMap = new Vector2(idealBitMap.X - 1, idealBitMap.Y - 1);

        _nextGroup = new GroupBlock(typePicked, ScallingUtil.FromBitmapToDisplay(idealBitMap,FinkiTetrisPlayingGame.BlockSizeX,FinkiTetrisPlayingGame.BlockSizeY*2), _textureGen.Generate());
        
        Node2D abstractParent = new Node2D();

        _nextGroup.AttachAsGroup(abstractParent);
        _nextGroupWindow.AddChild(abstractParent);
    }

    public GroupBlock GetNext()
    {
        _nextGroupWindow.RemoveChild(_nextGroupWindow.GetChildren()[0]);
        var enteredGroup = _nextGroup;

        var middlePoint = _scene.GetNode<ColorRect>("CanvasLayer/SpawingRectangle").Size.X / 2;
        
        enteredGroup.AttachAsGroup(_scene, new Vector2(middlePoint,0), _randomRotationPicker.Generate());
        GenerateNextGroup();
        return enteredGroup;
    }
    
    private void InitGroupTypePicker()
    {
        _groupTypePicker = new EqualPropabilityPicker<IGroupTypes>();
        _groupTypePicker.SetValues(new Collection<IGroupTypes>
        {
            new LBlock(),
            new SkewBlock(),
            new Square(),
            new StraightBlock(),
            new TBlock()
        });
    }

    private void InitRandomRotationPicker()
    {
        _randomRotationPicker = new EqualPropabilityPicker<int>();
        _randomRotationPicker.SetValues(new Collection<int>
        {
            0, 1, 2, 3, 4
        });
    }

    private Texture2D LoadTexture(string path) => ResourceLoader.Load<Texture2D>(path);

    private void InitTextureGen()
    {
        _textureGen = new EqualPropabilityPicker<Texture2D>();
        _textureGen.SetValues(new Collection<Texture2D>()
        {
            LoadTexture("res://BasicNetworking/assets/newBlockTextures/Antenna.png"),
            LoadTexture("res://BasicNetworking/assets/newBlockTextures/Router.png"),
            LoadTexture("res://BasicNetworking/assets/newBlockTextures/Satellite.png"),
            LoadTexture("res://BasicNetworking/assets/newBlockTextures/Server.png")
        });
    }
}