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
        otherBtn.gameObject.SetActive(true);
        otherBtn.onClick.AddListener(() => Equip(index));

        amtSlider.gameObject.SetActive(false);
        amtText.gameObject.SetActive(false);
    }

    public void SetItem(Item i, Canvas c, bool b){
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

            Game.Notification(canvas,"Purchased", true);
        } else {
            Game.Notification(canvas,"Not enough units...", true);
        }
    }

    public void Sell(){
        Player p = Game.GetPlayer();
        int index = p.inventory.GetIndexOf(item);
        if ( index != -1 ){
            p.inventory.AddCurrency((int)(amtSlider.value*item.cost*0.5f));
            p.inventory.RemoveItem(index,(int)amtSlider.value);
            canvas.GetComponent<NPCCanvas>().Sell();
            Game.Notification(canvas,"Sold", true);
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
}
