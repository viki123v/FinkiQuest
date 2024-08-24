using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code.AnimationHandler;

public class RightState : MovementState
{
    public override void Move(AnimatedSprite2D player)
    {
        player.Play("move_right");
    }

    public override void Attack(AnimatedSprite2D player)
    {
      player.Play("attack_right");
    }
}