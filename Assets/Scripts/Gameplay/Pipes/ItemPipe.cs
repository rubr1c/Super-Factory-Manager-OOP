using Core;
using Data.Items;
using Gameplay.Entities;
using Gameplay.Machines.Extraction;
using Presentation.UI;
using System.Collections.Generic;
using Systems.Inventory;
using UnityEngine;

namespace Gameplay.Pipes
{
    public class ItemPipe : PipeEntity
    {
        private const string WaterRegistryName = "water";
        private const string ModeLabelKey = "item-pipe-filter-mode";
        private const string ItemLabelKey = "item-pipe-filter-item";

        private static readonly string[] DrillOutputItemIds =
        {
            "iron_ore",
            "copper_ore",
            "quartz",
            "coal"
        };

        [SerializeField] private bool useWhitelist;
        [SerializeField] private Item filteredItem;

        protected override bool UseIntegerTransfers => true;

        public override bool CanHoldItem(Item item)
        {
            if (item == null)
            {
                return false;
            }

            if (item.RegistryName == WaterRegistryName)
            {
                return false;
            }

            if (item is IEnergyItem)
            {
                return false;
            }

            if (useWhitelist)
            {
                return filteredItem != null && item == filteredItem;
            }

            return filteredItem == null || item != filteredItem;
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            base.BuildInfoPanel(panel);

            panel.AddLiveLabel(ModeLabelKey, GetModeLabelText());
            panel.AddLiveLabel(ItemLabelKey, GetFilterItemLabelText());
            panel.AddButton("Toggle Whitelist/Blacklist", ToggleFilterMode);
            panel.AddButton("Cycle Filter Item", CycleFilterItem);
        }

        public override void RefreshInfoPanel(EntityInfoPanel panel)
        {
            base.RefreshInfoPanel(panel);
            panel.SetLiveLabelText(ModeLabelKey, GetModeLabelText());
            panel.SetLiveLabelText(ItemLabelKey, GetFilterItemLabelText());
        }

        private string GetModeLabelText()
        {
            return useWhitelist ? "Mode: Whitelist" : "Mode: Blacklist";
        }

        private string GetFilterItemLabelText()
        {
            var itemName = filteredItem != null ? filteredItem.DisplayName : "None";
            return $"Filter Item: {itemName}";
        }

        private void ToggleFilterMode()
        {
            useWhitelist = !useWhitelist;
            EntityInfoPanel.Instance?.Show(this);
        }

        protected override void PullFromProducer(IProducer producer)
        {
            if (producer is not StarterDrill)
            {
                base.PullFromProducer(producer);
                return;
            }

            for (var itemIdIndex = 0; itemIdIndex < DrillOutputItemIds.Length; itemIdIndex++)
            {
                if (!ItemCatalog.TryGet(DrillOutputItemIds[itemIdIndex], out var drillOutputItem) || drillOutputItem == null)
                {
                    continue;
                }

                if (!CanHoldItem(drillOutputItem))
                {
                    continue;
                }

                var requestedAmount = Mathf.Min(TransferRate, GetRemainingCapacity(new InventorySlot(drillOutputItem, TransferRate)));
                if (requestedAmount <= 0f)
                {
                    continue;
                }

                var wholeRequestedAmount = Mathf.Floor(requestedAmount);
                if (wholeRequestedAmount <= 0f)
                {
                    wholeRequestedAmount = 1f;
                }

                var request = new InventorySlot(drillOutputItem, wholeRequestedAmount);
                var extractedSlot = producer.TryExtract(request);
                if (extractedSlot.IsEmpty)
                {
                    continue;
                }

                TryInsert(extractedSlot);
                return;
            }
        }

        private static List<Item> GetDrillOutputItems()
        {
            var drillOutputItems = new List<Item>();

            for (var itemIdIndex = 0; itemIdIndex < DrillOutputItemIds.Length; itemIdIndex++)
            {
                if (!ItemCatalog.TryGet(DrillOutputItemIds[itemIdIndex], out var outputItem) || outputItem == null)
                {
                    continue;
                }

                drillOutputItems.Add(outputItem);
            }

            return drillOutputItems;
        }

        private void CycleFilterItem()
        {
            var drillOutputItems = GetDrillOutputItems();
            if (drillOutputItems.Count == 0)
            {
                filteredItem = null;
                EntityInfoPanel.Instance?.Show(this);
                return;
            }

            var currentIndex = -1;
            for (var itemIndex = 0; itemIndex < drillOutputItems.Count; itemIndex++)
            {
                if (drillOutputItems[itemIndex] == filteredItem)
                {
                    currentIndex = itemIndex;
                    break;
                }
            }

            if (currentIndex == drillOutputItems.Count - 1)
            {
                filteredItem = null;
            }
            else
            {
                filteredItem = drillOutputItems[currentIndex + 1];
            }

            EntityInfoPanel.Instance?.Show(this);
        }
    }
}
