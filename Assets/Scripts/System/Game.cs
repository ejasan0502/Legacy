using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;

// Note: GameData, NetworkManager, DataSaver should be on the first scene
public class Game : MonoBehaviour {

    private SmartFox connection;
    private GameData gameData;
    private MonsterData monsterData;
    private SkillsData skillsData;
    private QuestData questData;
    private NPCData npcData;
    private NetworkManager networkManager;
    private DataSaver dataSaver;
    private EventManager eventManager;
    private string email;
    private Player player;
    private PlayerObject playerObject;
    private static GameObject notificationPref;

    #region Unity Methods
    void Awake(){
        // Destroy duplicates
        Game[] list = GameObject.FindObjectsOfType<Game>();
        if ( list.Length > 1 ){
            // Found more than 1 object, meaning this instance is a duplicate
            Destroy(gameObject);
        }

        DontDestroyOnLoad(this);

        connection = new SmartFox(true);
        gameData = GameObject.FindObjectOfType<GameData>();
        monsterData = GameObject.FindObjectOfType<MonsterData>();
        skillsData = GameObject.FindObjectOfType<SkillsData>();
        npcData = GameObject.FindObjectOfType<NPCData>();
        questData = GameObject.FindObjectOfType<QuestData>();
        networkManager = GameObject.FindObjectOfType<NetworkManager>();
        dataSaver = GameObject.FindObjectOfType<DataSaver>();
        eventManager = new EventManager();

        notificationPref = Resources.Load("Notification", typeof(GameObject)) as GameObject;
    }
    void Start(){
        gameData.ExtractItemsXmlData();
        gameData.ExtractUsablesXmlData();
        gameData.ExtractEquipsXmlData();
        skillsData.ExtractXmlData();
    }
    #endregion
    #region Public Static Methods
    public static void Notification(Canvas c, string s, bool b){
        GameObject o = Instantiate(notificationPref) as GameObject;
        o.transform.GetComponent<DestroyWithDelay>().canTapDestroy = b;
        o.transform.GetChild(0).GetComponent<Text>().text = s;
        o.transform.SetParent(c.transform);
        o.transform.localPosition = Vector3.zero;
    }
    public static void LoadScene(string s, bool b){
        if ( b ) LoadingScreen.Load(3f);

        Game.GetInstance().networkManager.JoinRoom(s);
        Application.LoadLevel(s);
    }
    public static CharacterObject CreateCharacter(Character p, string t, Vector3 pos){
        if ( p.model == null ) return null;

        GameObject o = Instantiate(p.model) as GameObject;
        o.name = p.name;
        o.tag = t;
        o.transform.position = pos;
        o.layer = LayerMask.NameToLayer("Targetable");

        GameObject canvas = Instantiate(Resources.Load("Character Info")) as GameObject;
        canvas.transform.SetParent(o.transform);
        canvas.GetComponent<CharacterInfo>().SetVariables(p);
        
        if ( p.IsPlayer ){
            PlayerObject po = o.AddComponent<PlayerObject>();
            po.GetComponent<Animator>().runtimeAnimatorController = p.animator;
            po.GetComponent<Animator>().updateMode = AnimatorUpdateMode.AnimatePhysics;
            po.SetCharacter(p);
            po.SetCanvas(canvas);
            p.SetCharacterObject(po);

            Vector3 point = new Vector3(0f,o.GetComponent<NavMeshAgent>().height,0f);
            canvas.transform.position = o.transform.position + point;

            return po;
        } else {
            MonsterObject mo = o.AddComponent<MonsterObject>();
            mo.GetComponent<Animator>().runtimeAnimatorController = p.animator;
            mo.GetComponent<Animator>().updateMode = AnimatorUpdateMode.AnimatePhysics;
            mo.SetCharacter(p);
            mo.SetCanvas(canvas);
            p.SetCharacterObject(mo);

            Vector3 point = new Vector3(0f,o.GetComponent<NavMeshAgent>().height,0f);
            canvas.transform.position = o.transform.position + point;

            return mo;
        }
    }
    #endregion
    #region Get Static Methods
    public static Game GetInstance(){
        return GameObject.FindObjectOfType<Game>();
    }
    public static SmartFox GetConnection(){
        return GetInstance().connection;
    }
    public static GameData GetGameData(){
        return GetInstance().gameData;
    }
    public static MonsterData GetMonsterData(){
        return GetInstance().monsterData;
    }
    public static SkillsData GetSkillData(){
        return GetInstance().skillsData;
    }
    public static QuestData GetQuestData(){
        return GetInstance().questData;
    }
    public static NPCData GetNPCData(){
        return GetInstance().npcData;
    }
    public static NetworkManager GetNetworkManager(){
        return GetInstance().networkManager;
    }
    public static DataSaver GetDataSaver(){
        return GetInstance().dataSaver;
    }
    public static string GetEmail(){
        return GetInstance().email;
    }
    public static Player GetPlayer(){
        return GetInstance().player;
    }
    public static PlayerObject GetPlayerObject(){
        return GetInstance().playerObject;
    }
    public static EventManager GetEventManager(){
        return GetInstance().eventManager;
    }
    #endregion
    #region Set Methods
    public static void SetEmail(string s){
        GetInstance().email = s;
        Console.Log("Email set to " + s);
    }
    public static void SetPlayer(Player p){
        Vector3 v = Vector3.zero;
        GameObject o = GameObject.Find("Default");
        if ( o != null ) v = o.transform.position;

        GetInstance().player = p;
        CharacterObject co = CreateCharacter(GetInstance().player,"Player",v);
        co.gameObject.layer = LayerMask.NameToLayer("Self");
        GetInstance().playerObject = GetInstance().player.characterObject as PlayerObject;

        GetInstance().player.SetPlayerInfo(GameObject.FindObjectOfType<PlayerInfo>());

        Console.Log("Player has been set");
    }
    #endregion
}
