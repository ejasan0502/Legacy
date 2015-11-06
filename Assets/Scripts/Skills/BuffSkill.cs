using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class BuffSkill : Skill {

    public Stats baseStats;
    public Traits baseTraits;
    public Stats stats;
    public Traits traits;
    public float duration;

    private float startTime;

    public override Skill GetAsSkillType() {
        return this;
    }
    public override void Apply(Character caster, Character target){
        throw new System.NotImplementedException();
    }
    protected override void UpdateSelf(){
        base.UpdateSelf();

        FieldInfo[] f1 = stats.GetType().GetFields();
        foreach (FieldInfo fi in f1){
            fi.SetValue(this, baseStats*(1+0.125f*(level-1)+0.125f*((level-1)/10f)*2f) );
        }

        f1 = traits.GetType().GetFields();
        foreach (FieldInfo fi in f1){
            fi.SetValue(this, traits*(1+0.125f*(level-1)+0.125f*((level-1)/10f)*2f) );
        }
    }

    public void SetTime(float t){
        startTime = Time.time;
    }
    public bool CheckToEnd(){
        if ( Time.time - startTime >= duration ){
            return true;
        }
        return false;
    }
}
