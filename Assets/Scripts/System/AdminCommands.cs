using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

// GiveItem (Item Id) (Quality) (Amount)
public class AdminCommands : MonoBehaviour {
    
    public Text windowText;
    public InputField inputField;
    public Dictionary<string,AdminCommand> commands;

    void Awake(){
        commands = new Dictionary<string,AdminCommand>();
        commands.Add("GiveItem",GiveItem);
    }

    private void Log(string s){
        windowText.text += "\n" + s;
    }

    public void Enter(){
        if ( inputField.text != "" ){
            string[] text = inputField.text.Split(' ');
            if ( commands.ContainsKey(text[0]) ){
                commands[text[0]](text[1],text[2],text[3]);
            }
        }
    }

    public void GiveItem(params object[] args){
        if ( args.Length == 3 ){
            
        } else {
            Log("Invalid amount of parameters for command: GiveItem");
        }
    }
}

public delegate void AdminCommand(params object[] args);
