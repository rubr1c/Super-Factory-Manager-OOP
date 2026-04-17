using System;
using Data.Items;
using UnityEngine;

namespace Systems.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [Serializable]
        private struct StartingSlot
        {
            public int slotIndex;
            public string itemId;
            public int count;
        }

        public const int HotbarSlotCount = 8;
        public const int InventoryRowCount = 3;
        public const int InventoryColumnCount = 8;
        public const int InventorySlotCount = InventoryRowCount * InventoryColumnCount;
        public const int TotalSlotCount = HotbarSlotCount + InventorySlotCount;

        public static PlayerInventory Instance { get; private set; }

        [SerializeField]
        private StartingSlot[] startingSlots =
        {
            new()
            {
                slotIndex = 0,
                itemId = "starter_drill",
                count = 1
            },
            new()
            {
                slotIndex = 1,
                itemId = "industrial_assembler",
                count = 1
            },
            new()
            {
                slotIndex = 2,
                itemId = "coal_burner",
                count = 1
            }
        };

        private ItemContainer _items;

        public event Action InventoryChanged;

        public int SelectedHotbarSlotIndex { get; private set; } = -1;

        public InventorySlot SelectedHotbarSlot =>
            SelectedHotbarSlotIndex < 0 ? InventorySlot.Empty : _items.GetSlot(SelectedHotbarSlotIndex);

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _items = new ItemContainer(TotalSlotCount);
        }

        private void Start()
        {
            for (var i = 0; i < startingSlots.Length; i++)
            {
                var entry = startingSlots[i];
                if (entry.slotIndex < 0 || entry.slotIndex >= TotalSlotCount) continue;
                if (string.IsNullOrWhiteSpace(entry.itemId) || entry.count <= 0) continue;
                if (!ItemCatalog.TryGet(entry.itemId, out var item)) continue;

                ref var slot = ref _items.GetSlot(entry.slotIndex);
                slot = new InventorySlot(item, entry.count);
            }

            InventoryChanged?.Invoke();
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        public InventorySlot GetSlot(int index)
        {
            return index < 0 || index >= TotalSlotCount
                ? InventorySlot.Empty
                : _items.GetSlot(index);
        }

        public void ToggleHotbarSlotSelection(int hotbarSlotIndex)
        {
            if (hotbarSlotIndex < 0 || hotbarSlotIndex >= HotbarSlotCount) return;

            SelectedHotbarSlotIndex = SelectedHotbarSlotIndex == hotbarSlotIndex ? -1 : hotbarSlotIndex;

            InventoryChanged?.Invoke();
        }

        public bool TryConsumeSelectedHotbarItem(int amount)
        {
            if (SelectedHotbarSlotIndex < 0) return false;

            ref var slot = ref _items.GetSlot(SelectedHotbarSlotIndex);
            if (slot.IsEmpty || slot.Count < amount) return false;

            slot.Remove(amount);
            InventoryChanged?.Invoke();
            return true;
        }

        public float CountItem(Item item)
        {
            if (item == null) return 0f;

            var total = 0f;
            for (var i = 0; i < TotalSlotCount; i++)
            {
                var slot = _items.GetSlot(i);
                if (!slot.IsEmpty && slot.Held == item) total += slot.Count;
            }

            return total;
        }

        public bool TryConsumeItem(Item item, float amount)
        {
            if (item == null || amount < 0f) return false;
            if (amount == 0f) return true;
            if (CountItem(item) < amount) return false;

            var remaining = amount;
            for (var i = 0; i < TotalSlotCount && remaining > 0f; i++)
            {
                ref var slot = ref _items.GetSlot(i);
                if (slot.IsEmpty || slot.Held != item) continue;

                var consumed = Mathf.Min(slot.Count, remaining);
                slot.Remove(consumed);
                remaining -= consumed;
            }

            InventoryChanged?.Invoke();
            return true;
        }

        public void MoveSlot(int fromIndex, int toIndex)
        {
            if (fromIndex == toIndex) return;

            ref var from = ref _items.GetSlot(fromIndex);
            ref var to = ref _items.GetSlot(toIndex);

            if (from.IsEmpty) return;

            if (to.IsEmpty)
            {
                to = from;
                from = InventorySlot.Empty;
            }
            else if (to.Held == from.Held && to.Count < to.Held.MaxStackSize)
            {
                var moved = Mathf.Min(from.Count, to.Held.MaxStackSize - to.Count);
                to.Add(new InventorySlot(from.Held, moved));
                from.Remove(moved);
            }
            else
            {
                var destinationSlotBeforeSwap = to;
                to = from;
                from = destinationSlotBeforeSwap;
            }

            InventoryChanged?.Invoke();
        }

        public void Add(ItemContainer container)
        {
            for (var i = 0; i < container.Capacity; i++)
            {
                ref var slot = ref container.GetSlot(i);
                slot = Add(slot);
            }

            InventoryChanged?.Invoke();
        }

        public InventorySlot AddSlot(InventorySlot incoming)
        {
            var remainder = Add(incoming);
            InventoryChanged?.Invoke();
            return remainder;
        }

        private InventorySlot Add(InventorySlot incoming)
        {
            if (incoming.IsEmpty) return InventorySlot.Empty;

            for (var i = 0; i < TotalSlotCount; i++)
            {
                ref var slot = ref _items.GetSlot(i);
                if (slot.IsEmpty || !slot.CanAdd(incoming)) continue;

                var space = incoming.Held.MaxStackSize - slot.Count;
                var take = Mathf.Min(space, incoming.Count);
                slot.Add(new InventorySlot(incoming.Held, take));
                incoming.Remove(take);

                if (incoming.IsEmpty) return InventorySlot.Empty;
            }

            for (var i = 0; i < TotalSlotCount; i++)
            {
                ref var slot = ref _items.GetSlot(i);
                if (!slot.IsEmpty) continue;

                var take = Mathf.Min(incoming.Held.MaxStackSize, incoming.Count);
                slot.Add(new InventorySlot(incoming.Held, take));
                incoming.Remove(take);

                if (incoming.IsEmpty) return InventorySlot.Empty;
            }

            return incoming;
        }
    }
}
