using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class CircuitFabricator : UpgradableEntity, IConsumptionTickable, IProducer, IConsumer
    {
        private InventorySlot _waferInput;
        private InventorySlot _wireInput;
        private InventorySlot _boardOutput;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _waferInput = InventorySlot.Empty;
            _wireInput = InventorySlot.Empty;
            _boardOutput = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnConsumptionTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _boardOutput;

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
