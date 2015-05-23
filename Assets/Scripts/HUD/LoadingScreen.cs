using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void LoadingMethod();
public class LoadingScreen : MonoBehaviour {

    private LoadingMethod[] methods = null;
    private float delay = 0f;
    private bool complete = false;

    void Awake(){
        DontDestroyOnLoad(this);
    }

    public static void Load(float waitTime){
        GameObject o = Instantiate(Resources.Load("LoadingScreen")) as GameObject;
        LoadingScreen ls = o.GetComponent<LoadingScreen>();
        ls.SetVariables(waitTime,null);
        ls.Wait();
    }

    public static void Load(float waitTime, params LoadingMethod[] m){
        GameObject o = Instantiate(Resources.Load("LoadingScreen")) as GameObject;
        LoadingScreen ls = o.GetComponent<LoadingScreen>();
        ls.SetVariables(waitTime,m);
        ls.Begin();
    }

    public void SetVariables(float w, LoadingMethod[] m){
        delay = w;
        methods = m;
    }

    public void Begin(){
        StartCoroutine("StartLoading");
    }

    public void OnTap(){
        if ( complete ){
            Destroy(gameObject);
        }
    }

    private IEnumerator Wait(){
        yield return new WaitForSeconds(delay);
        complete = true;
    }

    private IEnumerator StartLoading(){
        if ( methods.Length > 0 ){
            int count = 0;

            Console.Log("Start loading...");
            while (count <= methods.Length ){
                yield return new WaitForSeconds(delay);
                Console.Log("Method " + count + " start");
                methods[count]();
                count++;
                Console.Log("Method " + count + " end");
            }
            Console.Log("Loading complete");
        } else {
            Console.Log("No methods set in loading screen.");
        }

        complete = true;
    }
}
