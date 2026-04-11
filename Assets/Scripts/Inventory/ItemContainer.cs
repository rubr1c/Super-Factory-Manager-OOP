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
            if (incoming.IsEmpty)
            {
                return true;
            }

            var remaining = incoming;
            for (var i = 0; i < _slots.Length; i++)
            {
                ref var slot = ref _slots[i];
                if (!slot.CanAdd(remaining))
                {
                    continue;
                }

                var space = slot.IsEmpty
                    ? remaining.Held.MaxStackSize
                    : remaining.Held.MaxStackSize - slot.Count;
                var take = Mathf.Min(space, remaining.Count);
                slot.Add(new InventorySlot(remaining.Held, take));
                remaining.Remove(take);
                if (remaining.IsEmpty)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
