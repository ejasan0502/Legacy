using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;

public class Login : MonoBehaviour {
	public string serverName = "127.0.0.1";		
	public int serverPort = 9933;			
    public string zone = "Purgatory";	

    public Text emailText;
    public Text passText;
    public GameObject createAccountCanvas;
	
	private SmartFox smartfox;

	void Awake(){
		if (Application.isWebPlayer) {
			if (!Security.PrefetchSocketPolicy(serverName, serverPort, 500)) {
				Console.Log("Security Exception. Policy file loading failed!");
			}
		}		

		smartfox = Game.GetConnection();
		smartfox.AddEventListener(SFSEvent.CONNECTION, OnConnection);
		smartfox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        smartfox.AddEventListener(SFSEvent.LOGIN, OnLogin);
        smartfox.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        smartfox.AddEventListener(SFSEvent.LOGOUT, OnLogout);
        smartfox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnLoginResponse);
        smartfox.AddLogListener(LogLevel.DEBUG, OnDebugMessage);

        smartfox.Connect(serverName,serverPort);
	}

	void FixedUpdate() {
		smartfox.ProcessEvents();
	}

    public void LoginToServer(){
        Console.Log("Attempting to login...");

        ISFSObject param = new SFSObject();
        param.PutUtfString("email",emailText.text);
        param.PutUtfString("password",passText.text);
        
        smartfox.Send(new ExtensionRequest("login",param));
    }

    public void CreateAccount(){
        smartfox.RemoveAllEventListeners();
        createAccountCanvas.SetActive(true);
        gameObject.SetActive(false);
    }
	
	public void OnApplicationQuit() {
        Console.Log("Disconnect from server");
        smartfox.RemoveAllEventListeners();
		smartfox.Disconnect();
	}

    #region Server Handler Methods
	public void OnConnection(BaseEvent evt) {
		bool success = (bool)evt.Params["success"];
		if (success) {
			Console.Log("Connected to server");
            smartfox.Send(new LoginRequest("","",zone));
		} else {
			Console.Log("Unable to connect to server");
		}
	}

	public void OnConnectionLost(BaseEvent evt) {
		Console.Log("Connection lost! " + (string)evt.Params["reason"]);
	}

    public void OnLogin(BaseEvent evt) {
        Console.Log("Logged in as guest");
    }

    public void OnLoginError(BaseEvent evt) {
        Console.Log("Login error: " + (string)evt.Params["errorMessage"]);
    }

    public void OnLogout(BaseEvent evt) {
        Console.Log("Logged out");
    }

    public void OnLoginResponse(BaseEvent evt) {
        Console.Log("Event received: " + evt.Params["cmd"]);
        ISFSObject data = (SFSObject) evt.Params["params"];

        if ( (string)evt.Params["cmd"] == "login" ){
            if ( data.GetBool("success") ){
                Console.Log("Login successful");
                Game.SetEmail(emailText.text);
                Application.LoadLevel("Game Lobby");
            } else {
                Console.Log("Login failed: " + data.GetUtfString("message"));
            }
        }
    }

    public void OnDebugMessage(BaseEvent evt){
        string message = (string)evt.Params["message"];
        Console.Log(message);
    }
#endregion
}