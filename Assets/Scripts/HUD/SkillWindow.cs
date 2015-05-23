using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SkillWindow : MonoBehaviour {
    
    public RectTransform contentsRectTransform;
    public Text lvlExpText;
    public Text skillPointsText;

    private Player p;
    private List<GameObject> skillObjects = new List<GameObject>();

    void Awake(){
        p = Game.GetPlayer();
        UpdateDisplay();
    }

    void OnEnable(){
        UpdateDisplay();
    }

    public void LevelUp(int x){

    }

    public void UpdateDisplay(){
        // Clear objects list
        if ( skillObjects.Count > 0 ){
            foreach (GameObject o in skillObjects){
                Destroy(o);
            }

            skillObjects = new List<GameObject>();
        }

        transform.GetChild(2).GetComponent<Text>().text = "Class Level: " + p.classLevel + " - " + p.classExp + " / " + p.maxClassExp;
        transform.GetChild(3).GetComponent<Text>().text = "Skill Points: " + p.classPoints;

        // Fill objects list
        Vector3 startPos = new Vector3(-contentsRectTransform.rect.width/2.0f,10f,0f);      // Based on local
        float maxWidth = 0f;

        for (int i = 0; i < p.skills.Count; i++){
            GameObject o = Instantiate(Resources.Load("Skill Info")) as GameObject;
            RectTransform rt = o.transform as RectTransform;

            o.transform.localScale = new Vector3(1f,1f,1f);
            o.transform.localPosition = new Vector3(startPos.x+rt.rect.width*i,startPos.y,startPos.z);

            o.transform.GetChild(0).GetComponent<Text>().text = p.skills[i].name;
            o.transform.GetChild(1).GetComponent<Image>().sprite = p.skills[i].icon;

            string descriptionText = "";
            float maxHeight = 90f;
            descriptionText += "Required Class Level: " + p.skills[i].reqLevel + "\n";
            switch(p.skills[i].skillType){
            case SkillType.aoe:
            descriptionText += "Aoe";
            break;
            case SkillType.buff:
            descriptionText += "Buff";
            break;
            case SkillType.passive:
            descriptionText += "Passive";
            break;
            case SkillType.singleTarget:
            descriptionText += "Single Target";
            break;
            case SkillType.summon:
            descriptionText += "Summon";
            break;
            }

            string description = p.skills[i].description;
            if ( description.Contains("{X}") ){
                float percent = 0f;
                if ( p.skills[i].stats.meleeMinDmg > 0 ){
                    percent = p.skills[i].stats.meleeMinDmg;
                } else if ( p.skills[i].stats.rangeMinDmg > 0 ){
                    percent = p.skills[i].stats.rangeMinDmg;
                } else {
                    percent = p.skills[i].stats.magicMinDmg;
                }
                description.Trim().Replace("{X}",percent+"");
            }

            descriptionText += p.skills[i].description + "\n";
            foreach (Skill s in p.skills[i].reqs){
                descriptionText += "Requires " + s.name + "\n";
                maxHeight += 30f;
            }
            o.transform.GetChild(2).GetChild(0).GetComponent<Text>().text = descriptionText;
            RectTransform descriptionRt = o.transform.GetChild(2).GetChild(0) as RectTransform;
            descriptionRt.sizeDelta = new Vector2(descriptionRt.sizeDelta.x,maxHeight);

            if ( p.skills[i].CanLevelUp(p) )
                o.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(() => LevelUp(i));
            else
                o.transform.GetChild(3).gameObject.SetActive(false);

            o.transform.SetParent(contentsRectTransform);

            skillObjects.Add(o);

            maxWidth += rt.rect.width;
        }

        // Adjust Contents Rect
        contentsRectTransform.sizeDelta = new Vector2(maxWidth,contentsRectTransform.sizeDelta.y);
    }
}
