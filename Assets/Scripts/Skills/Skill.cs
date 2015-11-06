using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public abstract class Skill {
    public string name;
    public string id;
    public int requiredLevel;
    public string description;

    public int iteration;
    public float cooldown;
    public List<SkillType> skillType;
    public List<Skill> connectors;

    public int Level {
        get {
            return level;
        }
    }

    protected int level;
    private float exp;
    private float maxExp;

    public Skill(){
        name = "";
        id = "";
        requiredLevel = 0;
        description = "";
        iteration = 0;
        cooldown = 0f;
        skillType = new List<SkillType>();
        connectors = new List<Skill>();
        level = 0;
        exp = 0f;
        maxExp = 0f;
    }
    public Skill(Skill s){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = s.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,fields2[i].GetValue(s));
        }
        UpdateSelf();
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

    private void LevelUp(){
        exp = maxExp - exp;
        level++;
        UpdateSelf();
    }
    
    public virtual Skill GetAsSkillType(){
        return this;
    }
    public abstract void Apply(Character caster, Character target);

    protected virtual void UpdateSelf(){
        maxExp = 9f + 9f*(level-1)*level*0.125f + 9f*9f*0.0478f*((level-1)/10);
    }
}
public enum SkillType {
    singleTarget,   // Applies to a single target
    charge,         // Has a charge up time
    aoeFront,       // Applies to multiple targets in front of caster
    aoeAround,      // Applies to multiple targets around the caster
    aoeTarget,      // Applies to multiple targets around a target
    buff,           // Increases traits or stats of target(s)
    debuff,         // Decreases traits or stats of target(s)
    status,         // Applies a status effect
    summon,         // Instantiates an AI object
    toggle,         // Remains active until deactivated, decreases mana when active
    restore,        // Increases hp or mp by a percentage or amount
    special,        // Adds more functionality
    passive,        // Applies stats or traits to caster
    extension       // Adds on to an existing skill
}
