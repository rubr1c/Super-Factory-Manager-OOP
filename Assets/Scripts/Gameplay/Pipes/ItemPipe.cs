using System.Collections.Generic;
using Core;
using Data.Items;
using Gameplay.Entities;
using Gameplay.Machines.Extraction;
using Presentation.UI;
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

        public override bool CanHoldItem(Item item)
        {
            if (item == null || item.RegistryName == WaterRegistryName || item is IEnergyItem) return false;

            return useWhitelist
                ? filteredItem != null && item == filteredItem
                : filteredItem == null || item != filteredItem;
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            base.BuildInfoPanel(panel);

            panel.AddLiveLabel(ModeLabelKey, useWhitelist ? "Mode: Whitelist" : "Mode: Blacklist");
            panel.AddLiveLabel(ItemLabelKey, $"Filter Item: {(filteredItem != null ? filteredItem.DisplayName : "None")}");
            panel.AddButton("Toggle Whitelist/Blacklist", ToggleFilterMode);
            panel.AddButton("Cycle Filter Item", CycleFilterItem);
        }

        public override void RefreshInfoPanel(EntityInfoPanel panel)
        {
            base.RefreshInfoPanel(panel);
            panel.SetLiveLabelText(ModeLabelKey, useWhitelist ? "Mode: Whitelist" : "Mode: Blacklist");
            panel.SetLiveLabelText(ItemLabelKey, $"Filter Item: {(filteredItem != null ? filteredItem.DisplayName : "None")}");
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
                if (!ItemCatalog.TryGet(DrillOutputItemIds[itemIdIndex], out var drillOutputItem) || drillOutputItem == null) continue;

                if (!CanHoldItem(drillOutputItem)) continue;

                var requestedAmount = Mathf.Min(TransferRate, GetRemainingCapacity(new InventorySlot(drillOutputItem, TransferRate)));
                if (requestedAmount <= 0f) continue;

                var wholeRequestedAmount = Mathf.Floor(requestedAmount);
                if (wholeRequestedAmount <= 0f) continue;

                var request = new InventorySlot(drillOutputItem, wholeRequestedAmount);
                var extractedSlot = producer.TryExtract(request);
                if (extractedSlot.IsEmpty) continue;

                TryInsert(extractedSlot);
                return;
            }
        }

        private void CycleFilterItem()
        {
            var drillOutputItems = new List<Item>();
            for (var itemIdIndex = 0; itemIdIndex < DrillOutputItemIds.Length; itemIdIndex++)
            {
                if (ItemCatalog.TryGet(DrillOutputItemIds[itemIdIndex], out var outputItem) && outputItem != null) drillOutputItems.Add(outputItem);
            }

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

            if (currentIndex == drillOutputItems.Count - 1) filteredItem = null;
            else filteredItem = drillOutputItems[currentIndex + 1];

            EntityInfoPanel.Instance?.Show(this);
        }
    }
}
