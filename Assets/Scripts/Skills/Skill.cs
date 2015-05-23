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

    public float level;

    public int reqLevel;
    public Skill[] reqs;

    public Skill(Skill s){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = s.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            if ( fields[i].GetType().Equals(typeof(Stats)) )
                fields[i].SetValue(this,new Stats( (Stats)fields2[i].GetValue(s) ));
            else
                fields[i].SetValue(this,fields2[i].GetValue(s));
        }
    }

    public bool CanLevelUp(Player p){
        if ( p.level >= reqLevel ){
            return true;
        }

        return false;
    }

    public void LevelUp(){
        level++;

    }   

    public virtual void Cast(Character caster){

    }

    public virtual void Apply(Character caster){
        
    }

    public virtual void Apply(Character caster, Character target){
        
    }

    public virtual void Apply(Character caster, List<Character> targets){
        
    }
}

public enum SkillType {
    buff,
    passive,
    singleTarget,
    aoe,
    summon
}
