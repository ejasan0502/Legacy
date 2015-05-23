using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HealthBar : MonoBehaviour {
    
    private Character c;
    private Image img;

    public void SetCharacter(Character ch){
        c = ch;
    }

    void Start(){
        img = GetComponent<Image>();
    }

    void FixedUpdate(){
        if ( c != null && img != null ){
            img.fillAmount = c.currentStats.health/c.stats.health;
        }
    }

}
