using System;
using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.FinkiQuest.scenes
{
public abstract partial class BaseAttack : Area2D
{

    [Signal]
    public delegate void AttackSuccessEventHandler(float damage);
    public abstract float GetAttackSpeed();
    public abstract float GetDamage();
    public abstract float GetAttackRange();

    public abstract void ScaleHp();

    protected readonly Random Rng = new();

    public abstract string GetContainerName();

    public abstract string GetIconPath();

    public abstract int AvailableAtWave();

    public abstract void Ability();
    
    public void _on_area_entered(Area2D area)
    {
        var audio = GetNode<AudioStreamPlayer2D>("HitSoundEffect");
        audio.Play();
        
        Ability();
        
    }
    
}
    
}

