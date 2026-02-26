using UnityEngine;

[CreateAssetMenu(fileName = "NewCurrency", menuName = "Inventory/Currency")]
public class CurrencyData : ScriptableObject
{
    public string currencyName;
    public Sprite currencyIcon;
    public int amount; // Số lượng tiền hiện có

    public void Add(int value)
    {
        amount += value;
    }

    public bool TrySpend(int value)
    {
        if (amount >= value)
        {
            amount -= value;
            return true;
        }
        return false;
    }
}