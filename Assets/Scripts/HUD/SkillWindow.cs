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

    public void HotkeySelect(Skill s){
        skillInfo.GetChild(6).gameObject.SetActive(true);

        for (int i = 0; i < skillInfo.GetChild(6).childCount; i++){
            Button btn = skillInfo.GetChild(6).GetChild(i).GetComponent<Button>();
            btn.onClick.RemoveAllListeners();

            int x = i;
            btn.onClick.AddListener(() => SetHotkey(x,s));
        }
    }

    public void SetHotkey(int index, Skill s){
        skillInfo.GetChild(6).gameObject.SetActive(false);
        Game.Notification("Assigned " + s.name + " to Hotkey " + (index+1),true);
        Game.GetPlayer().SetHotkey(index, new SkillHotkey(s));
    }

    public void Learn(string id){
        Skill s = Game.GetSkillData().GetSkill(id);
        p.AddSkill(s);
        skillInfo.GetChild(3).gameObject.SetActive(false);
        if ( s.skillType == SkillType.singleTarget ){
            Button hotkeyBtn = skillInfo.GetChild(5).GetComponent<Button>();
            hotkeyBtn.gameObject.SetActive(true);
            hotkeyBtn.onClick.RemoveAllListeners();
            hotkeyBtn.onClick.AddListener(() => HotkeySelect(s));
        }
    }
    public void LevelUp(string id){
        p.classPoints--;
        p.GetSkill(id).LevelUp();
    }
    public void HideSkillInfo(){
        skillInfo.gameObject.SetActive(false);
    }
    
    public void UpdateDisplay(){
        lvlExpText.text = "Class Level: " + p.classLevel + " - " + p.classExp + " / " + p.maxClassExp;
        skillPointsText.text = "Class Points: " + p.classPoints;   
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
            Skill skill = p.GetSkill(s.id);
            if ( s.CanLevelUp(p) ){
                btn.gameObject.SetActive(true);
                btn.transform.GetChild(0).GetComponent<Text>().text = "Level Up";
                btn.onClick.AddListener(() => LevelUp(s.id));
            } else {
                btn.gameObject.SetActive(false);
            }
            if ( skill.skillType == SkillType.singleTarget ){
                Button hotkeyBtn = skillInfo.GetChild(5).GetComponent<Button>();
                hotkeyBtn.gameObject.SetActive(true);
                hotkeyBtn.onClick.RemoveAllListeners();
                hotkeyBtn.onClick.AddListener(() => HotkeySelect(skill));
            }
        } else if ( s.CanLearn(p) ){
            btn.gameObject.SetActive(true);
            btn.transform.GetChild(0).GetComponent<Text>().text = "Learn";
            btn.onClick.AddListener(() => Learn(s.id));
        } else {
            btn.gameObject.SetActive(false);
        }
    }
}
