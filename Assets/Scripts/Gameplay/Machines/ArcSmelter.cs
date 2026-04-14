using Core;
using Data.Items;
using Gameplay.Entities;
using Systems.Inventory;
using Gameplay.World;
using Presentation.UI;
using UnityEngine;

namespace Gameplay.Machines
{
    public class ArcSmelter : UpgradableEntity, IConsumptionTickable, IProducer, IConsumer
    {
        private InventorySlot _energyInput;
        private InventorySlot _metalInput;
        private InventorySlot _alloyOutput;

        [SerializeField] private float energyCostPerCycle = 1000f;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _energyInput = InventorySlot.Empty;
            _metalInput = InventorySlot.Empty;
            _alloyOutput = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnConsumptionTick()
        {
            var requiredEnergy = Mathf.Max(0f, energyCostPerCycle * Modifiers.Energy);
            if (_energyInput.IsEmpty || _energyInput.Held != ItemCatalog.ENERGY || _energyInput.Count < requiredEnergy)
            {
                return;
            }

            if (_metalInput.IsEmpty || _metalInput.Held != ItemCatalog.IRON_PLATE)
            {
                return;
            }

            if (!_alloyOutput.IsEmpty && _alloyOutput.Held != ItemCatalog.STEEL_INGOT)
            {
                return;
            }

            _energyInput.Remove(requiredEnergy);
            _metalInput.Remove(1f);
            _alloyOutput.Add(new InventorySlot(ItemCatalog.STEEL_INGOT, Mathf.Max(1f, Modifiers.Yield)));
        }

        public InventorySlot PeekOutput() => _alloyOutput;

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _alloyOutput.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            if (slot.IsEmpty)
            {
                return InventorySlot.Empty;
            }

            if (slot.Held == ItemCatalog.ENERGY)
            {
                return _energyInput.TryInsert(slot, item => item == ItemCatalog.ENERGY);
            }

            if (slot.Held == ItemCatalog.IRON_PLATE)
            {
                return _metalInput.TryInsert(slot, item => item == ItemCatalog.IRON_PLATE);
            }

            return slot;
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            panel.AddButton("Collect", CollectOutput);
        }

        private void CollectOutput()
        {
            var inventory = PlayerInventory.Instance;
            if (inventory == null)
            {
                return;
            }

            _alloyOutput = inventory.AddSlot(_alloyOutput);
        }
    }
}
