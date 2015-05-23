using UnityEngine;
using System.Reflection;
using System.Collections;

[System.Serializable]
public class Equip : Item {

    public EquipType equipType;
    public Characteristics requirements;
    public int masteryLevel;
    public Stats bonusStats;
    public Ability[] abilities;

    public Equip(Equip e){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = e.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            if ( fields[i].GetType().Equals(typeof(Stats)) )
                fields[i].SetValue(this,new Stats( (Stats)fields2[i].GetValue(e) ));
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
