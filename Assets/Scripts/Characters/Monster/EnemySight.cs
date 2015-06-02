using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SphereCollider))]
public class EnemySight : MonoBehaviour {
    
    [HideInInspector] public bool targetInSight;
    [HideInInspector] public Character target = null;
    
    void OnTriggerStay(Collider other){
        if ( target == null ){
            Vector3 direction = other.transform.position - transform.parent.position;
            RaycastHit hit;

            if ( Physics.Raycast(transform.parent.position, direction, out hit) ){
                Character c = other.GetComponent<CharacterObject>().GetCharacter();
                if ( hit.collider == other && transform.parent.tag != other.tag && c.IsAlive && !c.characterObject.inSafeZone ){
                    target = c;
                    targetInSight = true;
                }
            } 
        } else {
            if ( !target.IsAlive || target.characterObject.inSafeZone ){
                target = null;
                targetInSight = false;
            }
        }
    }

    void OnTriggerExit(Collider other){
        CharacterObject c = other.GetComponent<CharacterObject>();
        if ( c != null ){
            if ( c.GetCharacter() == target ){
                target = null;
                targetInSight = false;
            }
        }
    }

    public void SetTarget(Character c){
        target = c;
        targetInSight = true;
    }
}
