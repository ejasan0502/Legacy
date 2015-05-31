using UnityEngine;
using System.Collections;

[RequireComponent(typeof(InputControls))]
public class PlayerObject : CharacterObject {

    private InputControls controls;
    private Character target = null;
    private float atkTime;
    private float battleEndTime;

    protected override void Awake(){
        base.Awake();
        DontDestroyOnLoad(this);

        controls = GetComponent<InputControls>();

        atkTime = Time.time;
    }
    
    void Start(){
        StartCoroutine("StateMachine");
    }

    #region Controls
    public void SetControls(bool b){
        controls.enabled = b;
    }
    #endregion
    #region Override Methods
    public override void OnDeath(){
        
    }
    #endregion
    #region Set Methods
    public void SetTarget(Character c){
        target = c;
    }
    #endregion
    #region Get Methods
    public override bool HasTarget(){
        return target != null;
    }
    public override bool TargetIsAlive(){
        return target != null && target.currentStats.health > 0;
    }
    public Character GetTarget(){
        return target;
    }
    public CharacterObject GetTargetObject(){
        return target.characterObject;
    }
    #endregion
    #region State Machine Methods
    protected override void Idle(){
        if ( target != null ){
            anim.SetBool("Battle",true);
            SetState(CharacterState.battling);
            characterInfo.SetActive(true);
        }
    }
    protected override void Battling(){
        if ( target != null && target.IsAlive && target.characterObject != null ){
            CharacterObject co = target.characterObject;
            Vector3 v = co.transform.position;
            SetEndPoint(v);
            if ( Vector3.Distance(transform.position,co.transform.position) < c.atkDistance ){
                anim.SetBool("Attack",true);
                SetEndPoint(transform.position);
                SetState(CharacterState.attacking);
            } else {
                //anim.SetBool("Attack",false);
            }
        } else if ( Time.time - battleEndTime >= 3f ){
            anim.SetBool("Battle",false);
            characterInfo.SetActive(false);
            SetState(CharacterState.idle);
        }

        if ( target != null && !target.IsAlive ){
            battleEndTime = Time.time;
            target = null;
        }
    }
    protected override void Attacking(){
        if ( target != null && target.IsAlive && c.IsAlive ){
            if ( Vector3.Distance(transform.position,target.characterObject.transform.position) > c.atkDistance ){
                anim.SetBool("Attack",false);
                SetState(CharacterState.battling);
            }
        } else {
            anim.SetBool("Attack",false);
            SetState(CharacterState.battling);
        }
        if ( target != null && !target.IsAlive ){
            battleEndTime = Time.time;
            target = null;
        }
    }

    protected override void Dying(){
        if ( !anim.GetBool("Death") ) {
            anim.SetBool("Death",true);
            controls.enabled = false;
        }
    }

    public override void ApplyDamage(){
        if ( target != null ) {
            target.PhysicalHit(c);
            transform.LookAt(target.characterObject.transform.position);
        }
    }
    #endregion
}
