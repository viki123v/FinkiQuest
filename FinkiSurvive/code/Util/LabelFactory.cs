using System;
using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code.Util;

public class LabelFactory
{
    public static Random rng = new Random();
    public static void DisplayDamageLabel(Node2D context, int amount)
    {
        var container = context.GetNode<MarginContainer>("HitLabelsCont");
        var tween = container.GetTree().CreateTween();
        var label = new Label();
        label.Text = amount.ToString();
        label.AddThemeFontSizeOverride("font_size",50);
        container.AddChild(label);

        var fv = new FontVariation();

        fv.BaseFont = ResourceLoader.Load<FontFile>(ProjectPaths.DefaultPath + "tmp_assets/FONTS/videophreak/VIDEOPHREAK.ttf");
        fv.VariationEmbolden = 1.2f;

        label.AddThemeFontOverride("font", fv);
        label.AddThemeColorOverride("font_color", Colors.White);

        //label.Scale = Vector2.Zero;
        var endPos = label.Position;
        Vector2 offset = new Vector2(rng.Next(0,150),rng.Next(0,250));
        endPos += offset;
			
        tween.TweenProperty(label, "scale",Vector2.One, 0.3f);
        tween.TweenProperty(label, "position", endPos, 0.5f); // radi ova imat warinngs vo debug
        tween.TweenProperty(label, "scale",Vector2.Zero, 0.5f);
			
        tween.TweenCallback(Callable.From(label.QueueFree)).SetDelay(0.8f);
    }

    public static Label CreateLabel(Color color)
    {
        var label = new Label();
        var fv = new FontVariation();
        fv.BaseFont = ResourceLoader.Load<FontFile>(ProjectPaths.DefaultPath + "tmp_assets/FONTS/videophreak/VIDEOPHREAK.ttf");
        fv.VariationEmbolden = 1.2f;
        label.AddThemeFontOverride("font", fv);
        label.AddThemeColorOverride("font_color",color);

        return label;
    }
}