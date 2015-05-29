using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class InventoryWindow : MonoBehaviour {
    
    public RectTransform contentsRectTransform;
    public Text weightText;
    public Text unitsText;

    private Player p;
    private List<GameObject> inventorySlotObjects = new List<GameObject>();

    void Awake(){
        p = Game.GetPlayer();

        UpdateDisplay();
    }

    void OnEnable(){
        UpdateDisplay();
    }

    public void UpdateDisplay(){
        // Clear objects list
        if ( inventorySlotObjects.Count > 0 ){
            foreach (GameObject o in inventorySlotObjects){
                Destroy(o);
            }

            inventorySlotObjects = new List<GameObject>();
        }

        // Fill objects list
        float maxWidth = 0f;
        for (int i = 0; i < p.inventory.slots.Count; i++){
            GameObject o = Instantiate(Resources.Load("Item Info")) as GameObject;
            o.transform.SetParent(contentsRectTransform);
            
            RectTransform rt = o.transform as RectTransform;
            Vector2 pos = new Vector2(contentsRectTransform.rect.min.x + rt.rect.width/2.0f + rt.rect.width*i,0);
            o.transform.localPosition = pos;
            o.transform.localScale = new Vector3(1f,1f,1f);

            o.GetComponent<ItemInfo>().SetItemAsDisplay(i,p.inventory.slots[i],GetComponent<Canvas>());
            
            inventorySlotObjects.Add(o);

            maxWidth += rt.rect.width;
        }

        // Adjust Contents Rect
        if ( maxWidth > contentsRectTransform.sizeDelta.x ) 
            contentsRectTransform.sizeDelta = new Vector2(maxWidth,contentsRectTransform.sizeDelta.y);

        // Set Units Text
        unitsText.text = p.inventory.currency + "u";

        // Set Weights Text
        weightText.text = "Weight: " + p.inventory.weight + " / " + p.inventory.maxWeight;
    }
}
