using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class HUD : MonoBehaviour {
    
    public RectTransform[] menusRectTransform;
    public RectTransform[] windowsRectTransform;

    public void SetMenuDisplay(bool b){
        if ( menusRectTransform[0].gameObject.activeSelf != b ){
            menusRectTransform[0].gameObject.SetActive(b);

            if ( b ){
                Vector2 localpoint = Vector2.zero;
                if ( RectTransformUtility.ScreenPointToLocalPointInRectangle(menusRectTransform[0],Input.mousePosition,Camera.main, out localpoint) ){
                    menusRectTransform[0].localPosition = localpoint;
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
                } else if ( rt.name == "Settings" ){
                    
                } else if ( rt.name == "Log Off" ){
                    
                } else if ( rt.name != "Menu" ){
                    Console.Log("Do not recognize '" + rt.name + "' as a menu type");
                }
            }
        }
        SetMenuDisplay(false);
    }

}

