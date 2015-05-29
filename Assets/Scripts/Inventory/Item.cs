using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Item {
    public string name;
    public string id;
    public Tier tier;
    public string description;
    public float weight;
    public float cost;
    public float craftCost;
    public int craftChance;             // 0 - 100
    public List<Ingredient> ingredients;
    public Sprite icon;
    public Stats stats;

    public Item(){
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
    }
    public Item(Item item){
        name = item.name;
        id = item.id;
        tier = item.tier;
        description = item.description;
        weight = item.weight;
        cost = item.cost;
        craftCost = item.craftCost;
        craftChance = item.craftChance;
        ingredients = item.ingredients;
        icon = item.icon;
        stats = new Stats(item.stats);
    }

    public virtual bool IsEquip() { return false; }
    public virtual bool IsUsable() { return false; }
    public virtual Equip GetAsEquip() { return null; }
    public virtual Usable GetAsUsable() { return null; }
}

public class Ingredient {
    public string item;
    public int amount;

    public Ingredient(string i, int amt){
        item = i;
        amount = amt;
    }
}

public enum Tier {
    common = 1,
    rare = 2,
    unique = 4,
    legendary = 8,
    godly = 16
}
