using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;

public class MoveToLastSafeZone : MonoBehaviour {
    
    private SmartFox smartfox;

    void Start(){
        PlayerObject playerObject = Game.GetPlayerObject();
        if ( playerObject != null ){
            string s = Game.GetPlayer().lastSafeZone;
            if ( s == "" ){
                s = "Default";
            }
            GameObject o = GameObject.Find(s);
            if ( o != null ){
                playerObject.gameObject.transform.position = o.transform.position;
                Destroy(gameObject);
            } else {
                Console.Log(s + " safe zone cannot be found");
            }
        } else {
            Console.Log("Player object is null");
        }
    }

}
