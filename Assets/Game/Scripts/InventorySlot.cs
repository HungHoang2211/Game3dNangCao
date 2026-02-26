[System.Serializable]
public class InventorySlot
{
    public ItemStatus itemData;
    public int count;

    public InventorySlot(ItemStatus data, int amount = 1)
    {
        itemData = data;
        count = amount;
    }
}