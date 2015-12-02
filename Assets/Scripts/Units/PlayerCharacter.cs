using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;

// Base stats will remain constant and initialized during character creation
[System.Serializable]
public class PlayerCharacter : Character {

    public Traits baseTraits;
    public Traits traits;

    public Equip[] equipment = new Equip[6];
    public float maxExp;
    public int traitPoints;
    public string lastSavedScene;

    private bool hostile = false;
    private GameObject primaryWeapon = null;
    private GameObject secondaryWeapon = null;
    private Model armor = new Model();

    public override bool isPlayer{
        get{
            return true;
        }
    }
    public override bool isFriendly {
        get {
            return hostile;
        }
    }

    public PlayerCharacter(){
        inventory = new Inventory();
        baseTraits = new Traits();
        traits = new Traits();
        baseStats = new Stats();
        currentStats = new Stats();
        maxStats = new Stats();
        skills = new List<Skill>();
        equipment = new Equip[6]{   null,                   // Primary
                                    null,                   // Secondary
                                    null,                   // Armor
                                    null, null, null };     // Accessories
        level = 1;
        exp = 0f;
        maxExp = 0f;
        traitPoints = 0;
        buffs = new List<BuffSC>();
        debuffs = new List<BuffSC>();
    }

    public void IncrementTraits(int pwr, int frt, int con){
        baseTraits.pwr += pwr;
        baseTraits.frt += frt;
        baseTraits.con += con;
        UpdateStats();
    }
    public void AddExp(float xp){
        exp += xp;
        if ( exp >= maxExp )
            LevelUp();
    }
    public void Equip(Equip e){
        int index = (int) e.equipType;

        // Remove previous model from character, if necessary
        if ( equipment[index] != null ){
            if ( index < 3 ){
                // Equip has a model
                if ( index < 2 ){
                    // Equip is a weapon
                    if ( primaryWeapon != null ){
                        GameObject.DestroyImmediate(primaryWeapon);
                        primaryWeapon = null;
                    }
                    primaryWeapon = (GameObject) GameObject.Instantiate(Resources.Load(e.GetModelPath()));
                } else {
                    // Equip is armor
                }
                equipment[index] = e;
            } else {
                // Equip is an accessory
                // Add accessory to next empty slot
                bool equipped = false;
                for (int i = 3; i < equipment.Length; i++){
                    if ( equipment[i] == null ){
                        equipment[i] = e;
                        equipped = true;
                    }
                }

                // Empty slot wasn't found, set it to first accessory slot
                if ( !equipped )
                    equipment[index] = e;
            }
        }

        // Update Stats
        UpdateTraits();
        UpdateStats();

        // Update Graphics
    }
    public void Equip(Equip e, int slot){
        int index = (int) e.equipType;

        if ( index == slot ||           // Check if equipType is appropriate to desired slot
             slot < 2 && index < 2 ||   // Check if index and slot is a weapon
             slot > 2 && index > 2 ){   // Check if index and slot is an accessory
            equipment[slot] = e;

            // Update Stats
            UpdateTraits();
            UpdateStats();

            // Update Graphics
        }
    }
    public void UpdateTraits(){
        traits.pwr = Mathf.CeilToInt(baseTraits.pwr + 0.125f*baseTraits.pwr*level);
        traits.frt = Mathf.CeilToInt(baseTraits.frt + 0.125f*baseTraits.frt*level);
        traits.con = Mathf.CeilToInt(baseTraits.con + 0.125f*baseTraits.con*level);

        Traits bonusTraits = new Traits(1);
        foreach(Equip e in equipment){
            bonusTraits += e.bonusTraits;
        }
        traits *= bonusTraits;
    }
    public override void UpdateStats(){
        // Update stats
	    Stats s = new Stats();
	    s.baseDmg = baseStats.baseDmg + 0.125f*baseStats.baseDmg*level + 0.125f*traits.pwr;
	    s.critDmgMultiplier = baseStats.critDmgMultiplier + 0.00113f*level*baseStats.critDmgMultiplier + 0.00142f*traits.pwr;
	    s.skillDmgMultiplier = baseStats.skillDmgMultiplier + 0.00263f*baseStats.skillDmgMultiplier*level + 0.00263f*traits.pwr;
	    s.baseDef = baseStats.baseDef + 0.125f*level*baseStats.baseDef + 0.165f*traits.frt;
	    s.critDefMultiplier = baseStats.critDefMultiplier + 0.00113f*level*baseStats.critDefMultiplier + 0.00142f*traits.frt;
	    s.skillDefMultiplier = baseStats.skillDefMultiplier + 0.00243f*level*baseStats.skillDefMultiplier + 0.00243f*traits.frt;
	    s.hp = baseStats.hp + 0.421f*level*baseStats.hp + 0.954f*traits.con;
	    s.mp = baseStats.mp + 0.421f*level*baseStats.mp + 0.85f*traits.con;
	    s.recov = baseStats.recov + 0.000125f*level*baseStats.recov + 0.000142f*traits.con;
	    s.critChance = baseStats.critChance + 0.03f*level*baseStats.critChance;
	    s.defPen = baseStats.defPen + 0.01f*level*baseStats.defPen;
	    s.resist = baseStats.resist + 0.213f*level*baseStats.resist;
	    s.luck = baseStats.luck + 0.025f*level*baseStats.luck;
	    s.movSpd = baseStats.movSpd + 0.125f*level*baseStats.movSpd;
	    s.maxWeight = baseStats.maxWeight + 0.125f*level*baseStats.maxWeight + 0.142f*traits.con;
	    s = EquipmentStats(s);
	    SetStats(s);
    }
    public override void UpdateBuffs(){
        UpdateTraits();

        Traits sumOfTraits = new Traits(1);
        foreach (BuffSC s in buffs){
            sumOfTraits += s.traits;
        }
        foreach (BuffSC s in debuffs){
            sumOfTraits *= s.traits;
        }
        traits += traits*sumOfTraits;

        base.UpdateBuffs();
    }

    private Stats EquipmentStats(Stats stats){
        Stats sumOfEquipmentStats = new Stats();
        Stats sumOfEquipmentBonusStats = new Stats();
        foreach (Equip e in equipment){
            sumOfEquipmentStats += e.stats;
            sumOfEquipmentBonusStats += e.bonusStats;
        }

        return (stats+sumOfEquipmentStats)*sumOfEquipmentBonusStats;
    }
    private void LevelUp(){
        exp = maxExp - exp;
        maxExp = 9f + 9f*(level-1)*level*0.125f + 9f*9f*level*level*0.0487f*((level-1)/10f);
        level++;
        traitPoints += 3;
        UpdateTraits();
        UpdateStats();
    }
}
