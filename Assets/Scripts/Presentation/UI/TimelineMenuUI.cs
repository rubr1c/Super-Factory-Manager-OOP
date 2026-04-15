using Application.Managers;
using Data.Items;
using Systems.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace Presentation.UI
{
    [RequireComponent(typeof(UIDocument))]
    public class TimelineMenuUI : MonoBehaviour
    {
        private VisualElement _menuPanel;
        private Button _menuButton;
        private Label _timelineIdLabel;
        private Label _speedModifierLabel;
        private Label _energyModifierLabel;
        private Label _fragmentsLabel;
        private Label _costLabel;
        private Button _previousTimelineButton;
        private Button _nextTimelineButton;
        private Button _buyTimelineButton;
        private Item _chronosFragment;
        private PlayerInventory _inventory;
        private TimelineManager _timelineManager;

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;

            _menuPanel = root.Q<VisualElement>("timeline-panel");
            _menuButton = root.Q<Button>("timeline-menu-button");
            _timelineIdLabel = root.Q<Label>("timeline-id-value");
            _speedModifierLabel = root.Q<Label>("timeline-speed-value");
            _energyModifierLabel = root.Q<Label>("timeline-energy-value");
            _fragmentsLabel = root.Q<Label>("timeline-fragments-value");
            _costLabel = root.Q<Label>("timeline-cost-value");
            _previousTimelineButton = root.Q<Button>("timeline-previous-button");
            _nextTimelineButton = root.Q<Button>("timeline-next-button");
            _buyTimelineButton = root.Q<Button>("timeline-buy-button");

            _menuButton.clicked += ToggleMenu;
            _previousTimelineButton.clicked += OnPreviousTimelineClicked;
            _nextTimelineButton.clicked += OnNextTimelineClicked;
            _buyTimelineButton.clicked += OnBuyTimelineClicked;

            SetMenuOpen(false);
        }

        private void Start()
        {
            _inventory = PlayerInventory.Instance;
            _timelineManager = TimelineManager.Instance;
            ItemCatalog.TryGet("chronos_fragment", out _chronosFragment);

            _inventory.InventoryChanged += RefreshView;
            _timelineManager.ActiveTimelineChanged += OnActiveTimelineChanged;
            _timelineManager.TimelinesChanged += RefreshView;

            RefreshView();
        }

        private void OnDestroy()
        {
            _menuButton.clicked -= ToggleMenu;
            _previousTimelineButton.clicked -= OnPreviousTimelineClicked;
            _nextTimelineButton.clicked -= OnNextTimelineClicked;
            _buyTimelineButton.clicked -= OnBuyTimelineClicked;

            if (_inventory != null) _inventory.InventoryChanged -= RefreshView;
            if (_timelineManager != null)
            {
                _timelineManager.ActiveTimelineChanged -= OnActiveTimelineChanged;
                _timelineManager.TimelinesChanged -= RefreshView;
            }
        }

        private void OnPreviousTimelineClicked()
        {
            var previousTimelineId = _timelineManager.ActiveTimeline.TimelineID - 1;
            _timelineManager.SwitchToTimeline(previousTimelineId);
        }

        private void OnNextTimelineClicked()
        {
            var nextTimelineId = _timelineManager.ActiveTimeline.TimelineID + 1;
            _timelineManager.SwitchToTimeline(nextTimelineId);
        }

        private void OnBuyTimelineClicked()
        {
            _timelineManager.TryBuyNewTimeline();
        }

        private void OnActiveTimelineChanged(Gameplay.World.Timeline _)
        {
            RefreshView();
        }

        private void ToggleMenu()
        {
            SetMenuOpen(_menuPanel.style.display == DisplayStyle.None);
        }

        private void RefreshView()
        {
            var activeTimeline = _timelineManager.ActiveTimeline;
            var fragmentCount = _chronosFragment != null ? _inventory.CountItem(_chronosFragment) : 0f;
            var timelineCost = _timelineManager.GetNewTimelineCost();
            var timelineCount = _timelineManager.AllTimelines.Count;
            var activeTimelineId = activeTimeline.TimelineID;

            _timelineIdLabel.text = activeTimelineId.ToString();
            _speedModifierLabel.text = $"{activeTimeline.SpeedModifier:0.##}x";
            _energyModifierLabel.text = $"{activeTimeline.EnergyUsageModifier:0.##}x";
            _fragmentsLabel.text = fragmentCount.ToString("0.##");
            _costLabel.text = timelineCost.ToString("0.##");

            _previousTimelineButton.SetEnabled(activeTimelineId > 0);
            _nextTimelineButton.SetEnabled(activeTimelineId < timelineCount - 1);
            _buyTimelineButton.SetEnabled(fragmentCount >= timelineCost);
        }

        private void SetMenuOpen(bool isOpen)
        {
            _menuPanel.style.display = isOpen ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
