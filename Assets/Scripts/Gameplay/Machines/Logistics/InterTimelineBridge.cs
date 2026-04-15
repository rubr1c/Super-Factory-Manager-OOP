using Core;
using Data.Items;
using Gameplay.Entities;
using Systems.Inventory;
using Gameplay.World;
using UnityEngine;

namespace Gameplay.Machines.Logistics
{
    public class InterTimelineBridge : UpgradableEntity, ILogisticsTickable, IProducer, IConsumer
    {
        private InventorySlot _buffer;

        [SerializeField] private InterTimelineBridge linkedBridge;
        [SerializeField] private float transferRate = 50f;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _buffer = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnLogisticsTick()
        {
            if (linkedBridge == null || linkedBridge == this || _buffer.IsEmpty)
            {
                return;
            }

            var amountToTransfer = Mathf.Min(_buffer.Count, transferRate);
            var extracted = TryExtract(new InventorySlot(_buffer.Held, amountToTransfer));
            if (extracted.IsEmpty)
            {
                return;
            }

            var remainder = linkedBridge.TryInsert(extracted);
            if (!remainder.IsEmpty)
            {
                TryInsert(remainder);
            }
        }

        public InventorySlot PeekOutput() => _buffer;

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _buffer.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            return _buffer.TryInsert(slot);
        }
    }
}
