using Godot;
using System;


namespace FinkiAdventureQuest.FinkiSurvive.FinkiQuest.scenes
{
    public partial class MobCollisions : Area2D
    {
        
        Random rng = new Random();
        private Mob Mob;
		
        public override void _Ready()
        {
            Mob = GetParent<Mob>();
        }
		
        public override void _Process(double delta)
        {
        }
        
        private void _on_mob_collisions_area_entered(Area2D area)
        {
            var mobParent = GetParent<Mob>();
            var attack = area as BaseAttack;
            mobParent.TakeDamage((int) attack!.GetDamage());
            
            GD.Print("Mob Damaged: " + attack.GetDamage());
			
        }
		
    }
}