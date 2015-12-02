using UnityEngine;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;

public class GameManager : MonoBehaviour {
    
    public PlayerCharacter pc;
    private NetworkManager networkManager;
    public DataSaver dataSaver;
    private SmartFox smartfox;
    public ContentData contentData;

    private static Object lockObj = new Object();
    private static GameManager _instance;
    public static GameManager instance {
        get {
            lock (lockObj){
                if ( _instance == null ){
                    GameObject o = new GameObject("GameManager");
                    _instance = o.AddComponent<GameManager>();
                }
            }
            return _instance;
        }
    }

    void Awake(){
        if ( GameObject.FindObjectsOfType<GameManager>().Length > 1 )
            DestroyImmediate(gameObject);
    }
    void Start(){
        DontDestroyOnLoad(this);

        networkManager = NetworkManager.instance;
        dataSaver = new DataSaver();
        smartfox = NetworkManager.GetConnection();
        contentData = new ContentData();

        LoadContentData();
    }

    #region Private Methods
    private void LoadContentData(){
        DebugWindow.Log("Loading content data...");

        TextAsset textAsset = (TextAsset) Resources.Load(GlobalVariables.PATH_XMLDATA_WEAPONS);
        contentData.LoadFromXml( textAsset.text , typeof(Equip) );
        //textAsset = (TextAsset) Resources.Load(GlobalVariables.PATH_XMLDATA_ARMORS);
        //contentData.LoadFromXml( textAsset.text , typeof(Equip) );
        //textAsset = (TextAsset) Resources.Load(GlobalVariables.PATH_XMLDATA_USABLES);
        //contentData.LoadFromXml( textAsset.text , typeof(Usable) );
        //textAsset = (TextAsset) Resources.Load(GlobalVariables.PATH_XMLDATA_MATERIALS);
        //contentData.LoadFromXml( textAsset.text , typeof(Item) );

        DebugWindow.Log("Content data loaded!");
    }
    #endregion
    #region Public Methods
    // Save player data to Server
    public void SavePlayerData(){
        DebugWindow.Log("Saving player data...");
        dataSaver.SaveData(pc);
    }
    #endregion
    #region Event Listeners
    #endregion
}
