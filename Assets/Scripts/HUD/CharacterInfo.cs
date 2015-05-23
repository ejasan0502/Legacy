using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CharacterInfo : MonoBehaviour {

    private Text textName;
    private HealthBar health;

    void Awake(){
        textName = transform.GetChild(0).GetComponent<Text>();
        health = transform.GetChild(2).GetComponent<HealthBar>();
    }

    public void SetVariables(Character c){
        textName.text = c.name;
        health.SetCharacter(c);
    }

}
