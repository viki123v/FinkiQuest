using System;
using System.Collections.Generic;
using Godot;

namespace FinkiAdventureQuest.FinkiSurvive.code;

[GlobalClass]
public abstract partial class BaseAttack : Area2D
{
    protected readonly Random Rng = new();

    [Signal]
    public delegate void AttackSuccessEventHandler(float damage);
    [Export]
    protected float AttackSpeed;
    [Export]
    protected int Damage;
    [Export]
    protected float AttackRange;
    [Export]
    protected int AvailableAtWave;
    [Export]
    protected Texture2D Icon;
    
    public static readonly List<string> AttackScenes = new List<string>(){"attack1","attack2","attack3"};
    public abstract float GetAttackSpeed();
    public abstract int GetDamage();
    public abstract float GetAttackRange();

    protected abstract void ScaleHp();
    
    public abstract string GetContainerName();

    public abstract string GetIconPath();

    public abstract int GetAvailableAtWave();

    protected abstract void Ability();

    public Timer GetAttackSpeedTimer()
    {
        return GetNode<Timer>("AttackSpeed");
    }
    
    private void _on_area_entered(Area2D area)
    {
        var audio = GetNode<AudioStreamPlayer2D>("HitSoundEffect");
        audio.Play();
        Ability();
        
    }
    
    
}