using Godot;
using System;

namespace FinkiAdventureQuest.FinkiSurvive.code
{
    public partial class ZombieMob : Mob
    {
        [Export]
        public int BaseHp = 150;
        [Export]
        public int AppearsAtLevel = 2;
        [Export]
        private float _hpScaleFactor = 0.2f;
        public override void _Ready()
        {
            HpScaling.Add(BaseHp);
            MaxHealth = BaseHp;
            if(Map.WaveCount > AppearsAtLevel)
                ScaleHp();
            Speed = rng.Next(90,100);
            base._Ready();
            AttackSpeedTimer.WaitTime = 0.5f;
            AddChild(AttackSpeedTimer);
        }

        public override PackedScene DropCoin()
        {
            return GD.Load<PackedScene>("res://FinkiSurvive/scenes/silver_coin.tscn");
        }

        public override void Attack()
        {
            if (!CanAttack) return;
            
            GetNode<Player>("/root/Level/Player").TakeDamage(GetDamage());
            _animSprite.Play("attack");
            CanAttack = false;
        }

        public override int GetDamage()
        {
            return 30;
        }

        public override void ScaleHp()
        {
            MaxHealth = GetScaledHp(_hpScaleFactor);
        }
    }
}


