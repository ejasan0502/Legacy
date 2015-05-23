using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;

public class CreateAccount : MonoBehaviour {

    public Text emailText;
    public Text cEmailText;

    public Text passText;
    public Text cPassText;

    public GameObject loginCanvas;

    private SmartFox smartfox;
    
    void Start(){
        smartfox = Game.GetConnection();
    }

	void FixedUpdate() {
		smartfox.ProcessEvents();
	}

    public void Back(){
        loginCanvas.SetActive(true);
        gameObject.SetActive(false);
    }

    public void Create(){
        if ( emailText.text != cEmailText.text ){
            Console.Log("Emails do not match!");
            return;
        }
        if ( passText.text != cPassText.text ){
            Console.Log("Passwords do not match!");
            return;
        }

        SmartFox smartfox = Game.GetConnection();
        smartfox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnRegisterResponse);

        ISFSObject param = new SFSObject();
        param.PutUtfString("email",emailText.text);
        param.PutUtfString("password",passText.text);
        
        smartfox.Send(new ExtensionRequest("register",param));
    }

    public void OnRegisterResponse(BaseEvent evt){
        Console.Log("Event received: " + evt.Params["cmd"]);
        ISFSObject data = (SFSObject) evt.Params["params"];

        smartfox.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE, OnRegisterResponse);

        if ( (string)evt.Params["cmd"] == "register" ){
            Console.Log("Received message");
            if ( data.GetBool("success") ){
                Console.Log("Successfully registered.");
        
                loginCanvas.SetActive(true);
                gameObject.SetActive(false);
            } else {
                Console.Log("Failed to register. " + data.GetUtfString("message"));
            }
        }
    }
}
