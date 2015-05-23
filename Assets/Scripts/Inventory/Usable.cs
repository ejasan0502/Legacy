using UnityEngine;
using System.Reflection;
using System.Collections;

[System.Serializable]
public class Usable : Item {

    public Usable(Usable u){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = u.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            if ( fields[i].GetType().Equals(typeof(Stats)) )
                fields[i].SetValue(this,new Stats( (Stats)fields2[i].GetValue(u) ));
            else
                fields[i].SetValue(this,fields2[i].GetValue(u));
        }
    }

    public virtual void Use(Character user){}
}
