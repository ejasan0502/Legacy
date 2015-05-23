using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EventManager {

	private Dictionary<string,EventHandler<MyEventArgs>> subscribers = new Dictionary<string,EventHandler<MyEventArgs>>();

	// EventManager.instance.AddEventHandler(EVENT_NAME, new EventHandler<MyEventArgs>(METHOD_NAME));
	// > EVENT_NAME = Any string variable pertaining to an event, "OnTutorialComplete"
	// > METHOD_NAME = Context of the name of the method to be used when the event is triggered, public void Method(object sender, MyEventArgs args){}
	public void AddEventHandler(string eventName, EventHandler<MyEventArgs> method){
		if ( subscribers.ContainsKey(eventName) ){
			subscribers[eventName] += method;
		} else {
			subscribers.Add(eventName,method);
		}
	}

    public void RemoveAllEventHandlers(){
        subscribers.Clear();
    }

	// EventManager.instance.RemoveEventHandler(EVENT_NAME, new EventHandler<MyEventArgs>(METHOD_NAME));
	// > EVENT_NAME = Any string variable pertaining to an event
	// > METHOD_NAME = Context of the name of the method to be removed
	public void RemoveEventHandler(string eventName, EventHandler<MyEventArgs> method){
		if ( subscribers.ContainsKey(eventName) ){
			subscribers[eventName] -= method;
			if ( subscribers[eventName] == null ) subscribers.Remove(eventName);
		}
	}

	// EventManager.instance.TriggerEvent("Hello",EventArgs.Empty);
	// EventManager.instance.TriggerEvent("Hello",new MyEventArgs(ARRAY_LIST));
	// > ARRAY_LIST = An arraylist variable that is used to send data to events
	public void TriggerEvent(string eventName, MyEventArgs args){
		if ( subscribers.ContainsKey(eventName) ) subscribers[eventName].Invoke(this,args);
	}
}

// You can modify variables used in MyEventArgs
public class MyEventArgs : EventArgs {
	public ArrayList args;

	public MyEventArgs(ArrayList a){
		args = a;
	}
}