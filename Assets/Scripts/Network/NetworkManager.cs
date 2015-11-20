using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;

// NetworkManager.cs
// Used to manage messages between the smartfox 2x server and unity client.
// *Place this script on an empty game object.
public class NetworkManager : MonoBehaviour {

	public string serverName = "127.0.0.1";		
	public int serverPort = 9933;			
    public string zone = "";
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

    private CharacterObject localPlayer = null;
    private Dictionary<SFSUser,CharacterObject> remotePlayers = new Dictionary<SFSUser,CharacterObject>();

    #region Unity Methods
    void Awake(){
        // Destroy duplicates
        NetworkManager[] list = GameObject.FindObjectsOfType<NetworkManager>();
        if ( list.Length > 1 ){
            // Found more than 1 object, meaning this instance is a duplicate
            Destroy(gameObject);
        }

		if (Application.isWebPlayer) {
			if (!Security.PrefetchSocketPolicy(serverName, serverPort, 500)) {
				DebugWindow.Log("Security Exception. Policy file loading failed!");
			}
		}	
    }
    void Start(){
        DontDestroyOnLoad(this);

        smartfox = NetworkManager.GetConnection();
		smartfox.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		smartfox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        smartfox.AddLogListener(LogLevel.DEBUG, OnDebugMessage);

        DebugWindow.Log("Attempting to connect...");
        smartfox.Connect(serverName,serverPort);
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
    public void SpawnLocalPlayer(string _modelPath, PlayerCharacter _player){
        smartfox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        smartfox.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
        smartfox.AddEventListener(SFSEvent.USER_EXIT_ROOM, OnUserExitRoom);
        smartfox.AddEventListener(SFSEvent.USER_ENTER_ROOM, OnUserEnterRoom);

        // Instantiate Character Model
        CharacterObject co = Resources.Load(_modelPath, typeof(CharacterObject)) as CharacterObject;
        //if ( co == null )
        //    co = Resources.Load(GlobalVariables.PATH_CHARACTERMODEL_DEFAULT, typeof(CharacterObject)) as CharacterObject;

        co.transform.position = Vector3.zero;

        // Initialize Character
        co.SetCharacter(_player);

        // Set camera fixed on player
        Camera.main.GetComponent<CameraControls>().SetPlayer(co.transform);

        localPlayer = co;

		List<UserVariable> userVariables = new List<UserVariable>();
		//userVariables.Add(new SFSUserVariable("model", numModel));
		//userVariables.Add(new SFSUserVariable("mat", numMaterial));
		smartfox.Send(new SetUserVariablesRequest(userVariables));
    }
    public void JoinRoom(string s){
        if ( smartfox.RoomManager.ContainsRoom(s) ){
            smartfox.AddEventListener(SFSEvent.ROOM_JOIN,OnRoomJoin);
            smartfox.AddEventListener(SFSEvent.ROOM_JOIN_ERROR,OnRoomJoinError);
            smartfox.Send(new JoinRoomRequest(s));
        } else {
            RoomSettings settings = new RoomSettings(s);
            settings.MaxUsers = 40;
            smartfox.Send(new CreateRoomRequest(settings, true));
        }
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
        smartfox.RemoveAllEventListeners();
        
        DebugWindow.Log("Loading level (" + evt.Params["message"] + ")...");
        Application.LoadLevel(evt.Params["message"].ToString());
    }
    private void OnRoomJoinError(BaseEvent evt){
        DebugWindow.Log("Failed to join room. " + evt.Params["errorMessage"]);

        smartfox.RemoveEventListener(SFSEvent.ROOM_JOIN,OnRoomJoin);
        smartfox.RemoveEventListener(SFSEvent.ROOM_JOIN_ERROR,OnRoomJoinError);
    }
	public void OnConnection(BaseEvent evt) {
		bool success = (bool)evt.Params["success"];
		if (success) {
			DebugWindow.Log("Connected to server");
            smartfox.RemoveEventListener(SFSEvent.CONNECTION, OnConnection);

            smartfox.AddEventListener(SFSEvent.LOGIN, OnLogin);
            smartfox.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
            smartfox.AddEventListener(SFSEvent.LOGOUT, OnLogout);
            smartfox.Send(new LoginRequest("","",zone));
		} else {
			DebugWindow.Log("Unable to connect to server");
		}
	}
	public void OnConnectionLost(BaseEvent evt) {
		DebugWindow.Log("Connection lost! " + (string)evt.Params["reason"]);
        smartfox.RemoveAllEventListeners();
	}
    public void OnLogin(BaseEvent evt) {
        DebugWindow.Log("Logged in as guest");
        smartfox.RemoveEventListener(SFSEvent.LOGIN, OnLogin);
        smartfox.RemoveEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
    }
    public void OnLoginError(BaseEvent evt) {
        DebugWindow.Log("Login error: " + (string)evt.Params["errorMessage"]);
    }
    public void OnLogout(BaseEvent evt) {
        DebugWindow.Log("Logged out");
    }
    public void OnDebugMessage(BaseEvent evt){
        string message = (string)evt.Params["message"];
        DebugWindow.Log(message);
    }
    public void OnUserVariableUpdate(BaseEvent evt){

    }
    public void OnUserExitRoom(BaseEvent evt){

    }
    public void OnUserEnterRoom(BaseEvent evt){

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
