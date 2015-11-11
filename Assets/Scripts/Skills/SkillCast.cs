using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

// Handles multiple skill types of 1 skill
public class SkillCast {
    public Skill skill;
    public SkillType skillType;

    public SkillCast(){
        skill = null;
        skillType = SkillType.damage;
    }

    public virtual void Apply(Character caster, Character target){}
    public virtual void UpdateSelf(){}
}

public class DamageSC : SkillCast {
    
    public Stats baseStats;
    public Stats stats;

    public DamageSC(Skill _skill, Stats _baseStats){
        skill = _skill;
        skillType = SkillType.damage;

        baseStats = _baseStats;
        UpdateSelf();
    }

    public override void Apply(Character caster, Character target){
        float inflictDmg = caster.currentStats.baseDmg*caster.currentStats.skillDmgMultiplier;
        if ( Random.Range(0f,1000.00f) <= caster.currentStats.critChance*1000.00f ){
            target.MagicHit(inflictDmg, caster.currentStats.defPen);   
        } else {
            target.CriticalMagicHit(inflictDmg, caster.currentStats.defPen);
        }
    }
    public override void UpdateSelf(){
        FieldInfo[] fields = stats.GetType().GetFields();
        FieldInfo[] fields2 = baseStats.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            float val = (float)fields2[i].GetValue(baseStats);
            fields[i].SetValue(stats,val*skill.Level*0.125f);
        }
    }
}
public class BuffSC : SkillCast {
    public Stats baseStats;
    public Traits baseTraits;
    public float duration;
    public float startTime;

    public Stats stats;
    public Traits traits;

    public bool isDone {
        get {
            return Time.time - startTime >= duration;
        }
    }

    public BuffSC(Skill _skill, Stats _baseStats, Traits _baseTraits, float _duration){
        skill = _skill;
        skillType = SkillType.buff;

        baseStats = _baseStats;
        baseTraits = _baseTraits;
        duration = _duration;
        startTime = 0f;

        UpdateSelf();
    }
    public override void Apply(Character caster, Character target){
        caster.AddBuff(this);
    }
    public override void UpdateSelf(){
        FieldInfo[] fields = stats.GetType().GetFields();
        FieldInfo[] fields2 = baseStats.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            float val = (float)fields2[i].GetValue(baseStats);
            fields[i].SetValue(stats,val*skill.Level*0.125f);
        }
        
        fields = traits.GetType().GetFields();
        fields2 = baseTraits.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            float val = (float)fields2[i].GetValue(baseTraits);
            fields[i].SetValue(traits,val*skill.Level*0.125f);
        }
    }
}
public class StatusSC : SkillCast {
    public Status status;
    public float duration;
    public float dmgInflict;
    public float chance;

    public StatusSC(Skill _skill, Status _status, float _duration, float _dmgInflict, float _chance){
        skill = _skill;
        skillType = SkillType.status;

        status = _status;
        duration = _duration;
        dmgInflict = _dmgInflict;
        chance = _chance;
    }

    public override void Apply(Character caster, Character target){
        if ( Random.Range(0f,1000.00f) <= chance*1000.00f ){
            target.GetStatusManager().AddStatus(status, duration, dmgInflict);
        }
    }
    public override void UpdateSelf(){
        
    }
}
public class HealSC : SkillCast {

    public float baseHp;
    public float baseMp;
    public float hp;
    public float mp;
    public bool percent;

    public HealSC(Skill _skill, float _baseHp, float _baseMp, bool _percent){
        skill = _skill;
        skillType = SkillType.heal;

        baseHp = _baseHp;
        baseMp = _baseMp;
        percent = _percent;

        UpdateSelf();
    }

    public override void Apply(Character caster, Character target){
        target.Heal(hp,mp,percent);
    }
    public override void UpdateSelf(){
        hp = baseHp*skill.Level*0.125f;
        mp = baseMp*skill.Level*0.125f;
    }
}
public class CureSC : SkillCast {
    public List<Status> statuses;

    public CureSC(Skill _skill, List<Status> _statuses){
        skill = _skill;
        skillType = SkillType.cure;

        statuses = _statuses;
    }

    public void AddStatus(params Status[] s){
        foreach (Status ss in s){
            if ( !statuses.Contains(ss) )
                statuses.Add(ss);
        }
    }
    public void AddStatus(List<Status> s){
        foreach (Status ss in s){
            if ( !statuses.Contains(ss) )
                statuses.Add(ss);
        }
    }

    public override void Apply(Character caster, Character target){
        target.GetStatusManager().RemoveStatus(statuses);
    }
    public override void UpdateSelf(){
        
    }
}
public class ChargeSC : SkillCast {

    public float basePercentIncrease;
    public float percentIncrease;
    public int maxCharge;
    public float chargeRate;

    public float currentPercent;
    public bool MaxCharged {
        get {
            return currentPercent >= maxCharge*percentIncrease;
        }
    }

    public ChargeSC(Skill _skill, float _basePercentIncrease, float _chargeRate){
        skill = _skill;
        skillType = SkillType.charge;

        basePercentIncrease = _basePercentIncrease;
        chargeRate = _chargeRate;
        maxCharge = 1;
        currentPercent = 0f;

        UpdateSelf();
    }

    public void Charge(){
        if ( !MaxCharged ){
            currentPercent += percentIncrease;
        }
    }
    public void Release(){
        currentPercent = 0f;
    }
    public override void Apply(Character caster, Character target){
        caster.GetStatusManager().AddStatus(this);
    }
    public override void UpdateSelf(){
        percentIncrease = basePercentIncrease*skill.Level*0.125f;
        if ( skill.Level%3 == 0 && maxCharge < 3 ){
            maxCharge = skill.Level/3;
        }
    }
}
public class AoeSC : SkillCast {

    public float baseRadius;
    public float radius;

    public AoeSC(Skill _skill, float _baseRadius){
        skill = _skill;
        skillType = SkillType.status;
        baseRadius = _baseRadius;
        radius = 0f;

        UpdateSelf();
    }

    public override void Apply(Character caster, Character target){
        
    }
    public override void UpdateSelf(){
        if ( skillType == SkillType.aoeAround || skillType == SkillType.aoeWithin ){
            radius = 0.125f*skill.Level*baseRadius;
        }   
    }
}
public class ToggleSC : SkillCast {
    public ToggleSC(Skill _skill){
        skill = _skill;
        skillType = SkillType.toggle;
    }
    
    public override void Apply(Character caster, Character target){
        caster.GetStatusManager().AddStatus(this);
    }
    public override void UpdateSelf(){
        
    }
}
public class SummonSC : SkillCast {

    public CharacterObject creature;
    public int maxCreatures;
    public float baseDuration;
    public float duration;
    
    public List<SummonedCreature> summonedCreatures;

    public SummonSC(Skill _skill, CharacterObject _creature, int _maxCreatures, float _baseDuration){
        skill = _skill;
        skillType = SkillType.summon;

        creature = _creature;
        maxCreatures = _maxCreatures;
        baseDuration = _baseDuration;

        UpdateSelf();
    }

    public override void Apply(Character caster, Character target){
        if ( summonedCreatures.Count < maxCreatures ){
            CharacterObject o = MonoBehaviour.Instantiate(creature);
            SummonedCreature sc = o.gameObject.AddComponent<SummonedCreature>();
            sc.Initialize(duration, skill);
            summonedCreatures.Add(sc);
        }
    }
    public override void UpdateSelf(){
        duration = 0.125f*baseDuration*skill.Level;
        maxCreatures = 1;
    }
}
public class PassiveSC : SkillCast {

    public Stats baseStats;
    public Traits baseTraits;
    public bool percent;
    public Stats stats;
    public Traits traits;

    public PassiveSC(Skill _skill, Stats _baseStats, Traits _baseTraits, bool _percent){
        skill = null;
        skillType = SkillType.status;

        baseStats = _baseStats;
        baseTraits = _baseTraits;
        percent = _percent;

        UpdateSelf();
    }

    public override void Apply(Character caster, Character target){
        if ( caster.isPlayer ){
            PlayerCharacter p = (PlayerCharacter) caster;
            p.UpdateTraits();
            p.traits *= traits;

            p.UpdateStats();
            p.maxStats *= stats;

            p.UpdateBuffs();
        }
    }
    public override void UpdateSelf(){
        stats = baseStats*0.125f*skill.Level;
        traits = baseTraits*0.125f*skill.Level;
    }
}
public class _EmptySkillCast : SkillCast {
    public _EmptySkillCast(){
        skill = null;
        skillType = SkillType.status;
    }

    public override void Apply(Character caster, Character target){
        
    }
    public override void UpdateSelf(){
        
    }
}
