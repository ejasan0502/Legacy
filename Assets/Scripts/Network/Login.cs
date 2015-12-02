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

    public InputField emailText;
    public InputField passText;
	
	private SmartFox smartfox;
    private string savedEmail = "";

	void Awake(){	
		smartfox = NetworkManager.GetConnection();
        smartfox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnLoginResponse);
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
        savedEmail = emailText.text;
        
        smartfox.Send(new ExtensionRequest("login",param));
    }

    #region Server Handler Methods
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
    #endregion
    #region Private Methods
    private void OnLoginSuccess(){
        NetworkManager.instance.SetEmail(savedEmail);
        NetworkManager.instance.LoadPlayerDataFromDatabase();
    }
    private void OnLoginFailure(){

    }
    #endregion
}