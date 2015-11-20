using UnityEngine;
using System.Xml;
using System.Collections;

public class GameManager : MonoBehaviour {
    
    private DataSaver dataSaver;
    public ContentData gameData;

    void Start(){
        dataSaver = DataSaver.GetInstance();
        gameData = ContentData.instance;

        LoadGameData();
        LoadPlayerData();
    }

    private void LoadGameData(){
        DebugWindow.Log("Loading game data...");

        TextAsset textAsset = (TextAsset) Resources.Load(GlobalVariables.PATH_XMLDATA_WEAPONS);
        gameData.LoadFromXml( textAsset.text , typeof(Equip) );
        //textAsset = (TextAsset) Resources.Load(GlobalVariables.PATH_XMLDATA_ARMORS);
        //gameData.LoadFromXml( textAsset.text , typeof(Equip) );
        //textAsset = (TextAsset) Resources.Load(GlobalVariables.PATH_XMLDATA_USABLES);
        //gameData.LoadFromXml( textAsset.text , typeof(Usable) );
        //textAsset = (TextAsset) Resources.Load(GlobalVariables.PATH_XMLDATA_MATERIALS);
        //gameData.LoadFromXml( textAsset.text , typeof(Item) );

        DebugWindow.Log("Game data loaded!");
    }
    private void LoadPlayerData(){

    }

}
