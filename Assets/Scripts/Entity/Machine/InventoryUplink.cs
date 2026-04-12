using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class InventoryUplink : UpgradableEntity, ILogisticsTickable, IConsumer
    {
        private InventorySlot _incomingBuffer;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _incomingBuffer = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnLogisticsTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            throw new System.NotImplementedException();
        }
    }
}
