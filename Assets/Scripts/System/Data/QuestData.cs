using UnityEngine;
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
}
