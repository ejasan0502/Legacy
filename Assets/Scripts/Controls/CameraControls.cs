using UnityEngine;
using System.Collections;

public class CameraControls : MonoBehaviour {

    public float cameraFollowSpeed = 1f;
    public Transform player = null;

    void Update(){
        if ( player != null ){
            Vector3 desiredPos = player.position;
            desiredPos.z = transform.position.z;
            transform.position = Vector3.Lerp(transform.position,desiredPos,Time.deltaTime*cameraFollowSpeed);
        }
    }

    public void SetPlayer(Transform t){
        player = t;
    }
}
