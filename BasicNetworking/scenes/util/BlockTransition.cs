using System.Collections.Generic;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.util;

public class BlockTransition
{
    public static List<Vector2> FindBlocksBitmapPosition(Vector2 pivotPosition, Vector2[] fromPivotToBlcok,
        (Vector2, Vector2) direction)
    {
        List<Vector2> newPositions = new List<Vector2>(fromPivotToBlcok.Length); 
        newPositions.Add(pivotPosition);

        for (int i = 1; i < fromPivotToBlcok.Length; i++)
        {
            var newPosition = new Vector2(pivotPosition.X, pivotPosition.Y);

            var currentDisFromPivot = fromPivotToBlcok[i];

            var xAxis = direction.Item1;
            var yAxis = direction.Item2;

            var xAxisTranslation = new Vector2(currentDisFromPivot.X * xAxis.X, currentDisFromPivot.X * xAxis.Y);
            var yAxisTranslation = new Vector2(currentDisFromPivot.Y * yAxis.X, currentDisFromPivot.Y * yAxis.Y);

            newPosition.X += xAxisTranslation.X + yAxisTranslation.X;
            newPosition.Y += xAxisTranslation.Y + yAxisTranslation.Y;
            
            newPositions.Add(newPosition);
        }

        return newPositions;
    }
    
     public static TextureRect Create(Vector2 displayPosiiton,Texture2D texture)
    {
        TextureRect colorRect = new TextureRect();

        colorRect.ExpandMode = TextureRect.ExpandModeEnum.FitWidth;
        colorRect.Position = displayPosiiton;
        colorRect.Texture = texture;
        colorRect.Size = new Vector2(FinkiTetrisPlayingGame.BlockSizeX,FinkiTetrisPlayingGame.BlockSizeY);
        
        return colorRect;
    }

}


   