using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// [Start cast animation] -> [Cast loop animation] -> [Cast end animation] (OnCastEnd) -> [Idle Animation]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(TargetManager))]
public class CharacterObject : MonoBehaviour {

    public Character character;
    [HideInInspector] public Animator anim;
    private CharacterState state;
    private TargetManager targetManager;
    private List<Character> targets;

    protected virtual void Start(){
        targetManager = GetComponent<TargetManager>();
        anim = GetComponent<Animator>();
        targets = new List<Character>();

        SetState(CharacterState.idle);
        character.SetCharacterObject(this);
    }
    protected virtual void Update(){
        if ( character != null ){
            character.OnUpdate();
            StateMachine();
        }
    }

    public Character GetCharacter(){
        return character;
    }
    public TargetManager GetTargetManager(){
        return targetManager;
    }
    public Character GetTarget(){
        return targetManager.GetClosestFrontTarget();
    }
    public List<Character> GetTargets(){
        return targets;
    }

    public void SetCharacter(Character c){
        character = c;
    }
    public void SetState(CharacterState s){
        state = s;
    }
    public void UpdateTargets(Skill s){
        if ( s.isAoeFront ){
            targets = targetManager.GetFrontTargetsWithin(s.castDistance);
        } else if ( s.isAoeAround ){
            targets = targetManager.GetTargetsAround(transform.forward*s.castDistance,s.CastRadius);
        } else {
            targets = targetManager.GetTargetsWithin(s.castDistance);
        }
    }

    public virtual void Idle(){
        
    }
    public virtual void Cast(){
        if ( !anim.GetCurrentAnimatorStateInfo(0).IsName(character.GetCastingSkill().animation) ){
            anim.SetBool(GlobalVariables.ANIM_CAST, true);
            character.GetCastingSkill().SetStartCastTime(Time.time);
        }
    }

    public void OnCastEnd(){
        Skill s = character.GetCastingSkill();
        if ( targets.Count > 0 ){
            foreach (Character c in targets){
                s.Apply(character,c);
            }
        } else {
            s.Apply(character,GetTarget());
        }
        
        character.SetCastingSkill(null);
        SetState(CharacterState.battle);
    }

    private void StateMachine(){
        switch(state){
            case CharacterState.idle:
                Idle();
            break;
            case CharacterState.cast:
                Cast();
            break;
        }
    }
    private void CheckTarget(){
        
    }
}

public enum CharacterState {
    idle,
    walk,
    run,
    battle,
    attack,
    cast,
    death
}
