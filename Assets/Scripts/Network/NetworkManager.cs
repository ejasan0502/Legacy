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

    private string email = "";
    private MMOPlayer localPlayer = null;
    private List<MMOPlayer> remotePlayers = new List<MMOPlayer>();

    #region Unity Methods
    void Awake(){
        // Destroy duplicates
        NetworkManager[] list = GameObject.FindObjectsOfType<NetworkManager>();
        if ( list.Length > 1 ){
            // Found more than 1 object, meaning this instance is a duplicate
            DestroyImmediate(gameObject);
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
    public void SetEmail(string e){
        email = e;
    }
    public void LoadPlayerDataFromDatabase(){
        if ( email == "" ){
            DebugWindow.Log("email variable is not set.");
        } else {
            smartfox.AddEventListener(SFSEvent.EXTENSION_RESPONSE,OnDatabaseResponseLoad);

            ISFSObject param = new SFSObject();
            param.PutUtfString("email",email);
            param.PutUtfString("command","load");

            smartfox.Send(new ExtensionRequest("database",param));
        }
    }
    public void SavePlayerDataToDatabase(byte[] data){
        smartfox.AddEventListener(SFSEvent.EXTENSION_RESPONSE,OnDatabaseResponseSave);

        ISFSObject param = new SFSObject();
        param.PutUtfString("email",email);
        param.PutUtfString("command","save");
        param.PutByteArray("data",new ByteArray(data));

        smartfox.Send(new ExtensionRequest("database",param));
    }
    public void SpawnPlayer(SFSUser user){
        
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
    #endregion
    #region EventListener methods
    private void OnDatabaseResponseSave(BaseEvent evt){
        smartfox.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE,OnDatabaseResponseSave);

        if ( (bool) evt.Params["success"] ){
            DebugWindow.Log("Successfully saved player data to database!");
        } else {
            DebugWindow.Log("Failed to save player data to database.");
        }
    }
    private void OnDatabaseResponseLoad(BaseEvent evt){
        smartfox.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE,OnDatabaseResponseLoad);
        
        if ( (bool) evt.Params["success"] ){
            DebugWindow.Log("Successfully received player data from database!");
            GameManager.instance.dataSaver.LoadDataFromBytes( ((ByteArray)evt.Params["data"]).Bytes );
        } else {
            DebugWindow.Log(evt.Params["message"]+"");
            DebugWindow.Log("Loading character creation...");
        }
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

public class MMOPlayer {
    public SFSUser user;
    public CharacterObject characterObject;
    public PlayerCharacter player;
}
