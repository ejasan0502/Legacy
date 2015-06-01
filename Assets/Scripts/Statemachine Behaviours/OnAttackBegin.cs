using UnityEngine;
using System.Collections;

public class OnAttackBegin : StateMachineBehaviour {

    private float startTime = 0f;

	 // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetBool("Attack",false);
        startTime = Time.time;
	}

	// OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    CharacterObject co = animator.GetComponent<CharacterObject>();
        if ( Time.time - startTime >= co.c.currentStats.atkSpd ){
            if ( co.GetTarget() != null && co.GetTarget().IsAlive )
                animator.SetBool("Attack",true);
        }
	}

	// OnStateExit is called when a transition ends and the state machine finishes evaluating this state
	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	    CharacterObject co = animator.GetComponent<CharacterObject>();
	    if ( co.c.IsPlayer )
            animator.SetInteger("Attack Animation",Random.Range(0,2));

        if ( co.GetTarget() != null )
            animator.transform.LookAt(co.GetTarget().characterObject.transform);
	}

	// OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
	//override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}

	// OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
	//override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
	//
	//}
}
