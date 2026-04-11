using GameItems;

public readonly struct InventorySlot
{
    public InventorySlot(Item item, float count)
    {
        Definition = item;
        Count = count;
    }

    public Item Definition { get; }

    public float Count { get; }
}