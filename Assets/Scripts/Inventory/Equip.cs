using UnityEngine;
using System.Reflection;
using System.Collections;

[System.Serializable]
public class Equip : Item {

    public int level;
    public Stats stats;
    public Stats bonusStats;
    public Traits bonusTraits;
    public EquipType equipType;

    public Equip(){
        name = "";
        id = "";
        description = "";
        icon = null;
        stackable = false;
        weight = 0f;   

        level = 0;
        stats = new Stats();
        bonusStats = new Stats();
        bonusTraits = new Traits();
        equipType = 0;
    }
    public Equip(Equip equip){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] itemFields = equip.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,itemFields[i].GetValue(equip));
        }
    }

    public override Equip GetAsEquip(){
        return this;
    }
}

public enum EquipType {
    sword = 0,
    shield = 1,
    broadsword = 0,
    spear = 0,
    axe = 0,
    staff = 0,
    wand = 0,
    dagger = 0,
    mace = 0,
    bow = 0,
    heavyArmor = 2,
    lightArmor = 2,
    clothArmor = 2,
    accessory = 3
}
