using UnityEngine;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Monster : Character {
    
    public string description;
    public string id;
    public float sightRange = 10f;
    public List<Drop> drops;
    public float currency;

    public Monster(){
        name = "";
        level = 1;
        description = "";
        id = "";
        model = null;
        stats = new Stats();
        currentStats = new Stats();
        characterObject = null;
        skills = new List<Skill>();
        sightRange = 0f;
        atkDistance = 0f;
        exp = 0f;
        drops = new List<Drop>();
        currency = 0f;
    }
    public Monster(Monster m){
        name = m.name;
        level = m.level;
        description = m.description;
        id = m.id;
        model = m.model;
        stats = new Stats(m.stats);
        currentStats = new Stats(m.currentStats);
        characterObject = m.characterObject;
        skills = m.skills;
        sightRange = m.sightRange;
        atkDistance = m.atkDistance;
        exp = m.exp;
        currency = m.currency;
        drops = m.drops;
    }

    #region Override Methods
    protected override void CheckDeath(Character atker){
        if ( ((MonsterObject)characterObject).isDummy ) return;
        
        base.CheckDeath(atker);

        if ( atker.IsPlayer && currentStats.health < 1 ){
            float units = Random.Range(currency-level,currency);
            units = Mathf.Abs(units);
            
            string id = "";
            #region Drop
            float rNum = Random.Range(0,1000);
            rNum = rNum + rNum*atker.currentStats.dropRate;
            if ( rNum >= 935.5f ){
                // Godly
                List<string> ids = new List<string>();
                foreach (Drop d in drops){
                    if ( d.tier == Tier.godly ){
                        ids.Add(d.id);
                    }
                }

                if ( ids.Count > 0 ){
                    id = ids[Random.Range(0,ids.Count)];
                }
            } 
            if ( rNum >= 875f && id == "" ){
                // Legendary
                List<string> ids = new List<string>();
                foreach (Drop d in drops){
                    if ( d.tier == Tier.legendary ){
                        ids.Add(d.id);
                    }
                }

                if ( ids.Count > 0 ){
                    id = ids[Random.Range(0,ids.Count)];
                }
            }  
            if ( rNum >= 750f && id == ""  ){
                // Unique
                List<string> ids = new List<string>();
                foreach (Drop d in drops){
                    if ( d.tier == Tier.unique ){
                        ids.Add(d.id);
                    }
                }

                if ( ids.Count > 0 ){
                    id = ids[Random.Range(0,ids.Count)];
                }
            }  
            if ( rNum >= 500f && id == ""  ){
                // Rare
                List<string> ids = new List<string>();
                foreach (Drop d in drops){
                    if ( d.tier == Tier.rare ){
                        ids.Add(d.id);
                    }
                }

                if ( ids.Count > 0 ){
                    id = ids[Random.Range(0,ids.Count)];
                }
            }  
            if ( rNum >= 100f && id == ""  ){
                // Common
                List<string> ids = new List<string>();
                foreach (Drop d in drops){
                    if ( d.tier == Tier.common ){
                        ids.Add(d.id);
                    }
                }

                if ( ids.Count > 0 ){
                    id = ids[Random.Range(0,ids.Count)];
                }
            } else if ( id == "" ){
                // Nothing
            }
            #endregion

            Player p = atker as Player;
            p.AddExp(exp);
            if ( id != "" ){
                Item item = Game.GetGameData().GetItem(id);
                int amt = 1;
                if ( !item.IsEquip() ){
                    amt = Random.Range(1,5);
                }
                p.inventory.AddItem(item,amt);
            }
        }
    }
    public override void Miss(Character atker){
        base.Miss(atker);

        if ( currentStats.health > 0 ){
            MonsterObject mo = characterObject as MonsterObject;
            if ( mo.enemySight.target == null ) mo.enemySight.SetTarget(atker);
        }
    }
    public override void PhysicalHit(Character atker, float rawDmg){
        base.PhysicalHit(atker,rawDmg);

        if ( currentStats.health > 0 ){
            MonsterObject mo = characterObject as MonsterObject;
            if ( mo.enemySight.target == null ) mo.enemySight.SetTarget(atker);
        }
    }
    public override void MagicalHit(Character atker, float rawDmg){
        base.MagicalHit(atker, rawDmg);
        
        if ( currentStats.health > 0 ){
            MonsterObject mo = characterObject as MonsterObject;
            if ( mo.enemySight.target == null ) mo.enemySight.SetTarget(atker);
        }
    }
    #endregion

}

public class Drop {
    public string id;
    public Tier tier;

    public Drop(string i, Tier t){
        id = i;
        tier = t;
    }
}
