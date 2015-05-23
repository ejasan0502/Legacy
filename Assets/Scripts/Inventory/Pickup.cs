using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Pickup : MonoBehaviour {
    public string itemId;
    public int amt;
    public GameObject canvas;

    public void SetItem(Item item, int a){
        itemId = item.id;
        amt = a;

        name = item.name;
    }

    public void PickupItem(){
        Player p = Game.GetPlayer();
        if ( p != null )
            p.inventory.AddItem(Game.GetGameData().GetItem(itemId),amt);
        else
            Console.Log("Player is null!");
    }
}
