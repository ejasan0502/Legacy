using UnityEngine;
using System.Collections;

public class SkillsData : MonoBehaviour {
    
    #region Variables
    public Skill[] novice;
    #endregion

    public Skill GetSkill(string id){
        string part1 = id.Split('-')[0];

        Skill[] skills = null;
        switch(part1){
        #region Tier 0
        case "novice":
        skills = novice;
        break;
        #endregion
        }

        foreach (Skill ss in skills){
            if ( ss.id == id )
                return ss;
        }

        return null;
    }
}
