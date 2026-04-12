using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class ArcSmelter : UpgradableEntity, IConsumptionTickable, IProducer, IConsumer
    {
        private InventorySlot _energyInput;
        private InventorySlot _metalInput;
        private InventorySlot _alloyOutput;

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
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _alloyOutput;

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
