using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Monster : Character {
    
    public string description;
    public string id;
    public float sightRange = 10f;

    public Monster(){
        name = "";
        description = "";
        id = "";
        model = null;
        stats = new Stats();
        currentStats = new Stats();
        characterObject = null;
        skills = new List<Skill>();
        sightRange = 0f;
        atkDistance = 0f;
    }

    public Monster(Monster m){
        name = m.name;
        description = m.description;
        id = m.id;
        model = m.model;
        stats = new Stats(m.stats);
        currentStats = new Stats(m.currentStats);
        characterObject = m.characterObject;
        skills = m.skills;
        sightRange = m.sightRange;
        atkDistance = m.atkDistance;
    }

    #region Override Methods
    public override void Miss(Character atker){
        base.Miss(atker);

        if ( currentStats.health > 0 ){
            MonsterObject mo = characterObject as MonsterObject;
            if ( mo.enemySight.target == null ) mo.enemySight.SetTarget(atker);
        }
    }
    public override void PhysicalHit(Character atker){
        base.PhysicalHit(atker);

        if ( currentStats.health > 0 ){
            MonsterObject mo = characterObject as MonsterObject;
            if ( mo.enemySight.target == null ) mo.enemySight.SetTarget(atker);
        }
    }
    public override void MagicalHit(Character atker, float percent){
        base.MagicalHit(atker, percent);
        
        if ( currentStats.health > 0 ){
            MonsterObject mo = characterObject as MonsterObject;
            if ( mo.enemySight.target == null ) mo.enemySight.SetTarget(atker);
        }
    }
    #endregion

}
