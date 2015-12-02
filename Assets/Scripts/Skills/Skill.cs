using UnityEngine;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Skill {
    #region Base Variables
    public string name;
    public string id;
    public int requiredLevel;
    public string description;

    public int maxLevel;
    public int iteration;
    public float castTime;
    public float castDistance;
    public float cooldown;
    public bool percentageCost;
    public float hpCost;
    public float mpCost;
    public List<SkillCast> skillCasts;
    public List<Skill> connectors;
    public string animation;

    public TargetType targetType;
    #endregion
    #region Read-only Variables
    public int Level {
        get {
            return level;
        }
    }
    public float StartCastTime {
        get {
            return startCastTime;
        }
    }
    public float CastRadius {
        get {
            foreach (SkillCast sc in skillCasts){
                if ( sc.skillType == SkillType.aoeAround || sc.skillType == SkillType.aoeWithin )
                    return ((AoeSC)sc).radius;
            }
            return 0f;
        }
    }
    public bool isAoe {
        get {
            foreach (SkillCast sc in skillCasts){
                if ( sc.skillType == SkillType.aoeAround || 
                     sc.skillType == SkillType.aoeFront || 
                     sc.skillType == SkillType.aoeWithin )
                    return true;
            }   
            return false;
        }
    }
    public bool isAoeFront {
        get {
            foreach (SkillCast sc in skillCasts){
                if ( sc.skillType == SkillType.aoeFront )
                    return true;
            }   
            return false;
        }
    }
    public bool isAoeAround {
        get {
            foreach (SkillCast sc in skillCasts){
                if ( sc.skillType == SkillType.aoeAround )
                    return true;
            }   
            return false;
        }
    }
    public bool isAoeWithin {
        get {
            foreach (SkillCast sc in skillCasts){
                if ( sc.skillType == SkillType.aoeWithin )
                    return true;
            }   
            return false;
        }
    }
    public bool isToggle {
        get {
            return skillCasts[0].skillType == SkillType.toggle;
        }
    }
    public bool isBuff {
        get {
            foreach (SkillCast sc in skillCasts){
                if ( sc.skillType == SkillType.buff || 
                     sc.skillType == SkillType.debuff )
                    return true;
            }   
            return false;
        }
    }
    public bool isPassive {
        get {
            return skillCasts[0].skillType == SkillType.passive;
        }
    }
    public bool isCharge {
        get {
            return skillCasts[0].skillType == SkillType.charge;
        }
    }
    #endregion
    protected int level;
    private float exp;
    private float maxExp;
    private float startCDTime;
    private float startCastTime;

    public Skill(){
        name = "";
        id = "";
        requiredLevel = 0;
        description = "";
        iteration = 0;
        cooldown = 0f;
        skillCasts = new List<SkillCast>();
        connectors = new List<Skill>();
        level = 0;
        exp = 0f;
        maxExp = 0f;

        startCDTime = 0f;
        hpCost = 0f;
        mpCost = 0f;
        targetType = TargetType.self;
        castTime = 0f;
        maxLevel = 1;
        animation = "";
        castDistance = 0f;
        percentageCost = false;
    }
    public Skill(Skill s){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = s.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,fields2[i].GetValue(s));
        }
        UpdateSelf();
    }   

    public virtual bool CanCast(Character caster){
        return  caster.currentStats.hp > hpCost &&
                caster.currentStats.mp >= mpCost &&
                caster.CanCast &&
                Time.time - startCDTime >= cooldown;
    }
    public virtual XmlElement ToXmlElement(XmlDocument xmlDoc){
        XmlElement root = xmlDoc.CreateElement("Skill");

        XmlElement xmlId = xmlDoc.CreateElement("Id"); xmlId.InnerText = id; root.AppendChild(xmlId);
        XmlElement xmlLevel = xmlDoc.CreateElement("Level"); xmlLevel.InnerText = level+""; root.AppendChild(xmlLevel);
        XmlElement xmlExp = xmlDoc.CreateElement("Exp"); xmlExp.InnerText = exp+""; root.AppendChild(xmlExp);

        return root;
    }

    public void SetStartCastTime(float t){
        startCastTime = t;
    }
    public void SetCDTime(float t){
        startCDTime = t;
    }
    public void SetLevel(int x){
        level = x;
        UpdateSelf();
    }
    public void AddExp(float xp){
        exp += xp;
        if ( exp >= maxExp )
            LevelUp();
    }
    
    public void Apply(Character caster, Character target){
        foreach (SkillCast sc in skillCasts){
            sc.Apply(caster,target);
        }
    }
    public void UpdateSelf(){
        maxExp = 9f + 9f*(level-1)*level*0.125f + 9f*9f*0.0478f*((level-1)/10);
        foreach (SkillCast sc in skillCasts){
            sc.UpdateSelf();

            if ( sc.skillType == SkillType.charge ){
                ChargeSC csc = (ChargeSC) sc;
                castTime = csc.maxCharge * csc.chargeRate;
            }
        }
    }

    private void LevelUp(){
        exp = maxExp - exp;
        level++;
        UpdateSelf();
    }
}

// 1 represents primary skill types, Can only be assigned on skilltype[0]
// 1+ represents secondary skill types, Can be assigned on skilltype[x] where x = any number
public enum SkillType {
    buff,           // Increases traits or stats of target(s), 1+
    debuff,         // Decreases traits or stats of target(s), 1+
    status,         // Applies a status effect, 1+
    heal,           // Increases hp or mp by a percentage, 1+
    aoeFront,       // Applies to multiple targets in front of caster, 1+
    aoeAround,      // Applies to multiple targets around a target, 1+
    aoeWithin,      // Applies to multiple targets around the caster, 1+
    damage,         // Applies damage to a single target, 1+
    cure,           // Removes statuses, 1+

    charge,         // Has a charge up time, 1
    toggle,         // Remains active until deactivated, decreases mana when active, 1
    summon,         // Instantiates an AI object, 1
    special,        // Adds more functionality, 1 (extends Skill.cs)

    passive,        // Applies stats or traits to caster, 1
    extension       // Adds on to an existing skill, 1 (extends Skill.cs)
}
