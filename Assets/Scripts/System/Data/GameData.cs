using UnityEngine;
using System.Xml;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class GameData : MonoBehaviour {

    #region Variables
    
    #region Weapons
    public List<Equip> oneHandSwords;
    public List<Equip> twoHandSwords;
    public List<Equip> oneHandAxes;
    public List<Equip> twoHandAxes;
    public List<Equip> oneHandMaces;
    public List<Equip> twoHandMaces;
    public List<Equip> daggers;
    public List<Equip> shields;
    public List<Equip> staffs;
    public List<Equip> wands;
    public List<Equip> bows;
    public List<Equip> crossbows;
    public List<Equip> guns;
    public List<Equip> rifles;
    #endregion
    #region Armors
    public List<Equip> helms;
    public List<Equip> chests;
    public List<Equip> pants;
    public List<Equip> boots;
    public List<Equip> gloves;
    #endregion
    #region Accessories
    public List<Equip> rings;
    public List<Equip> necklaces;
    public List<Equip> capes;
    #endregion
    public List<Usable> usables = new List<Usable>();
    public List<Item> materials = new List<Item>();

    #endregion

    void Awake(){
        // Destroy duplicates
        GameData[] list = GameObject.FindObjectsOfType<GameData>();
        if ( list.Length > 1 ){
            // Found more than 1 object, meaning this instance is a duplicate
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);
    }

    #region Xml Extract Methods
    public void ExtractEquipsXmlData(){
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml( ((TextAsset)Resources.Load("data/RPG Equips Xml")).text );

        XmlNode root = xmlDoc.DocumentElement;

        foreach (XmlNode n1 in root.ChildNodes){
            Equip e = new Equip();
            foreach (XmlNode n2 in n1.ChildNodes){
                if ( n2.InnerText != "" ){
                    switch(n2.Name){
                    case "name":
                    e.name = n2.InnerText;
                    break;
                    case "id":
                    e.id = n2.InnerText;
                    break;
                    case "tier":
                    if ( n2.InnerText.ToLower() == "common" ) e.tier = Tier.common;
                    else if ( n2.InnerText.ToLower() == "rare" ) e.tier = Tier.rare;
                    else if ( n2.InnerText.ToLower() == "unique" ) e.tier = Tier.unique;
                    else if ( n2.InnerText.ToLower() == "legendary" ) e.tier = Tier.legendary;
                    else if ( n2.InnerText.ToLower() == "godly" ) e.tier = Tier.godly;
                    break;
                    case "description":
                    e.description = n2.InnerText;
                    break;
                    case "weight":
                    e.weight = float.Parse(n2.InnerText);
                    break;
                    case "cost":
                    e.cost = float.Parse(n2.InnerText);
                    break;
                    case "craftCost":
                    e.craftCost = float.Parse(n2.InnerText);
                    break;
                    case "craftChance":
                    e.craftChance =int.Parse(n2.InnerText);
                    break;
                    case "ingredients":
                    foreach (string s in n2.InnerText.Split(',')){
                        if ( s != "" ){
                            e.ingredients.Add(new Ingredient( s.Split('_')[0] , int.Parse(s.Split('_')[1]) ) );
                        }
                    }
                    break;
                    case "icon":
                    e.icon = Resources.Load(n2.InnerText+e.name,typeof(Sprite)) as Sprite;
                    if ( e.icon == null ) e.icon = Resources.Load("inventory/icons/weapons/Default",typeof(Sprite)) as Sprite;
                    break;
                    case "stats":
                    FieldInfo[] fields = e.stats.GetType().GetFields();

                    string[] stats = n2.InnerText.Split(',');
                    foreach (string ss in stats){
                        string statName = ss.Split('-')[0];
                        string statValue = ss.Split('-')[1];

                        foreach (FieldInfo f in fields){
                            if ( f.Name == statName ){
                                f.SetValue(e.stats,float.Parse(statValue));
                                break;
                            }
                        }
                    }
                    break;
                    case "model":
                    e.model = Resources.Load(n2.InnerText+"/"+e.name,typeof(GameObject)) as GameObject;
                    if ( e.model == null ) e.model = Resources.Load(n2.InnerText+"/Default",typeof(GameObject)) as GameObject;
                    break;
                    case "masteryLevel":
                    e.masteryLevel = int.Parse(n2.InnerText);
                    break;
                    case "equipType":
                    if ( n2.InnerText == "primaryWeapon" ) e.equipType = EquipType.primaryWeapon;
                    else if ( n2.InnerText == "secondaryWeapon" ) e.equipType = EquipType.secondaryWeapon;
                    else if ( n2.InnerText == "helm" ) e.equipType = EquipType.helm;
                    else if ( n2.InnerText == "chest" ) e.equipType = EquipType.primaryWeapon;
                    else if ( n2.InnerText == "pants" ) e.equipType = EquipType.pants;
                    else if ( n2.InnerText == "boots" ) e.equipType = EquipType.boots;
                    else if ( n2.InnerText == "gloves" ) e.equipType = EquipType.gloves;
                    else if ( n2.InnerText == "ring" ) e.equipType = EquipType.ring;
                    else if ( n2.InnerText == "necklace" ) e.equipType = EquipType.necklace;
                    else if ( n2.InnerText == "cape" ) e.equipType = EquipType.cape;
                    break;
                    case "requirements":
                    FieldInfo[] reqFields = e.requirements.GetType().GetFields();
                    foreach (string s in n2.InnerText.Split(',')){
                        string reqName = s.Split('-')[0];
                        string reqValue = s.Split('-')[1];

                        foreach (FieldInfo f in reqFields){
                            if ( f.Name == reqName ){
                                f.SetValue(e.requirements,float.Parse(reqValue));
                            }
                        }
                    }
                    break;
                    case "bonusStats":
                    FieldInfo[] bonusStatFields = e.stats.GetType().GetFields();

                    foreach (string ss in n2.InnerText.Split(',')){
                        string bonusStatName = ss.Split('-')[0];
                        string bonusStatValue = ss.Split('-')[1];

                        foreach (FieldInfo f in bonusStatFields){
                            if ( f.Name == bonusStatName ){
                                f.SetValue(e.stats,float.Parse(bonusStatValue));
                                break;
                            }
                        }
                    }
                    break;
                    case "abilities":
                    break;
                    }
                }
            }

            // Add to lists
            switch(e.id.ToLower().Split('.')[0]){
            case "w":
            AddWeapon(e);
            break;
            case "ar":
            AddArmor(e);
            break;
            case "a":
            AddAccessory(e);
            break;
            }
        }

        Console.Log("Equips Xml Data extracted.");
    }
    public void ExtractItemsXmlData(){
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml( ((TextAsset)Resources.Load("data/RPG Items Xml")).text );

        XmlNode root = xmlDoc.DocumentElement;

        foreach (XmlNode n1 in root.ChildNodes){
            Item e = new Item();
            foreach (XmlNode n2 in n1.ChildNodes){
                if ( n2.InnerText != "" ){
                    switch(n2.Name){
                    case "name":
                    e.name = n2.InnerText;
                    break;
                    case "id":
                    e.id = n2.InnerText;
                    break;
                    case "tier":
                    if ( n2.InnerText.ToLower() == "common" ) e.tier = Tier.common;
                    else if ( n2.InnerText.ToLower() == "rare" ) e.tier = Tier.rare;
                    else if ( n2.InnerText.ToLower() == "unique" ) e.tier = Tier.unique;
                    else if ( n2.InnerText.ToLower() == "legendary" ) e.tier = Tier.legendary;
                    else if ( n2.InnerText.ToLower() == "godly" ) e.tier = Tier.godly;
                    break;
                    case "description":
                    e.description = n2.InnerText;
                    break;
                    case "weight":
                    e.weight = float.Parse(n2.InnerText);
                    break;
                    case "cost":
                    e.cost = float.Parse(n2.InnerText);
                    break;
                    case "craftCost":
                    e.craftCost = float.Parse(n2.InnerText);
                    break;
                    case "craftChance":
                    e.craftChance =int.Parse(n2.InnerText);
                    break;
                    case "ingredients":
                    foreach (string s in n2.InnerText.Split(',')){
                        if ( s != "" ){
                            e.ingredients.Add(new Ingredient( s.Split('_')[0] , int.Parse(s.Split('_')[1]) ) );
                        }
                    }
                    break;
                    case "icon":
                    e.icon = Resources.Load(n2.InnerText+e.name,typeof(Sprite)) as Sprite;
                    if ( e.icon == null ) e.icon = Resources.Load("inventory/icons/materials/Default",typeof(Sprite)) as Sprite;
                    break;
                    case "stats":
                    FieldInfo[] fields = e.stats.GetType().GetFields();

                    string[] stats = n2.InnerText.Split(',');
                    foreach (string ss in stats){
                        string statName = ss.Split('-')[0];
                        string statValue = ss.Split('-')[1];

                        foreach (FieldInfo f in fields){
                            if ( f.Name == statName ){
                                f.SetValue(e.stats,float.Parse(statValue));
                                break;
                            }
                        }
                    }
                    break;
                    }
                }
            }

            // Add to lists
            int index = int.Parse(e.id.Split('-')[1]);
            materials.Insert(index,e);
        }

        Console.Log("Items Xml Data extracted.");
    }
    public void ExtractUsablesXmlData(){
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml( ((TextAsset)Resources.Load("data/RPG Usables Xml")).text );

        XmlNode root = xmlDoc.DocumentElement;

        foreach (XmlNode n1 in root.ChildNodes){
            Usable e = new Usable();
            foreach (XmlNode n2 in n1.ChildNodes){
                if ( n2.InnerText != "" ){
                    switch(n2.Name){
                    case "name":
                    e.name = n2.InnerText;
                    break;
                    case "id":
                    e.id = n2.InnerText;
                    break;
                    case "tier":
                    if ( n2.InnerText.ToLower() == "common" ) e.tier = Tier.common;
                    else if ( n2.InnerText.ToLower() == "rare" ) e.tier = Tier.rare;
                    else if ( n2.InnerText.ToLower() == "unique" ) e.tier = Tier.unique;
                    else if ( n2.InnerText.ToLower() == "legendary" ) e.tier = Tier.legendary;
                    else if ( n2.InnerText.ToLower() == "godly" ) e.tier = Tier.godly;
                    break;
                    case "description":
                    e.description = n2.InnerText;
                    break;
                    case "weight":
                    e.weight = float.Parse(n2.InnerText);
                    break;
                    case "cost":
                    e.cost = float.Parse(n2.InnerText);
                    break;
                    case "craftCost":
                    e.craftCost = float.Parse(n2.InnerText);
                    break;
                    case "craftChance":
                    e.craftChance =int.Parse(n2.InnerText);
                    break;
                    case "ingredients":
                    foreach (string s in n2.InnerText.Split(',')){
                        if ( s != "" ){
                            e.ingredients.Add(new Ingredient( s.Split('_')[0] , int.Parse(s.Split('_')[1]) ) );
                        }
                    }
                    break;
                    case "icon":
                    e.icon = Resources.Load(n2.InnerText+e.name,typeof(Sprite)) as Sprite;
                    if ( e.icon == null ) e.icon = Resources.Load("inventory/icons/usables/Default",typeof(Sprite)) as Sprite;
                    break;
                    case "stats":
                    FieldInfo[] fields = e.stats.GetType().GetFields();

                    string[] stats = n2.InnerText.Split(',');
                    foreach (string ss in stats){
                        string statName = ss.Split('-')[0];
                        string statValue = ss.Split('-')[1];

                        foreach (FieldInfo f in fields){
                            if ( f.Name == statName ){
                                f.SetValue(e.stats,float.Parse(statValue));
                                break;
                            }
                        }
                    }
                    break;
                    case "usableType":
                    if ( n2.InnerText.ToLower() == "potion" ) e.usableType = UsableType.potion;
                    else if ( n2.InnerText.ToLower() == "replenish" ) e.usableType = UsableType.replenish;
                    else if ( n2.InnerText.ToLower() == "buff" ) e.usableType = UsableType.buff;
                    else if ( n2.InnerText.ToLower() == "buffPercent" ) e.usableType = UsableType.buffPercent;
                    else if ( n2.InnerText.ToLower() == "damage" ) e.usableType = UsableType.damage;
                    else if ( n2.InnerText.ToLower() == "aoe" ) e.usableType = UsableType.aoe;
                    else if ( n2.InnerText.ToLower() == "dot" ) e.usableType = UsableType.dot;
                    break;
                    case "duration":
                    e.duration = float.Parse(n2.InnerText);
                    break;
                    case "friendly":
                    if ( n2.InnerText == "true" ) e.friendly = true;
                    else e.friendly = false;
                    break;
                    }
                }
            }

            // Add to lists
            int index = int.Parse(e.id.Split('-')[1]);
            usables.Insert(index,e);
        }

        Console.Log("Usables Xml Data extracted.");
    }
    #endregion
    #region Add Methods
    private void AddWeapon(Equip e){
        string itemType = e.id.Split('.')[1].Split('-')[0];
        int index = int.Parse(e.id.Split('.')[1].Split('-')[1]);
        switch(itemType){
        case "1hs":
        oneHandSwords.Insert(index,e);
        break;
        case "2hs":
        twoHandSwords.Insert(index,e);
        break;
        case "1ha":
        oneHandAxes.Insert(index,e);
        break;
        case "2ha":
        twoHandAxes.Insert(index,e);
        break;
        case "1hm":
        oneHandMaces.Insert(index,e);
        break;
        case "2hm":
        twoHandMaces.Insert(index,e);
        break;
        case "d":
        daggers.Insert(index,e);
        break;
        case "s":
        shields.Insert(index,e);
        break;
        case "b":
        bows.Insert(index,e);
        break;
        case "c":
        crossbows.Insert(index,e);
        break;
        case "st":
        staffs.Insert(index,e);
        break;
        case "w":
        wands.Insert(index,e);
        break;
        case "g":
        guns.Insert(index,e);
        break;
        case "r":
        rifles.Insert(index,e);
        break;
        }
    }
    private void AddArmor(Equip e){
        string itemType = e.id.Split('.')[1].Split('-')[0];
        int index = int.Parse(e.id.Split('.')[1].Split('-')[1]);

        switch(itemType){
        case "h":
        helms.Insert(index,e);
        break;
        case "c":
        chests.Insert(index,e);
        break;
        case "p":
        pants.Insert(index,e);
        break;
        case "b":
        boots.Insert(index,e);
        break;
        case "g":
        gloves.Insert(index,e);
        break;
        }
    }
    private void AddAccessory(Equip e){
        string itemType = e.id.Split('.')[1].Split('-')[0];
        int index = int.Parse(e.id.Split('.')[1].Split('-')[1]);

        switch(itemType){
        case "r":
        rings.Insert(index,e);
        break;
        case "n":
        necklaces.Insert(index,e);
        break;
        case "c":
        capes.Insert(index,e);
        break;
        }
    }
    #endregion
    #region Get Methods
    public Item GetItem(string id){
        if ( id.ToLower().StartsWith("w") ){
            return GetWeapon(id);
        } else if ( id.ToLower().StartsWith("u") ){
            return GetUsable(id);
        } else if ( id.ToLower().StartsWith("m") ){
            return GetMaterial(id);
        } else if ( id.ToLower().StartsWith("ar") ){
            return GetArmor(id);
        } else if ( id.ToLower().StartsWith("a") ){
            return GetAccessory(id);
        } 

        return null;
    }

    public Equip GetWeapon(string id){
        string s = id.ToLower().Split('.')[1];
        string subItemType = s.Split('-')[0];
        int index = int.Parse(s.Split('-')[1]);

        List<Equip> list = new List<Equip>();
        switch(subItemType){
        case "1hs":
        list = oneHandSwords;
        break;
        case "2hs":
        list = twoHandSwords;
        break;
        case "1ha":
        list = oneHandAxes;
        break;
        case "2ha":
        list = twoHandAxes;
        break;
        case "1hm":
        list = oneHandMaces;
        break;
        case "2hm":
        list = twoHandMaces;
        break;
        case "d":
        list = daggers;
        break;
        case "s":
        list = shields;
        break;
        case "b":
        list = bows;
        break;
        case "c":
        list = crossbows;
        break;
        case "st":
        list = staffs;
        break;
        case "w":
        list = wands;
        break;
        case "g":
        list = guns;
        break;
        case "r":
        list = rifles;
        break;
        }

        if ( index < list.Count )
            return list[index];
        else
            return null;
    }

    public Equip GetArmor(string id){
        string s = id.ToLower().Split('.')[1];
        string subItemType = s.Split('-')[0];
        int index = int.Parse(s.Split('-')[1]);

        List<Equip> list = new List<Equip>();

        switch(subItemType){
        case "h":
        list = helms;
        break;
        case "c":
        list = chests;
        break;
        case "p":
        list = pants;
        break;
        case "b":
        list = boots;
        break;
        case "g":
        list = gloves;
        break;
        }

        if ( index < list.Count )
            return list[index];
        else
            return null;
    }

    public Equip GetAccessory(string id){
        string s = id.ToLower().Split('.')[1];
        string subItemType = s.Split('-')[0];
        int index = int.Parse(s.Split('-')[1]);

        List<Equip> list = new List<Equip>();

        switch(subItemType){
        case "r":
        list = rings;
        break;
        case "n":
        list = necklaces;
        break;
        case "c":
        list = capes;
        break;
        }

        if ( index < list.Count )
            return list[index];
        else
            return null;
    }

    public Usable GetUsable(string id){
        int index = int.Parse(id.Split('-')[1]);
        
        if ( index < usables.Count )
            return usables[index];
        else
            return null;
    }

    public Item GetMaterial(string id){
        Debug.Log(id);
        int index = int.Parse(id.Split('-')[1]);
        
        if ( index < materials.Count )
            return materials[index];
        else
            return null;
    }

    public Item GetRandomMaterial(){
        return materials[Random.Range(0,materials.Count)];
    }
    #endregion
}
