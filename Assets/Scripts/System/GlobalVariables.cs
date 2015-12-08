using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GlobalVariables {
    
    public static int MAX_AMOUNT = 99;

    public static string PATH_XMLDATA_WEAPONS = "Data/weapons";
    public static string PATH_XMLDATA_ARMORS = "Data/armors.xml";
    public static string PATH_XMLDATA_USABLES = "Data/usables.xml";
    public static string PATH_XMLDATA_MATERIALS = "Data/materials.xml";
    public static string PATH_XMLDATA_SCENES = "Data/scenes.xml";
    public static string PATH_XMLDATA_SCENES_FULL = "Resources/Data/scenes.xml";

    public static string PATH_CONTENTDATA = "Content/";
    public static string PATH_CONTENTDATA_CHARACTERS = "Content/c/";

    public static string PATH_SCENEOBJECTS = "SceneObjects/";

    public static string ANIM_WALK = "walk";
    public static string ANIM_RUN = "run";
    public static string ANIM_CAST = "cast";

    public static Traits DEFAULT_PLAYER_TRAITS = new Traits(3);
    public static Stats DEFAULT_PLAYER_STATS = new Stats(
                                                            3f,
                                                            1.25f,
                                                            1.5f,
                                                            2f,
                                                            1f,
                                                            1f,
                                                            10f,
                                                            5f,
                                                            0.06f,
                                                            0.0125f,
                                                            0.022f,
                                                            0.0125f,
                                                            1f,
                                                            30f
                                                        );

    public enum UnityScenes {
        login = 0,
        characterCreation = 1,
        tutorial = 2,
        game = 3
    }
}
