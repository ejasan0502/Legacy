using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoxCollider))]
public class MonsterSpawnPoint : MonoBehaviour {
    
    public float spawnRate = 3f;
    public int maxSpawnCount = 10;
    public string monsterId;
    public string monsterTag;
    public Monster monster;

    private List<CharacterObject> monsters = new List<CharacterObject>();

    void OnEnable(){
        StartCoroutine("Spawning");
    }

    void OnDisable(){
        StopCoroutine("Spawning");
    }

    private IEnumerator Spawning(){
        while(true){
            yield return new WaitForSeconds(spawnRate);
            CheckMonstersList();
            if ( monsters.Count < maxSpawnCount && Game.GameDataLoaded ){
                CharacterObject co = null;
                Monster m = null;
                if ( monsterId == "" ) m = new Monster(monster);
                else m = new Monster(Game.GetMonsterData().GetMonster(monsterId));

                co = Game.CreateCharacter(m,monsterTag,transform.position);
                monsters.Add(co);
            }
        }
    }

    private void CheckMonstersList(){
        for (int i = 0; i < monsters.Count; i++){
            if ( monsters[i] == null ) monsters.RemoveAt(i);
        }
    }
}
