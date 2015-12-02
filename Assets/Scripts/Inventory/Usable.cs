using UnityEngine;
using System.Xml;
using System.Reflection;
using System.Collections;

[System.Serializable]
public class Usable : Item {

    public Stats stats;
    public Traits traits;
    public int targetAmount;
    public float duration;
    public UsableType usableType;
    public float cooldown;

    public Usable(){
        name = "";
        id = "";
        description = "";
        icon = null;
        weight = 0f;   
        recipe = null;
        tier = Tier.common;

        stats = new Stats();
        traits = new Traits();
        targetAmount = 0;
        duration = 0f;
        usableType = UsableType.restore;
        cooldown = 0f;
    }
    public Usable(Usable usable){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = usable.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,fields2[i].GetValue(usable));
        }
    }

    public override Usable GetAsUsable(){
        return this;
    }
    public override XmlElement ToXmlElement(XmlDocument xmlDoc){
        XmlElement root = xmlDoc.CreateElement("Usable");

        XmlElement xmlId = xmlDoc.CreateElement("Id"); xmlId.InnerText = id; root.AppendChild(xmlId);
        XmlElement xmlTier = xmlDoc.CreateElement("Tier"); xmlTier.InnerText = tier.ToString(); root.AppendChild(xmlTier);

        return root;
    }
}

public enum UsableType {
    restore,
    instantRestore,
    cure,
    buff,
    enchantScroll,
    upgradeCrystal
}
