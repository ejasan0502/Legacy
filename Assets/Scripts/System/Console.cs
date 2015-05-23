using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class Console : MonoBehaviour {
    #region Variables
    public InputField minimizedInputField;
    public Text minimizedText;
    public InputField maximizedInputField;
    public Text maximizedText;

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
        else if ( instance.maximizedText.gameObject.activeSelf ) Log(instance.maximizedInputField.text);

        instance.minimizedInputField.text = "";
        instance.maximizedInputField.text = "";
    }

    public void Minimize(){
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(1).gameObject.SetActive(false);
    }

    public void Maximize(){
        transform.GetChild(0).gameObject.SetActive(false);
        transform.GetChild(1).gameObject.SetActive(true);
    }

    public void SetDisplay(bool b){
        foreach (Transform t in transform){
            t.gameObject.SetActive(b);
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

        instance.minimizedText.text = s;
        instance.maximizedText.text = instance.logsText;

        if ( instance.logCount*30f > instance.maximizedText.rectTransform.sizeDelta.y ){
            instance.maximizedText.rectTransform.sizeDelta = new Vector2(instance.maximizedText.rectTransform.sizeDelta.x,instance.logCount*30f);
        }

        instance.StopCoroutine("Fade");
        instance.StartCoroutine("Fade");
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