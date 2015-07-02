using UnityEngine;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class MonsterData : MonoBehaviour {

    public List<Monster> tutorial;

    void Awake(){
        // Destroy duplicates
        MonsterData[] list = GameObject.FindObjectsOfType<MonsterData>();
        if ( list.Length > 1 ){
            // Found more than 1 object, meaning this instance is a duplicate
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    public void ExtractMonsterXmlData(){
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml( ((TextAsset)Resources.Load("data/RPG Monsters Xml")).text );

        XmlNode root = xmlDoc.DocumentElement;

        foreach (XmlNode n1 in root.ChildNodes){
            Monster e = new Monster();
            foreach (XmlNode n2 in n1.ChildNodes){
                if ( n2.InnerText != "" ){
                    switch(n2.Name){
                    case "name":
                    e.name = n2.InnerText;
                    break;
                    case "id":
                    e.id = n2.InnerText;
                    break;
                    case "level":
                    e.level = int.Parse(n2.InnerText);
                    break;
                    case "description":
                    e.description = n2.InnerText;
                    break;
                    case "model":
                    e.model = Resources.Load(n2.InnerText+e.name,typeof(GameObject)) as GameObject;
                    if ( e.model == null ) e.model = Resources.Load("monsters/Default",typeof(GameObject)) as GameObject;
                    break;
                    case "stats":
                    FieldInfo[] fields = e.stats.GetType().GetFields();

                    string[] stats = n2.InnerText.Split(',');
                    foreach (string ss in stats){
                        if ( ss != "" ){
                            string statName = ss.Split('-')[0];
                            string statValue = ss.Split('-')[1];

                            foreach (FieldInfo f in fields){
                                if ( f.Name == statName ){
                                    f.SetValue(e.stats,float.Parse(statValue));
                                    break;
                                }
                            }
                        }
                    }
                    break;
                    case "skills":
                    string[] skillList = n2.InnerText.Split(',');
                    foreach (string s in skillList){
                        if ( s != "" ){
                            string skillId = s.Split('-')[0];
                            int skillLvl = int.Parse(s.Split('-')[1]);

                            Skill sk = Game.GetSkillData().GetSkill(skillId);
                            if ( sk != null ){
                                sk.level = skillLvl;
                                e.skills.Add(sk);
                            } else {
                                Console.Error("MonsterData.cs - ExtractMonsterXmlData(): Skill id does not exist! " + skillId);
                            }
                        }
                    }
                    break;
                    case "sightRange":
                    e.sightRange = float.Parse(n2.InnerText);
                    break;
                    case "atkDistance":
                    e.atkDistance = float.Parse(n2.InnerText);
                    break;
                    case "exp":
                    e.exp = float.Parse(n2.InnerText);
                    break;
                    case "drops":
                    string[] dropList = n2.InnerText.Split(',');
                    foreach (string s in dropList){
                        if ( s != "" ){
                            string t = s.Split('.')[0];
                            string id = s.Split('.')[1];

                            if ( id != "" ){
                                Tier tier = Tier.common;
                                if ( t == "r" ) tier = Tier.rare;
                                else if ( t == "u" ) tier = Tier.unique;
                                else if ( t == "l" ) tier = Tier.legendary;
                                else if ( t == "g" ) tier = Tier.godly;

                                e.drops.Add(new Drop(id,tier));
                            }
                        }
                    }
                    break;
                    case "currency":
                    e.currency = float.Parse(n2.InnerText);
                    break;
                    }
                }
            }

            e.currentStats = new Stats(e.stats);

            // Add to lists
            string region = e.id.Split('-')[0];
            int index = int.Parse(e.id.Split('-')[1]);
            if ( region == "tutorial" ) tutorial.Insert(index,e);
        }

        Console.System("Monsters Xml Data extracted.");
    }

    public Monster GetMonster(string id){
        string part1 = id.Split('-')[0];
        switch (part1.ToLower()){
        case "tutorial":
        foreach (Monster m in tutorial){
            if ( m.id == id ) return m;
        }
        break;
        }
        Console.Error("MonsterData.cs - GetMonster(string): Returned a null value. Id value = " + id);
        return null;
    }
}
