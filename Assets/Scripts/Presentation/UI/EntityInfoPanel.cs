using System.Collections.Generic;
using System;
using Gameplay.Entities;
using Systems.Inventory;
using UnityEngine.UIElements;
using UnityEngine;

namespace Presentation.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class EntityInfoPanel : MonoBehaviour
    {
        public static EntityInfoPanel Instance { get; private set; }

        private VisualElement _panel;
        private Label _title;
        private VisualElement _content;
        private Button _deleteButton;
        private PlaceableGridEntity _currentEntity;
        private readonly Dictionary<string, Label> _liveLabels = new();

        public PlaceableGridEntity CurrentEntity => _currentEntity;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            var root = GetComponent<UIDocument>().rootVisualElement;
            _panel = root.Q<VisualElement>("entity-info-panel");
            _title = root.Q<Label>("panel-title");
            _content = root.Q<VisualElement>("panel-content");
            _deleteButton = root.Q<Button>("delete-button");
            _deleteButton.clicked += DeleteCurrentEntity;

            SetPanelVisible(false);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (_panel.style.display == DisplayStyle.None) return;

            if (!_currentEntity)
            {
                Hide();
                return;
            }

            _title.text = _currentEntity.Held ? _currentEntity.Held.DisplayName : "Unknown";
            _currentEntity.RefreshInfoPanel(this);
            if (_currentEntity is UpgradableEntity upgradableRefresh) upgradableRefresh.RefreshUpgradeSection(this);
        }

        public void Show(PlaceableGridEntity entity)
        {
            if (!entity) return;

            _currentEntity = entity;
            RefreshCurrentEntityInfo();
            SetPanelVisible(true);
        }

        public void Hide()
        {
            _currentEntity = null;
            SetPanelVisible(false);
        }

        public void AddButton(string text, Action onClick)
        {
            var button = new Button(() => onClick())
            {
                text = text
            };

            button.AddToClassList("panel-button");
            _content.Add(button);
        }

        public void AddLabel(string text)
        {
            var label = new Label(text);
            label.AddToClassList("panel-label");
            _content.Add(label);
        }

        public void AddLiveLabel(string key, string text)
        {
            if (!_liveLabels.TryGetValue(key, out var label))
            {
                label = new Label(text);
                label.AddToClassList("panel-label");
                _liveLabels[key] = label;
                _content.Add(label);
            }
            else label.text = text;
        }

        public void SetLiveLabelText(string key, string text)
        {
            if (_liveLabels.TryGetValue(key, out var label)) label.text = text;
        }

        public void AddContentChild(VisualElement child)
        {
            _content.Add(child);
        }

        private void SetPanelVisible(bool visible)
        {
            _panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }

        private void RefreshCurrentEntityInfo()
        {
            if (!_currentEntity) return;

            _title.text = _currentEntity.Held ? _currentEntity.Held.DisplayName : "Unknown";
            _content.Clear();
            _liveLabels.Clear();
            _currentEntity.BuildInfoPanel(this);
            if (_currentEntity is UpgradableEntity upgradable) upgradable.BuildUpgradeSection(this);
        }

        private void DeleteCurrentEntity()
        {
            if (!_currentEntity) return;

            if (_currentEntity.Held != null)
            {
                var inventory = PlayerInventory.Instance;
                if (inventory == null) return;
                if (!inventory.AddSlot(new InventorySlot(_currentEntity.Held, 1f)).IsEmpty) return;
            }

            _currentEntity.Remove();
            Hide();
        }
    }
}
