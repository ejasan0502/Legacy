using UnityEngine;
using System.Linq;
using System.Collections;

public class SummonedCreature : MonoBehaviour {

    private float duration;
    private Skill skillRef;

    public void Initialize(float d, Skill s){
        duration = d;
        skillRef = s;
    }

    private IEnumerator DestroyWithDelay(){
        yield return new WaitForSeconds(duration);

        SummonSC ssc = (SummonSC) skillRef.skillCasts.Where(sc => sc.skillType == SkillType.summon);
        if ( ssc != null ){
            ssc.summonedCreatures.Remove(this);
        }

        Destroy(gameObject);
    }
}
