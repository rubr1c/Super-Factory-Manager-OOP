using UnityEngine;

namespace Inventory
{
    public class ItemContainer
    {
        private readonly InventorySlot[] _slots;

        public int Capacity => _slots.Length;

        public ItemContainer(int size)
        {
            _slots = new InventorySlot[size];
        }

        public ref InventorySlot GetSlot(int index) => ref _slots[index];

        public InventorySlot GetFirst()
        {
            for (var i = 0; i < _slots.Length; i++)
            {
                if (!_slots[i].IsEmpty)
                {
                    return _slots[i];
                }
            }

            return InventorySlot.Empty;
        }

        public bool TryAdd(InventorySlot incoming)
        {
            return TryInsert(incoming).IsEmpty;
        }

        public InventorySlot TryInsert(InventorySlot incoming)
        {
            if (incoming.IsEmpty)
            {
                return InventorySlot.Empty;
            }

            var remaining = incoming;
            for (var i = 0; i < _slots.Length; i++)
            {
                ref var slot = ref _slots[i];
                remaining = slot.TryInsert(remaining);
                if (remaining.IsEmpty)
                {
                    return InventorySlot.Empty;
                }
            }

            return remaining;
        }

        public InventorySlot TryExtract(InventorySlot request)
        {
            if (request.IsEmpty)
            {
                return InventorySlot.Empty;
            }

            for (var i = 0; i < _slots.Length; i++)
            {
                ref var slot = ref _slots[i];
                if (!slot.CanConsume() || slot.Held != request.Held)
                {
                    continue;
                }

                return slot.TryExtract(request);
            }

            return InventorySlot.Empty;
        }
    }
}
