using UnityEngine;
using System.Reflection;
using System.Collections;

public class Test : MonoBehaviour {

    public Player player;

    void Start(){
        player.CalculateStats();
        player.currentStats = new Stats(player.stats);
        Game.SetPlayer(player);
    }
}
