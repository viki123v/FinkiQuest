using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code.AnimationHandler;

public class LeftState : MovementState
{
    public override void Move(AnimatedSprite2D player)
    {
        player.Play("move_left");
    }

    public override void Attack(AnimatedSprite2D player)
    {
        player.Play("attack_left");
    }
}