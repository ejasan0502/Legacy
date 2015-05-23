using UnityEngine;
using System.Collections;

public abstract class Ability {
    public string name;
    public string id;
    public string description;

    public abstract void Apply(Player p);
}
