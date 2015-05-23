using UnityEngine;
using System.Reflection;
using System.Collections;

public class Test : MonoBehaviour {

    public Player player;

    void Start(){
        Game.SetPlayer(player);
    }
}
