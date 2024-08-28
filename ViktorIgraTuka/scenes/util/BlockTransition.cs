using System.Linq;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.util;

public class BlockTransition
{
    public static Vector2[] FindBlocksPosition(Vector2 pivotPosition, Vector2[] fromPivotToBlcok,
        (Vector2, Vector2) direction)
    {
        Vector2[] newPositions = new Vector2[fromPivotToBlcok.Length];
        newPositions[0] = pivotPosition;

        var blockSize = FinkiTetrisPlayingGame.BlockSize;

        for (int i = 1; i < fromPivotToBlcok.Length; i++)
        {
            var newPosition = new Vector2(pivotPosition.X, pivotPosition.Y);

            var currentDisFromPivot = fromPivotToBlcok[i];

            var xAxis = direction.Item1;
            var yAxis = direction.Item2;

            var xAxisTranslation = new Vector2(currentDisFromPivot.X * xAxis.X, currentDisFromPivot.X * xAxis.Y);
            var yAxisTranslation = new Vector2(currentDisFromPivot.Y * yAxis.X, currentDisFromPivot.Y * yAxis.Y);
            var all = xAxisTranslation + yAxisTranslation;

            newPosition.X += blockSize * all.X;
            newPosition.Y += blockSize * all.Y;

            newPositions[i] = newPosition;
        }

        return newPositions;
    }

    public static Vector2[] MoveBlocks(ColorRect[] nodePositions, Vector2 direction)
        => nodePositions.Select(node => node.Position).Select(pos => pos + direction).ToArray();

    public static ColorRect Create(Vector2 position)
    {
        ColorRect colorRect = new ColorRect();

        colorRect.Color = Colors.Black;
        colorRect.Size = new Vector2(32, 32);
        colorRect.Position = position;

        return colorRect;
    }
}