using UnityEngine;
using System.Collections;

public class Usable {

    public Stats stats;
    public Traits traits;
    public int targetAmount;
    public float duration;

}

public enum UsableType {
    restore,
    instantRestore,
    cure,
    buff,
    enchantScroll,
    upgradeCrystal
}
