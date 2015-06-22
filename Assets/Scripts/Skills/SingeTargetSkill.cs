using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SingleTargetSkill : Skill {
    
    public bool instant;
    public bool friendly;
    public string atkType;
    public float castDelay;

    public SingleTargetSkill(){
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
        instant = false;
        friendly = false;
        atkType = "physical";
        castDelay = 0f;
    }
    public SingleTargetSkill(Skill s){
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
        instant = false;
        friendly = false;
        atkType = "physical";
        castDelay = 0f;
    }

    public override bool CanCast(Character p){
        Console.System("SingleTargetSkill.cs - CanCast(Character)");

        if ( p.currentStats.health < healthCost ) return false;
        if ( p.currentStats.mana < manaCost ) return false;

        if ( p.characterObject.GetTarget() == null ) return false;
        else {
            Character target = p.characterObject.GetTarget();
            bool friend = target.characterObject.IsFriendly(p.characterObject);
            if ( friend != friendly ) return false;
        }   

        return true;
    }

    public override void Cast(Character caster){
        Console.System("SingleTargetSkill.cs - Cast(Character)");
        if ( caster.characterObject.GetTarget() != null ){
            caster.SetCastSkill(this);
        }
    }

    public override void Apply(Character caster, Character target){
        Console.System("SingeTargetSkill.cs - Apply(Character,Character)");
        if ( atkType == "physical" ){
            float dmg = Random.Range(caster.currentStats.meleeMinDmg*stats.meleeMinDmg,caster.currentStats.meleeMaxDmg*stats.meleeMaxDmg);
            target.PhysicalHit(caster,dmg);
        } else if ( atkType == "magical" ){
            float dmg = Random.Range(caster.currentStats.magicMinDmg*stats.magicMinDmg,caster.currentStats.magicMaxDmg*stats.magicMaxDmg);
            target.MagicalHit(caster,dmg);
        } else if ( atkType == "ranged" ){
            float dmg = Random.Range(caster.currentStats.rangeMinDmg*stats.rangeMinDmg,caster.currentStats.rangeMaxDmg*stats.rangeMaxDmg);
            target.PhysicalHit(caster,dmg);
        }
    }
}
