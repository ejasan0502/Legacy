using UnityEngine;
using System.Xml;
using System.Collections;

public class GameManager : MonoBehaviour {
    
    private DataSaver dataSaver;
    public GameData gameData;

    void Start(){
        dataSaver = DataSaver.GetInstance();
        gameData = GameData.instance;

        LoadGameData();
    }

    private void LoadGameData(){
        DebugWindow.Log("Loading game data...");

        TextAsset textAsset = (TextAsset) Resources.Load(GlobalVariables.PATH_XMLDATA_WEAPONS);
        gameData.LoadFromXml( textAsset.text , typeof(Equip) );

        DebugWindow.Log("Game data loaded!");
    }

}
