using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class NPCCanvas : MonoBehaviour {
    
    public GameObject craftshop;
    public GameObject civillian;
    public GameObject teacher;
    public GameObject transporter;
    public GameObject buyList;
    public GameObject sellList;
    public GameObject craftList;
    public GameObject questList;
    public GameObject itemInfoPref;
    public GameObject questInfoPref;
    public GameObject craftInfoPref;

    private NPC npc;
    private Item[] purchaseList = null;
    private Item[] craftsList = null;
    private bool purchaseListCreated = false;
    private bool craftListCreated = false;

    #region Public Methods
    public void SetNPC(NPC n){
        npc = n;
        purchaseList = npc.GetItemsList();
        craftsList = npc.GetCraftsList();

        SetDisplayButtons(true);
    }
    public void Back(int x){
        if ( x == 0 ){
            SetDisplayButtons(true);
            SetDisplayBuyList(false);
        } else if ( x == 1 ){
            SetDisplayButtons(true);
            SetDisplaySellList(false);
        } else if ( x == 2 ){
            SetDisplayButtons(true);
            SetDisplayQuestList(false);
        } else if ( x == 4 ){
            SetDisplayButtons(true);
            SetDisplayCraftList(false);
        }   
    }
    #endregion
    #region Start Buttons Methods
    public void Quest(){
        SetDisplayButtons(false);
        SetDisplayQuestList(true);
        ClearList(questList.transform);
        CreateList(npc.GetQuestList(Game.GetPlayer()));
    }
    public void Buy(){
        SetDisplayButtons(false);
        SetDisplayBuyList(true);
        if ( !purchaseListCreated ){
            purchaseListCreated = true;
            CreateList(purchaseList,true);
        }

        Text unitsText = buyList.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        unitsText.text = "Units: " + Game.GetPlayer().inventory.currency;
    }
    public void Sell(){
        SetDisplayButtons(false);
        SetDisplaySellList(true);
        ClearList(sellList.transform);
        CreateList(Game.GetPlayer().inventory.GetItemsList(),false);

        Text unitsText = sellList.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        unitsText.text = "Units: " + Game.GetPlayer().inventory.currency;
    }
    public void Craft(){
        SetDisplayButtons(false);
        SetDisplayCraftList(true);
        if ( !craftListCreated && craftsList != null ){
            craftListCreated = true;
            CreateList(craftsList);
        }

        Text unitsText = craftList.transform.GetChild(1).GetChild(0).GetComponent<Text>();
        unitsText.text = "Units: " + Game.GetPlayer().inventory.currency;
    }
    public void Leave(){
        npc.npcObject.StopInteract();
        Destroy(gameObject);
    }
    public void Teleport(){
        
    }
    #endregion
    #region Private Methods
    private void ClearList(Transform list){
        Transform contentObj = list.GetChild(0).GetChild(0);
        foreach (Transform t in contentObj){
            Destroy(t.gameObject);
        }
    }
    private void SetDisplayButtons(bool b){
        if ( npc.npcType == NPCType.civilian ){
            civillian.SetActive(b);
        } else if ( npc.npcType == NPCType.transporter ){
            transporter.SetActive(b);
        } else {
            craftshop.SetActive(b);
        }
    }
    private void SetDisplayBuyList(bool b){
        buyList.SetActive(b);
    }
    private void SetDisplaySellList(bool b){
        sellList.SetActive(b);
    }
    private void SetDisplayCraftList(bool b){
        craftList.SetActive(b);
    }
    private void SetDisplayQuestList(bool b){
        questList.SetActive(b);
    }
    // @param2: true if player is buying, false if player is selling
    private void CreateList(Item[] items, bool b){
        RectTransform contentObj = new RectTransform();
        if ( b ) contentObj = buyList.transform.GetChild(0).GetChild(0) as RectTransform;
        else contentObj = sellList.transform.GetChild(0).GetChild(0) as RectTransform;

        float maxWidth = 0f;
        for (int i = 0; i < items.Length; i++){
            GameObject o = Instantiate(itemInfoPref) as GameObject;
            o.transform.SetParent(contentObj);
            RectTransform rt = o.transform as RectTransform;
            Vector2 pos = new Vector2(contentObj.rect.min.x + rt.rect.width/2.0f + rt.rect.width*i,0);
            o.transform.localPosition = pos;
            o.transform.localScale = new Vector3(1f,1f,1f);

            o.GetComponent<ItemInfo>().SetItem(items[i],GetComponent<Canvas>(),b);
        }

        if ( maxWidth > contentObj.sizeDelta.x )
            contentObj.sizeDelta = new Vector2(maxWidth,contentObj.sizeDelta.y);
    }
    private void CreateList(List<Quest> quests){
        RectTransform contentObj = questList.transform.GetChild(0).GetChild(0) as RectTransform;
        RectTransform questInfoPrefRectTransform = questInfoPref.GetComponent<RectTransform>();
        contentObj.sizeDelta = new Vector3(questInfoPrefRectTransform.rect.width*quests.Count,contentObj.rect.height);

        Player p = Game.GetPlayer();
        List<Quest> availableQuests = quests;
        for (int i = 0; i < availableQuests.Count; i++){
            if ( p.HasCompleteQuest(availableQuests[i].id) ){
                availableQuests.RemoveAt(i);
            }
        }

        for (int i = 0; i < availableQuests.Count; i++){
            GameObject o = Instantiate(questInfoPref) as GameObject;
            o.transform.SetParent(contentObj);
            Vector2 pos = new Vector2(contentObj.rect.min.x + questInfoPrefRectTransform.rect.width/2.0f + questInfoPrefRectTransform.rect.width*i,0);
            o.transform.localPosition = pos;
            o.transform.localScale = new Vector3(1f,1f,1f);

            o.GetComponent<QuestInfo>().SetQuest(availableQuests[i],GetComponent<Canvas>(),false);
        }
    }
    private void CreateList(Item[] items){
        RectTransform contentObj = craftList.transform.GetChild(0).GetChild(0) as RectTransform;

        RectTransform craftInfoPrefRectTransform = craftInfoPref.GetComponent<RectTransform>();
        contentObj.sizeDelta = new Vector2(craftInfoPrefRectTransform.rect.width*items.Length,contentObj.rect.height);

        for (int i = 0; i < items.Length; i++){
            GameObject o = Instantiate(itemInfoPref) as GameObject;
            o.transform.SetParent(contentObj);
            Vector2 pos = new Vector2(contentObj.rect.min.x + craftInfoPrefRectTransform.rect.width/2.0f + craftInfoPrefRectTransform.rect.width*i,0);
            o.transform.localPosition = pos;
            o.transform.localScale = new Vector3(1f,1f,1f);

            int amt = 10;
            if ( items[i].IsEquip() ) amt = 1;
            o.GetComponent<CraftInfo>().SetItem(items[i],amt);
        }
    }
    #endregion

    public void UpdateUnitsText(){
        Transform t = buyList.transform;
        if ( sellList.activeSelf ){
            t = sellList.transform;
        }

        t.GetChild(1).GetChild(0).GetComponent<Text>().text = Game.GetPlayer().inventory.currency+"u";
    }
}
