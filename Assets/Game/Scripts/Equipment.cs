using UnityEngine;
using UnityEngine.UI;

public class EquipmentUI : MonoBehaviour
{
    public EquipmentSlotUI weaponSlot;
    public EquipmentSlotUI ringSlot;
    public EquipmentSlotUI necklessSlot;
    public EquipmentSlotUI poisonSlot;

    public void UpdateWeapon(ItemStatus weapon)
    {
        if (weapon != null)
            weaponSlot.SetItem(weapon);
        else
            weaponSlot.ClearSlot();
    }

   
}
