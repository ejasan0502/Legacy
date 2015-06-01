using UnityEngine;
using System.Collections;

public class UsableHotkey : Hotkey {

    private Usable usable;
    public int inventoryIndex;

    public UsableHotkey(int x, Usable u){
        inventoryIndex = x;
        usable = u;
    }

    public override bool CanApply(){
        if ( Time.time - cdStartTime >= usable.cd ){
            return true;
        }
        return false;
    }

    public override void ApplyHotkey(Player p, int index){
        Character c = p;
        InventoryItem ii = p.inventory.GetInventorySlot(usable);

        if ( ((PlayerObject)p.characterObject).GetTarget() != null && !usable.friendly && !((PlayerObject)p.characterObject).GetTarget().IsPlayer ){
            c = ((PlayerObject)p.characterObject).GetTarget();
        }

        if ( ii.amount > 0 && usable.CanUse() ){
            usable.Use(c);
            ii.amount--;
            HotkeyManager.instance.UpdateHotkeyText(index);
            HotkeyManager.instance.UpdateHotkeyDisplay(index);
        }
    }

    public override Sprite GetIcon(){
        return usable.icon;
    }

    public override float GetCD(){
        return usable.cd;
    }

    public override UsableHotkey GetAsUsableHotkey(){
        return this;
    }

    public override string GetId(){
        return usable.id;
    }

}
