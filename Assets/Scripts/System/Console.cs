using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Console : MonoBehaviour {
    #region Variables
    public InputField minimizedInputField;
    public Text minimizedText;

    public string[] commands;

	private static object _lock = new object();
	private static Console _instance;
	public static Console instance {
		get {
			if ( _instance == null ) {
				lock(_lock){
					GameObject c = GameObject.Find("Console");
					if ( c == null ){
						GameObject o = new GameObject("Console");
						_instance = o.AddComponent<Console>();
					} else {
						_instance = c.GetComponent<Console>();
					}
				}
			}
			return _instance;
		}
	}

    private int logCount = 0;
    private string logsText = "";
    private TouchScreenKeyboard keyboard;
    private float startTime = 0f;
    private float logTime = 1f;
    #endregion

	void Awake(){
        GameObject o = GameObject.Find("Console");
        if ( o != null ){
            if ( o != gameObject )
                Destroy(gameObject);
        }

		DontDestroyOnLoad(this);
	}

    void OnEnable(){
        Minimize();
    }

    public void Submit(){
        if ( instance.minimizedText.gameObject.activeSelf ) Log(instance.minimizedInputField.text);

        instance.Command(instance.minimizedInputField.text);

        instance.minimizedInputField.text = "";
    }

    public void Minimize(){
        transform.GetChild(0).gameObject.SetActive(true);
    }

    public void SetDisplay(bool b){
        foreach (Transform t in transform){
            t.gameObject.SetActive(b);
        }
    }

    private void Command(string s){
        foreach (string c in commands){
            if ( s.Split(' ')[0] == c ){
                Log("Command initialized...");
                switch(c){
                case "setunits":
                Game.GetPlayer().inventory.currency = float.Parse(s.Split(' ')[1]);
                break;
                }

                return;
            }
        }
    }

    private IEnumerator Fade(){
        if ( minimizedText.gameObject.activeSelf ){
            Color c = minimizedText.color;
            c.a = 1f;
            minimizedText.color = c;
            while (c.a > 0){
                yield return new WaitForEndOfFrame();
                c.a -= 0.005f;
                minimizedText.color = c;
            }
        }
    }

    #region Log Methods
	public static void Log(string s){
		Debug.Log(s);

        instance.logsText += "\n" + s;
        instance.logCount++;

        if ( Time.time - instance.startTime < instance.logTime ){
            instance.minimizedText.text += "\n" + s;
        } else {
            instance.minimizedText.text = s;
        }

        instance.StopCoroutine("Fade");
        instance.StartCoroutine("Fade");

        instance.startTime = Time.time;
	}
	public static void Log(float x){
		Log(x+"");
	}
	public static void Log(int x){
		Log(x+"");
	}
	public static void Log(Type x){
		Log(x.ToString());
	}
	public static void Log(string text, string className, string methodName){
		string s = className + "." + methodName + " - " + text;
        Log(s);
	}
    #endregion
}