using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DebugWindow : MonoBehaviour {

    public Text debugText;

    private static DebugWindow _instance;
    public static DebugWindow instance {
        get {
            if ( _instance == null )
                _instance = GameObject.FindObjectOfType<DebugWindow>();
            return _instance;
        }
    }

    public static void Log(string s){
        if ( instance != null )
            instance.debugText.text += "\n" + s;
        Debug.Log(s);
    }

}
