using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;

public class ChatWindow : MonoBehaviour {
    
    public int maxLineLength = 32;
    public InputField inputField;
    public Text displayText;
    public Button enterBtn;
    
    private string playerName;
    private string text;
    private SmartFox smartfox;

    void Start(){
        playerName = "Player " + GameObject.FindGameObjectWithTag("Player").name;
        smartfox = NetworkManager.GetConnection();

        smartfox.AddEventListener(SFSEvent.PUBLIC_MESSAGE,OnPublicMessage);
    }

    public void AddText(string _text){
        text += "\n" + _text;
        UpdateDisplay();
    }
    public void OnChatEnter(){
        if ( inputField.text != "" ){
            NetworkManager.instance.SendPublicMessage(playerName + ": " + inputField.text);
            inputField.text = "";
        }
    }
    public void OnExit(){
        Application.Quit();
    }

    private void UpdateDisplay(){
        displayText.text = text;
    }
    private void OnPublicMessage(BaseEvent evt){
        string message = (string) evt.Params["message"];

        AddText(message);
    }
}
