using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// Handles potential surrounding targets
[RequireComponent(typeof(CircleCollider2D))]
public class TargetManager : MonoBehaviour {

    private List<CharacterObject> surroundingCharacters;
    private CircleCollider2D circleCollider2D;

    void Start(){
        surroundingCharacters = new List<CharacterObject>();
        circleCollider2D = GetComponent<CircleCollider2D>();
    }

    void OnTriggerEnter(Collider other){
        if ( other.gameObject.layer == LayerMask.NameToLayer("Character") ){
            CharacterObject co = other.GetComponent<CharacterObject>();
            if ( co != null && co.character.isAlive )
                surroundingCharacters.Add(co);
        }
    }
    void OnTriggerExit(Collider other){
        if ( other.gameObject.layer == LayerMask.NameToLayer("Character") ){
            CharacterObject co = other.GetComponent<CharacterObject>();
            if ( co != null && surroundingCharacters.Contains(co) )
                surroundingCharacters.Remove(co);
        }
    }

    public void RemoveCharacterObject(CharacterObject co){
        if ( surroundingCharacters.Contains(co) )
            surroundingCharacters.Remove(co);
    }

    public Character GetClosestFrontTarget(){
        return GetClosestFrontTarget(circleCollider2D.radius);
    }
    public Character GetClosestFrontTarget(float maxDistance){
        Ray ray = new Ray(transform.position,transform.forward-transform.position);
        RaycastHit hit;
        if ( Physics.Raycast(ray, out hit, maxDistance, LayerMask.NameToLayer("Character")) ){
            return hit.transform.GetComponent<CharacterObject>().character;
        }
        return null;
    }
    public List<Character> GetFrontTargetsWithin(float maxDistance){
        List<Character> targets = new List<Character>();
        RaycastHit[] hits = Physics.RaycastAll(transform.position,transform.forward-transform.position,maxDistance,LayerMask.NameToLayer("Character"));
        for (int i = 0; i < hits.Length; i++){
            targets.Add(hits[i].transform.GetComponent<CharacterObject>().character);
        }
        return targets;
    }
    public List<Character> GetTargetsWithin(float radius){
        List<Character> targets = new List<Character>();
        foreach (CharacterObject co in surroundingCharacters){
            if ( Vector2.Distance(transform.position,co.transform.position) <= radius ){
                targets.Add(co.character);
            }
        }
        return targets;
    }
    public List<Character> GetTargetsAround(Vector2 position, float radius){
        List<Character> targets = new List<Character>();
        foreach (CharacterObject co in surroundingCharacters){
            if ( Vector2.Distance(position,co.transform.position) <= radius ){
                targets.Add(co.character);
            }
        }
        return targets;
    }

}
