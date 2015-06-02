using UnityEngine;
using System.Collections;

public class SafeZone : MonoBehaviour {

    void OnTriggerEnter(Collider other){
        if ( other.GetComponent<CharacterObject>() != null ){
            CharacterObject co = other.GetComponent<CharacterObject>();
            co.inSafeZone = true;

            if ( co.c.IsPlayer ){
                Player p = co.c as Player;
                p.lastSafeZone = name;
            }
        }
    }

    void OnTriggerExit(Collider other){
        if ( other.GetComponent<CharacterObject>() != null ){
            other.GetComponent<CharacterObject>().inSafeZone = false;
        }
    }
}
