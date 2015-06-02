using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Console : MonoBehaviour {
    #region Variables
    public InputField minimizedInputField;
    public Text minimizedText;
    public RectTransform errorField;
    public RectTransform debug;

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

    private int systemLogCount = 0;
    private string systemLogs = "";
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

    public void CloseErrorMessage(){
        errorField.gameObject.SetActive(false);
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
        transform.GetChild(0).gameObject.SetActive(b);
    }

    private bool Command(string s){
        switch(s){
        case "setunits":
        Game.GetPlayer().inventory.currency = float.Parse(s.Split(' ')[1]);
        Log("Console.cs - Command(string): Player's units set to " + s.Split(' ')[1]);
        return true;
        case "show debug":
        debug.gameObject.SetActive(true);
        ((RectTransform)debug.GetChild(0).GetChild(0)).sizeDelta = new Vector2(((RectTransform)debug.GetChild(0).GetChild(0)).sizeDelta.x,systemLogCount*25f);
        debug.GetChild(0).GetChild(0).GetComponent<Text>().text = systemLogs;
        return true;
        case "close debug":
        debug.gameObject.SetActive(false);
        return true;
        }
        return false;
    }
    private IEnumerator Fade(){
        if ( minimizedText.gameObject.activeSelf ){
            Color c = minimizedText.color;
            c.a = 1f;
            minimizedText.color = c;
            while (c.a > 0){
                yield return new WaitForEndOfFrame();
                c.a -= 0.001f;
                minimizedText.color = c;
            }
        }
    }

    #region Log Methods
    public static void System(string s){
        instance.systemLogs += "\n" + s;
        instance.systemLogCount++;
    }
    public static void Error(string s){
        Debug.Log(s);

        instance.errorField.gameObject.SetActive(true);
        instance.errorField.GetChild(0).GetComponent<Text>().text = s;
    }
	public static void Log(string s){
        if ( !instance.Command(s) ){
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