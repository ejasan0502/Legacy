using UnityEngine;
using System.Collections;

public class OnCastEnd : StateMachineBehaviour {

    private CharacterObject characterObject;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        characterObject = animator.GetComponent<CharacterObject>();
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        characterObject.OnCastEnd();
    }
}
