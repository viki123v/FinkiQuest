using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code.AnimationHandler;

public class UpState : MovementState
{
    public override void Move(AnimatedSprite2D player)
    {
       player.Play("move_up");
    }

    public override void Attack(AnimatedSprite2D player)
    {
        player.Play("attack_up");
    }
}