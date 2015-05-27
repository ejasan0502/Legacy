using UnityEngine;
using System.Collections;
using System.Reflection;

[System.Serializable]
public class Monster : Character {
    
    public string id;
    public float sightRange = 10f;

    public Monster(Monster m){
        name = m.name;
        model = m.model;
        animator = m.animator;
        stats = new Stats(m.stats);
        currentStats = new Stats(m.currentStats);
        characterObject = m.characterObject;
        skills = m.skills;
        aggro = m.aggro;
        id = m.id;
        sightRange = m.sightRange;
        atkDistance = m.atkDistance;
    }

    #region Override Methods
    public override void PhysicalHit(Character atker){
        base.PhysicalHit(atker);

        if ( currentStats.health > 0 ){
            MonsterObject mo = characterObject as MonsterObject;
            if ( mo.enemySight.target == null ) mo.enemySight.SetTarget(atker);
        } else {
            
        }
    }
    public override void MagicalHit(Character atker, float percent){
        base.MagicalHit(atker, percent);
        
        if ( currentStats.health > 0 ){
            MonsterObject mo = characterObject as MonsterObject;
            if ( mo.enemySight.target == null ) mo.enemySight.SetTarget(atker);
        } else {

        }
    }
    #endregion

}
