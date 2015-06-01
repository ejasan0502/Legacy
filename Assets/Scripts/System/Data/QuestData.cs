using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class QuestData : MonoBehaviour {
    
    public List<Quest> tutorial;

    public Quest GetQuest(string id){
        string part1 = id.Split('.')[0];
        switch (part1.ToLower()){
        case "tutorial":
        foreach (Quest q in tutorial){
            if ( q.id == id ) return q;
        }
        break;
        }
        return null;
    }

    public void ExtractQuestXmlData(){
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml( ((TextAsset)Resources.Load("data/RPG Quests Xml")).text );

        XmlNode root = xmlDoc.DocumentElement;

        foreach (XmlNode n1 in root.ChildNodes){
            Quest e = new Quest();
            foreach (XmlNode n2 in n1.ChildNodes){
                if ( n2.InnerText != "" ){
                    switch(n2.Name){
                    case "name":
                    e.name = n2.InnerText;
                    break;
                    case "id":
                    e.id = n2.InnerText;
                    break;
                    case "icon":
                    e.icon = Resources.Load(n2.InnerText,typeof(Sprite)) as Sprite;
                    if ( e.icon == null ) e.icon = Resources.Load("quest icons/Default",typeof(Sprite)) as Sprite;
                    break;
                    case "description":
                    e.description = n2.InnerText;
                    break;
                    case "questType":
                    e.questType = n2.InnerText;
                    break;
                    case "questObjectives":
                    string[] questObjectivesList = n2.InnerText.Split('|');
                    foreach (string qo in questObjectivesList){
                        if ( qo != "" ){
                            string[] parts = qo.Split(',');
                            e.questObjectives.Add(new QuestObjective(parts[0],parts[1],int.Parse(parts[2])));
                        }
                    }
                    break;
                    case "levelReq":
                    e.levelReq = int.Parse(n2.InnerText);
                    break;
                    }
                }
            }

            // Add to lists
            string region = e.id.Split('-')[0];
            int index = int.Parse(e.id.Split('-')[1]);
            if ( region == "tutorial" ) tutorial.Insert(index,e);
        }

        Console.Log("Quest Xml Data extracted.");
    }
}
