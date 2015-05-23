using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class QuestWindow : MonoBehaviour {
    
    public RectTransform contentsRectTransform;

    private Player p;
    private List<GameObject> questSlotObjects = new List<GameObject>();

    void Awake(){
        p = Game.GetPlayer();

        UpdateDisplay();
    }

    void OnEnable(){
        UpdateDisplay();
    }

    public void UpdateDisplay(){
        // Clear objects list
        if ( questSlotObjects.Count > 0 ){
            foreach (GameObject o in questSlotObjects){
                Destroy(o);
            }

            questSlotObjects = new List<GameObject>();
        }

        // Fill objects list
        Vector3 startPos = new Vector3(-contentsRectTransform.rect.width/2.0f,10f,0f);      // Based on local
        float maxWidth = 0f;
        for (int i = 0; i < p.quests.Count; i++){
            GameObject o = Instantiate(Resources.Load("Quest Info")) as GameObject;
            RectTransform rt = o.transform as RectTransform;

            o.transform.localScale = new Vector3(1f,1f,1f);
            o.transform.localPosition = new Vector3(startPos.x+rt.rect.width*i,startPos.y,startPos.z);

            o.GetComponent<QuestInfo>().SetQuestDisplay(p.quests[i],GetComponent<Canvas>());
            
            o.transform.SetParent(contentsRectTransform);

            questSlotObjects.Add(o);

            maxWidth += rt.rect.width;
        }

        // Adjust Contents Rect
        contentsRectTransform.sizeDelta = new Vector2(maxWidth,contentsRectTransform.sizeDelta.y);
    }
}
