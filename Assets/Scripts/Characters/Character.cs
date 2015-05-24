﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Character {
    public string name;
    public GameObject model;
    public RuntimeAnimatorController animator;
    
    public int level = 1;
    public float exp;

    public float atkDistance = 3f;
    public Stats stats;
    public Stats currentStats;
    public CharacterObject characterObject;
    public List<Skill> skills = new List<Skill>();

    [HideInInspector] public float aggro = 0f;

    #region Public Methods
    public virtual void PhysicalHit(Character atker){
        float rawDmg = Random.Range(atker.currentStats.meleeMinDmg,atker.currentStats.meleeMaxDmg);
        float dmg = rawDmg - currentStats.physDef;
        if ( dmg < 1 ) dmg = 1f;

        currentStats.health -= dmg;

        Color c = Color.red;
        string s = (dmg+0f)+"";
        if ( dmg < 0 ) {
            c = Color.green;
            s = Mathf.Abs(dmg+0f)+"";
        }
        characterObject.CreateText(s,c);

        CheckDeath();
    }
    public virtual void MagicalHit(Character atker, float percent){
        float rawDmg = percent*Random.Range(atker.currentStats.magicMinDmg,atker.currentStats.magicMaxDmg);
        float dmg = rawDmg - currentStats.magDef;
        if ( dmg < 1 ) dmg = 1f;

        currentStats.health -= dmg;
        CheckDeath();
    }
    #endregion
    #region Private Methods
    private void CheckDeath(){
        if ( currentStats.health < 1 ){
            currentStats.health = 0f;
            Death();
        }
    }
    private void Death(){
        characterObject.SetState(CharacterState.dying);
    }
    #endregion
    #region Get Methods
    public virtual bool IsPlayer {
        get {
            return false;
        }
    }
    public bool IsAlive {
        get {
            return currentStats.health > 0;
        }
    }
    public bool HasSkill(string id){
        foreach (Skill s in skills){
            if ( s.id == id ) return true;
        }

        return false;
    }
    public Skill GetSkill(string id){
        foreach (Skill s in skills){
            if ( s.id == id ){
                return s;
            }
        }
        return null;
    }
    #endregion
    #region Set Methods
    public void ResetAggro(){
        aggro = 0f;
    }
    public void AddAggro(float x){
        aggro += x;
    }
    public void SetCharacterObject(CharacterObject o){
        characterObject = o;
    }
    #endregion
}