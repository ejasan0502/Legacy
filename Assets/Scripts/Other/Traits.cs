using UnityEngine;
using System.Reflection;
using System.Collections;

[System.Serializable]
public class Traits {
    public int pwr;
    public int frt;
    public int con;

    public Traits(){
        FieldInfo[] fields = GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,0);
        }
    }
    public Traits(int x){
        FieldInfo[] fields = GetType().GetFields();
        for (int i = 0; i < fields.Length; i++){
            fields[i].SetValue(this,x);
        }
    }
    public Traits(Traits s){
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
            if ( (int)fields[i].GetValue(this) < 0 )
                fields[i].SetValue(this, 0);
        }
    }

    #region Operators
    public static Traits operator+(Traits s1, Traits s2){
        Traits s = new Traits();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(float)f1[i].GetValue(s1)+(float)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Traits operator-(Traits s1, Traits s2){
        Traits s = new Traits();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(float)f1[i].GetValue(s1)-(float)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Traits operator*(Traits s1, Traits s2){
        Traits s = new Traits();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(float)f1[i].GetValue(s1)*(float)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Traits operator/(Traits s1, Traits s2){
        Traits s = new Traits();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();
        FieldInfo[] f2 = s2.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(float)f1[i].GetValue(s1)/(float)f2[i].GetValue(s2));
        }

        return s;
    }
    public static Traits operator*(Traits s1, float val){
        Traits s = new Traits();

        FieldInfo[] f = s.GetType().GetFields();
        FieldInfo[] f1 = s1.GetType().GetFields();

        for (int i = 0; i < f.Length; i++){
            f[i].SetValue(s,(float)f1[i].GetValue(s1)*val);
        }

        return s;
    }
    #endregion
}
