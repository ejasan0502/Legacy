using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Usable : Item {

    public UsableType usableType;

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
    }

    public void Use(Character target){
        if ( usableType == UsableType.instant ){
            
        } else if ( usableType == UsableType.replenish ){
            
        } else if ( usableType == UsableType.buff ){

        } else if ( usableType == UsableType.teleport ){

        } else if ( usableType == UsableType.damage ){

        } else if ( usableType == UsableType.aoe ){

        }
    }
}

public enum UsableType {
    instant,
    replenish,
    buff,
    teleport,
    damage,
    aoe
}
