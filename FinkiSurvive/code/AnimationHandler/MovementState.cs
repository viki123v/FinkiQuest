using System;
using System.IO;
using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code.AnimationHandler;

public abstract class MovementState
{
    public static float Tolerance = 0.8f;
    public static MovementState GetState(float angle, Vector2 direction)
    {
        if (direction == Vector2.Zero) return new IdleState();
        
        if (angle > -Math.PI/2 + Tolerance && angle < Math.PI / 2 - Tolerance)
        {
            return new RightState();
        }

        if (Math.Abs(angle) > Math.PI / 2 + Tolerance && Math.Abs(angle) < 3 * Math.PI / 2 + Tolerance)
        {
           return new LeftState();
        } 
        if (angle > -Math.PI / 2 - Tolerance && angle < -Math.PI / 2 + Tolerance)
        {
            return new UpState();
        } 
        if (angle > Math.PI / 2 - Tolerance && angle < Math.PI / 2 + Tolerance)
        {
            return new DownState();
        }

        return new IdleState();
    }
    
    public abstract void Move(AnimatedSprite2D player);
    public abstract void Attack(AnimatedSprite2D player);
}