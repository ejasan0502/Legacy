using UnityEngine;
using System.Reflection;
using System.Collections;

public class Buff : MonoBehaviour, Status {

    public float duration = 15f;
    public Stats stats;
    public bool percent;

    public void Initialize(Stats s, float d, bool p){
        stats = new Stats(s);
        duration = d;
        percent = p;

        StartCoroutine("Progress");
    }

    private IEnumerator Progress(){
        CharacterObject co = GetComponent<CharacterObject>();
        if ( co != null ){
            FieldInfo[] f1 = co.c.currentStats.GetType().GetFields();
            FieldInfo[] f2 = co.c.stats.GetType().GetFields();
            FieldInfo[] f3 = stats.GetType().GetFields();

            for (int i = 0; i < f1.Length; i++){
                if ( f1[i].Name != "health" && f1[i].Name != "mana" && (float)f3[i].GetValue(stats) > 0f ){
                    float val = (float)f3[i].GetValue(stats);
                    if ( percent ) val *= (float)f2[i].GetValue(co.c.stats);
                    f1[i].SetValue(co.c.currentStats,val);
                }
            }

            yield return new WaitForSeconds(duration);

            for (int i = 0; i < f1.Length; i++){
                if ( f1[i].Name != "health" && f1[i].Name != "mana" && (float)f3[i].GetValue(stats) > 0f ){
                    f1[i].SetValue(co.c.currentStats,(float)f2[i].GetValue(co.c.stats));
                }
            }

            Destroy(this);
        }
    }
}
