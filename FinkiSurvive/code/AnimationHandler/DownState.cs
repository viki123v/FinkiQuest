using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code.AnimationHandler;

public class DownState : MovementState
{
    public override void Move(AnimatedSprite2D player)
    {
        player.Play("move_down");
    }

    public override void Attack(AnimatedSprite2D player)
    {
        player.Play("attack_down");
    }
}