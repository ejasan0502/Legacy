using UnityEngine;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Equip : Item {

    public int level;
    public Stats stats;
    public Stats bonusStats;
    public Traits bonusTraits;
    public EquipType equipType;
    public int upgrades;
    public List<Enchant> enchants;

    public bool isWeapon {
        get {
            return  (int) equipType == 0;
        }
    }
    public bool isArmor {
        get {
            return (int) equipType == 2;
        }
    }
    public bool isAccessory {
        get {
            return (int) equipType == 3;
        }
    }
    public bool oneHanded {
        get {
            return  equipType == EquipType.sword ||
                    equipType == EquipType.axe ||
                    equipType == EquipType.wand ||
                    equipType == EquipType.dagger ||
                    equipType == EquipType.mace;
        }
    }

    public string modelPath {
        get {
            string s = "";

            if ( isWeapon )
                s = GlobalVariables.PATH_CONTENTDATA + id[0] + "/" + id.Substring(1,3) + "/" + name + "/model";
            else if ( isArmor )
                s = GlobalVariables.PATH_CONTENTDATA + id[0] + "/" + id.Substring(1,2) + "/" + name;

            return s;
        }
    }
    public string iconPath {
        get {
            string s = "";

            if ( isArmor )
                s = GlobalVariables.PATH_CONTENTDATA + id[0] + "/" + id.Substring(1,2) + "/" + name + "/icon";
            else 
                s = GlobalVariables.PATH_CONTENTDATA + id[0] + "/" + id.Substring(1,3) + "/" + name + "/icon";

            return s;
        }
    }

    public Equip(){
        name = "";
        id = "";
        description = "";
        icon = null;
        weight = 0f;   
        recipe = null;
        tier = Tier.common;

        level = 0;
        stats = new Stats();
        bonusStats = new Stats();
        bonusTraits = new Traits();
        equipType = 0;
        upgrades = 0;
        enchants = new List<Enchant>();
    }
    public Equip(Equip equip){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] itemFields = equip.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,itemFields[i].GetValue(equip));
        }
    }

    public override Equip GetAsEquip(){
        return this;
    }
    public override XmlElement ToXmlElement(XmlDocument xmlDoc){
        XmlElement root = xmlDoc.CreateElement("Equip");

        XmlElement xmlId = xmlDoc.CreateElement("Id"); xmlId.InnerText = id; root.AppendChild(xmlId);
        XmlElement xmlTier = xmlDoc.CreateElement("Tier"); xmlTier.InnerText = tier.ToString(); root.AppendChild(xmlTier);
        // Check if equip has been modified by the player
        if ( id.EndsWith("_M") ){
            XmlElement xmlUpgrades = xmlDoc.CreateElement("Upgrades"); xmlUpgrades.InnerText = upgrades+""; root.AppendChild(xmlUpgrades);
            if ( enchants.Count > 0 ){
                XmlElement xmlEnchants = xmlDoc.CreateElement("Enchants");
                for (int i = 0; i < enchants.Count; i++){
                    if ( i == 0 )
                        xmlEnchants.InnerText = enchants[i].ToString();
                    else
                        xmlEnchants.InnerText += "," + enchants[i].ToString();
                }
                root.AppendChild(xmlEnchants);
            }
        }

        return root;
    }
}

public enum EquipType {
    sword = 0,
    shield = 1,
    broadsword = 0,
    spear = 0,
    axe = 0,
    staff = 0,
    wand = 0,
    dagger = 0,
    mace = 0,
    bow = 0,
    heavyArmor = 2,
    lightArmor = 2,
    clothArmor = 2,
    accessory = 3
}
