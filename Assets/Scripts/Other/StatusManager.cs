using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class StatusManager {
    
    public List<StatusState> statuses;

    private Character character;

    public StatusManager(){
        statuses = new List<StatusState>();
        character = null;
    }
    public StatusManager(Character c){
        character = c;
        statuses = new List<StatusState>();
    }

    private bool pauseUpdate = false;

    public void OnUpdate(){
        if ( pauseUpdate ){
            foreach (StatusState ss in statuses){
                if ( ss.CheckToEnd() ){
                    RemoveStatus(ss.status);
                } else if ( ss.status == Status.dot && ((Time.time-ss.StartTime)%3) == 0 ){
                    character.Dot(ss.amt);
                } else if ( ss.status == Status.restore && ((Time.time-ss.StartTime)%3) == 0 ){
                    character.Heal(ss.amt,0f,true);
                } else if ( ss.status == Status.charging ){
                    ChargingStatusState css = (ChargingStatusState) ss;
                    if ( css.CanChargeUp() )
                        css.ChargeUp();
                    else if ( css.chargeSkillCast.MaxCharged ){
                        css.chargeSkillCast.Release();
                    }
                } else if ( ss.status == Status.toggling ){
                    ToggleStatusState tss = (ToggleStatusState) ss;
                    tss.ContiniousApply(character);
                }
            }
        }
    }   
    public void AddStatus(Status s, float d, float a){
        StatusState statusState = (StatusState) statuses.Where(ss => ss.status == s);
        if ( statusState == null ){
            statuses.Add(new StatusState(s,d,a));
            ApplyStatus(s);
        } else {
            statusState.RestartTime();
            if ( a > statusState.amt )
                statusState.amt = a;
        }
    }
    public void AddStatus(ChargeSC csc){
        StatusState statusState = (StatusState) statuses.Where(ss => ss.status == Status.charging);
        if ( statusState == null ){
            statuses.Add(new ChargingStatusState(csc));
        } else {
            statusState.RestartTime();
        }
    }
    public void AddStatus(ToggleSC tsc){
        StatusState statusState = (StatusState) statuses.Where(ss => ss.status == Status.toggling);
        if ( statusState == null ){
            statuses.Add(new ToggleStatusState(tsc));
        }
    }
    public void RemoveStatus(){
        pauseUpdate = true;

        foreach (StatusState ss in statuses){
            UnapplyStatus(ss.status);
        }
        statuses.Clear();

        pauseUpdate = false;
    }
    public void RemoveStatus(params Status[] s){
        pauseUpdate = true;

        foreach (StatusState ss in statuses){
            for (int i = 0; i < s.Length; i++){
                if ( ss.status == s[i] ){
                    UnapplyStatus(s[i]);
                    statuses.Remove(ss);
                }
            }
        }

        pauseUpdate = false;
    }
    public void RemoveStatus(List<Status> s){
        pauseUpdate = true;

        foreach (StatusState ss in statuses){
            for (int i = 0; i < s.Count; i++){
                if ( ss.status == s[i] ){
                    UnapplyStatus(s[i]);
                    statuses.Remove(ss);
                }
            }
        }

        pauseUpdate = false;
    }

    private void ApplyStatus(Status s){
        switch(s){
            case Status.stun:
            break;
            case Status.paralysis:
            break;
            case Status.knockdown:
            break;
            case Status.frostbit:
            break;
            case Status.frozen:
            break;
            case Status.sleep:
            break;
            case Status.silence:
            break;
            case Status.blind:
            break;
            case Status.rooted:
            break;
        }
    }
    private void UnapplyStatus(Status s){
        switch(s){
            case Status.stun:
            break;
            case Status.paralysis:
            break;
            case Status.knockdown:
            break;
            case Status.dot:
            break;
            case Status.frostbit:
            break;
            case Status.frozen:
            break;
            case Status.sleep:
            break;
            case Status.silence:
            break;
            case Status.blind:
            break;
            case Status.rooted:
            break;
        }
    }

}

public class StatusState {
    public Status status;
    public float duration;
    public float amt;

    protected float startTime;
    public float StartTime {
        get {
            return startTime;
        }
    }

    public StatusState(){
        status = Status.stun;
        duration = 0f;
        amt = 0f;
        startTime = 0f;
    }
    public StatusState(Status s, float d, float a){
        status = s;
        duration = d;
        amt = a;

        startTime = Time.time;
    }

    public virtual void RestartTime(){
        startTime = Time.time;
    }
    public bool CheckToEnd(){
        if ( Time.time - startTime >= duration ){
            return true;
        }
        return false;
    }
}
public class ChargingStatusState : StatusState {

    public ChargeSC chargeSkillCast;

    public ChargingStatusState(ChargeSC csc){
        status = Status.charging;
        startTime = Time.time;
    }

    public bool CanChargeUp(){
        if ( Time.time - startTime >= chargeSkillCast.chargeRate && !chargeSkillCast.MaxCharged ){
            return true;
        }
        return false;
    }
    public void ChargeUp(){
        chargeSkillCast.Charge();
    }

    public override void RestartTime(){
        chargeSkillCast.Release();
        startTime = Time.time;
    }
}
public class ToggleStatusState : StatusState {
    
    public ToggleSC toggleSC;

    public ToggleStatusState(ToggleSC tsc){
        status = Status.toggling;

        startTime = Time.time;
    }

    public void ContiniousApply(Character c){
        if ( toggleSC.skill.isAoe ){
            c.GetCharacterObject().UpdateTargets(toggleSC.skill);
            List<Character> targets = c.GetCharacterObject().GetTargets();
            if ( targets.Count > 0 ){
                foreach (Character target in targets){
                    toggleSC.skill.Apply(c,target);
                }
            }
        } else {
            toggleSC.skill.Apply(c,c);
        }
        c.Heal(toggleSC.skill.hpCost,toggleSC.skill.mpCost,toggleSC.skill.percentageCost);
    }

}

public enum Status {
    toggling,       // Depletes mana 
    charging,       // Unable to perform any action, Charge Up
    stun,           // Stars above characters head, lasts for a short time period
    paralysis,      // Lies on the floor, gets up after a duration
    knockdown,      // Lies on the floor, gets up almost immediately
    dot,            // Damage over time
    frostbit,       // Slow animations, character is blue
    frozen,         // Unable to perform any action, character is encased in ice
    sleep,          // Lies on the floor, gets up after a long duration, character has 'Zzz' animation
    silence,        // Unable to cast spells for a duration
    blind,          // Screen goes black
    rooted,         // Unable to move
    restore         // Heal hp over time by a percentage
}
