using System;
using Data.Items;
using UnityEngine;

namespace Systems.Inventory
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

        public InventorySlot TryInsert(InventorySlot incoming, Func<Item, bool> canAccept = null)
        {
            if (incoming.IsEmpty)
            {
                return incoming;
            }

            if (canAccept != null && !canAccept(incoming.Held))
            {
                return incoming;
            }

            if (!CanAdd(incoming))
            {
                return incoming;
            }

            var maxCount = incoming.Held.MaxStackSize;
            var availableSpace = IsEmpty
                ? maxCount
                : maxCount - Count;
            var movedAmount = Mathf.Min(availableSpace, incoming.Count);
            if (movedAmount <= 0f)
            {
                return incoming;
            }

            Add(new InventorySlot(incoming.Held, movedAmount));
            var remainder = incoming;
            remainder.Remove(movedAmount);
            return remainder;
        }

        public InventorySlot TryExtract(InventorySlot request)
        {
            if (request.IsEmpty || IsEmpty || Held != request.Held)
            {
                return Empty;
            }

            var extractedAmount = Mathf.Min(Count, request.Count);
            if (extractedAmount <= 0f)
            {
                return Empty;
            }

            Remove(extractedAmount);
            return new InventorySlot(request.Held, extractedAmount);
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
