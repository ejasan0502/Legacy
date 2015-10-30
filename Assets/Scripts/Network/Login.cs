using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;

// Login.cs
// Used to login to the server. This script implements an login extension of smartfox 2x to confirm other parameters such as mac address.
// *Place this script on the Login canvas.
public class Login : MonoBehaviour {
	public string serverName = "127.0.0.1";		
	public int serverPort = 9933;			
    public string zone = "";	

    public InputField emailText;
    public InputField passText;

    public GameObject chatWindow;
	
	private SmartFox smartfox;

	void Awake(){
		if (Application.isWebPlayer) {
			if (!Security.PrefetchSocketPolicy(serverName, serverPort, 500)) {
				DebugWindow.Log("Security Exception. Policy file loading failed!");
			}
		}		

		smartfox = NetworkManager.GetConnection();
		smartfox.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		smartfox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        smartfox.AddEventListener(SFSEvent.LOGIN, OnLogin);
        smartfox.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        smartfox.AddEventListener(SFSEvent.LOGOUT, OnLogout);
        smartfox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnLoginResponse);
        smartfox.AddLogListener(LogLevel.DEBUG, OnDebugMessage);
        
        DebugWindow.Log("Attempting to connect...");
        smartfox.Connect(serverName,serverPort);
	}
    void Update(){
        #if UNITY_STANDALONE || UNITY_EDITOR
            if ( Input.GetKeyUp(KeyCode.Tab) ){
                if ( EventSystem.current.currentSelectedGameObject == emailText.gameObject ){
                    EventSystem.current.SetSelectedGameObject(passText.gameObject, null);
                    passText.OnPointerClick(new PointerEventData(EventSystem.current));
                } else {
                    EventSystem.current.SetSelectedGameObject(emailText.gameObject, null);
                    emailText.OnPointerClick(new PointerEventData(EventSystem.current));
                }
            }
            if ( Input.GetKeyUp(KeyCode.Return) ){
                LoginToServer();
            }
        #elif UNITY_ANDROID || UNITY_IPHONE || UNITY_IOS || UNITY_WP8
            
        #endif
    }
    public void LoginToServer(){
        if ( emailText.text == "" && passText.text == "" ) return;

        DebugWindow.Log("Attempting to login...");

        ISFSObject param = new SFSObject();
        param.PutUtfString("email",emailText.text);
        param.PutUtfString("password",passText.text);
        
        smartfox.Send(new ExtensionRequest("login",param));
    }

    #region Server Handler Methods
	public void OnConnection(BaseEvent evt) {
		bool success = (bool)evt.Params["success"];
		if (success) {
			DebugWindow.Log("Connected to server");
            smartfox.Send(new LoginRequest("","",zone));
		} else {
			DebugWindow.Log("Unable to connect to server");
		}
	}
	public void OnConnectionLost(BaseEvent evt) {
		DebugWindow.Log("Connection lost! " + (string)evt.Params["reason"]);
	}

    public void OnLogin(BaseEvent evt) {
        DebugWindow.Log("Logged in as guest");
    }
    public void OnLoginError(BaseEvent evt) {
        DebugWindow.Log("Login error: " + (string)evt.Params["errorMessage"]);
    }
    public void OnLogout(BaseEvent evt) {
        DebugWindow.Log("Logged out");
    }
    public void OnLoginResponse(BaseEvent evt) {
        DebugWindow.Log("Event received: " + evt.Params["cmd"]);
        ISFSObject data = (SFSObject) evt.Params["params"];

        if ( (string)evt.Params["cmd"] == "login" ){
            if ( data.GetBool("success") ){
                DebugWindow.Log("Login successful");
                OnLoginSuccess();
            } else {
                DebugWindow.Log("Login failed: " + data.GetUtfString("message"));
                OnLoginFailure();
            }
        }
    }

    public void OnDebugMessage(BaseEvent evt){
        string message = (string)evt.Params["message"];
        DebugWindow.Log(message);
    }
    #endregion
    #region Private Methods
    private void OnLoginSuccess(){
        smartfox.RemoveAllEventListeners();

        chatWindow.SetActive(true);
        gameObject.SetActive(false);

        NetworkManager.instance.JoinRoom("Lobby");
    }
    private void OnLoginFailure(){

    }
    #endregion
}