using UnityEngine;
using System.Collections;

public class MenusWindow : MonoBehaviour {

	public CharacterWindow characterWindow;
    public SkillWindow skillWindow;
    public InventoryWindow inventoryWindow;
    public QuestWindow questWindow;

    void OnEnable(){
        characterWindow.gameObject.SetActive(true);
    }

    public void DisplayWindow(int x){
        characterWindow.gameObject.SetActive(false);
        skillWindow.gameObject.SetActive(false);
        inventoryWindow.gameObject.SetActive(false);
        questWindow.gameObject.SetActive(false);

        if ( x == 0 ){
            characterWindow.gameObject.SetActive(true);
        } else if ( x == 1 ){
            skillWindow.gameObject.SetActive(true);
        } else if ( x == 2 ){
            inventoryWindow.gameObject.SetActive(true);
        } else if ( x == 3 ){
            questWindow.gameObject.SetActive(true);
        }
    }

    public void Exit(){
        Game.GetPlayerObject().GetComponent<PlayerObject>().SetControls(true);
        Console.instance.SetDisplay(true);

        gameObject.SetActive(false);
        characterWindow.gameObject.SetActive(false);
        skillWindow.gameObject.SetActive(false);
        inventoryWindow.gameObject.SetActive(false);
        questWindow.gameObject.SetActive(false);
    }

}
