using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;

// NetworkManager.cs
// Used to manage messages between the smartfox 2x server and unity client.
// *Place this script on an empty game object.
// *This script is required to be on the scene when using other smartfox 2x scripts (Login.cs, CreateAccount.cs, etc)
public class NetworkManager : MonoBehaviour {

    public List<SqlType> sqlTypes;              // All available sql types on the server

    private static SmartFox smartfox;
    public static SmartFox GetConnection(){
        if ( smartfox == null ) 
            smartfox = new SmartFox(true);
        return smartfox;
    }

    private static NetworkManager _instance;
    public static NetworkManager instance {
        get {
            if ( _instance == null )
                _instance = GameObject.FindObjectOfType<NetworkManager>();
            return _instance;
        }
    }

    private string[] dataTypes = new string[6]{ "byteArray", "int", "float", "string", "bool", "double" };

    #region Unity Methods
    void Awake(){
        // Destroy duplicates
        NetworkManager[] list = GameObject.FindObjectsOfType<NetworkManager>();
        if ( list.Length > 1 ){
            // Found more than 1 object, meaning this instance is a duplicate
            Destroy(gameObject);
        }
    }
    void Start(){
        DontDestroyOnLoad(this);

        smartfox = NetworkManager.GetConnection();
    }
    void FixedUpdate(){
        if ( smartfox != null ) 
            smartfox.ProcessEvents();
    }
	void OnApplicationQuit() {
        smartfox.RemoveAllEventListeners();
		smartfox.Disconnect();
	}
    #endregion
    #region Public Methods
    public void JoinRoom(string s){
        smartfox.RemoveAllEventListeners();
        smartfox.AddEventListener(SFSEvent.ROOM_JOIN,OnRoomJoin);
        smartfox.AddEventListener(SFSEvent.ROOM_JOIN_ERROR,OnRoomJoinError);
        smartfox.Send(new JoinRoomRequest(s));
    }
    public void SendPublicMessage(string _message){
        if ( _message != "" ){
            smartfox.Send(new PublicMessageRequest(_message));
        }
    }
    #region SaveToDatabase Overload Methods
    public void SaveToDatabase(string id, byte[] rawData){
        SqlType s = GetSqlType(id);
        if ( s == null ) {
            DebugWindow.Log("Invalid id");
            return;
        }

        if ( dataTypes[(int)s.dataType] != dataTypes[0] ){
            DebugWindow.Log("Invalid data type.");
            return;
        }

        ISFSObject param = new SFSObject();
        Sfs2X.Util.ByteArray data = new Sfs2X.Util.ByteArray(rawData);
        param.PutByteArray(id,data);

        SaveToDatabase(id,param);
    }
    public void SaveToDatabase(string id, int rawData){
        SqlType s = GetSqlType(id);
        if ( s == null ) {
            DebugWindow.Log("Invalid id");
            return;
        }

        if ( dataTypes[(int)s.dataType] != dataTypes[1] ){
            DebugWindow.Log("Invalid data type.");
            return;
        }

        ISFSObject param = new SFSObject();
        param.PutInt(id,rawData);

        SaveToDatabase(id,param);
    }
    public void SaveToDatabase(string id, float rawData){
        SqlType s = GetSqlType(id);
        if ( s == null ) {
            DebugWindow.Log("Invalid id");
            return;
        }

        if ( dataTypes[(int)s.dataType] != dataTypes[2] ){
            DebugWindow.Log("Invalid data type.");
            return;
        }

        ISFSObject param = new SFSObject();
        param.PutFloat(id,rawData);

        SaveToDatabase(id,param);
    }
    public void SaveToDatabase(string id, string rawData){
        SqlType s = GetSqlType(id);
        if ( s == null ) {
            DebugWindow.Log("Invalid id");
            return;
        }

        if ( dataTypes[(int)s.dataType] != dataTypes[3] ){
            DebugWindow.Log("Invalid data type.");
            return;
        }

        ISFSObject param = new SFSObject();
        param.PutUtfString(id,rawData);

        SaveToDatabase(id,param);
    }
    public void SaveToDatabase(string id, bool rawData){
        SqlType s = GetSqlType(id);
        if ( s == null ) {
            DebugWindow.Log("Invalid id");
            return;
        }

        if ( dataTypes[(int)s.dataType] != dataTypes[4] ){
            DebugWindow.Log("Invalid data type.");
            return;
        }

        ISFSObject param = new SFSObject();
        param.PutBool(id,rawData);

        SaveToDatabase(id,param);
    }
    public void SaveToDatabase(string id, double rawData){
        SqlType s = GetSqlType(id);
        if ( s == null ) {
            DebugWindow.Log("Invalid id");
            return;
        }

        if ( dataTypes[(int)s.dataType] != dataTypes[5] ){
            DebugWindow.Log("Invalid data type.");
            return;
        }

        ISFSObject param = new SFSObject();
        param.PutDouble(id,rawData);

        SaveToDatabase(id,param);
    }
    #endregion
    #endregion
    #region Private Methods
    private void SaveToDatabase(string id, ISFSObject data){
        smartfox.AddEventListener(SFSEvent.EXTENSION_RESPONSE,OnDatabaseResponse);

        smartfox.Send(new ExtensionRequest("database",data));
    }
    private SqlType GetSqlType(string id){
        return  (SqlType) sqlTypes.Where(st => st.id.ToLower() == id.ToLower());
    }
    #endregion
    #region EventListener methods
    private void OnDatabaseResponse(BaseEvent evt){
        DebugWindow.Log("Database response received: " + evt.Params["message"]);

        smartfox.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE,OnDatabaseResponse);
    }
    private void OnRoomJoin(BaseEvent evt){
        DebugWindow.Log("Joined room: " + evt.Params["message"]);

        smartfox.RemoveEventListener(SFSEvent.ROOM_JOIN,OnRoomJoin);
        smartfox.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR,OnRoomJoinError);
    }
    private void OnRoomJoinError(BaseEvent evt){
        DebugWindow.Log("Failed to join room. " + evt.Params["errorMessage"]);

        smartfox.RemoveEventListener(SFSEvent.ROOM_JOIN,OnRoomJoin);
        smartfox.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR,OnRoomJoinError);
    }
    #endregion
}

[System.Serializable]
public class SqlType {
    public string id;
    public DataType dataType;
}

public enum DataType {
    byteArray,
    integer,
    floatType,
    stringType,
    boolean,
    doubleType
}
