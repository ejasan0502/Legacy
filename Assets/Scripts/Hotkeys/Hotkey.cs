using UnityEngine;
using System.Collections;

public class Hotkey {

    public float cdStartTime;

    public virtual bool CanApply(){
        return false;
    }
    public virtual void ApplyHotkey(Player p, int index){
        
    }
    public virtual Sprite GetIcon(){
        return null;
    }
    public virtual float GetCD(){
        return 0f;
    }
    public virtual bool IsSkill {
        get {
            return false;
        }
    }

    public virtual UsableHotkey GetAsUsableHotkey(){
        return null;
    }

    public virtual SkillHotkey GetAsSkillHotkey(){
        return null;
    }

}
