using UnityEngine;
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

    public Monster GetMonster(string id){
        string part1 = id.Split('.')[0];
        switch (part1.ToLower()){
        case "tutorial":
        foreach (Monster m in tutorial){
            if ( m.id == id ) return m;
        }
        break;
        }
        return null;
    }
}
