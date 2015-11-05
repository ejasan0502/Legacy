using UnityEngine;
using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameData {
    
    public List<Equip> weapons;
    public List<Equip> armors;

    public List<Usable> usables;
    public List<Item> materials; 

    private static GameData _instance = null;
    public static GameData instance {
        get {
            if ( _instance == null )
                _instance = new GameData();
            return _instance;
        }
    }

    private GameData(){
        weapons = new List<Equip>();
        armors = new List<Equip>();

        usables = new List<Usable>();
        materials = new List<Item>();
    }
    private Equip LoadXmlToEquip(XmlNode node){
        Equip e = new Equip();

        foreach (XmlNode content in node.ChildNodes){
            switch( content.Name.ToLower() ){
                case "name":
                    e.name = content.InnerText;
                break;
                case "id":
                    e.id = content.InnerText;
                break;
                case "description":
                    e.description = content.InnerText;
                break;
                case "icon":
                    string name = e.name;
                    if ( e.name == "" ) name = "_Default";
                    Texture2D texture = (Texture2D) Resources.Load("Textures/Icons/Equips/"+name,typeof(Texture2D));
                    e.icon = texture;
                break;
                case "weight":
                    e.weight = float.Parse(content.InnerText);
                break;
                case "level":
                    e.level = int.Parse(content.InnerText);
                break;
                case "stats":
                    if ( content.InnerText != "" ){
                        Stats stats = new Stats();
                        string[] s = content.InnerText.Split(',');
                        for (int i = 0; i < s.Length; i++){
                            stats.Set( s[i].Split('-')[0] , float.Parse(s[i].Split('-')[1]) );
                        }
                        e.stats = stats;
                    }
                break;
                case "bonusStats":
                    if ( content.InnerText != "" ){
                        Stats stats = new Stats();
                        string[] s = content.InnerText.Split(',');
                        for (int i = 0; i < s.Length; i++){
                            stats.Set( s[i].Split('-')[0] , float.Parse(s[i].Split('-')[1]) );
                        }
                        e.bonusStats = stats;
                    }
                break;
                case "bonusTraits":
                    if ( content.InnerText != "" ){
                        Traits traits = new Traits();
                        string[] s = content.InnerText.Split(',');
                        for (int i = 0; i < s.Length; i++){
                            traits.Set( s[i].Split('-')[0] , float.Parse(s[i].Split('-')[1]) );
                        }
                        e.bonusTraits = traits;
                    }
                break;
                case "type":
                    foreach (EquipType et in Enum.GetValues(typeof(EquipType))){
                        if ( et.ToString() == content.InnerText ){
                            e.equipType = et;
                            break;
                        }
                    }
                break;
            }
        }

        return e;
    }
    private Usable LoadXmlToUsable(XmlNode node){
        Usable u = new Usable();

        foreach (XmlNode content in node.ChildNodes){
            switch( content.Name.ToLower() ){
                case "name":
                    u.name = content.InnerText;
                break;
                case "id":
                    u.id = content.InnerText;
                break;
                case "traits":
                    if ( content.InnerText != "" ){
                        Traits traits = new Traits();
                        string[] s = content.InnerText.Split(',');
                        for (int i = 0; i < s.Length; i++){
                            traits.Set( s[i].Split('-')[0] , float.Parse(s[i].Split('-')[1]) );
                        }
                        u.traits = traits;
                    }
                break;
                case "stats":
                    if ( content.InnerText != "" ){
                        Stats stats = new Stats();
                        string[] s = content.InnerText.Split(',');
                        for (int i = 0; i < s.Length; i++){
                            stats.Set( s[i].Split('-')[0] , float.Parse(s[i].Split('-')[1]) );
                        }
                        u.stats = stats;
                    }
                break;
                case "target amount":
                    u.targetAmount = int.Parse(content.InnerText);
                break;
                case "duration":
                    u.duration = int.Parse(content.InnerText);
                break;
                case "weight":
                    u.weight = float.Parse(content.InnerText);
                break;
                case "type":
                    foreach (UsableType et in Enum.GetValues(typeof(UsableType))){
                        if ( et.ToString() == content.InnerText ){
                            u.usableType = et;
                            break;
                        }
                    }
                break;
                case "cooldown":
                    u.cooldown = float.Parse(content.InnerText);
                break;
                case "description":
                    u.description = content.InnerText;
                break;
                case "icon":
                    string name = u.name;
                    if ( u.name == "" ) name = "_Default";
                    Texture2D texture = (Texture2D) Resources.Load("Textures/Icons/Usables/"+name,typeof(Texture2D));
                    u.icon = texture;
                break;
            }
        }

        return u;
    }
    private Item LoadXmlToItem(XmlNode node){
        Item u = new Item();

        foreach (XmlNode content in node.ChildNodes){
            switch( content.Name.ToLower() ){
                case "name":
                    u.name = content.InnerText;
                break;
                case "id":
                    u.id = content.InnerText;
                break;
                case "weight":
                    u.weight = float.Parse(content.InnerText);
                break;
                case "description":
                    u.description = content.InnerText;
                break;
                case "icon":
                    string name = u.name;
                    if ( u.name == "" ) name = "_Default";
                    Texture2D texture = (Texture2D) Resources.Load("Textures/Icons/Materials/"+name,typeof(Texture2D));
                    u.icon = texture;
                break;
            }
        }

        return u;
    }

    public void LoadFromXml(string textData, Type type){
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(textData);

        XmlNode root = xmlDoc.FirstChild;
        foreach (XmlNode node in root.ChildNodes){
            if ( type.Name == "Equip" ){
                Equip e = LoadXmlToEquip(node);
                if ( (int)e.equipType < 2 ){
                    if ( !weapons.Contains(e) ) 
                        weapons.Add(e);
                } else {
                    if ( !armors.Contains(e) ) 
                        armors.Add(e);
                }
            } else if ( type.Name == "Usable" ){
                Usable u = LoadXmlToUsable(node);
                if ( usables.Where(usable => usable.id == u.id) == null ) 
                    usables.Add(u);
            } else {
                Item i = LoadXmlToItem(node);
                if ( materials.Where(material => material.id == i.id) == null ) 
                    materials.Add(i);
            }
        }
    }
}
