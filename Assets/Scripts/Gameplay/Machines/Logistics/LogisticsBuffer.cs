using Core;
using Data.Items;
using Gameplay.Entities;
using Gameplay.World;
using Presentation.UI;
using Systems.Inventory;
using UnityEngine.UIElements;
using UnityEngine;

namespace Gameplay.Machines.Logistics
{
    public class LogisticsBuffer : PlaceableGridEntity, IProducer, IConsumer
    {
        private const int BufferSlotColumns = 4;

        private ItemContainer _storage;

        [SerializeField] private int capacity = 16;

        private int _selectedBufferSlotIndex = -1;
        private VisualElement[] _bufferSlotRoots;
        private VisualElement[] _bufferSlotIcons;
        private Label[] _bufferSlotCounts;
        private Button _takeFromBufferButton;

        public int SlotCount => _storage != null ? _storage.Capacity : Mathf.Max(0, capacity);

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _storage = new ItemContainer(capacity);
        }

        public InventorySlot PeekOutput() => _storage.GetFirst();

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _storage.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            return _storage.TryInsert(slot);
        }

        public InventorySlot PeekSlot(int slotIndex)
        {
            if (_storage == null || slotIndex < 0 || slotIndex >= _storage.Capacity) return InventorySlot.Empty;

            return _storage.GetSlot(slotIndex);
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            _selectedBufferSlotIndex = -1;
            panel.AddLabel("Storage — click a slot, then Take.");

            var grid = new VisualElement();
            grid.AddToClassList("entity-buffer-grid");
            grid.AddToClassList("logistics-buffer-grid");

            _bufferSlotRoots = new VisualElement[capacity];
            _bufferSlotIcons = new VisualElement[capacity];
            _bufferSlotCounts = new Label[capacity];

            VisualElement row = null;
            for (var i = 0; i < capacity; i++)
            {
                if (i % BufferSlotColumns == 0)
                {
                    row = new VisualElement();
                    row.AddToClassList("entity-buffer-row");
                    row.AddToClassList("logistics-buffer-row");
                    grid.Add(row);
                }

                var slotRoot = new VisualElement();
                slotRoot.AddToClassList("entity-buffer-slot");
                var index = i;
                slotRoot.RegisterCallback<ClickEvent>(_ =>
                {
                    _selectedBufferSlotIndex = index;
                    RefreshBufferSlotSelectionStyles();
                });

                var icon = new VisualElement();
                icon.AddToClassList("slot-icon");
                slotRoot.Add(icon);

                var count = new Label();
                count.AddToClassList("slot-count");
                slotRoot.Add(count);

                row.Add(slotRoot);
                _bufferSlotRoots[i] = slotRoot;
                _bufferSlotIcons[i] = icon;
                _bufferSlotCounts[i] = count;
            }

            panel.AddContentChild(grid);

            _takeFromBufferButton = new Button(OnTakeFromBufferClicked) { text = "Take" };
            _takeFromBufferButton.AddToClassList("panel-button");
            panel.AddContentChild(_takeFromBufferButton);

            RefreshBufferPanel();
        }

        public override void RefreshInfoPanel(EntityInfoPanel panel)
        {
            RefreshBufferPanel();
        }

        private void RefreshBufferPanel()
        {
            if (_bufferSlotRoots == null || _storage == null) return;

            for (var i = 0; i < capacity; i++)
            {
                var slot = _storage.GetSlot(i);
                var hasItem = !slot.IsEmpty;

                _bufferSlotIcons[i].style.display = hasItem ? DisplayStyle.Flex : DisplayStyle.None;
                _bufferSlotCounts[i].style.display = hasItem ? DisplayStyle.Flex : DisplayStyle.None;

                if (hasItem && slot.Held.Icon != null) _bufferSlotIcons[i].style.backgroundImage = new StyleBackground(slot.Held.Icon);
                else _bufferSlotIcons[i].style.backgroundImage = StyleKeyword.None;

                _bufferSlotCounts[i].text = hasItem ? $"{slot.Count:0.##}" : string.Empty;
            }

            RefreshBufferSlotSelectionStyles();

            var canTake = _selectedBufferSlotIndex >= 0
                && _selectedBufferSlotIndex < capacity
                && !_storage.GetSlot(_selectedBufferSlotIndex).IsEmpty;
            if (_takeFromBufferButton != null) _takeFromBufferButton.SetEnabled(canTake);
        }

        private void RefreshBufferSlotSelectionStyles()
        {
            if (_bufferSlotRoots == null) return;

            for (var i = 0; i < _bufferSlotRoots.Length; i++)
            {
                _bufferSlotRoots[i].EnableInClassList("entity-buffer-slot--selected", i == _selectedBufferSlotIndex);
            }
        }

        private void OnTakeFromBufferClicked()
        {
            if (_selectedBufferSlotIndex < 0 || _selectedBufferSlotIndex >= capacity || _storage == null) return;

            ref var slot = ref _storage.GetSlot(_selectedBufferSlotIndex);
            if (slot.IsEmpty) return;

            var inventory = PlayerInventory.Instance;
            if (inventory == null) return;

            var remainder = inventory.AddSlot(slot);
            slot = remainder;
        }
    }
}
