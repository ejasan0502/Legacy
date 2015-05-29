using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class NPC {
    public string name;
    public string id;
    public Sprite portrait;
    public NPCType npcType;
    public string[] buyList;
    public string[] craftList;
    public string[] questList;

    public NPCObject npcObject;

    public Item[] GetItemsList(){
        Item[] items = new Item[buyList.Length];
        
        for (int i = 0; i < items.Length; i++){
            items[i] = Game.GetGameData().GetItem(buyList[i]);
            if ( items[i] == null ){
                Console.Error("NPC.cs - GetItemsList(): buyList index " + i + " with id of " + buyList[i] + " is null");
            }
        }

        return items;
    }
    public Item[] GetCraftsList(){
        Item[] items = new Item[craftList.Length];
        
        for (int i = 0; i < items.Length; i++){
            items[i] = Game.GetGameData().GetItem(craftList[i]);
        }

        return items;
    }
    public List<Quest> GetQuestList(Player p){
        List<Quest> quests = new List<Quest>();

        for (int i = 0; i < quests.Count; i++){
            if ( !p.HasQuest(questList[i]) )
                quests.Add(Game.GetQuestData().GetQuest(questList[i]));
        }
        return quests;
    }
}

public enum NPCType {
    civilian,
    blacksmith,
    alchemist,
    tailor,
    merchant,
    transporter
}
