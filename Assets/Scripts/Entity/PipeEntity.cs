using Core;
using GameItems;
using UnityEngine;
using Inventory;

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
            Buffer = InventorySlot.Empty;

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
            if (slot.IsEmpty || !CanHoldItem(slot.Held) || !Buffer.CanAdd(slot))
            {
                return slot;
            }

            var spaceAvailable = GetRemainingCapacity(slot);
            if (spaceAvailable <= 0f)
            {
                return slot;
            }

            var movedAmount = Mathf.Min(spaceAvailable, slot.Count);
            Buffer.Add(new InventorySlot(slot.Held, movedAmount));
            var remainder = slot;
            remainder.Remove(movedAmount);
            return remainder;
        }

        public InventorySlot PeekOutput()
        {
            return Buffer;
        }

        public InventorySlot PeekBuffer()
        {
            return Buffer;
        }

        public InventorySlot TryExtract(InventorySlot request)
        {
            if (request.IsEmpty || Buffer.IsEmpty || Buffer.Held != request.Held)
            {
                return InventorySlot.Empty;
            }

            var extractedAmount = Mathf.Min(Buffer.Count, request.Count);
            if (extractedAmount <= 0f)
            {
                return InventorySlot.Empty;
            }

            Buffer.Remove(extractedAmount);
            return new InventorySlot(request.Held, extractedAmount);
        }

        public float GetRemainingCapacity(InventorySlot typeSlot)
        {
            if (typeSlot.IsEmpty || !CanHoldItem(typeSlot.Held))
            {
                return 0f;
            }

            if (Buffer.IsEmpty || Buffer.Held == typeSlot.Held)
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

                if (Buffer.IsEmpty)
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
            if (availableOutput.IsEmpty || !CanHoldItem(availableOutput.Held))
            {
                return;
            }

            var amountToExtract = Mathf.Min(GetRemainingCapacity(availableOutput), TransferRate);
            if (amountToExtract <= 0f)
            {
                return;
            }

            var extractedSlot = producer.TryExtract(new InventorySlot(availableOutput.Held, amountToExtract));
            if (!extractedSlot.IsEmpty)
            {
                TryInsert(extractedSlot);
            }
        }

        private void BalanceWith(ITransport transport)
        {
            var neighborBuffer = transport.PeekBuffer();
            if (!neighborBuffer.IsEmpty && neighborBuffer.Held != Buffer.Held)
            {
                return;
            }

            if (Buffer.Count <= neighborBuffer.Count)
            {
                return;
            }

            var difference = Buffer.Count - neighborBuffer.Count;
            var transferAmountNeeded = difference / 2f;
            var spaceAvailable = transport.GetRemainingCapacity(Buffer);
            var amountToTransfer = Mathf.Min(Mathf.Min(TransferRate, transferAmountNeeded), spaceAvailable);

            TransferTo(transport, amountToTransfer);
        }

        private void TransferTo(IConsumer consumer, float amountToMove)
        {
            if (Buffer.IsEmpty || amountToMove <= 0f)
            {
                return;
            }

            var outgoingSlot = new InventorySlot(Buffer.Held, amountToMove);
            var remainder = consumer.TryInsert(outgoingSlot);
            var movedAmount = outgoingSlot.Count - remainder.Count;
            if (movedAmount > 0f)
            {
                Buffer.Remove(movedAmount);
            }
        }
    }
}