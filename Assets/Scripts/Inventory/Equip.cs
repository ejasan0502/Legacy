using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Equip : Item {

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
        masteryLevel = 0;
        equipType = EquipType.primaryWeapon;
        requirements = new Characteristics();
        bonusStats = new Stats();
    }

    public Equip(Equip e){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = e.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            if ( fields[i].GetType().Equals(typeof(Stats)) )
                fields[i].SetValue(this,new Stats( (Stats)fields2[i].GetValue(e) ));
            else if ( fields[i].GetType().Equals(typeof(Characteristics)) )
                fields[i].SetValue(this,new Characteristics( (Characteristics)fields2[i].GetValue(e) ));
            else
                fields[i].SetValue(this,fields2[i].GetValue(e));
        }
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
