using UnityEngine;
using System.Collections;

public class RemoteInterpolation : MonoBehaviour {

    private Vector3 desiredPos;
    private Quaternion desiredRot;

    private float dampingFactor = 5f;

    void Start(){
        desiredPos = transform.position;
        desiredRot = transform.rotation;
    }

    public void SimpleInterpolation(Vector3 pos, Quaternion rot, bool interpolate){
        if ( interpolate ){
            desiredPos = pos;
            desiredRot = rot;
        } else {
            transform.position = pos;
            transform.rotation = rot;
        }
    }

    void Update(){
        transform.position = Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * dampingFactor);
        transform.rotation = Quaternion.Slerp(transform.rotation, desiredRot, Time.deltaTime * dampingFactor);
    }
}
