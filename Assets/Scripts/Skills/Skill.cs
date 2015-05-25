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
    
    public Skill(){
        level = 0;
        stats = new Stats();
        reqs = new List<string>();
    }
    
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
        if ( p.classLevel >= reqLevel && p.classPoints > 0 ){
            return true;
        }

        return false;
    }
    
    public bool CanLearn(Player p){
        if ( p.classLevel >= reqLevel ){
            foreach (string s in reqs){
                if ( !p.HasSkill(s) ){
                    return false;
                }
            }
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
