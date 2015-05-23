using UnityEngine;
using System.Collections;

public class NPCObject : MonoBehaviour {
    
    public NPC npc;

    private PlayerObject player = null;

    void Awake(){
        npc.npcObject = this;
        name = npc.name;
    }

    public void Interact(PlayerObject p){
        Console.instance.SetDisplay(false);

        GameObject o = Instantiate(Resources.Load("NPCCanvas")) as GameObject;
        NPCCanvas nc = o.GetComponent<NPCCanvas>();
        nc.SetNPC(npc);
        player = p;
    }

    public void StopInteract(){
        player.SetControls(true);
        Console.instance.SetDisplay(true);
    }
}
