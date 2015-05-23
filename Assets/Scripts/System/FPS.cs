using UnityEngine;
using UnityEngine.UI;
using System.Collections;
 
[RequireComponent(typeof(Text))]
public class FPS : MonoBehaviour {
    public  float updateInterval = 0.5F;
 
    private float accum   = 0; 
    private int   frames  = 0; 
    private float timeleft;
    private new Text guiText;
 
    void Awake(){
        timeleft = updateInterval;  
        guiText = GetComponent<Text>();
    }
 
    void Update(){
        timeleft -= Time.deltaTime;
        accum += Time.timeScale/Time.deltaTime;
        ++frames;
 
        if( timeleft <= 0.0 ){
	        float fps = accum/frames;
	        string format = System.String.Format("{0:F2} FPS",fps);
	        guiText.text = format;
 
	        if(fps < 30)
		        guiText.color = Color.yellow;
	        else 
		        if(fps < 10)
			        guiText.color = Color.red;
		        else
			        guiText.color = Color.green;

            timeleft = updateInterval;
            accum = 0.0F;
            frames = 0;
        }
    }
}