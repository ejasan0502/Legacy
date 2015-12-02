using UnityEngine;
using System.Xml;
using System.Reflection;
using System.Collections;

[System.Serializable]
public class Item {
    public string name;
    public string id;
    public string description;
    public Sprite icon;
    public float weight;
    public Recipe recipe;
    public Tier tier;

    public bool stackable {
        get {
            return !isEquip;
        }
    }
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
        weight = 0f;   
        recipe = null;
        tier = Tier.common;
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
                id == ((Item)obj).id &&
                tier == ((Item)obj).tier;
    }
    public override int GetHashCode(){
        return base.GetHashCode();
    }

    // Returns the item as a xml element
    // Used to save player data to server
    public virtual XmlElement ToXmlElement(XmlDocument xmlDoc){
        XmlElement root = xmlDoc.CreateElement("Item");

        XmlElement xmlId = xmlDoc.CreateElement("Id"); xmlId.InnerText = id; root.AppendChild(xmlId);
        XmlElement xmlTier = xmlDoc.CreateElement("Tier"); xmlTier.InnerText = tier.ToString(); root.AppendChild(xmlTier);

        return root;
    }
}

public enum Tier {
    common,
    rare,
    unique
}

// ID Composition
// ( Type )( Secondary Type ) - # (_M)
// _M indicates that the item has been modified by the player
// Type = E (Equip), U (Usable), M (Material), C (Character), S (Skill)
// Secondary Type = swd (Sword), shd (Shield), brd (Broadsword), spr (Spear), axe, stf (Staff), wad (Wand), dar (Dagger), mae (Mace), bow, hA (Heavy Armor), lA (Light Armor), cA (Cloth Armor), acy (Accessory)
//                  ree (Restore), ine (Instant Restore), cue (Cure), buf (Buff), enl (Enchant Scroll), upl (Upgrade Crystal)
//                  bld (Blood), clh (Cloth), dew, ler (Leather), otr (Other), stl (Steel), ucp (Upgrade Crystal Piece), wod (Wood)
//                  hem (Helm), sht (Shirt), pat (Pants), gls (Gloves), bos (Boots), eas (Ears), tal (Tail)
//                  
