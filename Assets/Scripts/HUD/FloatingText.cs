using UnityEngine;
using System.Collections;

public class FloatingText : MonoBehaviour {

    public float speed = 0.1f;
    public float waitTime = 3f;

    void Start(){
        StartCoroutine("WaitForDestroy");
    }

    void FixedUpdate(){
        transform.position += new Vector3(0,speed,0);
        transform.LookAt(Camera.main.transform);
    }

    private IEnumerator WaitForDestroy(){
        yield return new WaitForSeconds(waitTime);
        Destroy(gameObject);
    }
}
