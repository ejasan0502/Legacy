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

    public List<BuffSC> buffs;
    public List<BuffSC> debuffs;

    protected bool canAttack;
    protected bool canCast;
    protected bool canMove;
    protected CharacterObject characterObject;
    protected StatusManager statusManager;
    protected Skill castingSkill;

    public bool CanCast {
        get {
            return canCast;
        }
    }
    public bool CanAttack {
        get {
            return canAttack;
        }
    }
    public bool CanMove {
        get {
            return canMove;
        }
    }
    public bool isAlive {
        get {
            return currentStats.hp > 0;
        }
    }
    public virtual bool isPlayer {
        get {
            return false;
        }
    }
    public virtual bool isFriendly {
        get {
            return false;
        }
    }

    #region Constructors
    public Character(){
        inventory = new Inventory();
        currentStats = new Stats();
        maxStats = new Stats();
        skills = new List<Skill>();
        level = 1;
        exp = 0f;
        buffs = new List<BuffSC>();
        debuffs = new List<BuffSC>();

        canAttack = true;
        canCast = true;
        canMove = true;
        characterObject = null;
        statusManager = new StatusManager();
        castingSkill = null;
    }
    #endregion
    #region Public Get Methods
    public StatusManager GetStatusManager(){
        return statusManager;
    }
    public Skill GetCastingSkill(){
        return castingSkill;
    }
    public CharacterObject GetCharacterObject(){
        return characterObject;
    }
    #endregion
    #region Public Methods
    public virtual void OnUpdate(){
        statusManager.OnUpdate();
        CheckBuffs();
    }

    public void SetCastingSkill(Skill s){
        castingSkill = s;
    }
    public void SetCanAttack(bool b){
        canAttack = b;
    }
    public void SetCanCast(bool b){
        canCast = b;
    }
    public void SetCanMove(bool b){
        canMove = b;
    }
    public void SetStats(Stats s){
        float hp = currentStats.hp/maxStats.hp;
        float mp = currentStats.mp/maxStats.mp;

        maxStats = s;
        currentStats = maxStats;
        currentStats.hp *= hp;
        currentStats.mp *= mp;
    }
    public void SetCharacterObject(CharacterObject co){
        characterObject = co;
    }
    public void Heal(float hp, float mp, bool percent){
        if ( percent ){
            currentStats.hp += maxStats.hp*hp;
            currentStats.mp += maxStats.mp*mp;
        } else {
            currentStats.hp += hp;
            currentStats.mp += mp;
        }

        if ( currentStats.hp > maxStats.hp ) currentStats.hp = maxStats.hp;
        if ( currentStats.mp > maxStats.mp ) currentStats.mp = maxStats.mp;

        CheckDeath();
    }

    public void AddBuff(BuffSC s){
        if ( s.skillType == SkillType.buff || s.skillType == SkillType.debuff ){
            BuffSC buff = (BuffSC) buffs.Where<BuffSC>(b => b.skill.id == s.skill.id);
            if ( buff == null ){
                s.startTime = Time.time;
                if ( s.skillType == SkillType.buff ) buffs.Add(s);
                else debuffs.Add(s);
                UpdateBuffs();
            } else if ( buff.skill.cooldown != 0f ){
                if ( buff.skill.Level == s.skill.Level )
                    buff.startTime = Time.time;
                else if ( buff.skill.Level < s.skill.Level ){
                    buff = s;
                    buff.startTime = Time.time;
                    UpdateBuffs();
                }
            }
        }
    }
    public void RemoveBuff(BuffSC s){
        if ( s.skillType == SkillType.buff ){
            BuffSC buff = (BuffSC) buffs.Where<BuffSC>(b => b.skill.id == s.skill.id);
            if ( buff == null ){
                DebugWindow.Log("Cannot find buff, '"+s.skill.name+"'");
            } else {
                buffs.Remove(buff);
                UpdateBuffs();
            }
        }
    }
    public void RemoveDebuff(BuffSC s){
        if ( s.skillType == SkillType.debuff ){
            BuffSC debuff = (BuffSC) debuffs.Where<BuffSC>(b => b.skill.id == s.skill.id);
            if ( debuff == null ){
                DebugWindow.Log("Cannot find buff, '"+s.skill.name+"'");
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

    public void Cast(Skill s){
        if ( castingSkill == null ){
            if ( !s.isPassive ||
                 s.targetType == TargetType.enemy && characterObject.GetTarget() != null && !characterObject.GetTarget().isFriendly ||
                 s.targetType == TargetType.friendly && characterObject.GetTarget() != null && characterObject.GetTarget().isFriendly || 
                 s.targetType == TargetType.self ||
                 s.isAoe ||
                 s.isToggle ){
                
                if ( s.isAoe )
                    characterObject.UpdateTargets(s);

                castingSkill = s;
                characterObject.SetState(CharacterState.cast);
            }
        }
    }

    public virtual void UpdateStats(){
        
    }
    public virtual void UpdateBuffs(){
        UpdateStats();

        Stats sumOfStats = new Stats(1f);
        foreach (BuffSC s in buffs){
            sumOfStats += s.stats;   
        }
        foreach (BuffSC s in debuffs){
            sumOfStats *= s.stats;
        }

        maxStats += maxStats*sumOfStats;
    }
    public virtual void UpdatePassives(){
        foreach (Skill s in skills){
            if ( s.isPassive ){
                s.Apply(this,null);
            }
        }
    }
    #endregion
    #region Protected Methods
    protected virtual void Death(){
        characterObject.GetTargetManager().RemoveCharacterObject(characterObject);
        characterObject.SetState(CharacterState.death);
    }
    #endregion
    #region Private Methods
    private void CheckDeath(){
        if ( currentStats.hp < 1 ){
            Death();
        }
    }
    private void CheckBuffs(){
        foreach (BuffSC bsc in buffs){
            if ( bsc.isDone ){
                RemoveBuff(bsc);
            }
        }
        foreach (BuffSC bsc in debuffs){
            if ( bsc.isDone ){
                RemoveDebuff(bsc);
            }
        }
    }
    #endregion
}
