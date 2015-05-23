using UnityEngine;
using System.Collections;

public class UsableHotkey : Hotkey {

    private Usable usable;

    public UsableHotkey(Usable u){
        usable = u;
    }

    public override void ApplyHotkey(Player p){
        usable.Use(p);
    }

}
