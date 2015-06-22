using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Skill {
    public string name;
    public string id;
    public Sprite icon;
    public string description;
    public SkillType skillType;

    public Stats stats;                 // Passive skills will ALWAYS have stats as percents
    public float cd;

    public int level;

    public int reqLevel;
    public List<string> reqs;
    public float healthCost;
    public float manaCost;

    public float castTime = 0f;
    
    public Skill(){
        name = "";
        id = "";
        icon = null;
        description = "";
        skillType = SkillType.singleTarget;
        stats = new Stats();
        level = 0;
        reqLevel = 0;
        reqs = new List<string>();
        healthCost = 0f;
        manaCost = 0f;
    }
    public Skill(Skill s){
        name = s.name;
        id = s.id;
        icon = s.icon;
        description = s.description;
        skillType = s.skillType;
        stats = new Stats(s.stats);
        level = s.level;
        reqLevel = s.reqLevel;
        reqs = s.reqs;
        healthCost = s.healthCost;
        manaCost = s.manaCost;
    }
    
    public bool CanLevelUp(Player p){
        if ( p.classLevel >= reqLevel && p.classPoints > 0 ){
            return true;
        }

        return false;
    }
    public bool CanLearn(Player p){
        if ( p.HasSkill(id) ) return false;
        //if ( p.classLevel < reqLevel ) return false;
        //foreach (string s in reqs){
        //    if ( !p.HasSkill(s) ) return false;
        //}
        return true;
    }
    
    public void LevelUp(){
        level++;
    }   

    public virtual bool CanCast(Character p){
        Console.System("Skill.cs - CanCast(Character)");
        if ( p.currentStats.health < healthCost ) return false;
        if ( p.currentStats.mana < manaCost ) return false;

        return true;
    }
    public virtual void Cast(Character caster){}
    public virtual void Apply(Character caster){}
    public virtual void Apply(Character caster, Character target){}
    public virtual void Apply(Character caster, List<Character> targets){}
}

public enum SkillType {
    buff,
    passive,
    singleTarget,
    aoe,
    summon
}

public enum CastAnim {
    instant_buff = 0,
    instant_patk = 1,
    instant_matk = 2,
    long_buff = 3,
    long_patk = 4,
    long_matk = 5
}
