using UnityEngine;
using System.Collections;

public class NPCObject : MonoBehaviour {
    
    public NPC npc;

    private PlayerObject player = null;

    void Awake(){
        StartCoroutine("WaitForLoad");
    }

    private IEnumerator WaitForLoad(){
        while (!Game.GameDataLoaded){
            yield return new WaitForSeconds(1f);
        }

        if ( npc.id != "" ){
            npc = Game.GetNPCData().GetNPC(npc.id);
            npc.npcObject = this;
            name = npc.name;
        } else {
            npc.npcObject = this;
            name = npc.name;
        }
    }

    public void Interact(PlayerObject p){
        Console.instance.SetDisplay(false);
        HotkeyManager.instance.SetDisplay(false);
        Game.GetPlayer().GetPlayerInfo().gameObject.SetActive(false);

        GameObject o = Instantiate(Resources.Load("NPCCanvas")) as GameObject;
        NPCCanvas nc = o.GetComponent<NPCCanvas>();
        nc.SetNPC(npc);
        player = p;
    }

    public void StopInteract(){
        Game.GetPlayer().GetPlayerInfo().gameObject.SetActive(true);
        player.SetControls(true);
        Console.instance.SetDisplay(true);
        HotkeyManager.instance.SetDisplay(true);
    }
}
