using Core;
using GameItems;
using Inventory;
using UI;
using UnityEngine;

namespace Entity.Machine
{
    public class LiquidExtractor : UpgradableEntity, IProductionTickable, IProducer, IConsumer
    {
        private InventorySlot _energyInput;
        private InventorySlot _fluidOutput;

        [SerializeField] private float energyCostPerCycle = 250f;
        [SerializeField] private float waterPerCycle = 50f;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _energyInput = InventorySlot.Empty;
            _fluidOutput = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnProductionTick()
        {
            var requiredEnergy = Mathf.Max(0f, energyCostPerCycle * Modifiers.Energy);
            if (_energyInput.IsEmpty || _energyInput.Held != Items.ENERGY || _energyInput.Count < requiredEnergy)
            {
                return;
            }

            var producedAmount = Mathf.Max(1f, waterPerCycle * Modifiers.Yield);
            _energyInput.Remove(requiredEnergy);
            _fluidOutput.Add(new InventorySlot(Items.WATER, producedAmount));
        }

        public InventorySlot PeekOutput() => _fluidOutput;

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _fluidOutput.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            return _energyInput.TryInsert(slot, item => item == Items.ENERGY);
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            panel.AddLiveLabel("stored-water", $"Stored Water: {_fluidOutput.Count:0.##}");
            panel.AddButton("Collect", CollectOutput);
        }

        public override void RefreshInfoPanel(EntityInfoPanel panel)
        {
            panel.SetLiveLabelText("stored-water", $"Stored Water: {_fluidOutput.Count:0.##}");
        }

        private void CollectOutput()
        {
            var inventory = PlayerInventory.Instance;
            if (inventory == null)
            {
                return;
            }

            _fluidOutput = inventory.AddSlot(_fluidOutput);
        }
    }
}
