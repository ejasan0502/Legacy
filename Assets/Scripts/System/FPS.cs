using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
[RequireComponent(typeof(Text))]
public class FPS : MonoBehaviour {
    public  float updateInterval = 0.5F;
 
    private float accum   = 0; 
    private int   frames  = 0; 
    private float timeleft;
    private Text text;
 
    void Awake(){
        timeleft = updateInterval;  
        text = GetComponent<Text>();
    }
 
    void Update(){
        timeleft -= Time.deltaTime;
        accum += Time.timeScale/Time.deltaTime;
        ++frames;
 
        if( timeleft <= 0.0 ){
	        float fps = accum/frames;
	        string format = System.String.Format("{0:F2} FPS",fps);
	        text.text = format;
 
	        if(fps < 30)
		        text.color = Color.yellow;
	        else 
		        if(fps < 10)
			        text.color = Color.red;
		        else
			        text.color = Color.green;

            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
}