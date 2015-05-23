using UnityEngine;
using System.Collections;

public class DestroyWithDelay : MonoBehaviour {

    public float delay = 3f;
    public bool canTapDestroy = true;

    void Start(){
        StartCoroutine("Wait");
    }

    private IEnumerator Wait(){
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }

    public void Destroy(){
        if ( canTapDestroy ){
            StopCoroutine("Wait");
            Destroy(gameObject);
        }
    }
}
