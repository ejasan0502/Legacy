using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;

public class NetworkManager : MonoBehaviour {
    private SmartFox smartfox;

    void Awake(){
        // Destroy duplicates
        NetworkManager[] list = GameObject.FindObjectsOfType<NetworkManager>();
        if ( list.Length > 1 ){
            // Found more than 1 object, meaning this instance is a duplicate
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        smartfox = Game.GetConnection();
    }

    void FixedUpdate(){
        if ( smartfox != null ) smartfox.ProcessEvents();
    }

    public void JoinRoom(string s){
        smartfox.RemoveAllEventListeners();
        smartfox.AddEventListener(SFSEvent.ROOM_JOIN,OnRoomJoin);
        smartfox.AddEventListener(SFSEvent.ROOM_JOIN_ERROR,OnRoomJoinError);
        smartfox.Send(new JoinRoomRequest(s));
    }

    public void SaveToDatabase(byte[] rawData){
        smartfox.RemoveAllEventListeners();
        smartfox.AddEventListener(SFSEvent.EXTENSION_RESPONSE,OnDatabaseResponse);

        ISFSObject param = new SFSObject();
        Sfs2X.Util.ByteArray data = new Sfs2X.Util.ByteArray(rawData);
        param.PutByteArray("avatar",data);

        smartfox.Send(new ExtensionRequest("database",param));
    }

    #region EventListener methods
    public void OnDatabaseResponse(BaseEvent evt){
        Console.Log("Database responded");
        
        smartfox.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE,OnDatabaseResponse);
    }
    public void OnRoomJoin(BaseEvent evt){
        Console.Log("Room joined");

        smartfox.RemoveEventListener(SFSEvent.ROOM_JOIN,OnRoomJoin);
        smartfox.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR,OnRoomJoinError);
    }
    public void OnRoomJoinError(BaseEvent evt){
        Console.Log("Failed to join room. " + evt.Params["errorMessage"]);

        smartfox.RemoveEventListener(SFSEvent.ROOM_JOIN,OnRoomJoin);
        smartfox.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR,OnRoomJoinError);
    }
    #endregion
}
