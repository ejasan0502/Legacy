using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class InputControls : MonoBehaviour {

    public string layerToDetectMovtTouch = "Floor";
    public string layerToDetectTargetTouch = "Targetable";

    public float camRotSpd = 0.05f;
    public float camZoomSpd = 10f;
    public float minFieldOfView = 20f;
    public float maxFieldOfView = 60f;
    public float minAngleY = -1000f;
    public float maxAngleY = 200f;

    private Vector3 camOffset;
    private float desiredAngleX = 0f;
    private float desiredAngleY = 0f;

    private Vector2 touchStartPos;
    private PlayerObject playerObject;
    private bool follow = false;
    private NPCObject npc = null;

    private float tapTime = 0.2f;
    private float inputTime = 0f;
    private bool hold = false;

    void Start(){
        camOffset = Camera.main.transform.position - transform.position;
        playerObject = GetComponent<PlayerObject>();

        // Decrease speeds for mobile platforms
        if ( Application.isMobilePlatform ) {
            camZoomSpd *= 0.01f;
            camRotSpd *= 0.25f;
        }
    }

    void Update(){
        #region Unity Editor
        if ( Application.isEditor ){
            #region Single Touch
            if ( Input.GetMouseButtonDown(0) ){
                inputTime = Time.time;
            }

            if ( Input.GetMouseButton(0) ){
                if ( Time.time - inputTime > tapTime ){
                    hold = true;
                    GameObject.FindObjectOfType<HUD>().SetMenuDisplay(true);
                }
            }

            if ( Input.GetMouseButtonUp(0) ){
                if ( hold ) {
                    GameObject.FindObjectOfType<HUD>().CheckInput();
                    hold = false;
                    return;
                }
                if ( InDeadZone() ) return;

                #region Movement/Select Controls
                Vector3 mousePos = Input.mousePosition;
                Ray ray = Camera.main.ScreenPointToRay(mousePos);
                RaycastHit hit;

                if ( Physics.Raycast(ray, out hit) ){
                    follow = false;
                    if ( hit.transform.gameObject.layer == LayerMask.NameToLayer(layerToDetectMovtTouch) ){
                        Vector3 pos = hit.point;
                        pos.y += transform.position.y;
                        playerObject.SetEndPoint(pos);
                        playerObject.SetTarget(null);
                    } else if ( hit.transform.gameObject.layer == LayerMask.NameToLayer(layerToDetectTargetTouch) ){
                        #region Player/Monster selected
                        if ( hit.transform.GetComponent<CharacterObject>() != null ){
                            Character c = hit.transform.GetComponent<CharacterObject>().GetCharacter();
                            if ( c.IsAlive ){
                                if ( playerObject.HasTarget() && playerObject.GetTarget() == c ){
                                    if ( c.IsPlayer ){
                                        follow = true;
                                    } else {
                                        playerObject.SetTarget(c);
                                        playerObject.transform.LookAt(playerObject.GetTarget().characterObject.transform);
                                    }
                                } else {
                                    playerObject.SetTarget(c);
                                    playerObject.transform.LookAt(playerObject.GetTarget().characterObject.transform);
                                }
                            }
                        } else if ( hit.transform.GetComponent<NPCObject>() != null )
                        #endregion
                        #region NPC selected
                        {   
                            follow = false;
                            npc = hit.transform.GetComponent<NPCObject>();
                            playerObject.SetEndPoint(npc.transform.position);
                        }
                        #endregion
                    }
                }
                #endregion
            }
            #endregion
            #region Camera Controls
            if ( Input.GetMouseButtonDown(1) ){
                touchStartPos = Input.mousePosition;
            }
            if ( Input.GetMouseButton(1) ){
                desiredAngleX += Input.mousePosition.x - touchStartPos.x;
                desiredAngleY += Input.mousePosition.y - touchStartPos.y;

                if ( desiredAngleY < minAngleY ) desiredAngleY = minAngleY;
                else if ( desiredAngleY > maxAngleY ) desiredAngleY = maxAngleY;
            }
            #endregion
            #region Zooming
            if ( Input.GetAxis("Mouse ScrollWheel") != 0 ){
                Camera.main.fieldOfView += -Input.GetAxis("Mouse ScrollWheel") * camZoomSpd;
            }
            #endregion
        } else 
        #endregion
        #region Android, iOS, Windows Phone
        if ( Application.isMobilePlatform ){
            #region Single Touch
            if ( Input.touchCount == 1 ){
                Touch touch = Input.touches[0];
                if ( touch.phase == TouchPhase.Began ){
                    touchStartPos = Input.mousePosition;
                    inputTime = Time.time;
                    GameObject.FindObjectOfType<HUD>().SetMenuDisplay(false);
                }

                if ( touch.phase == TouchPhase.Stationary ){
                    if ( Time.time - inputTime > tapTime ){
                        hold = true;
                        GameObject.FindObjectOfType<HUD>().SetMenuDisplay(true);
                    }
                }
                
                #region Camera Controls
                if ( touch.phase == TouchPhase.Moved ){
                    if ( !hold ){
                        inputTime = Time.time;

                        desiredAngleX += Input.mousePosition.x - touchStartPos.x;
                        desiredAngleY += Input.mousePosition.y - touchStartPos.y;

                        if ( desiredAngleY < minAngleY ) desiredAngleY = minAngleY;
                        else if ( desiredAngleY > maxAngleY ) desiredAngleY = maxAngleY;
                    }
                } 
                #endregion

                if ( touch.phase == TouchPhase.Ended ){
                    if ( hold ) {
                        GameObject.FindObjectOfType<HUD>().CheckInput();
                        hold = false;
                        return;
                    }
                    if ( InDeadZone() ) return;

                    #region Movement/Select Controls
                    Vector3 mousePos = Input.mousePosition;
                    Ray ray = Camera.main.ScreenPointToRay(mousePos);
                    RaycastHit hit;

                    if ( Physics.Raycast(ray, out hit) ){
                        follow = false;
                        if ( hit.transform.gameObject.layer == LayerMask.NameToLayer(layerToDetectMovtTouch) ){
                            Vector3 pos = hit.point;
                            pos.y += transform.position.y;
                            playerObject.SetEndPoint(pos);
                        } else if ( hit.transform.gameObject.layer == LayerMask.NameToLayer(layerToDetectTargetTouch) ){
                            #region Player/Monster selected
                            if ( hit.transform.GetComponent<CharacterObject>() != null ){
                                Character c = hit.transform.GetComponent<CharacterObject>().GetCharacter();
                                if ( c.IsAlive ){
                                    if ( playerObject.HasTarget() && playerObject.GetTarget() == c ){
                                        if ( c.IsPlayer ){
                                            follow = true;
                                        } else {
                                            playerObject.transform.LookAt(playerObject.GetTarget().characterObject.transform);
                                            playerObject.SetState(CharacterState.battling);
                                        }
                                    } else {
                                        playerObject.SetTarget(c);
                                    }  
                                }
                            } else if ( hit.transform.GetComponent<NPCObject>() != null )
                            #endregion
                            #region NPC selected
                            {
                                follow = false;
                                npc = hit.transform.GetComponent<NPCObject>();
                                playerObject.SetEndPoint(npc.transform.position);
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
            } else
            #endregion
            #region Zooming Controls
            if ( Input.touchCount == 2 ){
                Touch touch1 = Input.touches[0];
                Touch touch2 = Input.touches[1];

                if ( touch1.phase == TouchPhase.Moved && touch2.phase == TouchPhase.Moved ) {
                    Vector2 touch1PrevPos = touch1.position - touch1.deltaPosition;
                    Vector2 touch2PrevPos = touch2.position - touch2.deltaPosition;

                    float prevTouchDeltaMag = (touch1PrevPos - touch2PrevPos).magnitude;
                    float touchDeltaMag = (touch1.position - touch2.position).magnitude;

                    float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

                    Camera.main.fieldOfView += deltaMagnitudeDiff * camZoomSpd;
                    Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView, 0.1f, 179.9f);
                }
            } 
            #endregion
        }
        #endregion
        
        if ( follow ) playerObject.SetEndPoint(playerObject.GetTargetObject().transform.position);
        
        if ( npc != null && Vector3.Distance(transform.position,npc.transform.position) < 2f ){
            playerObject.SetEndPoint(transform.position);
            npc.Interact(playerObject);
            npc = null;
            enabled = false;
        }

        if ( Camera.main.fieldOfView > maxFieldOfView ) Camera.main.fieldOfView = maxFieldOfView;
        else if ( Camera.main.fieldOfView < minFieldOfView ) Camera.main.fieldOfView = minFieldOfView;
    }

    void LateUpdate(){
        Quaternion rotation = Quaternion.Euler(desiredAngleY*camRotSpd,desiredAngleX*camRotSpd,0);
        Camera.main.transform.position = transform.position + (rotation * camOffset);

        Camera.main.transform.LookAt(transform);
    }

    private bool InDeadZone(){
        return false;
    }
}
