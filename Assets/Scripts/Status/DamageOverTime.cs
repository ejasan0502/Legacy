using UnityEngine;
using System.Reflection;
using System.Collections;

public class DamageOverTime : MonoBehaviour, Status {

    public float duration = 15f;
    public float rate = 3f;
    public Stats stats;

    public void Initialize(Stats s, float r, float d){
        stats = new Stats(s);
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
                co.c.Damage(stats.health,stats.mana);
            }

            Destroy(this);
        }
    }
}
