using UnityEngine;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Inventory {
    [XmlArray("Inventory"),XmlArrayItem("InventoryItem")] public List<InventoryItem> slots;
    public float currency;
    public float weight;
    public float maxWeight;

    public Inventory(Player p){
        slots = new List<InventoryItem>();
        currency = 0f;
        maxWeight = 20f + p.attributes.strength*0.165f + p.level*p.level*0.33f;
        weight = 0f;
    }

    public Item[] GetItemsList(){
        Item[] items = new Item[slots.Count];

        for (int i = 0; i < items.Length; i++){
            items[i] = slots[i].item;
        }

        return items;
    }

    public void AddCurrency(int x){
        currency += x;
        if ( currency < 0f ){
            currency = 0f;
        }
    }

    public void AddItem(Equip item){
        slots.Add(new InventoryItem(new Equip(item),1));

        RecalculateWeight();
    }

    public void AddItem(Item item, int amt){
        if ( item.IsEquip() ){
            AddItem(item.GetAsEquip());
        } else {
            int i = GetIndexOf(item);

            if ( i != -1 ){
                slots[i].amount += amt;
            } else {
                slots.Add(new InventoryItem(item,amt));
            }
        }
        
        RecalculateWeight();
    }

    public void RemoveItem(int index, int amt){
        if ( index < slots.Count ){
            slots[index].amount -= amt;
            if ( slots[index].amount < 1 ) slots.RemoveAt(index);
            RecalculateWeight();
        }
    }

    public void RemoveItem(Item item, int amt){
        int i = GetIndexOf(item);

        if ( i != -1 ){
            slots[i].amount -= amt;
            if ( amt < 1 ){
                slots.RemoveAt(i);
            }
            RecalculateWeight();
        } else {
            Console.Log(item.name + " does not exist in inventory.");
        }
    }

    public int GetIndexOf(Item item){
        for (int i = 0; i < slots.Count; i++){
            if ( slots[i].item.id == item.id ){
                return i;
            }
        }
        return -1;
    }

    public InventoryItem GetInventorySlot(Item item){
        int index = GetIndexOf(item);
        if ( index != -1 ) 
            return slots[index];

        return null;
    }

    private void RecalculateWeight(){
        weight = 0f;
        foreach (Item i in GetItemsList()){
            weight += i.weight;
        }
    }
}

public class InventoryItem {
    public Item item;
    public int amount;

    public InventoryItem(Item i, int amt){
        item = i;
        amount = amt;
    }
}
