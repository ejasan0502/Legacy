﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CraftInfo : MonoBehaviour {
    
    private Item item;
    private int amount;

    public Text textName;
    public Image portrait;
    public Text statsText;
    public Text ingredientsText;
    public Button btn;
    public Text btnText;

    private bool canCraft = true;

    public void SetItem(Item it, int amt){
        item = it;
        amount = amt;

        textName.text = item.name;
        portrait.sprite = item.icon;
        statsText.text = item.stats.ToString();
        
        Player p = Game.GetPlayer();
        string s = "";
        float sizeY = 0f;
        for (int i = 0; i < item.ingredients.Count; i++){
            int have = 0;
            InventoryItem ii = p.inventory.GetInventorySlot(item.ingredients[i].item);
            if ( ii != null ) have = ii.amount;
            s += Game.GetGameData().GetItem(item.ingredients[i].item).name + " " + have + "/" + item.ingredients[i].amount;
            sizeY += 30f;

            if ( have < item.ingredients[i].amount ){
                canCraft = false;
            }
        }

        btnText.text = "Craft - " + item.craftCost + "u";
        btn.onClick.AddListener(() => Craft());

        RectTransform rt = (RectTransform)ingredientsText.transform;
        rt.sizeDelta = new Vector2(rt.rect.width,sizeY);
    }

    public void Craft(){
        Player p = Game.GetPlayer();
        if ( p.inventory.currency >= item.craftCost){
            if ( canCraft ){
                p.inventory.currency -= item.craftCost;
                if ( Random.Range(0,100)*p.attributes.luck <= item.craftChance ){
                    p.inventory.AddItem(item,amount);
                    Game.Notification("Crafting successful!", true);
                } else {
                    Game.Notification("Crafting failed...", true);
                }
            }
        } else {
            Game.Notification("Not enough units...", true);
        }
    }
}
