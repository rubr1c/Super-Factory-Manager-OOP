using Core;
using Data.Items;
using Gameplay.World;
using Presentation.UI;
using Systems.Inventory;
using UnityEngine;

namespace Gameplay.Entities
{
    public enum PipeMode { None, Push, Pull, Neutral }

    public abstract class PipeEntity : PlaceableGridEntity, ITransport, ILogisticsTickable
    {
        private const string BufferLabelKey = "pipe-buffer";

        private static readonly string[] DirectionNames =
        {
            "North",
            "East",
            "South",
            "West"
        };

        protected InventorySlot Buffer;
        protected float MaxCapacity;

        public float TransferRate = 10f;

        public PipeMode[] Connections = new PipeMode[4];

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

            ConfigureDefaultConnections();
        }

        public virtual bool CanHoldItem(Item item)
        {
            return item != null;
        }

        protected virtual bool UseIntegerTransfers => false;

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
            if (UseIntegerTransfers)
            {
                movedAmount = Mathf.Floor(movedAmount);
                if (movedAmount <= 0f)
                {
                    return slot;
                }
            }

            Buffer.Add(new InventorySlot(slot.Held, movedAmount));
            var remainder = slot;
            remainder.Remove(movedAmount);
            return remainder;
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            panel.AddLiveLabel(BufferLabelKey, GetBufferLabelText());

            for (var index = 0; index < Connections.Length; index++)
            {
                var connectionIndex = index;
                panel.AddButton(GetConnectionButtonText(connectionIndex), () => CycleConnectionMode(connectionIndex));
            }
        }

        public override void RefreshInfoPanel(EntityInfoPanel panel)
        {
            panel.SetLiveLabelText(BufferLabelKey, GetBufferLabelText());
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
            if (UseIntegerTransfers)
            {
                extractedAmount = Mathf.Floor(extractedAmount);
            }

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

                var neighborPos = GridPos + GridDirections.Cardinal[index];
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

            if (UseIntegerTransfers && !Buffer.IsEmpty)
            {
                var whole = Mathf.Floor(Buffer.Count);
                if (whole <= 0f)
                {
                    Buffer = InventorySlot.Empty;
                }
                else
                {
                    Buffer.Count = whole;
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
            amountToExtract = QuantizeOutboundAmount(amountToExtract);
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

        private void ConfigureDefaultConnections()
        {
            for (var index = 0; index < Connections.Length; index++)
            {
                var neighborPos = GridPos + GridDirections.Cardinal[index];
                if (ParentTimeline.EntityAt(neighborPos) is not PipeEntity neighborPipe)
                {
                    continue;
                }

                if (Connections[index] == PipeMode.None)
                {
                    Connections[index] = PipeMode.Neutral;
                }

                var oppositeIndex = (index + 2) % 4;
                if (neighborPipe.Connections[oppositeIndex] == PipeMode.None)
                {
                    neighborPipe.Connections[oppositeIndex] = PipeMode.Neutral;
                }
            }
        }

        private void BalanceWith(ITransport transport)
        {
            var neighborBuffer = transport.PeekBuffer();
            if (!neighborBuffer.IsEmpty && neighborBuffer.Held != Buffer.Held)
            {
                return;
            }

            float transferAmountNeeded;
            float spaceAvailable = transport.GetRemainingCapacity(Buffer);

            if (UseIntegerTransfers)
            {
                var a = Mathf.FloorToInt(Buffer.Count);
                var b = Mathf.FloorToInt(neighborBuffer.Count);
                if (a <= b)
                {
                    return;
                }

                var transferWhole = (a - b) / 2;
                if (transferWhole == 0 && b == 0)
                {
                    transferWhole = 1;
                }

                var rateCap = Mathf.FloorToInt(TransferRate);
                if (rateCap <= 0 && TransferRate > 0f)
                {
                    rateCap = 1;
                }

                var spaceCap = Mathf.FloorToInt(spaceAvailable);
                var amountToTransfer = Mathf.Min(Mathf.Min(rateCap, transferWhole), spaceCap);
                TransferTo(transport, amountToTransfer);
                return;
            }

            if (Buffer.Count <= neighborBuffer.Count)
            {
                return;
            }

            var difference = Buffer.Count - neighborBuffer.Count;
            transferAmountNeeded = difference / 2f;
            var amountToTransferFloat = Mathf.Min(Mathf.Min(TransferRate, transferAmountNeeded), spaceAvailable);
            TransferTo(transport, amountToTransferFloat);
        }

        private void TransferTo(IConsumer consumer, float amountToMove)
        {
            amountToMove = QuantizeOutboundAmount(amountToMove);
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

        private float QuantizeOutboundAmount(float amount)
        {
            if (!UseIntegerTransfers)
            {
                return amount;
            }

            var floored = Mathf.Floor(amount);
            if (floored <= 0f && amount > 0f)
            {
                return 1f;
            }

            return floored;
        }

        private string GetBufferLabelText()
        {
            if (Buffer.IsEmpty)
            {
                return "Buffer: Empty";
            }

            return $"Buffer: {Buffer.Count:0.##} {Buffer.Held.DisplayName}";
        }

        private string GetConnectionButtonText(int connectionIndex)
        {
            return $"{DirectionNames[connectionIndex]}: {Connections[connectionIndex]}";
        }

        private void CycleConnectionMode(int connectionIndex)
        {
            Connections[connectionIndex] = GetNextMode(Connections[connectionIndex]);
            EntityInfoPanel.Instance?.Show(this);
        }

        private static PipeMode GetNextMode(PipeMode currentMode)
        {
            return currentMode switch
            {
                PipeMode.None => PipeMode.Push,
                PipeMode.Push => PipeMode.Pull,
                PipeMode.Pull => PipeMode.Neutral,
                _ => PipeMode.None
            };
        }
    }
}