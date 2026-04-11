using GameItems;
using UnityEngine;

namespace Inventory
{
    public struct InventorySlot
    {
        public static InventorySlot Empty => default;

        public InventorySlot(Item held, float count)
        {
            Held = held;
            Count = count;
        }

        public Item Held { get; set; }

        public float Count { get; set; }

        public bool IsEmpty => Held == null || Count <= 0f;

        public bool CanAdd(InventorySlot incoming)
        {
            if (incoming.IsEmpty)
            {
                return false;
            }

            if (IsEmpty)
            {
                return true;
            }

            if (Held != incoming.Held)
            {
                return false;
            }

            return Count < incoming.Held.MaxStackSize;
        }

        public void Add(InventorySlot incoming)
        {
            if (incoming.IsEmpty)
            {
                return;
            }

            if (IsEmpty)
            {
                Held = incoming.Held;
                Count = Mathf.Min(incoming.Held.MaxStackSize, incoming.Count);
                return;
            }

            if (Held != incoming.Held)
            {
                return;
            }

            Count = Mathf.Min(incoming.Held.MaxStackSize, Count + incoming.Count);
        }

        public void Remove(float amount)
        {
            if (IsEmpty || amount <= 0f)
            {
                return;
            }

            var nextCount = Count - amount;
            if (nextCount <= 0f)
            {
                Held = null;
                Count = 0f;
            }
            else
            {
                Count = nextCount;
            }
        }
    }
}
