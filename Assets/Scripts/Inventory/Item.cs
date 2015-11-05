using UnityEngine;
using System.Reflection;
using System.Collections;

[System.Serializable]
public class Item {
    public string name;
    public string id;
    public string description;
    public Texture2D icon;
    public bool stackable;
    public float weight;

    public bool isEquip {
        get {
            return GetAsEquip() != null;
        }
    }
    public bool isUsable {
        get {
            return GetAsUsable() != null;
        }
    }

    public Item(){
        name = "";
        id = "";
        description = "";
        icon = null;
        stackable = true;
        weight = 0f;   
    }
    public Item(Item item){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] itemFields = item.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,itemFields[i].GetValue(item));
        }
    }

    public virtual Equip GetAsEquip(){
        return null;
    }
    public virtual Usable GetAsUsable(){
        return null;
    }
}
