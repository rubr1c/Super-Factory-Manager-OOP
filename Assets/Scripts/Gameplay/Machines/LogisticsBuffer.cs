using Core;
using Data.Items;
using Gameplay.Entities;
using Systems.Inventory;
using Gameplay.World;
using UnityEngine;

namespace Gameplay.Machines
{
    public class LogisticsBuffer : UpgradableEntity, IProducer, IConsumer
    {
        private ItemContainer _storage;

        [SerializeField] private int capacity = 16;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _storage = new ItemContainer(capacity);
            InitUpgrades();
        }

        public InventorySlot PeekOutput() => _storage.GetFirst();

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _storage.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            return _storage.TryInsert(slot);
        }
    }
}
