using UnityEngine;
using System.Reflection;
using System.Collections;

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
    public Ingredient[] ingredients;
    public Sprite icon;
    public Stats stats;

    public Item(){}
    public Item(Item item){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = item.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            if ( fields[i].GetType().Equals(typeof(Stats)) )
                fields[i].SetValue(this,new Stats( (Stats)fields2[i].GetValue(item) ));
            else
                fields[i].SetValue(this,fields2[i].GetValue(item));
        }
    }

    public virtual bool IsEquip() { return false; }
    public virtual bool IsUsable() { return false; }
    public virtual Equip GetAsEquip() { return null; }
    public virtual Usable GetAsUsable() { return null; }
}

public class Ingredient {
    public Item item;
    public int amount;

    public Ingredient(Item i, int amt){
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
