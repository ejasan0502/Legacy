using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerInfo : MonoBehaviour {

    public Text nameText;
    public Text levelText;
    public Image healthBar;
    public Image manaBar;
    public Image expBar;

    private Player p = null;

    public void SetPlayer(Player player){
        p = player;
        nameText.text = p.name;
        levelText.text = p.level + "";
    }

    void FixedUpdate(){
        if ( p != null ){
            levelText.text = p.level + "";

            healthBar.fillAmount = p.currentStats.health / p.stats.health;
            manaBar.fillAmount = p.currentStats.mana / p.stats.mana;
            expBar.fillAmount = p.exp / p.maxExp;
        }
    }
}
