using UnityEngine;
using System.Collections;
using System.Reflection;

[System.Serializable]
public class Stats {
    public float baseDmg;
    public float critDmgMultiplier;
    public float skillDmgMultiplier;

    public float baseDef;
    public float critDefMultiplier;
    public float skillDefMultiplier;

    public float hp;
    public float mp;
    public float recov;

    public float critChance;
    public float blockRate;
    public float defPen;
    public float resist;
    public float luck;
    
    public float movSpd;
    public float maxWeight;

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

    public void Set(string fieldName, float val){
        foreach (FieldInfo fields in GetType().GetFields()){
            if ( fields.Name.ToLower() == fieldName.ToLower() ){
                fields.SetValue(this,val);
            }
        }
    }
    public void MustBePositive(){
        FieldInfo[] fields = GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            if ( (float)fields[i].GetValue(this) < 0 )
                fields[i].SetValue(this, 0f);
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
