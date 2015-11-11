using UnityEngine;
using System.Collections;

public class OnCastLoop : StateMachineBehaviour {

    private CharacterObject characterObject;
    private Skill skill;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        characterObject = animator.GetComponent<CharacterObject>();
        skill = characterObject.character.GetCastingSkill();
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex){
        if ( skill != null ){
            if ( Time.time - skill.StartCastTime >= skill.castTime ){
                animator.SetBool(GlobalVariables.ANIM_CAST, false);
            }
        }
    }
}
