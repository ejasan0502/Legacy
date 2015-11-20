using UnityEngine;
using System.Reflection;
using System.Collections;

[System.Serializable]
public class Item {
    public string name;
    public string id;
    public string description;
    public Sprite icon;
    public bool stackable;
    public float weight;
    public Recipe recipe;

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
        recipe = null;
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

    public override bool Equals(object obj){
        return  obj != null && 
                id == ((Item)obj).id;
    }
    public override int GetHashCode(){
        return base.GetHashCode();
    }
}

// Item ID Composition
// ( Type )( Secondary Type ) - #
// Type = E (Equip), U (Usable), M (Material), C (Character)
// Secondary Type = swd (Sword), shd (Shield), brd (Broadsword), spr (Spear), axe, stf (Staff), wad (Wand), dar (Dagger), mae (Mace), bow, hA (Heavy Armor), lA (Light Armor), cA (Cloth Armor), acy (Accessory)
//                  ree (Restore), ine (Instant Restore), cue (Cure), buf (Buff), enl (Enchant Scroll), upl (Upgrade Crystal)
//                  bld (Blood), clh (Cloth), dew, ler (Leather), otr (Other), stl (Steel), ucp (Upgrade Crystal Piece), wod (Wood)
//                  hem (Helm), sht (Shirt), pat (Pants), gls (Gloves), bos (Boots), eas (Ears), tal (Tail)
//                  
