using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Equip : Item {

    public GameObject model;
    public int masteryLevel;
    public EquipType equipType;
    public Characteristics requirements;
    public Stats bonusStats;

    public Equip(){
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
        model = null;
        masteryLevel = 0;
        equipType = EquipType.primaryWeapon;
        requirements = new Characteristics();
        bonusStats = new Stats();
    }

    public Equip(Equip e){
        name = e.name;
        id = e.id;
        tier = e.tier;
        description = e.description;
        weight = e.weight;
        cost = e.cost;
        craftCost = e.craftCost;
        craftChance = e.craftChance;
        ingredients = e.ingredients;
        icon = e.icon;
        stats = new Stats(e.stats);
        model = e.model;
        masteryLevel = e.masteryLevel;
        equipType = e.equipType;
        requirements = new Characteristics(e.requirements);
        bonusStats = new Stats(e.bonusStats);
    }

    public override Equip GetAsEquip(){
        return this;
    }

    public override bool IsEquip(){
        return true;
    }

    public bool IsShield {
        get {
            return id.ToLower().Contains("w.s-");
        }
    }

    public bool CanEquip(Player p){
        if ( p.attributes >= requirements ) return true;

        return false;
    }
}

public enum EquipType {
    primaryWeapon = 0,
    secondaryWeapon = 1,

    helm = 2,
    chest = 3,
    pants = 4,
    boots = 5,
    gloves = 6,

    ring = 7,
    necklace = 8,
    cape = 9
}
