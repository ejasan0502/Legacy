using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Character {
    public string name;
    public GameObject model;
    
    public int level = 1;
    public float exp;

    public float atkDistance = 2f;
    public Stats stats;
    public Stats currentStats;
    public CharacterObject characterObject;
    public List<Skill> skills = new List<Skill>();

    protected Character killer = null;

    #region Public Methods
    public void Damage(float h, float m){
        currentStats.health -= h;
        currentStats.mana -= m;

        if ( h > 0 ) characterObject.CreateText("-" + h,Color.red,1f);
        else if ( m > 0 ) characterObject.CreateText("-" + m,Color.blue,1f);

        if ( currentStats.health < 1 ) currentStats.health = 1f;
    }
    public void Heal(float h, float m){
        if ( !IsAlive ) return;

        currentStats.health += h;
        currentStats.mana += m;

        if ( h > 0 ) characterObject.CreateText("+" + h,Color.green,1f);
        else if ( m > 0 ) characterObject.CreateText("+" + m,Color.blue,1f);

        if ( currentStats.health > stats.health ) currentStats.health = stats.health;
        if ( currentStats.mana > stats.mana ) currentStats.mana = stats.mana;
    }
    public virtual void Miss(Character atker){
        characterObject.CreateText("Miss",Color.yellow,1f);
    }
    public virtual void PhysicalHit(Character atker){
        float rawDmg = Random.Range(atker.currentStats.meleeMinDmg,atker.currentStats.meleeMaxDmg);
        float dmg = rawDmg - currentStats.physDef;
        if ( dmg < 1 ) dmg = 1f;

        float scale = 1f;
        if ( Random.Range(0,100) <= atker.currentStats.critChance ){
            dmg *= atker.currentStats.critDmg;
            scale = 2f;
        }

        currentStats.health -= dmg;

        Color c = Color.red;
        string s = ((int)dmg)+"";
        if ( dmg < 0 ) {
            c = Color.green;
            s = Mathf.Abs((int)dmg)+"";
        }
        characterObject.CreateText(s,c,scale);

        if ( currentStats.health > stats.health ) currentStats.health = stats.health;
        if ( currentStats.mana > stats.mana ) currentStats.mana = stats.mana;

        CheckDeath(atker);
    }
    public virtual void MagicalHit(Character atker, float percent){
        float rawDmg = percent*Random.Range(atker.currentStats.magicMinDmg,atker.currentStats.magicMaxDmg);
        float dmg = rawDmg - currentStats.magDef;
        if ( dmg < 1 ) dmg = 1f;

        float scale = 1f;
        if ( Random.Range(0,100) <= atker.currentStats.critChance ){
            dmg *= atker.currentStats.critDmg;
            scale = 2f;
        }

        currentStats.health -= dmg;

        Color c = Color.red;
        string s = ((int)dmg)+"";
        if ( dmg < 0 ) {
            c = Color.green;
            s = Mathf.Abs((int)dmg)+"";
        }
        characterObject.CreateText(s,c,scale);

        if ( currentStats.health > stats.health ) currentStats.health = stats.health;
        if ( currentStats.mana > stats.mana ) currentStats.mana = stats.mana;

        CheckDeath(atker);
    }
    public virtual void AddExp(float x){
        exp += x;
    }
    #endregion
    #region Protected Methods
    protected  virtual void CheckDeath(Character atker){
        if ( currentStats.health < 1 ){
            currentStats.health = 0f;
            Death();
        }
    }
    #endregion
    #region Private Methods
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
    public void SetCharacterObject(CharacterObject o){
        characterObject = o;
    }
    #endregion
}
