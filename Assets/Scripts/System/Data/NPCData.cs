using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;

public class NPCData : MonoBehaviour {

    public List<NPC> tutorial;

    public NPC GetNPC(string id){
        string part1 = id.Split('-')[0];
        switch (part1.ToLower()){
        case "tutorial":
        foreach (NPC m in tutorial){
            if ( m.id == id ) return m;
        }
        break;
        }
        Console.Error("NPCData.cs - GetNPC(string): Returned a null value. Id value = " + id);
        return null;
    }

    public void ExtractNPCXmlData(){
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml( ((TextAsset)Resources.Load("data/RPG NPCs Xml")).text );

        XmlNode root = xmlDoc.DocumentElement;

        foreach (XmlNode n1 in root.ChildNodes){
            NPC e = new NPC();
            foreach (XmlNode n2 in n1.ChildNodes){
                if ( n2.InnerText != "" ){
                    switch(n2.Name){
                    case "name":
                    e.name = n2.InnerText;
                    break;
                    case "id":
                    e.id = n2.InnerText;
                    break;
                    case "model":
                    e.model = Resources.Load(n2.InnerText+e.name,typeof(GameObject)) as GameObject;
                    if ( e.model == null ) e.model = Resources.Load("npcs/Default",typeof(GameObject)) as GameObject;
                    break;
                    case "npcType":
                    if ( n2.InnerText == "civillian" ) e.npcType = NPCType.civilian;
                    else if ( n2.InnerText == "alchemist" ) e.npcType = NPCType.alchemist;
                    else if ( n2.InnerText == "blacksmith" ) e.npcType = NPCType.blacksmith;
                    else if ( n2.InnerText == "merchant" ) e.npcType = NPCType.merchant;
                    else if ( n2.InnerText == "tailor" ) e.npcType = NPCType.tailor;
                    else if ( n2.InnerText == "transporter" ) e.npcType = NPCType.transporter;
                    break;
                    case "buyList":
                    e.buyList = n2.InnerText.Split(',');
                    break;
                    case "craftList":
                    e.craftList = n2.InnerText.Split(',');
                    break;
                    case "questList":
                    e.questList = n2.InnerText.Split(',');
                    break;
                    }
                }
            }

            // Add to lists
            string region = e.id.Split('-')[0];
            int index = int.Parse(e.id.Split('-')[1]);
            if ( region == "tutorial" ) tutorial.Insert(index,e);
        }

        Console.System("NPC Xml Data extracted.");
    }
}
