using UnityEngine;
using System.Collections;
using System.Reflection;

[System.Serializable]
public class Stats {
    public float health;
    public float mana;
    public float hpRecov;
    public float mpRecov;

    public float physMinDmg;
    public float physMaxDmg;
    public float magMinDmg;
    public float magMaxDmg;

    public float physDef;
    public float magDef;

    public float critChance;
    public float critDmg;
    
    public float evasion;
    public float accuracy;

    public float atkSpd;
    public float castSpd;
    public float movSpd;

    public float enhanceRate;
    public float enchantRate;
    public float dropRate;

    public Stats(){
        FieldInfo[] fields = GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,0f);
        }
    }
    public Stats(Stats s){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = s.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,fields2[i].GetValue(s));
        }
    }

    #region Operators
    public static Stats operator+(Stats s1, Stats s2){
        Stats s = new Stats();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(float)f1[i].GetValue(s1)+(float)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Stats operator-(Stats s1, Stats s2){
        Stats s = new Stats();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(float)f1[i].GetValue(s1)-(float)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Stats operator*(Stats s1, Stats s2){
        Stats s = new Stats();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(float)f1[i].GetValue(s1)*(float)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Stats operator/(Stats s1, Stats s2){
        Stats s = new Stats();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(float)f1[i].GetValue(s1)/(float)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Stats operator*(Stats s1, float val){
        Stats s = new Stats();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(float)f1[i].GetValue(s1)*val);
        }

        return s;
    }
    #endregion
}
