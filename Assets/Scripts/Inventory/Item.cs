using UnityEngine;
using System;
using System.Collections;

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

    public virtual Equip GetAsEquip(){
        return null;
    }
    public virtual Usable GetAsUsable(){
        return null;
    }
}
