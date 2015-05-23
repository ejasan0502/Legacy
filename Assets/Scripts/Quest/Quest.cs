using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Quest {
    public string name;
    public Sprite icon;
    public string id;
    public string description;
    public QuestType questType;
    public QuestObjective[] questObjectives;
    public int levelReq;

    public Quest(Quest q){
        name = q.name;
        icon = q.icon;
        id = q.id;
        description = q.description;
        questType = q.questType;
        questObjectives = new QuestObjective[questObjectives.Length];
        for (int i = 0; i < questObjectives.Length; i++){
            questObjectives[i] = new QuestObjective(q.questObjectives[i]);
        }

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

    public QuestObjective(QuestObjective qo){
        id = qo.id;
        objective = qo.objective;
        amt = qo.amt;

        if ( objective == "Collect" ){
            item = Game.GetGameData().GetMaterial(id);
        } else if ( objective == "Kill" ){
            monster = Game.GetMonsterData().GetMonster(id);
        } else if ( objective == "Talk" ){
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

public enum QuestType {
    main,
    side,
    evt
}
