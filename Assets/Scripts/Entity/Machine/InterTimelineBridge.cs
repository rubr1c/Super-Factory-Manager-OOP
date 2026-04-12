using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class InterTimelineBridge : UpgradableEntity, ILogisticsTickable, IProducer, IConsumer
    {
        private InventorySlot _buffer;

        [SerializeField] private InterTimelineBridge linkedBridge;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _buffer = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnLogisticsTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _buffer;

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
