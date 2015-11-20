using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Recipe {
    public List<Ingredient> ingredients;
}

public class Ingredient {
    public Item item;
    public int amt;
}
