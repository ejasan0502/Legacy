using UnityEngine;
using System.Collections;

public class SkillHotkey : Hotkey {

    private Skill skill;

    public SkillHotkey(Skill s){
        skill = s;
    }

    public override void ApplyHotkey(Player p){
        skill.Cast(p);
    }   

}
