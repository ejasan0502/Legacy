using UnityEngine;
using System.Collections;

public class Equip : Item {

    public int level;
    public Stats stats;
    public Stats bonusStats;
    public Traits bonusTraits;

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
