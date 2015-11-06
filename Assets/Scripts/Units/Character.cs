using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Character {
    
    public Inventory inventory;
    
    public Stats baseStats;
    public Stats currentStats;
    public Stats maxStats;

    public List<Skill> skills;

    public int level;
    public float exp;

    public List<BuffSkill> buffs;
    public List<DebuffSkill> debuffs;

    protected virtual bool isPlayer {
        get {
            return false;
        }
    }
    protected bool canAttack;
    protected bool canCast;

    #region Constructors
    public Character(){
        inventory = new Inventory();
        currentStats = new Stats();
        maxStats = new Stats();
        skills = new List<Skill>();
        level = 1;
        exp = 0f;
        buffs = new List<BuffSkill>();
        debuffs = new List<DebuffSkill>();
        canAttack = true;
        canCast = true;
    }
    public Character(Character c){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = c.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,fields2[i].GetValue(c));
        }
    }
    #endregion
    #region Public Methods
    public void SetStats(Stats s){
        float hp = currentStats.hp/maxStats.hp;
        float mp = currentStats.mp/maxStats.mp;

        maxStats = s;
        currentStats = maxStats;
        currentStats.hp *= hp;
        currentStats.mp *= mp;
    }
    public void Heal(float hp, float mp, bool percent){
        if ( percent ){
            currentStats.hp += maxStats.hp*hp;
            currentStats.mp += maxStats.mp*mp;
        } else {
            currentStats.hp += hp;
            currentStats.mp += mp;
        }
    }

    public void AddBuff(BuffSkill s){
        if ( s.skillType.Contains(SkillType.buff) ){
            BuffSkill buff = (BuffSkill) buffs.Where<BuffSkill>(b => b.id == s.id);
            if ( buff == null ){
                s.SetTime(Time.time);
                buffs.Add(s);
                UpdateBuffs();
            } else {
                if ( buff.Level == s.Level )
                    buff.duration = s.duration;
                else if ( buff.Level < s.Level ){
                    buff = s;
                    buff.SetTime(Time.time);
                    UpdateBuffs();
                }
            }
        }
    }
    public void AddDebuff(DebuffSkill s){
        if ( s.skillType.Contains(SkillType.debuff) ){
            DebuffSkill debuff = (DebuffSkill) debuffs.Where<DebuffSkill>(b => b.id == s.id);
            if ( debuff == null ){
                s.SetTime(Time.time);
                debuffs.Add(s);
                UpdateBuffs();
            } else {
                if ( debuff.Level == s.Level )
                    debuff.duration = s.duration;
                else if ( debuff.Level < s.Level ){
                    debuff = s;
                    debuff.SetTime(Time.time);
                    UpdateBuffs();
                }
            }
        }
    }
    public void RemoveBuff(BuffSkill s){
        if ( s.skillType.Contains(SkillType.buff) ){
            BuffSkill buff = (BuffSkill) buffs.Where<BuffSkill>(b => b.id == s.id);
            if ( buff == null ){
                DebugWindow.Log("Cannot find buff, '"+buff.name+"'");
            } else {
                buffs.Remove(buff);
                UpdateBuffs();
            }
        }
    }
    public void RemoveDebuff(DebuffSkill s){
        if ( s.skillType.Contains(SkillType.buff) ){
            DebuffSkill debuff = (DebuffSkill) debuffs.Where<DebuffSkill>(b => b.id == s.id);
            if ( debuff == null ){
                DebugWindow.Log("Cannot find buff, '"+debuff.name+"'");
            } else {
                debuffs.Remove(debuff);
                UpdateBuffs();
            }
        }
    }

    public void Dot(float dmg){
        currentStats.hp -= dmg;
        CheckDeath();
    }
    public void Hit(float rawDmg, float defPen){
        float totalDmg = rawDmg - currentStats.baseDef*(1f-defPen); 
        currentStats.hp -= totalDmg;
        CheckDeath();
    }
    public void MagicHit(float rawDmg, float defPen){
        float totalDmg = rawDmg - currentStats.baseDef*currentStats.skillDefMultiplier*(1f-defPen);
        if ( rawDmg > 0 )
            if ( totalDmg < 0 )
                totalDmg = 0f;
        else
            if ( totalDmg > 0 )
                totalDmg = 0f;

        currentStats.hp -= totalDmg;
        CheckDeath();
    }
    public void CriticalHit(float rawDmg, float defPen){
        float totalDmg = rawDmg - currentStats.baseDef*currentStats.critDefMultiplier*(1f-defPen);

        currentStats.hp -= totalDmg;
        CheckDeath();
    }
    public void CriticalMagicHit(float rawDmg, float defPen){
        float totalDmg = rawDmg - currentStats.baseDef*currentStats.skillDefMultiplier*currentStats.critDefMultiplier*(1f-defPen);
        if ( rawDmg > 0 )
            if ( totalDmg < 0 )
                totalDmg = 0f;
        else
            if ( totalDmg > 0 )
                totalDmg = 0f;

        currentStats.hp -= totalDmg;
        CheckDeath();
    }

    public void Stunned(){}
    public void Paralyzed(){}
    public void KnockedDown(){}
    public void FrostBit(){}
    public void Frozen(){}
    public void Silenced(){}
    public void Blinded(){}
    public void Rooted(){}
    #endregion
    #region Protected Methods
    protected virtual void Death(){

    }
    protected virtual void UpdateStats(){}
    protected virtual void UpdateBuffs(){
        UpdateStats();

        Stats sumOfStats = new Stats();
        foreach (BuffSkill s in buffs){
            sumOfStats += s.stats;   
        }
        foreach (DebuffSkill s in debuffs){
            sumOfStats -= s.stats;
        }

        sumOfStats.MustBePositive();

        maxStats *= sumOfStats;
    }
    #endregion
    #region Private Methods
    private void CheckDeath(){
        if ( currentStats.hp < 1 ){
            Death();
        }
    }
    #endregion
}
