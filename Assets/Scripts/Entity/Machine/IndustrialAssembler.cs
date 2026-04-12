using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class IndustrialAssembler : UpgradableEntity, IConsumptionTickable, IProducer, IConsumer
    {
        private ItemContainer _inputs;
        private InventorySlot _output;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _inputs = new ItemContainer(9);
            _output = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnConsumptionTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _output;

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
