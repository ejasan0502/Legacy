using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Inventory {
    
    public List<InventoryItem> items;

    public void Add(Item _item, int _amt){
        InventoryItem inventoryItem = GetInventoryItem(_item);
        if ( inventoryItem != null && inventoryItem.item.stackable ){
            inventoryItem.amt += _amt;
            if ( inventoryItem.amt > GlobalVariables.MAX_AMOUNT ){
                items.Add(new InventoryItem(_item,inventoryItem.amt-GlobalVariables.MAX_AMOUNT));
                inventoryItem.amt = GlobalVariables.MAX_AMOUNT;
            }
        } else {
            items.Add(new InventoryItem(_item,_amt));
        }
    }
    public void Remove(Item _item, int _amt){
        InventoryItem inventoryItem = GetInventoryItem(_item);
        if ( inventoryItem != null && inventoryItem.item.stackable ){
            inventoryItem.amt -= _amt;
            if ( inventoryItem.amt < 1 ){
                items.Remove(inventoryItem);
            }
        } else {
            items.Remove(inventoryItem);
        }
    }

    private InventoryItem GetInventoryItem(Item _item){
        return (InventoryItem) items.Where(i => i.item.id == _item.id);
    }
}

public class InventoryItem {
    public Item item;
    public int amt;

    public InventoryItem(Item i, int a){
        item = i;
        amt = a;
    }
}
