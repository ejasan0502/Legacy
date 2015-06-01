using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Usable : Item {

    public UsableType usableType;
    public float duration;
    public bool friendly;
    public float cd;
    private float cdStartTime = 0f;

    public Usable(){
        name = "";
        id = "";
        tier = Tier.common;
        description = "";
        weight = 0f;
        cost = 0f;
        craftCost = 0f;
        craftChance = 0;
        ingredients = new List<Ingredient>();
        icon = null;
        stats = new Stats();
        usableType = UsableType.replenish;
        duration = 0f;
        friendly = false;
        cd = 0f;
        cdStartTime = Time.time;
    }

    public Usable(Usable u){
        name = u.name;
        id = u.id;
        tier = u.tier;
        description = u.description;
        weight = u.weight;
        cost = u.cost;
        craftCost = u.craftCost;
        craftChance = u.craftChance;
        ingredients = u.ingredients;
        icon = u.icon;
        stats = new Stats(u.stats);
        usableType = u.usableType;
        duration = u.duration;
        friendly = u.friendly;
        cd = 0f;
        cdStartTime = Time.time;
    }

    public void Use(Character target){
        if ( usableType == UsableType.potion ){
            target.currentStats.health += stats.health;
            target.currentStats.mana += stats.mana;
            if ( target.currentStats.health > target.stats.health ) target.currentStats.health = target.stats.health;
            if ( target.currentStats.mana > target.stats.mana ) target.currentStats.mana = target.stats.mana;
        } else if ( usableType == UsableType.replenish ){
            Replenish r = target.characterObject.gameObject.AddComponent<Replenish>();
            r.Initialize(stats.health,stats.mana,3f,15f);
        } else if ( usableType == UsableType.buff ){
            Buff b = target.characterObject.gameObject.AddComponent<Buff>();
            b.Initialize(stats,duration,false);
        } else if ( usableType == UsableType.buffPercent ){
            Buff b = target.characterObject.gameObject.AddComponent<Buff>();
            b.Initialize(stats,duration,true);
        } else if ( usableType == UsableType.damage ){
            target.Damage(stats.health,stats.mana);
        } else if ( usableType == UsableType.aoe ){
            
        } else if ( usableType == UsableType.dot ){
            DamageOverTime dot = target.characterObject.gameObject.AddComponent<DamageOverTime>();
            dot.Initialize(stats,3f,duration);
        }

        cdStartTime = Time.time;
    }

    public bool CanUse(){
        return Time.time - cdStartTime >= cd;
    }

    public override Usable GetAsUsable(){
        return this;
    }

    public override bool IsUsable(){
        return true;
    }
}

public enum UsableType {
    potion,
    replenish,
    buff,
    buffPercent,
    damage,
    aoe,
    dot
}
