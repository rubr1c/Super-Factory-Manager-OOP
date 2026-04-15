using System.Collections.Generic;
using Core;
using Data.Items;
using Presentation.UI;
using Systems.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gameplay.Entities
{
    public abstract class UpgradableEntity : PlaceableGridEntity, IUpgradeable
    {
        private Label _upgradeSectionHeader;
        private readonly List<Label> _upgradeSlotLabels = new();
        private readonly List<Button> _upgradeRemoveButtons = new();

        public UpgradeSlots Upgrades { get; private set; }
        public MachineModifiers Modifiers => Upgrades != null ? Upgrades.ComputeModifiers() : MachineModifiers.Default;
        public MachineModifiers EffectiveModifiers => Modifiers.Scaled(GetTimelineSpeedModifier(), GetTimelineEnergyUsageModifier()).Clamped();

        public bool TryInstallUpgrade(UpgradeCardItem card) => TryInstallUpgrade(card, out _);

        public bool TryInstallUpgrade(UpgradeCardItem card, out int installedSlotIndex)
        {
            installedSlotIndex = -1;
            if (Upgrades == null)
            {
                return false;
            }

            return Upgrades.TryInstall(card, out installedSlotIndex);
        }

        public void BuildUpgradeSection(EntityInfoPanel panel)
        {
            if (Upgrades == null)
            {
                return;
            }

            _upgradeSlotLabels.Clear();
            _upgradeRemoveButtons.Clear();

            var section = new VisualElement();
            section.AddToClassList("upgrade-section");

            _upgradeSectionHeader = new Label();
            _upgradeSectionHeader.AddToClassList("upgrade-header");
            section.Add(_upgradeSectionHeader);

            for (var i = 0; i < Upgrades.Capacity; i++)
            {
                var row = new VisualElement();
                row.AddToClassList("upgrade-row");

                var nameLabel = new Label();
                nameLabel.AddToClassList("upgrade-slot-name");
                _upgradeSlotLabels.Add(nameLabel);
                row.Add(nameLabel);

                var slotIndex = i;
                var removeButton = new Button(() => RemoveUpgradeAt(slotIndex))
                {
                    text = "Remove"
                };
                removeButton.AddToClassList("panel-button");
                removeButton.AddToClassList("upgrade-remove-button");
                _upgradeRemoveButtons.Add(removeButton);
                row.Add(removeButton);

                section.Add(row);
            }

            panel.AddContentChild(section);
            RefreshUpgradeSection(panel);
        }

        public void RefreshUpgradeSection(EntityInfoPanel panel)
        {
            if (Upgrades == null || _upgradeSectionHeader == null)
            {
                return;
            }

            var installed = 0;
            for (var i = 0; i < Upgrades.Capacity; i++)
            {
                if (Upgrades.GetCard(i) != null)
                {
                    installed++;
                }
            }

            _upgradeSectionHeader.text = $"Upgrades ({installed}/{Upgrades.Capacity})";

            for (var i = 0; i < _upgradeSlotLabels.Count && i < Upgrades.Capacity; i++)
            {
                var card = Upgrades.GetCard(i);
                _upgradeSlotLabels[i].text = card != null ? card.DisplayName : "— Empty —";
                _upgradeRemoveButtons[i].SetEnabled(card != null);
            }
        }

        private void RemoveUpgradeAt(int slotIndex)
        {
            if (Upgrades == null)
            {
                return;
            }

            var removed = Upgrades.TryRemove(slotIndex);
            if (removed == null)
            {
                return;
            }

            var inventory = PlayerInventory.Instance;
            if (inventory != null)
            {
                var remainder = inventory.AddSlot(new InventorySlot(removed, 1f));
                if (!remainder.IsEmpty)
                {
                    Upgrades.TryInstall(removed);
                }
            }

            if (EntityInfoPanel.Instance != null && ReferenceEquals(EntityInfoPanel.Instance.CurrentEntity, this))
            {
                RefreshUpgradeSection(EntityInfoPanel.Instance);
            }
        }

        protected void InitUpgrades(int slotCount = 3)
        {
            Upgrades = new UpgradeSlots(slotCount);
        }

        protected float GetTimelineSpeedModifier()
        {
            return ParentTimeline != null ? ParentTimeline.SpeedModifier : 1f;
        }

        protected float GetTimelineEnergyUsageModifier()
        {
            return ParentTimeline != null ? ParentTimeline.EnergyUsageModifier : 1f;
        }
    }
}
