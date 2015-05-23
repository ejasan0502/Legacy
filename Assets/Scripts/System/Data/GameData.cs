using UnityEngine;
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
    public List<Usable> usables;
    public List<Item> materials;

    #endregion

    void Awake(){
        // Destroy duplicates
        GameData[] list = GameObject.FindObjectsOfType<GameData>();
        if ( list.Length > 1 ){
            // Found more than 1 object, meaning this instance is a duplicate
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        //ExtractXmlData("data/RPG Items Xml");
    }

    private void ExtractXmlData(string path){
        
    }

    #region Get Methods
    public Item GetItem(string id){
        switch(id.ToLower().Split('.')[0]){
        case "w":
        return GetWeapon(id);
        case "u":
        return GetUsable(id);
        case "m":
        return GetMaterial(id);
        case "ar":
        return GetArmor(id);
        case "a":
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
