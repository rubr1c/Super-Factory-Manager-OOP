using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class LogisticsBuffer : UpgradableEntity, ILogisticsTickable, IProducer, IConsumer
    {
        private ItemContainer _storage;

        [SerializeField] private int capacity = 16;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _storage = new ItemContainer(capacity);
            InitUpgrades();
        }

        public void OnLogisticsTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _storage.GetFirst();

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
