using Core;
using GameItems;
using UnityEngine;

namespace Entity
{
    public enum PipeMode { None, Push, Pull, Neutral }

    public abstract class PipeEntity : PlaceableGridEntity, ITransport, ILogisticsTickable
    {
        protected InventorySlot Buffer;
        protected float MaxCapacity;

        public float TransferRate = 10f;

        public PipeMode[] Connections = new PipeMode[4];

        private readonly Vector2Int[] _directions =
        {
            new(0, 1),
            new(1, 0),
            new(0, -1),
            new(-1, 0)
        };

        public override void Place(
            Item item,
            Vector2Int pos,
            float slotSize,
            Timeline timeline)
        {
            Place(item, pos, slotSize, timeline, 1000f);
        }

        public void Place(
            Item item,
            Vector2Int pos,
            float slotSize,
            Timeline timeline,
            float maxCapacity)
        {
            base.Place(item, pos, slotSize, timeline);
            MaxCapacity = maxCapacity;
            Buffer = InventorySlots.Empty;

            for (var index = 0; index < Connections.Length; index++)
            {
                Connections[index] = PipeMode.None;
            }
        }

        public virtual bool CanHoldItem(Item item)
        {
            return item != null;
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            if (InventorySlots.IsEmpty(slot) || !CanHoldItem(slot.Definition) || !InventorySlots.CanAdd(Buffer, slot.Definition))
            {
                return slot;
            }

            var spaceAvailable = GetRemainingCapacity(slot.Definition);
            if (spaceAvailable <= 0f)
            {
                return slot;
            }

            var movedAmount = Mathf.Min(spaceAvailable, slot.Count);
            Buffer = InventorySlots.Add(Buffer, slot.Definition, movedAmount);
            return InventorySlots.Remove(slot, movedAmount);
        }

        public InventorySlot PeekOutput()
        {
            return Buffer;
        }

        public InventorySlot PeekBuffer()
        {
            return Buffer;
        }

        public InventorySlot TryExtract(Item item, float maxAmount)
        {
            if (InventorySlots.IsEmpty(Buffer) || Buffer.Definition != item || maxAmount <= 0f)
            {
                return InventorySlots.Empty;
            }

            var extractedAmount = Mathf.Min(Buffer.Count, maxAmount);
            Buffer = InventorySlots.Remove(Buffer, extractedAmount);
            return new InventorySlot(item, extractedAmount);
        }

        public float GetRemainingCapacity(Item item)
        {
            if (!CanHoldItem(item))
            {
                return 0f;
            }

            if (InventorySlots.IsEmpty(Buffer) || Buffer.Definition == item)
            {
                return MaxCapacity - Buffer.Count;
            }

            return 0f;
        }

        public void OnLogisticsTick()
        {
            for (var index = 0; index < Connections.Length; index++)
            {
                var connection = Connections[index];
                if (connection == PipeMode.None)
                {
                    continue;
                }

                var neighborPos = GridPos + _directions[index];
                var neighbor = ParentTimeline.EntityAt(neighborPos);
                if (!neighbor)
                {
                    continue;
                }

                if (connection == PipeMode.Pull && neighbor is IProducer producer)
                {
                    PullFromProducer(producer);
                    continue;
                }

                if (InventorySlots.IsEmpty(Buffer))
                {
                    continue;
                }

                if (connection == PipeMode.Push && neighbor is IConsumer consumer)
                {
                    TransferTo(consumer, Mathf.Min(Buffer.Count, TransferRate));
                    continue;
                }

                if (connection == PipeMode.Neutral && neighbor is ITransport transport)
                {
                    BalanceWith(transport);
                }
            }
        }

        private void PullFromProducer(IProducer producer)
        {
            var availableOutput = producer.PeekOutput();
            if (InventorySlots.IsEmpty(availableOutput) || !CanHoldItem(availableOutput.Definition))
            {
                return;
            }

            var amountToExtract = Mathf.Min(GetRemainingCapacity(availableOutput.Definition), TransferRate);
            if (amountToExtract <= 0f)
            {
                return;
            }

            var extractedSlot = producer.TryExtract(availableOutput.Definition, amountToExtract);
            if (!InventorySlots.IsEmpty(extractedSlot))
            {
                TryInsert(extractedSlot);
            }
        }

        private void BalanceWith(ITransport transport)
        {
            var neighborBuffer = transport.PeekBuffer();
            if (!InventorySlots.IsEmpty(neighborBuffer) && neighborBuffer.Definition != Buffer.Definition)
            {
                return;
            }

            if (Buffer.Count <= neighborBuffer.Count)
            {
                return;
            }

            var difference = Buffer.Count - neighborBuffer.Count;
            var transferAmountNeeded = difference / 2f;
            var spaceAvailable = transport.GetRemainingCapacity(Buffer.Definition);
            var amountToTransfer = Mathf.Min(Mathf.Min(TransferRate, transferAmountNeeded), spaceAvailable);

            TransferTo(transport, amountToTransfer);
        }

        private void TransferTo(IConsumer consumer, float amountToMove)
        {
            if (InventorySlots.IsEmpty(Buffer) || amountToMove <= 0f)
            {
                return;
            }

            var outgoingSlot = new InventorySlot(Buffer.Definition, amountToMove);
            var remainder = consumer.TryInsert(outgoingSlot);
            var movedAmount = outgoingSlot.Count - remainder.Count;
            if (movedAmount > 0f)
            {
                Buffer = InventorySlots.Remove(Buffer, movedAmount);
            }
        }
    }
}