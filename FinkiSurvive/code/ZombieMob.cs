using Godot;
using System;

namespace FinkiAdventureQuest.FinkiSurvive.code
{
    public partial class ZombieMob : Mob
    {
        public int BaseHp = 150;
        public int AppearsAtLevel = 2;
        
        private float _hpScaleFactor = 0.2f;
        public override void _Ready()
        {
            HpScaling.Add(BaseHp);
            MaxHealth = BaseHp;
            if(Map.WaveCount > AppearsAtLevel)
                ScaleHp();
            Speed = rng.Next(90,100);
            base._Ready();
        }

        public override void Attack()
        {
            _animSprite.Play("attack");
            if (Map.FrameCount % 20 != 0) return;
            GetNode<Player>("/root/Level/Player").TakeDamage(GetDamage());
            _animSprite.Play("attack");
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


