using UnityEngine;
using System.Collections;

public static class GlobalVariables {
    
    public static int MAX_AMOUNT = 99;

    public static string PATH_PLAYERDATA = Application.persistentDataPath + "/data.data";
    public static string PATH_XMLDATA_WEAPONS = "Data/weapons";
    public static string PATH_XMLDATA_ARMORS = "Data/armors.xml";
    public static string PATH_XMLDATA_USABLES = "Data/usables.xml";
    public static string PATH_XMLDATA_MATERIALS = "Data/materials.xml";

    public static string ANIM_WALK = "walk";
    public static string ANIM_RUN = "run";
    public static string ANIM_CAST = "cast";

}
