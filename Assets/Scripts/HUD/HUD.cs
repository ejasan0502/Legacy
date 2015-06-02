using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {
    
    public Button resurrectBtn;
    public RectTransform[] menusRectTransform;
    public RectTransform[] windowsRectTransform;

    private static HUD _instance;
    public static HUD instance {
        get {
            if ( _instance == null ) _instance = GameObject.FindObjectOfType<HUD>();
            return _instance;
        }
    }

    void Awake(){
        // Destroy duplicates
        Game[] list = GameObject.FindObjectsOfType<Game>();
        if ( list.Length > 1 ){
            // Found more than 1 object, meaning this instance is a duplicate
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    public void SetMenuDisplay(bool b){
        if ( menusRectTransform[0].gameObject.activeSelf != b ){
            menusRectTransform[0].gameObject.SetActive(b);

            if ( b ){
                Vector3 pos = Vector2.zero;
                if ( RectTransformUtility.ScreenPointToWorldPointInRectangle(menusRectTransform[0],Input.mousePosition,Camera.main, out pos) ){
                    menusRectTransform[0].position = pos;
                }
            }
        }
    }
    public void CheckInput(){
        foreach (RectTransform rt in menusRectTransform){
            if ( RectTransformUtility.RectangleContainsScreenPoint(rt,Input.mousePosition,Camera.main) ){
                if ( rt.name == "Windows" ){
                    windowsRectTransform[0].gameObject.SetActive(true);
                    Game.GetPlayerObject().GetComponent<PlayerObject>().SetControls(false);
                    Console.instance.SetDisplay(false);
                    Game.GetPlayer().GetPlayerInfo().gameObject.SetActive(false);
                    HotkeyManager.instance.SetDisplay(false);
                } else if ( rt.name == "Settings" ){
                    
                } else if ( rt.name == "Log Off" ){
                    
                } else if ( rt.name != "Menu" ){
                    Console.Log("Do not recognize '" + rt.name + "' as a menu type");
                }
            }
        }
        SetMenuDisplay(false);
    }
    public void DisplayResurrectBtn(){
        Console.System(name + ": DisplayResurrectBtn()");

        resurrectBtn.gameObject.SetActive(true);
        resurrectBtn.onClick.RemoveAllListeners();
        resurrectBtn.onClick.AddListener(() => Game.GetPlayer().Resurrect());
    }

}

