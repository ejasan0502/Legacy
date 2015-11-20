using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerControls : MonoBehaviour {
    
    public GameObject head;
    public float jumpForce = 10f;
    public float freeFallDuration = 3f;

    [HideInInspector] public CharacterObject characterObject;
    private Character character;
    private CharacterController characterController;

    public Vector3 velocity = Vector3.zero;
    public Vector3 ledgePoint = Vector3.zero;
    public bool isGrounded = false;
    public bool isClimbing = false;
    public bool onLedge = false;
    public bool freeFalling = false;
    private Collider climbingWall = null;
    private float freeFallTime;

    void Start(){
        characterObject = GetComponent<CharacterObject>();
        character = characterObject.character;
        characterController = GetComponent<CharacterController>();
    }

    void Update(){
        if ( !onLedge ){
            if ( !isClimbing && isGrounded ) MoveLeftRight();
            if ( !isClimbing && !onLedge ) CheckIfOnGround();
        }
        Climb();
    }

    void LateUpdate(){
        // Do not apply gravity when climbing, hanging off ledge, of climbing up from ledge
        if ( !isClimbing && !onLedge )
            velocity.y += Physics.gravity.y * Time.deltaTime;
        // Do not apply controller movement when climbing up, this is handled in Update()
        if ( !onLedge )
            characterController.Move(velocity * Time.deltaTime);
        characterObject.anim.SetFloat("SpeedX",Mathf.Abs(characterController.velocity.x));
    }

    private void MoveLeftRight(){
        velocity = Vector3.zero;
        if ( Input.GetKey(KeyCode.A) ){
            transform.LookAt(Vector3.left+transform.position);
            velocity = transform.forward;
        }
        if ( Input.GetKey(KeyCode.D) ){
            transform.LookAt(Vector3.right+transform.position);
            velocity = transform.forward;
        }

        velocity *= character.currentStats.movSpd;
        if ( Input.GetButton("Jump") ){
            velocity.y = jumpForce;
            characterObject.anim.SetBool("Jump",true);
        }
    }
    private void Climb(){
        // Check if transform reached the top of wall
        if ( climbingWall != null && head.transform.position.y > climbingWall.bounds.max.y+characterController.height*0.025f ){
            onLedge = true;
            isClimbing = false;
            velocity = Vector3.zero;
            characterObject.anim.SetBool("Climbing",false);
            characterObject.anim.SetBool("Jump",false);
            characterObject.anim.SetBool("Ledge",true);
        }
        if ( !onLedge ){
            if ( Input.GetKey(KeyCode.W) ){
                // Check if transform is facing at right direction
                if ( Input.GetKey(KeyCode.A) && transform.forward == Vector3.left || Input.GetKey(KeyCode.D) && transform.forward == Vector3.right ){
                    // Check if transform is facing a wall
                    RaycastHit wallHit;
                    if ( Physics.Raycast(head.transform.position,transform.forward,out wallHit,0.2f,1 << LayerMask.NameToLayer("Wall")) ){
                        // Climb upward
                        climbingWall = wallHit.collider;
                        velocity.x = 0f;
                        velocity.y = character.currentStats.movSpd*0.15f;
                        isClimbing = true;
                        characterObject.anim.SetBool("Climbing",true);
                    }
                } else {
                    // Fall off if not facing right direction or let go of A or D
                    isClimbing = false;
                    characterObject.anim.SetBool("Climbing",false);
                }
            }
            if ( Input.GetKeyUp(KeyCode.W) ){
                // Fall off if climb key is not held
                isClimbing = false;
                climbingWall = null;
                characterObject.anim.SetBool("Climbing",false);
            }
        } else {
            // Climb Up on ledge
            if ( Input.GetKeyDown(KeyCode.W) ){
                freeFalling = false;
                characterObject.anim.SetBool("Ledge",false);
                ledgePoint = new Vector3(transform.position.x+transform.forward.x,climbingWall.bounds.max.y+characterController.height/2.0f,0f);
                climbingWall = null;
            }
        }
    }
    private void CheckIfOnGround(){
        RaycastHit hit;
        if ( Physics.Raycast(transform.position,transform.TransformDirection(Vector3.down),out hit,0.1f, 1 << LayerMask.NameToLayer("Wall"))){
            isGrounded = true;
            freeFalling = false;
            characterObject.anim.SetBool("Jump",false);
        } else {
            isGrounded = false;
            if ( !freeFalling ){
                freeFalling = true;
                freeFallTime = Time.time;
            } else if ( Time.time - freeFallTime >= freeFallDuration ){
                characterObject.anim.SetBool("Jump",true);
            }
        }
    }
}
