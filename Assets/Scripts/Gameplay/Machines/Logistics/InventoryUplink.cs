using Core;
using Data.Items;
using Gameplay.Entities;
using Systems.Inventory;
using Gameplay.World;
using UnityEngine;

namespace Gameplay.Machines.Logistics
{
    public class InventoryUplink : PlaceableGridEntity, ILogisticsTickable, IConsumer
    {
        private InventorySlot _incomingBuffer;

        [SerializeField] private float pullAmountPerTick = 25f;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _incomingBuffer = InventorySlot.Empty;
        }

        public void OnLogisticsTick()
        {
            var inventory = PlayerInventory.Instance;
            if (inventory == null || ParentTimeline == null)
            {
                return;
            }

            if (!_incomingBuffer.IsEmpty)
            {
                _incomingBuffer = inventory.AddSlot(_incomingBuffer);
            }

            if (!_incomingBuffer.IsEmpty)
            {
                return;
            }

            for (var index = 0; index < GridDirections.Cardinal.Length; index++)
            {
                var neighbor = ParentTimeline.EntityAt(GridPos + GridDirections.Cardinal[index]);
                if (neighbor is not IProducer producer)
                {
                    continue;
                }

                var availableOutput = producer.PeekOutput();
                if (availableOutput.IsEmpty)
                {
                    continue;
                }

                var amountToPull = Mathf.Min(availableOutput.Count, pullAmountPerTick);
                var extracted = producer.TryExtract(new InventorySlot(availableOutput.Held, amountToPull));
                if (extracted.IsEmpty)
                {
                    continue;
                }

                _incomingBuffer = inventory.AddSlot(extracted);
                if (!_incomingBuffer.IsEmpty)
                {
                    return;
                }
            }
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            return _incomingBuffer.TryInsert(slot);
        }
    }
}
