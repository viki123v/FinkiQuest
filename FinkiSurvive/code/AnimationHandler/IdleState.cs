using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code.AnimationHandler;

public class IdleState : MovementState
{
    public override void Move(AnimatedSprite2D player)
    {
        player.Play("idle");
    }

    public override void Attack(AnimatedSprite2D player)
    {
        player.Play("attack_down");
    }
}