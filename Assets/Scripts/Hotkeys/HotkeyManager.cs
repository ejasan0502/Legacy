using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HotkeyManager : MonoBehaviour {

    public RectTransform cameraRect;

    private static HotkeyManager _instance;
    public static HotkeyManager instance {
        get {
            if ( _instance == null ){
                _instance = GameObject.FindObjectOfType<HotkeyManager>();
            }
            return _instance;
        }
    }

    private bool hide = false;

    public void SetDisplay(bool b){
        hide = b;

        foreach (Transform t in transform){
            t.gameObject.SetActive(b);
        }
    }

    public void UseHotkey(int index){
        Player p = Game.GetPlayer();

        if ( p.hotkeys[index] != null && p.hotkeys[index].CanApply() ){
            p.hotkeys[index].ApplyHotkey(p,index);
        }
    }

    public void UpdateHotkeyDisplay(int index){
        StartCoroutine("UpdateHotkeyDisplayCoroutine",index);
    }

    public void UpdateHotkeyText(int index){
        Player p = Game.GetPlayer();

        if ( p.hotkeys[index] != null && !p.hotkeys[index].IsSkill ){
            int x = p.hotkeys[index].GetAsUsableHotkey().inventoryIndex;
            transform.GetChild(index).GetChild(2).GetComponent<Text>().text = p.inventory.slots[x].amount + "";
        }
    }

    private IEnumerator UpdateHotkeyDisplayCoroutine(int index){
        Player p = Game.GetPlayer();

        if ( p.hotkeys[index] != null ){
            float cdMax = 5f;
            if ( p.hotkeys[index].IsSkill ) cdMax = p.hotkeys[index].GetAsSkillHotkey().GetSkill().cd;
            
            Image img = transform.GetChild(index).GetChild(1).GetComponent<Image>();
            img.gameObject.SetActive(true);
            p.hotkeys[index].cdStartTime = Time.time;
            while (Time.time - p.hotkeys[index].cdStartTime < cdMax){
                img.fillAmount = (p.hotkeys[index].GetCD()-(Time.time-p.hotkeys[index].cdStartTime))/p.hotkeys[index].GetCD();
                yield return new WaitForSeconds(1f);
            }
            
            img.gameObject.SetActive(false);
        }
    }
}
