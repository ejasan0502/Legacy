using UnityEngine;
using System.Collections;

public class Replenish : MonoBehaviour, Status {

    public float duration = 15f;
    public float rate = 3f;
    public float hp;
    public float mp;

    public void Initialize(float h, float m, float r, float d){
        hp = h;
        mp = m;
        rate = r;
        duration = d;

        StartCoroutine("Progress");
    }

    private IEnumerator Progress(){
        CharacterObject co = GetComponent<CharacterObject>();
        if ( co != null ){
            float startTime = Time.time;

            while (Time.time - startTime < duration){
                yield return new WaitForSeconds(rate);
                co.c.currentStats.health += hp;
                co.c.currentStats.mana += mp;
            }
        }
    }
}
