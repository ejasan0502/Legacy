using UnityEngine;
using System.Collections;

public class FaceCamera : MonoBehaviour {

    void FixedUpdate(){
        transform.LookAt(Camera.main.transform);
    }

}
