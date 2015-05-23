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
        
        if ( items.Length > 0 ){
            for (int i = 0; i < items.Length; i++){
                items[i] = Game.GetGameData().GetItem(buyList[i]);
            }
            return items;
        } else {
            return null;
        }
    }
    public Item[] GetCraftsList(){
        Item[] items = new Item[craftList.Length];
        
        if ( items.Length > 0 ){
            for (int i = 0; i < items.Length; i++){
                items[i] = Game.GetGameData().GetItem(craftList[i]);
            }
            return items;
        } else {
            return null;
        }
    }
    public List<Quest> GetQuestList(Player p){
        List<Quest> quests = new List<Quest>();

        for (int i = 0; i < quests.Count; i++){
            if ( !p.HasQuest(questList[i]) )
                quests.Add(Game.GetQuestData().GetQuest(questList[i]));
        }
        return quests.Count > 0 ? quests : null;
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
