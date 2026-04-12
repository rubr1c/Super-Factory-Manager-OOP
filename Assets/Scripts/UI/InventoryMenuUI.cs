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

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _inventoryOverlay = root.Q<VisualElement>("inventory-overlay");
            _inventoryPanel = root.Q<VisualElement>("inventory-panel");
            _menuSlot = root.Q<VisualElement>("slot-menu");

            if (_inventoryOverlay == null || _inventoryPanel == null || _menuSlot == null)
            {
                Debug.LogError("[InventoryMenuUI] Missing required UI elements in Hotbar.uxml.");
                return;
            }

            SetInventoryVisible(false);

            _menuSlot.RegisterCallback<ClickEvent>(OnMenuSlotClicked);
            _inventoryOverlay.RegisterCallback<ClickEvent>(OnOverlayClicked);
            _inventoryPanel.RegisterCallback<ClickEvent>(OnPanelClicked);
        }

        private void OnDestroy()
        {
            if (_menuSlot != null)
            {
                _menuSlot.UnregisterCallback<ClickEvent>(OnMenuSlotClicked);
            }

            if (_inventoryOverlay != null)
            {
                _inventoryOverlay.UnregisterCallback<ClickEvent>(OnOverlayClicked);
            }

            if (_inventoryPanel != null)
            {
                _inventoryPanel.UnregisterCallback<ClickEvent>(OnPanelClicked);
            }
        }

        private void OnMenuSlotClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            SetInventoryVisible(_inventoryOverlay.style.display == DisplayStyle.None);
        }

        private void OnOverlayClicked(ClickEvent evt)
        {
            evt.StopPropagation();
            SetInventoryVisible(false);
        }

        private void OnPanelClicked(ClickEvent evt)
        {
            evt.StopPropagation();
        }

        private void SetInventoryVisible(bool visible)
        {
            if (_inventoryOverlay == null)
            {
                return;
            }

            _inventoryOverlay.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
