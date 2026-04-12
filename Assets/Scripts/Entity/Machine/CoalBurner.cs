using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class CoalBurner : UpgradableEntity, IProductionTickable, IProducer, IConsumer
    {
        private InventorySlot _fuelInput;
        private InventorySlot _energyOutput;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _fuelInput = InventorySlot.Empty;
            _energyOutput = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnProductionTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _energyOutput;

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
