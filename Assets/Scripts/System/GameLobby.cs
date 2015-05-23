using UnityEngine;
using System.Collections;
using Sfs2X;
using Sfs2X.Entities.Data;
using Sfs2X.Core;
using Sfs2X.Logging;
using Sfs2X.Requests;

// Moves the player to the correct scene
public class GameLobby : MonoBehaviour {
    
    private SmartFox smartfox;
    private byte[] playerData = null;

    void Awake(){
        smartfox = Game.GetConnection();
        smartfox.AddEventListener(SFSEvent.EXTENSION_RESPONSE,OnCheckAvatar);

        ISFSObject param = new SFSObject();
        param.PutUtfString("email",Game.GetEmail());
        param.PutUtfString("command","check avatar");

        smartfox.Send(new ExtensionRequest("database",param));
    }

    void FixedUpdate(){
        smartfox.ProcessEvents();
    }

    public void OnCheckAvatar(BaseEvent evt){
        Console.Log("Event received: " + evt.Params["cmd"]);
        ISFSObject data = (SFSObject) evt.Params["params"];

        smartfox.RemoveEventListener(SFSEvent.EXTENSION_RESPONSE,OnCheckAvatar);

        if ( (string)evt.Params["cmd"] == "database" ){
            if ( data.GetBool("success") ){
                // Avatar exists
                playerData = data.GetByteArray("avatar").Bytes;

                LoadingScreen.Load(3f,LoadPlayerData,LoadLastSavedScene);
            } else {
                // Avatar does not exist
                Application.LoadLevel("Character Creation");
            }
        }
    }

    public void LoadPlayerData(){
        Player.LoadData(playerData);
    }

    public void LoadLastSavedScene(){
        Player p = Game.GetPlayer();
        if ( p == null ){
            Console.Log("Player is null!");
            return;
        }

        Game.LoadScene(p.lastSavedScene,false);
    }
}
