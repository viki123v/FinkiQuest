using System;
using System.Linq;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes;

public class NodeMapRepository
{
    private static NodeMapRepository? Repository = null;
    private static (int,int)? Size = null; 
    
    private Node2D[,] _mapOfNodes;
    
    private NodeMapRepository()
    {
        var checkedSize = Size.Value;
        _mapOfNodes = new Node2D[checkedSize.Item1,checkedSize.Item2]; 
    }

    public bool CanMove(Vector2[] newPositions, Node2D[] listOfNodes)
    {
        if (newPositions.Length != listOfNodes.Length)
            throw new Exception("The given positions and nodes are not equal in size"); 
        
        var size = FinkiTetrisPlayingGame.BlockSize;
        (int, int)[] blockPositionTranslations = newPositions.Select(pos => TranslatePositionToBlock(pos,size)).ToArray();

        foreach (var block in blockPositionTranslations)
        {
            var x = block.Item1;
            var y = block.Item2; 
            
            if(!(BoundCheck(x,_mapOfNodes.Length) && BoundCheck(y,_mapOfNodes.GetLength(0)) || _mapOfNodes[x,y] is not null))
                return false; 
        }
        
        InsertAndRemove(blockPositionTranslations,listOfNodes,size);
        return true;
    }

    private (int, int) TranslatePositionToBlock(Vector2 pos, int size)
    {
        var x = (int)(pos.X - size) / size;
        var y = (int)pos.Y / size;
        return (x, y); 
    }
    
    private bool BoundCheck(int x, int bound)
    {
        return x > 0 && x < bound;
    }
    
    private void InsertAndRemove((int,int)[] newPoistions, Node2D[] listOfNodes, int size)
    {
        for (int i = 0; i < listOfNodes.Length; i++)
        {
            var pos = newPoistions[i];
            var oldPosition = TranslatePositionToBlock(listOfNodes[i].Position, size); 
            
            _mapOfNodes[pos.Item1, pos.Item2] = listOfNodes[i];
            _mapOfNodes[oldPosition.Item1, oldPosition.Item2] = null; 
            
            listOfNodes[i].Position = new Vector2(pos.Item1,pos.Item2); 
        }
    }

    public static NodeMapRepository GetInstance((int,int) size)
    {
        if (Size is null)
            Size = size;
        return GetInstance(); 
    }
    
    public static NodeMapRepository GetInstance(int size)
    {
        if (Size is null)
            Size = (size,size);
        return GetInstance(); 
    }
    
    public static NodeMapRepository GetInstance()
    {
        if (Size is null)
            throw new Exception("The size of the node map hasn't been set");
        if (Repository is null)
            Repository = new NodeMapRepository();
        return Repository; 
    }
}