using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BuffSkill : Skill {
    
    public BuffSkill(){
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
    }
    public BuffSkill(Skill s){
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
    }



}
