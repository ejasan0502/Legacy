using UnityEngine;
using System.Collections;

public class OnLedgeEnd : StateMachineBehaviour {

    private PlayerControls pc;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        pc = animator.GetComponent<PlayerControls>();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        pc.transform.position = Vector3.Lerp(pc.transform.position,pc.ledgePoint,Time.deltaTime);
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        pc.onLedge = false;
    }
}
