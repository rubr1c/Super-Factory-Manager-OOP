using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class PlateCompressor : UpgradableEntity, IConsumptionTickable, IProducer, IConsumer
    {
        private InventorySlot _oreInput;
        private InventorySlot _plateOutput;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _oreInput = InventorySlot.Empty;
            _plateOutput = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnConsumptionTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _plateOutput;

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
