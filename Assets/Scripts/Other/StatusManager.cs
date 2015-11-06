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

    public void Update(){
        if ( pauseUpdate ){
            foreach (StatusState ss in statuses){
                if ( ss.CheckToEnd() ){
                    RemoveStatus(ss);
                } else if ( ss.status == Status.dot ){
                    character.Dot(ss.amt);
                }
            }
        }
    }   
    public void AddStatus(Status s, float d, float a){
        StatusState statusState = (StatusState) statuses.Where(ss => ss.status == s);
        if ( statusState == null ){
            statuses.Add(new StatusState(s,d,a));
        } else {
            statusState.RestartTime();
            if ( a > statusState.amt )
                statusState.amt = a;
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
    public void RemoveStatus(StatusState ss){
        pauseUpdate = true;

        if ( statuses.Contains(ss) ){
            UnapplyStatus(ss.status);
            statuses.Remove(ss);
        }

        pauseUpdate = false;
    }

    private void ApplyStatus(Status s){
        switch(s){
            case Status.stun:
            character.Stunned();
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

    private float startTime;

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

    public void RestartTime(){
        startTime = Time.time;
    }
    public bool CheckToEnd(){
        if ( Time.time - startTime >= duration ){
            return true;
        }
        return false;
    }
}

public enum Status {
    stun,           // Stars above characters head, lasts for a short time period
    paralysis,      // Lies on the floor, gets up after a duration
    knockdown,      // Lies on the floor, gets up almost immediately
    dot,            // Damage over time
    frostbit,       // Slow animations, character is blue
    frozen,         // Unable to perform any action, character is encased in ice
    sleep,          // Lies on the floor, gets up after a long duration, character has 'Zzz' animation
    silence,        // Unable to cast spells for a duration
    blind,          // Screen goes black
    rooted          // Unable to move
}
