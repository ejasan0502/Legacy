using UnityEngine;
using System.Collections;

public class SkillHotkey : Hotkey {

    private Skill skill;

    public SkillHotkey(Skill s){
        skill = s;
    }

    public override bool CanApply(){
        if ( Time.time - cdStartTime >= skill.cd ){
            return true;
        }
        return false;
    }

    public override void ApplyHotkey(Player p, int index){
        Console.System("SkillHotkey.cs - ApplyHotkey(Player,int)");
        if ( skill.CanCast(p) ){
            //skill.Cast(p);
            HotkeyManager.instance.UpdateHotkeyDisplay(index);
        }
    }

    public override Sprite GetIcon(){
        return skill.icon;
    }

    public override float GetCD(){
        return skill.cd;
    }

    public override bool IsSkill{
        get{
            return true;
        }
    }

    public override SkillHotkey GetAsSkillHotkey(){
        return this;
    }

    public Skill GetSkill(){
        return skill;
    }

    public override string GetId(){
        return skill.id;
    }

}
