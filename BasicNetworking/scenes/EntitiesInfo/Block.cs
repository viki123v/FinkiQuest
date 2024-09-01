using System;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo.GroupBlocks;
using FinkiAdventureQuest.ViktorIgraTuka.scenes.util;
using Godot;

namespace FinkiAdventureQuest.ViktorIgraTuka.scenes.EntitiesInfo;

public class Block
{
    private FinkiTetrisPlayingGame _scene;

    public TextureRect Rect { get; set; }
    public Vector2 BitMapPosition;
    public GroupBlock Parent { get; set; }
    private Texture2D _texture; 

    public Block(Vector2 bitMapPosition, GroupBlock parent, Texture2D newTexture)
    {
        BitMapPosition = bitMapPosition;
        Parent = parent;
        Rect = BlockTransition.Create(ScallingUtil.FromBitmapToDisplay(bitMapPosition),newTexture);
    }
 
    public bool FallOneTile()
    {
        (int, int) prev = ((int)BitMapPosition.X, (int)BitMapPosition.Y);
        (int, int) newPos = ((int)BitMapPosition.X + 1, (int)BitMapPosition.Y);

        if (newPos.Item1 == FinkiTetrisPlayingGame.Dimensions.Item1 || Parent.SharedBitMap[newPos.Item1, newPos.Item2] is not null)
            return false;
        
        BitMapPosition += new Vector2(1,0);
        Bookeeping(prev,newPos);
        ResetDisplayPosition();

        return true;
    }

    public void Bookeeping((int,int) prevBitMapPos, (int,int) newPos)
    {
        Parent.SharedBitMap[prevBitMapPos.Item1, prevBitMapPos.Item2] = null;
        _scene.CountByRow[prevBitMapPos.Item1]--;
        
        Parent.SharedBitMap[newPos.Item1, newPos.Item2] = this;
        _scene.CountByRow[newPos.Item1]++;
    }
    
    public void AttachSelf(FinkiTetrisPlayingGame scene, Vector2 insertAtPosition)
    {
        if(Rect.GetParent() is not null)
            Rect.GetParent().RemoveChild(Rect);
        
        _scene = scene;
        _scene.GetNode<ColorRect>("CanvasLayer/SpawingRectangle").AddChild(Rect);

        (int, int) pos = ((int)insertAtPosition.X, (int)insertAtPosition.Y);

        if (Parent.SharedBitMap[pos.Item1, pos.Item2] is not null && Parent.SharedBitMap[pos.Item1,pos.Item2].Parent.IsStopped)
            throw new Exception("Game over");
        
        Parent.SharedBitMap[pos.Item1,pos.Item2] = this;
        BitMapPosition = insertAtPosition;
        ResetDisplayPosition();
    }

    public void AttachSelf(Node2D scene)
    {
        scene.AddChild(Rect);
    }

    public void SetPosition(Vector2 bitMapPosition)
    {
        (int, int) oldPos = ((int)BitMapPosition.X, (int)BitMapPosition.Y);
        (int, int) newPos = ((int)bitMapPosition.X, (int)bitMapPosition.Y);

        if (Parent.SharedBitMap[oldPos.Item1, oldPos.Item2] == this)
            Parent.SharedBitMap[oldPos.Item1, oldPos.Item2] = null;
        
        Parent.SharedBitMap[newPos.Item1,newPos.Item2] = this;
        BitMapPosition = bitMapPosition;

        if (Parent.IsStopped)
        {
            _scene.CountByRow[oldPos.Item1]--;
            _scene.CountByRow[newPos.Item1]++;
        }
        
        ResetDisplayPosition();
    }


    public void Remove()
    {
        if(!GodotObject.IsInstanceValid(Rect))return;
        
        (int, int) bitmapPos = ((int)BitMapPosition.X, (int)BitMapPosition.Y);
        
        Parent.SharedBitMap[bitmapPos.Item1, bitmapPos.Item2] = null;
        _scene.CountByRow[bitmapPos.Item1]--; 
        
        Rect.CallDeferred("queue_free");
    }

    public void ResetDisplayPosition()
         =>  Rect.Position = ScallingUtil.FromBitmapToDisplay(BitMapPosition); 
}