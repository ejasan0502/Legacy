using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterCreation : MonoBehaviour {
    public List<string> startItems;     // string list of item ids

    public GameObject window1;
    public GameObject window2;

    public InputField nameInputField;
    public Transform weapons;
    public Transform races;

    public Animator maleBtn;
    public Animator femaleBtn;

    public GameObject malePref;
    public GameObject femalePref;
    
    private bool male = true;
    private string race = "";

    public void GenderButton(bool b){
        male = b;
        malePref.SetActive(b);
        femalePref.SetActive(!b);
        maleBtn.SetBool("Selected",b);
        femaleBtn.SetBool("Selected",!b);
    }

    public void RaceButton(Animator a){
        foreach (Transform t in races){
            t.GetComponent<Animator>().SetBool("Selected",false);
        }

        race = a.name;
        a.SetBool("Selected",true);
    }

    public void Next(){
        window1.SetActive(false);
        window2.SetActive(true);
    }

    public void Back(){
        window1.SetActive(true);
        window2.SetActive(false);
    }

    public void Done(){
        LoadingScreen.Load(3f,SavePlayerData,LoadTutorial);
    }

    public void SavePlayerData(){
        Player p = new Player();
        p.male = male;
        p.race = race;
        Game.SetPlayer(p);
        Player.SaveData();
    }

    public void LoadTutorial(){
        Game.LoadScene("Tutorial",false);
    }
}
