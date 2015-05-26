using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Usable : Item {

    public UsableType usableType;

    public Usable(){
        name = "";
        id = "";
        tier = Tier.common;
        description = "";
        weight = 0f;
        cost = 0f;
        craftCost = 0f;
        craftChance = 0;
        ingredients = new List<Ingredient>();
        icon = null;
        stats = new Stats();
        usableType = UsableType.replenish;
    }

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

    public void Use(Character user){}
}

public enum UsableType {
    replenish,
    buff,
    teleport,
    damage,
    aoe
}
