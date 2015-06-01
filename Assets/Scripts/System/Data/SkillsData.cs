using UnityEngine;
using System.IO;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class SkillsData : MonoBehaviour {
    
    #region Variables
    public string path = "data/RPG Skills Xml";
    public List<Skill> novice = new List<Skill>();
    #endregion

    public Skill GetSkill(string id){
        string part1 = id.Split('-')[0];

        List<Skill> skills = null;
        switch(part1){
        #region Tier 0
        case "novice":
        skills = novice;
        break;
        #endregion
        }

        if ( skills != null ){
            foreach (Skill ss in skills){
                if ( ss.id == id ){
                    return ss;
                }
            }
        }

        return null;
    }

    public void ExtractXmlData(){
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml( ((TextAsset)Resources.Load(path)).text );

        XmlNode root = xmlDoc.DocumentElement;

        foreach (XmlNode n1 in root.ChildNodes){
            Skill s = new Skill();
            foreach (XmlNode n2 in n1.ChildNodes){
                if ( n2.InnerText != "" ){
                    switch (n2.Name){
                    case "name":
                    s.name = n2.InnerText;
                    break;
                    case "id":
                    s.id = n2.InnerText;
                    break;
                    case "levelReq":
                    s.reqLevel = int.Parse(n2.InnerText);
                    break;
                    case "icon":
                    s.icon = Resources.Load(n2.InnerText+s.name,typeof(Sprite)) as Sprite;
                    if ( s.icon == null ) s.icon = Resources.Load("skills/icons/Default",typeof(Sprite)) as Sprite;
                    break;
                    case "description":
                    s.description = n2.InnerText;
                    break;
                    case "skillType":
                    if ( n2.InnerText == "buff" ){
                        s.skillType = SkillType.buff;
                    } else if ( n2.InnerText == "passive" ){
                        s.skillType = SkillType.passive;
                    } else if ( n2.InnerText == "singleTarget" ){
                        s.skillType = SkillType.singleTarget;
                    } else if ( n2.InnerText == "aoe" ){
                        s.skillType = SkillType.aoe;
                    } else if ( n2.InnerText == "summon" ){
                        s.skillType = SkillType.summon;
                    }
                    break;
                    case "cd":
                    s.cd = float.Parse(n2.InnerText);
                    break;
                    case "stats":
                    FieldInfo[] fields = s.stats.GetType().GetFields();

                    string[] stats = n2.InnerText.Split(',');
                    foreach (string ss in stats){
                        if ( ss != "" ){
                            string statName = ss.Split('-')[0];
                            string statValue = ss.Split('-')[1];

                            foreach (FieldInfo f in fields){
                                if ( f.Name == statName ){
                                    f.SetValue(s.stats,float.Parse(statValue));
                                }
                            }
                        }
                    }
                    break;
                    case "reqs":
                    string[] reqs = n2.InnerText.Split(',');
                    foreach (string ss in reqs){
                        s.reqs.Add(ss);
                    }
                    break;
                    }
                }
            }

            if ( s.id.Split('-')[0] == "novice" ){
                novice.Add(s);
            }
        }

        Console.Log("Skills Xml Data extracted");
    }
}
