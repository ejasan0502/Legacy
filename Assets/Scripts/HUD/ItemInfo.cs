using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ItemInfo : MonoBehaviour {
    
    private Item item;
    private Canvas canvas;
    private bool buy = true;

    public Text textName;
    public Image portrait;
    public Text statsText;
    public Text descriptionText;
    public Button btn;
    public Text btnText;
    public Slider amtSlider;
    public Text amtText;
    public Button otherBtn;
    public Button otherBtn2;
    public GameObject hotkeyWindow;

    public void SetItemAsDisplay(int index, InventoryItem i, Canvas c){
        item = i.item;
        canvas = c;

        textName.text = item.name + " x" + i.amount;
        portrait.sprite = item.icon;
        statsText.text = item.stats.ToString();
        descriptionText.text = item.description;

        // Adjust description text rect
        Vector2 newPos = descriptionText.transform.localPosition; 
        Vector2 newRect = descriptionText.rectTransform.sizeDelta;
        newPos.y -= 10f;
        newRect.y += 20f;
        descriptionText.rectTransform.sizeDelta = newRect;
        descriptionText.transform.localPosition = newPos;

        btn.transform.GetChild(0).GetComponent<Text>().text = "Drop";
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => Drop());

        if ( item.IsEquip() ){
            otherBtn.gameObject.SetActive(true);
            otherBtn.onClick.RemoveAllListeners();
            otherBtn.onClick.AddListener(() => Equip(index));
        } else if ( item.IsUsable() ){
            otherBtn.gameObject.SetActive(true);
            otherBtn.onClick.RemoveAllListeners();
            otherBtn.transform.GetChild(0).GetComponent<Text>().text = "Use";
            otherBtn.onClick.AddListener(() => Use(index));

            otherBtn2.gameObject.SetActive(true);
            otherBtn2.onClick.RemoveAllListeners();
            otherBtn2.onClick.AddListener(() => Hotkey());
        }

        amtSlider.gameObject.SetActive(false);
        amtText.gameObject.SetActive(false);
    }

    public void SetItem(Item i, Canvas c, bool b){
        if ( i == null ) {
            Console.Error("ItemInfo.cs - SetItem(item,canvas,bool): @param1 is null");
            return;
        }

        item = i;
        buy = b;
        canvas = c;

        textName.text = item.name;
        portrait.sprite = item.icon;
        statsText.text = item.stats.ToString();
        descriptionText.text = item.description;

        if ( b ){
            btnText.text = "Buy - ";
            btn.onClick.AddListener(() => Buy());
            if ( item.IsEquip() ){
                amtSlider.gameObject.SetActive(false);
                amtText.gameObject.SetActive(false);
            }
        } else {
            btnText.text = "Sell - ";
            btn.onClick.AddListener(() => Sell());
            if ( item.IsEquip() ){
                amtSlider.gameObject.SetActive(false);
                amtText.gameObject.SetActive(false);
            } else 
                amtSlider.maxValue = Game.GetPlayer().inventory.GetInventorySlot(item).amount;
        }

        btnText.text += "U" + item.cost * amtSlider.value;
        amtText.text = amtSlider.value + "";
    }

    public void Buy(){
        Player p = Game.GetPlayer();
        if ( p.inventory.currency >= item.cost * amtSlider.value ){
            p.inventory.currency -= item.cost * amtSlider.value;
            p.inventory.AddItem(item,(int)amtSlider.value);
            canvas.GetComponent<NPCCanvas>().UpdateUnitsText();
            Game.Notification("Purchased", true);
        } else {
            Game.Notification("Not enough units...", true);
        }
    }

    public void Sell(){
        Player p = Game.GetPlayer();
        int index = p.inventory.GetIndexOf(item);
        if ( index != -1 ){
            p.inventory.AddCurrency((int)(amtSlider.value*item.cost*0.5f));
            p.inventory.RemoveItem(index,(int)amtSlider.value);
            canvas.GetComponent<NPCCanvas>().Sell();
            Game.Notification("Sold", true);
        }
    }

    public void Drop(){
        Player p = Game.GetPlayer();
        
        int index = p.inventory.GetIndexOf(item);
        if ( index != -1 ){
            p.inventory.RemoveItem(index,1);
            canvas.GetComponent<InventoryWindow>().UpdateDisplay();
        }
    }

    public void OnValueChange(){
        if ( buy ){
            btnText.text = "Buy - ";
        } else {
            btnText.text = "Sell - ";
        }

        btnText.text += item.cost * amtSlider.value + "u";
        amtText.text = amtSlider.value + "";
    }

    public void Equip(int x){
        Player p = Game.GetPlayer();
        if ( p.inventory.slots[x].item.GetAsEquip().CanEquip(p) )
            p.Equip(x);
    }

    public void Use(int x){
        Player p = Game.GetPlayer();
        if ( p.inventory.slots[x].item.GetAsUsable().CanUse() ){
            p.Use(x);
        }
    }

    public void Hotkey(){
        hotkeyWindow.SetActive(true);
    }

    public void SetHotkey(int index){
        Player p = Game.GetPlayer();
        if ( item.IsUsable() ){
            Game.Notification("Assigned " + item.name + " to Hotkey " + (index+1),true);
            p.SetHotkey(index,new UsableHotkey( p.inventory.GetIndexOf(item),item.GetAsUsable() ));
            hotkeyWindow.SetActive(false);
        } else {
            Game.Notification("Cannot assign this item to a hotkey",true);
        }
    }
}
