using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CharacterCreation : MonoBehaviour {
    
    private Object[] characterModels;
    private Vector2 characterListScrollPos = Vector2.zero;
    private GameObject characterModel = null;

    void Start(){
        characterModels = Resources.LoadAll(GlobalVariables.PATH_CONTENTDATA_CHARACTERS,typeof(GameObject));
    }

    void OnGUI(){
        characterListScrollPos = GUI.BeginScrollView(new Rect(0f,0f,Screen.width*0.25f,Screen.height),characterListScrollPos,new Rect(0f,0f,Screen.width*0.25f,Screen.height));

        for (int i = 0; i < characterModels.Length; i++){
            if ( GUI.Button(new Rect(0f,i*Screen.height*0.1f,Screen.width*0.25f,Screen.height*0.1f),characterModels[i].name) ){
                if ( characterModel != null )
                    DestroyImmediate(characterModel);
                characterModel = (GameObject) Instantiate(characterModels[i]);
                characterModel.transform.position = Vector3.zero;
            }
        }

        GUI.EndScrollView();
    }

    public void Create(){
        
    }

}
