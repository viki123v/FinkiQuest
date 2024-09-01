using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.util;

public class ScallingUtil
{
    public static Vector2 FromBitmapToDisplay(Vector2 bitMapCoordinated)
    {
        var blockSizeX = FinkiTetrisPlayingGame.BlockSizeX;
        var blockSizeY = FinkiTetrisPlayingGame.BlockSizeY; 
        
        float row = bitMapCoordinated.X * blockSizeY;
        float column = bitMapCoordinated.Y * blockSizeX ;

        return new Vector2(column,row); 
    }

    public static Vector2 FromBitmapToDisplay(Vector2 bitmapCoor, float marginX, float marginY)
    {
        return FromBitmapToDisplay(bitmapCoor) + new Vector2(marginX, marginY); 
    }

    public static Vector2 FromDisplayToBitmap(Vector2 positon, float marginX, float marginY)
    {

        int row =  (int) (positon.Y / marginY);
        int column = (int)(positon.X / marginX);

        return new Vector2(row,column);
    }

}