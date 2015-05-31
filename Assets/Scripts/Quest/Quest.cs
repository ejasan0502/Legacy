using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Quest {
    public string name;
    public Sprite icon;
    public string id;
    public string description;
    public string questType;
    public List<QuestObjective> questObjectives;
    public int levelReq;

    public Quest(){
        name = "";
        icon = null;
        id = "";
        description = "";
        questType = "";
        questObjectives = new List<QuestObjective>();
        levelReq = 1;
    }

    public Quest(Quest q){
        name = q.name;
        icon = q.icon;
        id = q.id;
        description = q.description;
        questType = q.questType;
        questObjectives = q.questObjectives;
        levelReq = q.levelReq;
    }

    public void DecrementAmount(string id){
        foreach (QuestObjective qo in questObjectives){
            if ( qo.id == id ){
                qo.amt--;
                return;
            }
        }
    }

    public void SetAmount(string id, int x){
        foreach (QuestObjective qo in questObjectives){
            if ( qo.id == id ){
                qo.amt = x;
                return;
            }
        }
    }

    public bool IsDone(){
        foreach (QuestObjective qo in questObjectives){
            if ( qo.amt > 0 ) return false;
        }
        return true;
    }
    
}

public class QuestObjective {
    public string id;
    public string objective;
    public int amt;

    private Monster monster = null;
    private Item item = null;
    private NPC npc = null;

    public QuestObjective(string i, string o, int a){
        id = i;
        objective = o;
        amt = a;

        if ( objective == "collect" ){
            item = Game.GetGameData().GetMaterial(id);
        } else if ( objective == "kill" ){
            monster = Game.GetMonsterData().GetMonster(id);
        } else if ( objective == "talk" ){
            npc = Game.GetNPCData().GetNPC(id);
        } else {
            Console.Log("Invalid objective");
        }
    }
    public QuestObjective(QuestObjective qo){
        id = qo.id;
        objective = qo.objective;
        amt = qo.amt;

        if ( objective == "collect" ){
            item = Game.GetGameData().GetMaterial(id);
        } else if ( objective == "kill" ){
            monster = Game.GetMonsterData().GetMonster(id);
        } else if ( objective == "talk" ){
            npc = Game.GetNPCData().GetNPC(id);
        } else {
            Console.Log("Invalid objective");
        }
    }

    public string GetObjectiveName(){
        if ( monster != null ) return monster.name;
        else if ( item != null ) return item.name;
        else if ( npc != null ) return npc.name;
        else Console.Log("Cannot find objective name");

        return "";
    }
}
