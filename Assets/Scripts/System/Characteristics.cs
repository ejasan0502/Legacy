using UnityEngine;
using System.Reflection;
using System.Collections;

[System.Serializable]
public class Characteristics {
    public int strength;
    public int vitality;
    public int dexterity;
    public int agility;
    public int intelligence;
    public int psyche;
    public int luck;

    public Characteristics(){
        FieldInfo[] fields = GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,0);
        }
    }

    public Characteristics(Characteristics s){
        FieldInfo[] fields = GetType().GetFields();
        FieldInfo[] fields2 = s.GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,fields2[i].GetValue(s));
        }
    }

    #region Operators
    public static Characteristics operator+(Characteristics s1, Characteristics s2){
        Characteristics s = new Characteristics();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(int)f1[i].GetValue(s1)+(int)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Characteristics operator-(Characteristics s1, Characteristics s2){
        Characteristics s = new Characteristics();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(int)f1[i].GetValue(s1)-(int)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Characteristics operator*(Characteristics s1, Characteristics s2){
        Characteristics s = new Characteristics();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(int)f1[i].GetValue(s1)*(int)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Characteristics operator/(Characteristics s1, Characteristics s2){
        Characteristics s = new Characteristics();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(int)f1[i].GetValue(s1)/(int)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Characteristics operator*(Characteristics s1, int val){
        Characteristics s = new Characteristics();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(int)f1[i].GetValue(s1)*val);
        }

        return s;
    }
    public static bool operator>=(Characteristics s1, Characteristics s2){
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f1.Length; i++){
            if ( (int)f1[i].GetValue(s1) < (int)f2[i].GetValue(s2) ){
                return false;
            }
        }

        return true;
    }
    public static bool operator<=(Characteristics s1, Characteristics s2){
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f1.Length; i++){
            if ( (int)f1[i].GetValue(s1) > (int)f2[i].GetValue(s2) ){
                return false;
            }
        }

        return true;
    }
    #endregion
}
