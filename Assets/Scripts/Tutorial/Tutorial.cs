using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
    
    private Player p;

    public static Tutorial GetInstance(){
        return GameObject.FindObjectOfType<Tutorial>();
    }

    void Start(){
        p = Game.GetPlayer();
        StartCoroutine("Progress");
    }

    IEnumerator Progress(){
        while(p.tutorialState != TutorialState.complete){
            yield return new WaitForEndOfFrame();
            switch(p.tutorialState){
            default:
            Console.Log("Invalid tutorial state");
            break;
            #region Start
            case TutorialState.start:
            break;
            #endregion
            }
        }
    }

    public void SetState(TutorialState ts){
        p.tutorialState = ts;
    }
}

public enum TutorialState {
    start,

    cameraControl,
    movement,
    npcInteraction,
    quest1,
    tutorialCity,
    blacksmith,
    tailor,
    alchemist,
    quest2,
    quest3,
    classes,
    quest4,
    simulationComplete,
    spaceship,

    complete
}
