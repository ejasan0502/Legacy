using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[System.Serializable]
public class CharacterObject : MonoBehaviour {
    
    public bool canMove = true;
    public bool canCast = true;

    public Character c;
    protected CharacterController cc;
    protected NavMeshAgent navAgent;
    protected Animator anim;
    protected CharacterState state = CharacterState.idle;
    protected Object floatingText;
    protected GameObject characterInfo;

    private Vector3 endPoint;
    private float recoveryTime = 0f;
    private float recoveryRate = 3f;

    #region Unity Methods
    protected virtual void Awake(){
        cc = GetComponent<CharacterController>();
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        floatingText = Resources.Load("FloatingText",typeof(GameObject));

        navAgent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    }
    #endregion
    #region Set Methods
    public void SetRecoveryRate(float x){
        recoveryRate = x;
    }
    public void SetState(CharacterState cs){
        state = cs;
    }
    public void SetCharacter(Character character){
        c = character;
    }
    public void SetEndPoint(Vector3 v){
        endPoint = v;
    }
    public void SetCanvas(GameObject o){
        characterInfo = o;
        characterInfo.SetActive(false);
    }

    public void SetMovement(bool b){
        canMove = b;
        if ( !canMove ) 
            endPoint = transform.position;
    }
    #endregion
    #region Get Methods
    public Character GetCharacter(){
        return c;
    }
    public virtual Character GetTarget(){
        return null;
    }
    #endregion
    #region State Machine Methods
    protected virtual IEnumerator StateMachine(){
        while ( c.IsAlive ){
            yield return new WaitForEndOfFrame();
            switch (state){
            case CharacterState.idle:
            Idle();
            Recovery();
            break;
            case CharacterState.battling:
            Battling();
            break;
            case CharacterState.attacking:
            Attacking();
            break;
            case CharacterState.casting:
            Casting();
            break;
            case CharacterState.dying:
            Dying();
            break;
            }

            if ( canMove ) {
                navAgent.SetDestination(endPoint);
                if ( Vector3.Distance(transform.position,endPoint) < 0.5f ){
                    if ( anim.GetBool("Move") ) anim.SetBool("Move",false);
                } else {
                   if ( !anim.GetBool("Move") )  anim.SetBool("Move",true);
                }
            }
        }
    }
    protected virtual void Idle(){}
    protected virtual void Battling(){}
    protected virtual void Casting(){}
    protected virtual void Attacking(){}
    protected virtual void Dying(){}
    public void Recovery(){
        if ( Time.time - recoveryTime >= recoveryRate ){
            c.currentStats.health += c.currentStats.hpRecov;
            c.currentStats.mana += c.currentStats.mpRecov;
            recoveryTime = Time.time;
        }
    }
    public void Cast(string id){
        if ( canCast ){
            if ( c.HasSkill(id) ){
                
            } else {
                Console.Log(c.name + " does not have the skill id, " + id);
            }
        } else {
            Console.Log(c.name + " cannot cast at this time");
        }
    }
    #endregion
    #region Public Methods
    public void StopMovement(){
        endPoint = transform.position;
    }
    public virtual void OnDeath(){
        StartCoroutine("WaitForDestroy");
    }
    public void CreateText(string s, Color c, float sc){
        GameObject o = GameObject.Instantiate(floatingText) as GameObject;
        o.transform.position = transform.position + transform.up;
        o.transform.localScale *= sc;

        Text text = o.transform.GetChild(0).GetComponent<Text>();
        text.text = s;
        text.color = c;
    }
    #endregion
    #region Private Methods
    private IEnumerator WaitForDestroy(){
        yield return new WaitForSeconds(5f);
        Destroy(gameObject);
    }
    #endregion
}

public enum CharacterState {
    idle,
    casting,
    attacking,
    dying,
    battling
}
