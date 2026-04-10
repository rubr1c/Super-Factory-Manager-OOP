using Item;

public struct InventorySlot
{
    private ItemData _item;
    private float _count;
    
    public static bool operator ==(InventorySlot left, InventorySlot right)
    {
        return left._item == right._item;
    }

    public static bool operator !=(InventorySlot left, InventorySlot right)
    {
        return left._item != right._item;
    }
}


