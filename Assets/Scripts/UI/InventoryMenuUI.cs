using Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public class InventoryMenuUI : MonoBehaviour
    {
        private VisualElement _inventoryOverlay;
        private VisualElement _inventoryPanel;
        private VisualElement _menuSlot;
        private VisualElement[] _slotRoots;
        private VisualElement[] _slotIcons;
        private Label[] _slotCounts;
        private PlayerInventory _inventory;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _inventoryOverlay = root.Q<VisualElement>("inventory-overlay");
            _inventoryPanel = root.Q<VisualElement>("inventory-panel");
            _menuSlot = root.Q<VisualElement>("slot-menu");

            SetInventoryVisible(false);
            BuildSlots(root);

            _menuSlot.RegisterCallback<ClickEvent>(evt =>
            {
                evt.StopPropagation();
                SetInventoryVisible(_inventoryOverlay.style.display == DisplayStyle.None);
            });

            _inventoryOverlay.RegisterCallback<ClickEvent>(evt =>
            {
                evt.StopPropagation();
                SetInventoryVisible(false);
            });

            _inventoryPanel.RegisterCallback<ClickEvent>(evt => evt.StopPropagation());
        }

        private void Start()
        {
            _inventory = PlayerInventory.Instance;
            if (_inventory == null) return;

            _inventory.InventoryChanged += RefreshSlots;
            RefreshSlots();
        }

        private void OnDestroy()
        {
            if (_inventory != null)
            {
                _inventory.InventoryChanged -= RefreshSlots;
            }
        }

        private void SetInventoryVisible(bool visible)
        {
            _inventoryOverlay.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void BuildSlots(VisualElement root)
        {
            _slotRoots = new VisualElement[PlayerInventory.TotalSlotCount];
            _slotIcons = new VisualElement[PlayerInventory.TotalSlotCount];
            _slotCounts = new Label[PlayerInventory.TotalSlotCount];

            for (var i = 0; i < PlayerInventory.HotbarSlotCount; i++)
            {
                AddSlot(root.Q<VisualElement>($"slot-{i}"), i, true);
            }

            for (var i = 0; i < PlayerInventory.InventorySlotCount; i++)
            {
                AddSlot(root.Q<VisualElement>($"inventory-slot-{i}"), PlayerInventory.HotbarSlotCount + i, false);
            }
        }

        private void AddSlot(VisualElement slotRoot, int index, bool isHotbar)
        {
            _slotRoots[index] = slotRoot;

            var icon = new VisualElement();
            icon.AddToClassList("slot-icon");
            slotRoot.Add(icon);
            _slotIcons[index] = icon;

            var count = new Label();
            count.AddToClassList("slot-count");
            slotRoot.Add(count);
            _slotCounts[index] = count;

            if (!isHotbar)
            {
                return;
            }

            slotRoot.RegisterCallback<ClickEvent>(_ =>
            {
                _inventory.ToggleHotbarSlotSelection(index);
            });
        }

        private void RefreshSlots()
        {
            for (var i = 0; i < _slotRoots.Length; i++)
            {
                var slot = _inventory.GetSlot(i);
                var hasItem = !slot.IsEmpty;

                _slotIcons[i].style.display = hasItem ? DisplayStyle.Flex : DisplayStyle.None;
                _slotCounts[i].style.display = hasItem ? DisplayStyle.Flex : DisplayStyle.None;
                _slotIcons[i].style.backgroundImage = hasItem && slot.Held.Icon
                    ? new StyleBackground(slot.Held.Icon)
                    : StyleKeyword.None;
                _slotCounts[i].text = hasItem ? ((int)slot.Count).ToString() : string.Empty;
                _slotRoots[i].EnableInClassList("selected-slot", i == _inventory.SelectedHotbarSlotIndex);
            }
        }
    }
}
