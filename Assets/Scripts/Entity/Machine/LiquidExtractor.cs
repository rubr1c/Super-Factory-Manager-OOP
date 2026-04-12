using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class LiquidExtractor : UpgradableEntity, IProductionTickable, IProducer, IConsumer
    {
        private InventorySlot _energyInput;
        private InventorySlot _fluidOutput;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _energyInput = InventorySlot.Empty;
            _fluidOutput = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnProductionTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _fluidOutput;

        public InventorySlot TryExtract(InventorySlot request)
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            throw new System.NotImplementedException();
        }
    }
}
