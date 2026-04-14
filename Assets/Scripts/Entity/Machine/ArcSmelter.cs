using Core;
using GameItems;
using Inventory;
using UI;
using UnityEngine;

namespace Entity.Machine
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
            if (_energyInput.IsEmpty || _energyInput.Held != Items.ENERGY || _energyInput.Count < requiredEnergy)
            {
                return;
            }

            if (_metalInput.IsEmpty || _metalInput.Held != Items.IRON_PLATE)
            {
                return;
            }

            if (!_alloyOutput.IsEmpty && _alloyOutput.Held != Items.STEEL_INGOT)
            {
                return;
            }

            _energyInput.Remove(requiredEnergy);
            _metalInput.Remove(1f);
            _alloyOutput.Add(new InventorySlot(Items.STEEL_INGOT, Mathf.Max(1f, Modifiers.Yield)));
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

            if (slot.Held == Items.ENERGY)
            {
                return _energyInput.TryInsert(slot, item => item == Items.ENERGY);
            }

            if (slot.Held == Items.IRON_PLATE)
            {
                return _metalInput.TryInsert(slot, item => item == Items.IRON_PLATE);
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
