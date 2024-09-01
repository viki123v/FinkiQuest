

using System;
using System.Collections.Generic;

namespace FinkiAdventureQuest.FinkiSurvive.code;

public class MobScaleHandler
{
    private  Dictionary<MobType, (bool EntryAdded,int Hp)> MobTypeToLastScaledHp = new();


    private static MobScaleHandler instance;

    private MobScaleHandler()
    {
        MobTypeToLastScaledHp[MobType.Orc] = (false, OrcMob.BaseHp);
        MobTypeToLastScaledHp[MobType.Zombie] = (false, ZombieMob.BaseHp);
        MobTypeToLastScaledHp[MobType.Knight] = (false,KnightMob.BaseHp);
    }
    

    public  void ResetEntries()
    {
        foreach (var key in MobTypeToLastScaledHp.Keys)
        {
            var tuple = MobTypeToLastScaledHp[key];
            tuple.EntryAdded = false;
            MobTypeToLastScaledHp[key] = tuple;
        }
    }

    public  void AddEntry(MobType type, int hp)
    { 
        var entry = MobTypeToLastScaledHp[type];
        if(entry.EntryAdded) return;

        MobTypeToLastScaledHp[type] = (true, hp);
    }

    public int GetHpEntry(MobType type)
    {
        return MobTypeToLastScaledHp[type].Hp;
    }

    public static MobScaleHandler GetInstance()
    {
        return instance ?? (instance = new MobScaleHandler());
    }
}