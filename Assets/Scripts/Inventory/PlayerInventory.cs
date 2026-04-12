using System;
using GameItems;
using UnityEngine;

namespace Inventory
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

        [SerializeField] private StartingSlot[] startingSlots =
        {
            new()
            {
                slotIndex = 0,
                itemId = "starter_drill",
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
                if (!Items.TryGet(entry.itemId, out var item)) continue;

                ref var slot = ref _items.GetSlot(entry.slotIndex);
                slot = new InventorySlot(item, entry.count);
            }

            InventoryChanged?.Invoke();
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public InventorySlot GetSlot(int index)
        {
            if (index < 0 || index >= TotalSlotCount)
            {
                return InventorySlot.Empty;
            }

            return _items.GetSlot(index);
        }

        public void ToggleHotbarSlotSelection(int hotbarSlotIndex)
        {
            if (hotbarSlotIndex < 0 || hotbarSlotIndex >= HotbarSlotCount)
            {
                return;
            }

            if (SelectedHotbarSlotIndex == hotbarSlotIndex)
            {
                SelectedHotbarSlotIndex = -1;
            }
            else
            {
                SelectedHotbarSlotIndex = hotbarSlotIndex;
            }

            InventoryChanged?.Invoke();
        }

        public bool TryConsumeSelectedHotbarItem(int amount)
        {
            if (SelectedHotbarSlotIndex < 0)
            {
                return false;
            }

            ref var slot = ref _items.GetSlot(SelectedHotbarSlotIndex);
            if (slot.IsEmpty || slot.Count < amount)
            {
                return false;
            }

            slot.Remove(amount);
            InventoryChanged?.Invoke();
            return true;
        }
    }
}
