using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterWindow : MonoBehaviour {
    
    public Text statsText;
    public Transform equipmentParentTransform;
    public GameObject equipmentInfoDisplay;

    private Player p;

    void Awake(){
        p = Game.GetPlayer();
    }

    void OnEnable(){
        UpdateDisplay();
    }

    public void DisplayEquip(int x){
        if ( p.equipment[x] != null ){
            equipmentInfoDisplay.SetActive(true);

            RectTransform rt = equipmentInfoDisplay.transform as RectTransform;
            rt.GetChild(0).GetComponent<Text>().text = p.equipment[x].name;
            rt.GetChild(1).GetComponent<Image>().sprite = p.equipment[x].icon;
            rt.GetChild(2).GetComponent<Text>().text = p.equipment[x].stats.ToString();
            rt.GetChild(3).GetComponent<Text>().text = p.equipment[x].description;
        }
    }

    public void CloseDisplay(){
        equipmentInfoDisplay.SetActive(false);
    }

    public void UpdateDisplay(){
        statsText.text = p.GetStatsString();

        for (int i = 0; i < p.equipment.Length; i++){
            if ( p.equipment[i].icon != null )
                equipmentParentTransform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = p.equipment[i].icon;
        }
    }
}
