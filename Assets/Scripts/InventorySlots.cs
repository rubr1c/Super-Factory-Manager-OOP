using GameItems;
using UnityEngine;

public static class InventorySlots
{
    public static InventorySlot Empty => new(null, 0f);

    public static bool IsEmpty(InventorySlot slot)
    {
        return slot.Definition == null || slot.Count <= 0f;
    }

    public static bool CanAdd(InventorySlot slot, Item item)
    {
        if (item == null)
        {
            return false;
        }

        return IsEmpty(slot) || (slot.Definition == item && slot.Count < item.MaxStackSize);
    }

    public static InventorySlot Add(InventorySlot slot, Item item, float amount)
    {
        if (item == null || amount <= 0f)
        {
            return slot;
        }

        if (IsEmpty(slot))
        {
            return new InventorySlot(item, Mathf.Min(item.MaxStackSize, amount));
        }

        if (slot.Definition != item)
        {
            return slot;
        }

        return new InventorySlot(item, Mathf.Min(item.MaxStackSize, slot.Count + amount));
    }

    public static InventorySlot Remove(InventorySlot slot, float amount)
    {
        if (IsEmpty(slot) || amount <= 0f)
        {
            return slot;
        }

        var nextCount = slot.Count - amount;
        return nextCount <= 0f ? Empty : new InventorySlot(slot.Definition, nextCount);
    }
}
