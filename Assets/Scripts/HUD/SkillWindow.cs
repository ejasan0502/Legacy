using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SkillWindow : MonoBehaviour {
    
    public Text lvlExpText;
    public Text skillPointsText;
    public RectTransform skillInfo;

    private Player p;

    void Awake(){
        p = Game.GetPlayer();
    }

    void OnEnable(){
        UpdateDisplay();
    }

    public void UpdateDisplay(){
        lvlExpText.text = "Class Level: " + p.classLevel + " - " + p.classExp + " / " + p.maxClassExp;
        skillPointsText.text = "Class Points: " + p.classPoints;   
    }

    public void Learn(string id){
        p.AddSkill(Game.GetSkillData().GetSkill(id));
    }

    public void LevelUp(string id){
        p.classPoints--;
        p.GetSkill(id).LevelUp();
    }

    public void HideSkillInfo(){
        skillInfo.gameObject.SetActive(false);
    }

    public void DisplaySkillInfo(string id){
        if ( !skillInfo.gameObject.activeSelf ) skillInfo.gameObject.SetActive(true);

        Skill s = Game.GetSkillData().GetSkill(id);

        string descriptionText = "";
        descriptionText += "Required Class Level: " + s.reqLevel + "\n";
        switch(s.skillType){
        case SkillType.aoe:
        descriptionText += "Aoe\n";
        break;
        case SkillType.buff:
        descriptionText += "Buff\n";
        break;
        case SkillType.passive:
        descriptionText += "Passive\n";
        break;
        case SkillType.singleTarget:
        descriptionText += "Single Target\n";
        break;
        case SkillType.summon:
        descriptionText += "Summon\n";
        break;
        }

        string description = s.description;
        if ( description.Contains("{X}") ){
            float percent = 0f;
            if ( s.stats.meleeMinDmg > 0 ){
                percent = s.stats.meleeMinDmg;
            } else if ( s.stats.rangeMinDmg > 0 ){
                percent = s.stats.rangeMinDmg;
            } else {
                percent = s.stats.magicMinDmg;
            }
            description = description.Replace("{X}",percent+"");
        }

        descriptionText += description + "\n";
        foreach (string ss in s.reqs){
            descriptionText += "Requires " + Game.GetSkillData().GetSkill(ss).name + "\n";
        }

        skillInfo.GetChild(0).GetComponent<Text>().text = s.name;
        skillInfo.GetChild(1).GetComponent<Text>().text = s.level+"";
        skillInfo.GetChild(2).GetComponent<Text>().text = descriptionText;
        
        Button btn = skillInfo.GetChild(3).GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        if ( p.HasSkill(s.id) ){
            if ( p.GetSkill(s.id).CanLevelUp(p) ){
                btn.transform.GetChild(0).GetComponent<Text>().text = "Level Up";
                btn.onClick.AddListener(() => LevelUp(s.id));
            } else {
                btn.gameObject.SetActive(false);
            }
        } else if ( s.CanLearn(p) ){
            btn.transform.GetChild(0).GetComponent<Text>().text = "Learn";
            btn.onClick.AddListener(() => Learn(s.id));
        } else {
            btn.gameObject.SetActive(false);
        }
    }
}
