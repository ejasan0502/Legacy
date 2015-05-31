using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterObject : CharacterObject {

    public float moveDelay = 5f;
    public float wanderDistance = 10f;
    public List<Transform> nodes = new List<Transform>();

    [HideInInspector] public EnemySight enemySight;

    private Vector3 startPos;
    private int nodeIndex = 0;
    private float waitTime;
    private float atkTime;

    void Start(){
        GameObject o = new GameObject("Enemy Sight");
        o.layer = LayerMask.NameToLayer("Ignore Raycast");
        o.transform.SetParent(transform);
        o.transform.position = transform.position;

        SphereCollider sc = o.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = ((Monster)c).sightRange;    
        wanderDistance = sc.radius;
        enemySight = o.AddComponent<EnemySight>();

        navAgent.speed = c.currentStats.movSpd;
        startPos = transform.position;
        atkTime = Time.time;

        SetEndPoint(startPos);

        StartCoroutine("StateMachine");
    }

    protected override void Idle(){
        if ( !enemySight.targetInSight ){
            #region Move to Nodes
            if ( nodes.Count > 0 ){
                if ( Time.time - waitTime > moveDelay ){
                    if ( Vector3.Distance(transform.position,nodes[nodeIndex].position) < 0.1f ){
                        waitTime = Time.time;
                        SetEndPoint(nodes[nodeIndex].position);
                        nodeIndex++;
                    }
                }
            } 
            #endregion
            #region Move to Random Point within Distance
            else {
                if ( navAgent.velocity.magnitude <= 0f ){
                    if ( Time.time - waitTime > moveDelay ){
                        float x = Random.Range(startPos.x-wanderDistance,startPos.x+wanderDistance);
                        float y = transform.position.y;
                        float z = Random.Range(startPos.z-wanderDistance,startPos.z+wanderDistance);

                        SetEndPoint(new Vector3(x,y,z));
                    }
                } else {
                    waitTime = Time.time;
                }
            }
            #endregion
        } else {
            anim.SetBool("Battle",true);
            characterInfo.SetActive(true);
            SetState(CharacterState.battling);
        }
    }

    protected override void Battling(){
        #region Move to target
        if ( enemySight.target != null && enemySight.target.characterObject != null && enemySight.target.IsAlive ){
            Vector3 v = enemySight.target.characterObject.transform.position;
            SetEndPoint(v);
            if ( Vector3.Distance(transform.position,v) < c.atkDistance ){
                anim.SetBool("Attack",true);
                transform.LookAt(v);
                SetEndPoint(transform.position);
                SetState(CharacterState.attacking);
            }
        } else {
            anim.SetBool("Battle",false);
            characterInfo.SetActive(false);
            SetState(CharacterState.idle);
        }
        #endregion
    }

    protected override void Attacking(){
        if ( enemySight.target != null && enemySight.target.IsAlive && c.IsAlive ){
            if ( Vector3.Distance(transform.position,enemySight.target.characterObject.transform.position) > c.atkDistance ){
                anim.SetBool("Attack",false);
                SetState(CharacterState.battling);
            }
        } else {
            anim.SetBool("Attack",false);
            SetState(CharacterState.battling);
        }
    }

    protected override void Dying(){
        if ( !anim.GetBool("Death") ) anim.SetBool("Death",true);
    }

    public override void ApplyDamage(){
        if ( enemySight.target != null ) {
            enemySight.target.PhysicalHit(c);
            transform.LookAt(enemySight.target.characterObject.transform.position);
        }
    }

    public override bool HasTarget(){
        return enemySight.target != null;
    }

    public override bool TargetIsAlive(){
        return enemySight.target != null && enemySight.target.currentStats.health > 0;
    }
}
