using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCData : MonoBehaviour {

    public List<NPC> tutorial;

    public NPC GetNPC(string id){
        string part1 = id.Split('.')[0];
        switch (part1.ToLower()){
        case "tutorial":
        foreach (NPC m in tutorial){
            if ( m.id == id ) return m;
        }
        break;
        }
        return null;
    }
}
