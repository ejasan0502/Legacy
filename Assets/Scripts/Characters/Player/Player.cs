using UnityEngine;
using UnityEngine.UI;
using System.Xml;
using System.Xml.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;

[XmlRoot("Player")]
[System.Serializable]
public class Player : Character {
    #region Public Variables
    public float maxExp;
    public float classLevel;
    public float classExp;
    public float maxClassExp;
    public float classPoints;

    public bool male;
    public string race;
    
    public Characteristics attributes = new Characteristics();
    [HideInInspector][XmlArray("Equipment"),XmlArrayItem("Equip")] public Equip[] equipment = new Equip[11];

    public Inventory inventory;

    [HideInInspector][XmlArray("Hotkeys"),XmlArrayItem("Hotkey")] public Hotkey[] hotkeys = new Hotkey[10];

    public string lastSavedScene = "";
    public string lastSafeZone = "";
    public TutorialState tutorialState = TutorialState.start;

    public List<Quest> quests = new List<Quest>();
    public List<string> completedQuests = new List<string>();
    #endregion

    private Stats baseStats;
    private Stats bonusStats;
    private PlayerInfo playerInfoDisplay;
    private InputControls controls;

    public override bool IsPlayer {
        get {
            return true;
        }
    }

    public Player(){
        inventory = new Inventory(this);
        CalculateStats();
    }

    #region Public Methods
    public void Resurrect(){
        // Move player
        if ( lastSafeZone != "" ){
            GameObject safeZone = GameObject.Find(lastSafeZone);
            if ( safeZone != null ){
                characterObject.transform.position = safeZone.transform.position;
            } else {
                Console.Error("Player.cs - Resurrect(): Safe zone does not exist in scene. Safe Zone = " + lastSafeZone);
            }
        } else {
            GameObject safeZone = GameObject.Find("Default Safe Zone");
            if ( safeZone != null ){
                characterObject.transform.position = safeZone.transform.position;
            } else {
                Console.Error("Player.cs - Resurrect(): Default safe zone cannot be found in scene.");
            }
        }

        // Reset stats
        currentStats = new Stats(stats);

        // Reset Controls
        PlayerObject po = characterObject as PlayerObject;
        po.SetTarget(null);
        po.SetState(CharacterState.idle);
        po.SetControls(true);
        po.StartStateMachine();
        po.StopMovement();

        // Reset animations
        Animator anim = characterObject.GetComponent<Animator>();
        anim.speed = 1f;
        anim.SetBool("Attack",false);
        anim.SetBool("Battle",false);
        anim.SetBool("Move",false);
        anim.SetBool("Death",false);
        anim.SetBool("Reset",true);

        // Reset HUD
        HUD.instance.resurrectBtn.gameObject.SetActive(false);
    }
    public void SetHotkey(int index, Hotkey h){
        if ( index >= 0 && index < hotkeys.Length ) {
            for (int i = 0; i < hotkeys.Length; i++){
                if ( hotkeys[i] != null && hotkeys[i].GetId() == h.GetId() ){
                    HotkeyManager.instance.transform.GetChild(i).GetChild(0).GetComponent<Image>().sprite = null;
                    if ( !hotkeys[i].IsSkill ) HotkeyManager.instance.transform.GetChild(i).GetChild(2).gameObject.SetActive(false);
                    hotkeys[i] = null;
                    break;
                }
            }

            hotkeys[index] = h;
            HotkeyManager.instance.transform.GetChild(index).GetChild(0).GetComponent<Image>().sprite = hotkeys[index].GetIcon();
            if ( !hotkeys[index].IsSkill ){
                HotkeyManager.instance.transform.GetChild(index).GetChild(2).gameObject.SetActive(true);
                HotkeyManager.instance.UpdateHotkeyText(index);
            } else {
                HotkeyManager.instance.transform.GetChild(index).GetChild(2).gameObject.SetActive(false);
            }
        }
    }
    public void Use(int index){
        PlayerObject po = characterObject as PlayerObject;
        if ( index >= 0 && index < inventory.slots.Count ){
            Item item = inventory.slots[index].item;
            if ( item.IsUsable() ){
                Usable u = item.GetAsUsable();
                if ( po.GetTarget() != null ){
                    if ( u.friendly && po.GetTarget().IsPlayer || !u.friendly && !po.GetTarget().IsPlayer ){
                        u.Use(po.GetTarget());
                        inventory.RemoveItem(index,1);
                        MenusWindow.instance.inventoryWindow.UpdateDisplay();
                        Game.Notification("Used " + u.name,true);
                    } else {
                        Game.Notification("Invalid target",true);
                    }
                } else {
                    if ( u.friendly ){
                        u.Use(po.c);
                        inventory.RemoveItem(index,1);
                        MenusWindow.instance.inventoryWindow.UpdateDisplay();
                        Game.Notification("Used " + u.name,true);
                    }
                }
            } else {
                Console.Log("Cannot use this item");
            }
        }
    }
    public void Equip(int index){
        if ( index >= 0 && index < inventory.slots.Count ){
            Item item = inventory.slots[index].item;
            if ( item.IsEquip() ){
                Equip e = item.GetAsEquip();
                if ( equipment[(int)e.equipType] != null ) Unequip((int)e.equipType);
                equipment[(int)e.equipType] = e;
                CalculateEquipmentStats();
                
                // Create Model
                Model model = characterObject.GetComponent<Model>();
                GameObject o = (GameObject) GameObject.Instantiate(e.model);

                if ( e.equipType == EquipType.primaryWeapon ){
                    o.transform.SetParent(model.primaryWeaponRoot);

                    Quaternion rotation = model.transform.rotation;
                    rotation = Quaternion.Euler(rotation.eulerAngles.x,rotation.eulerAngles.y-180f,rotation.eulerAngles.z);
                    o.transform.rotation = rotation;
                    Vector3 offset = o.transform.position - o.transform.GetChild(0).position;
                    o.transform.position = model.primaryWeaponRoot.position + offset;

                    // Set Animations
                    Animator anim = characterObject.GetComponent<Animator>();
                    if ( e.id.Contains(".1h") ){
                        anim.SetInteger("Attack Position",1);
                    } else if ( e.id.Contains(".2h") ){
                        anim.SetInteger("Attack Position",2);
                    } else if ( e.id.Contains(".b-") ){
                        anim.SetInteger("Attack Position",3);
                    }
                }

                // Remove from inventory
                inventory.RemoveItem(index,1);

                // Update inventory display
                MenusWindow.instance.inventoryWindow.UpdateDisplay();

                Game.Notification("Equipped " + e.name,true);
            } else {
                Console.Log("Cannot equip this item");
            }
        }
    }
    public void Unequip(int index){
        if ( index >= 0 && index < equipment.Length ){

            // Destroy Model
            Model model = characterObject.GetComponent<Model>();
            if ( equipment[index].equipType == EquipType.primaryWeapon ){
                if ( model.primaryWeaponRoot.childCount > 0 ) 
                    GameObject.Destroy(model.primaryWeaponRoot.GetChild(0).gameObject);

                // Set Animations
                Animator anim = characterObject.GetComponent<Animator>();
                anim.SetInteger("Attack Position",0);
            }

            if ( equipment[index].id != "" ) inventory.AddItem(equipment[index]);

            equipment[index] = null;
            CalculateEquipmentStats();

            MenusWindow.instance.characterWindow.UpdateDisplay();
        }
    }
    public void AddSkill(Skill s){
        if ( !HasSkill(s.id) ) {
            if ( s.skillType == SkillType.singleTarget ) skills.Add(new SingleTargetSkill(s));
            Game.Notification("Learned " + s.name,true);
        }
    }
    #endregion
    #region Set Methods
    public void SetPlayerInfo(PlayerInfo pi){
        playerInfoDisplay = pi;
        playerInfoDisplay.SetPlayer(this);
    }
    #endregion
    #region Get Methods
    public PlayerInfo GetPlayerInfo(){
        return playerInfoDisplay;
    }
    #endregion
    #region Experience Methods
    public override void AddExp(float x){
        base.AddExp(x);
        classExp += x*0.125f;

        Console.Log("Gained " + (int)x + " exp.");
        Console.Log("Gained " + (int)x*0.125f + " class exp.");
        
        ((PlayerObject)characterObject).CreateText("+" + x + " exp \n+" + x*0.125f + " cxp",Color.yellow,1f);

        if ( exp >= maxExp ){
            LevelUp();
        }
        if ( classExp >= maxClassExp ){
            ClassLevelUp();
        }
    }
    public void LevelUp(){
        level++;
        CalculateStats();
        exp = exp - maxExp;
        currentStats = new Stats(stats);
        ((PlayerObject)characterObject).CreateText("Level Up!",Color.white,3f);
    }
    public void ClassLevelUp(){
        classLevel++;
        classExp = classExp - maxClassExp;
        maxClassExp = (10f + level*level)*0.25f;
    }
    #endregion
    #region Quests Methods
    public void AddQuest(Quest q){
        if ( !HasQuest(q.id) && !HasCompleteQuest(q.id) ){
            quests.Add(new Quest(q));
        }
    }
    public Quest GetQuest(string id){
        foreach (Quest q in quests){
            if ( q.id == id ){
                return q;
            }
        }
        return null;
    }
    public void AbandonQuest(Quest quest){
        foreach (Quest q in quests){
            if ( q.id == quest.id ){
                quests.Remove(q);
                break;
            }
        }
    }
    public void CompleteQuest(Quest quest){
        foreach (Quest q in quests){
            if ( q.id == quest.id ){
                completedQuests.Add(q.id);
                quests.Remove(q);
                break;
            }
        }
    }
    public bool HasQuest(string id){
        foreach (Quest q in quests){
            if ( q.id == id ){
                return true;
            }
        }
        return false;
    }
    public bool HasCompleteQuest(string id){
        foreach (string s in completedQuests){
            if ( s == id ){
                return true;
            }
        }
        return false;
    }
    #endregion
    #region Override Methods
    public override void Miss(Character atker){
        base.Miss(atker);

        PlayerObject po = characterObject as PlayerObject;
        if ( po.GetTarget() == null ){
            po.SetTarget(atker);
        }
    }
    public override void PhysicalHit(Character atker, float rawDmg){
        base.PhysicalHit(atker,rawDmg);

        PlayerObject po = characterObject as PlayerObject;
        if ( po.GetTarget() == null ){
            po.SetTarget(atker);
        }
    }
    public override void MagicalHit(Character atker, float percent){
        base.MagicalHit(atker, percent);

        PlayerObject po = characterObject as PlayerObject;
        if ( po.GetTarget() == null ){
            po.SetTarget(atker);
        }
    }
    #endregion
    #region Stats Methods
    public string GetStatsString(){
        string text = "";

        FieldInfo[] currentStatsFields = currentStats.GetType().GetFields();
        FieldInfo[] statsField = stats.GetType().GetFields();

        for (int i = 0; i < currentStatsFields.Length; i++){
            switch(currentStatsFields[i].Name){
            case "health":
            text += "HP: " + currentStatsFields[i].GetValue(currentStats) + " / " + statsField[i].GetValue(stats);
            break;
            case "mana":
            text += "\nMP: " + currentStatsFields[i].GetValue(currentStats) + " / " + statsField[i].GetValue(stats);
            break;
            case "hpRecov":
            text += "\nHP Recovery: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "mpRecov":
            text += "\nMP Recovery: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "meleeMinDmg":
            text += "\nMelee Min.DMG: " + currentStatsFields[i].GetValue(currentStats) + " - " + currentStatsFields[i+1].GetValue(currentStats);
            break;
            case "rangeMinDmg":
            text += "\nRange Min.DMG: " + currentStatsFields[i].GetValue(currentStats) + " - " + currentStatsFields[i+1].GetValue(currentStats);
            break;
            case "magicMinDmg":
            text += "\nMagic Min.DMG: " + currentStatsFields[i].GetValue(currentStats) + " - " + currentStatsFields[i+1].GetValue(currentStats);
            break;
            case "physDef":
            text += "\nPhysical DEF: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "magDef":
            text += "\nMagical DEF: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "critChance":
            text += "\nCritical: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "critDmg":
            text += "\nCritical DMG %: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "evasion":
            text += "\nEvasion %: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "accuracy":
            text += "\nAccuracy %: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "atkSpd":
            text += "\nAtk SPD: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "castSpd":
            text += "\nCast SPD: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "movSpd":
            text += "\nMovt SPD: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "enchanceRate":
            text += "\nEnchance Rate %: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "enchantRate":
            text += "\nEnchant Rate %: " + currentStatsFields[i].GetValue(currentStats);
            break;
            case "dropRate":
            text += "\nDrop Rate %: " + currentStatsFields[i].GetValue(currentStats);
            break;
            }
        }

        return text;
    }
    public void CalculateBaseStats(){
        maxExp = 10f + level*level;

        baseStats = new Stats();
        baseStats.health = 10f + (attributes.strength*0.165f)*level + (attributes.vitality*0.33f)*level;
        baseStats.hpRecov = baseStats.health*0.01f + (attributes.vitality*0.165f)*level + (attributes.psyche*0.165f)*level;
        baseStats.mana = 5f + (attributes.intelligence*0.165f)*level + (attributes.psyche*0.33f)*level;
        baseStats.mpRecov = baseStats.mana*0.01f + (attributes.psyche*0.165f)*level;
        baseStats.meleeMinDmg = (attributes.strength*0.33f)*level;
        baseStats.meleeMaxDmg = 1f + (attributes.strength*0.33f)*level;
        baseStats.rangeMinDmg = (attributes.dexterity*0.33f)*level;
        baseStats.rangeMaxDmg = 1f + (attributes.dexterity*0.33f)*level;
        baseStats.magicMinDmg = (attributes.intelligence*0.33f)*level;
        baseStats.magicMaxDmg = 2f + (attributes.intelligence*0.33f)*level;
        baseStats.physDef = (attributes.strength*0.165f)*level + (attributes.vitality*0.33f)*level;
        baseStats.magDef = (attributes.intelligence*0.165f)*level + (attributes.psyche*0.33f)*level;
        baseStats.critChance = ((attributes.dexterity*0.165f)*level + (attributes.agility*0.33f)*level)/100f;
        baseStats.critDmg = 1f + ((attributes.intelligence*0.165f)*level + (attributes.agility*0.165f)*level)/100.0f;
        baseStats.accuracy = ((attributes.dexterity*0.165f)*level)/100.0f;
        baseStats.evasion = 1f - (attributes.agility*0.165f)*level;

        baseStats.atkSpd = 3f;
        baseStats.castSpd = 3f;
        baseStats.movSpd = 1f;

        baseStats.dropRate = ((attributes.luck*0.165f)*level)/100f;
        baseStats.enhanceRate = ((attributes.luck*0.165f)*level)/100f;
        baseStats.enchantRate = ((attributes.luck*0.165f)*level)/100f;

        stats = new Stats(1f);
        currentStats = new Stats(stats);
    }
    public void CalculateSkillStats(){

    }
    public void CalculateEquipmentStats(){
        stats = new Stats(baseStats);

        for (int i = 0; i < equipment.Length; i++){
            if ( equipment[i] != null && equipment[i].id != "" ){
                if ( i == (int)EquipType.secondaryWeapon && !equipment[i].IsShield )
                    stats += equipment[i].stats*0.1f;
                else
                    stats += equipment[i].stats;

                if ( i == (int)EquipType.primaryWeapon )
                    stats.atkSpd = equipment[i].stats.atkSpd;
            }
        }

        FieldInfo[] f1 = currentStats.GetType().GetFields();
        FieldInfo[] f2 = stats.GetType().GetFields();
        for (int i = 0; i < f1.Length; i++){
            if ( f1[i].Name != "health" && f1[i].Name != "mana" ){
                f1[i].SetValue(currentStats,(float)f2[i].GetValue(stats));
            }
        }
    }
    public void CalculateBonusStats(){
        bonusStats = new Stats();

        foreach (Equip e in equipment){
            if ( e != null )
                bonusStats += e.bonusStats;
        }

        FieldInfo[] f = stats.GetType().GetFields();
        FieldInfo[] f1 = bonusStats.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            float x = (float)f1[i].GetValue(bonusStats) + 1.00f;
            f[i].SetValue(stats,(float)f[i].GetValue(stats)*x);
        }
    }
    public void CalculateStats(){
        CalculateBaseStats();
        CalculateEquipmentStats();
        CalculateBonusStats();
    }
    public void CalculateLuckStats(){
        
    }
    #endregion
    #region Data Methods
    public static void LoadData(byte[] data){
        DataSaver ds = Game.GetDataSaver();
        ds.LoadData(data);
        XmlStringToPlayer(ds.GetDataAsString("avatar"));
    }
    public static void SaveData(){
        DataSaver ds = Game.GetDataSaver();
        if ( ds.CheckLabel("avatar") ){
            ds.SetData("avatar",PlayerToXmlString());
        } else {
            ds.RegisterLabel("avatar");
            ds.SetData("avatar",PlayerToXmlString());
        }

        Game.GetNetworkManager().SaveToDatabase(ds.ConvertToBytes());
    }
    // Converts current player data to xml as a string
    private static string PlayerToXmlString(){
        string s = "";
        
        if ( Game.GetPlayer() != null ){
            var serializer = new XmlSerializer(typeof(Player));

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UnicodeEncoding(false, false);
            settings.Indent = false;
            settings.OmitXmlDeclaration = false;

            using(StringWriter textWriter = new StringWriter()) {
                using(XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings)) {
                    serializer.Serialize(xmlWriter, Game.GetPlayer());
                }
                s = textWriter.ToString();
            }
        }

        return s;
    }
    // Converts xml as a string to player data
    private static void XmlStringToPlayer(string s){
        XmlSerializer serializer = new XmlSerializer(typeof(Player));

        XmlReaderSettings settings = new XmlReaderSettings();

        using(StringReader textReader = new StringReader(s)) {
            using(XmlReader xmlReader = XmlReader.Create(textReader, settings)) {
                Game.SetPlayer(serializer.Deserialize(xmlReader) as Player);
            }
        }
    }
    #endregion
}
