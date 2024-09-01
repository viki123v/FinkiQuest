

using System;
using System.Collections.Generic;

namespace FinkiAdventureQuest.FinkiSurvive.code;

public class MobScaleHandler
{
    private static Dictionary<MobType, (bool EntryAdded,int Hp)> MobTypeToLastScaledHp = new();
    private static bool _initialized = false;
    
    
    public static void Initialize()
    {
        if(_initialized) return;
        MobTypeToLastScaledHp[MobType.Orc] = (false, OrcMob.BaseHp);
        MobTypeToLastScaledHp[MobType.Zombie] = (false, ZombieMob.BaseHp);
        MobTypeToLastScaledHp[MobType.Knight] = (false,KnightMob.BaseHp);
        _initialized = true;
    }

    public static void ResetEntries()
    {
        foreach (var key in MobTypeToLastScaledHp.Keys)
        {
            var tuple = MobTypeToLastScaledHp[key];
            tuple.EntryAdded = false;
            MobTypeToLastScaledHp[key] = tuple;
        }
    }

    public static void AddEntry(MobType type, int hp)
    { 
        var entry = MobTypeToLastScaledHp[type];
        if(entry.EntryAdded) return;

        MobTypeToLastScaledHp[type] = (true, hp);
    }

    public static int GetHpEntry(MobType type)
    {
        return MobTypeToLastScaledHp[type].Hp;
    }
}