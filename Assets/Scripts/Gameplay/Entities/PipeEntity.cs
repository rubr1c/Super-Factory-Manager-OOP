using Core;
using Data.Items;
using Gameplay.Machines.Logistics;
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

        public InventorySlot TryInsert(InventorySlot slot)
        {
            if (slot.IsEmpty || !CanHoldItem(slot.Held) || !Buffer.CanAdd(slot)) return slot;

            var spaceAvailable = GetRemainingCapacity(slot);
            if (spaceAvailable <= 0f) return slot;

            var movedAmount = WholeUnits(Mathf.Min(spaceAvailable, slot.Count));
            if (movedAmount <= 0f) return slot;

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
            if (request.IsEmpty || Buffer.IsEmpty || Buffer.Held != request.Held) return InventorySlot.Empty;

            var extractedAmount = WholeUnits(Mathf.Min(Buffer.Count, request.Count));

            if (extractedAmount <= 0f) return InventorySlot.Empty;

            Buffer.Remove(extractedAmount);
            return new InventorySlot(request.Held, extractedAmount);
        }

        public float GetRemainingCapacity(InventorySlot typeSlot)
        {
            if (typeSlot.IsEmpty || !CanHoldItem(typeSlot.Held)) return 0f;

            if (Buffer.IsEmpty || Buffer.Held == typeSlot.Held) return MaxCapacity - Buffer.Count;

            return 0f;
        }

        public void OnLogisticsTick()
        {
            for (var index = 0; index < Connections.Length; index++)
            {
                if (Connections[index] != PipeMode.Pull) continue;

                var neighborPos = GridPos + GridDirections.Cardinal[index];
                var neighbor = ParentTimeline.EntityAt(neighborPos);
                if (!neighbor) continue;

                if (neighbor is IProducer producer) PullFromProducer(producer);
            }

            for (var index = 0; index < Connections.Length; index++)
            {
                if (Connections[index] != PipeMode.Push || Buffer.IsEmpty) continue;

                var neighborPos = GridPos + GridDirections.Cardinal[index];
                var neighbor = ParentTimeline.EntityAt(neighborPos);
                if (!neighbor || neighbor is not IConsumer consumer) continue;

                TransferTo(consumer, Mathf.Min(Buffer.Count, TransferRate));
            }

            for (var index = 0; index < Connections.Length; index++)
            {
                if (Connections[index] != PipeMode.Neutral || Buffer.IsEmpty) continue;

                var neighborPos = GridPos + GridDirections.Cardinal[index];
                var neighbor = ParentTimeline.EntityAt(neighborPos);
                if (!neighbor || neighbor is not ITransport transport) continue;

                BalanceWith(transport);
            }

            if (!Buffer.IsEmpty)
            {
                var whole = WholeUnits(Buffer.Count);
                if (whole <= 0f) Buffer = InventorySlot.Empty;
                else Buffer.Count = whole;
            }
        }

        protected virtual void PullFromProducer(IProducer producer)
        {
            if (producer is LogisticsBuffer logisticsBuffer)
            {
                var slotCount = logisticsBuffer.SlotCount;
                for (var slotIndex = 0; slotIndex < slotCount; slotIndex++)
                {
                    var slot = logisticsBuffer.PeekSlot(slotIndex);
                    if (slot.IsEmpty || !CanHoldItem(slot.Held)) continue;

                    var bufferedAmountToExtract = WholeUnits(Mathf.Min(GetRemainingCapacity(slot), TransferRate));
                    if (bufferedAmountToExtract <= 0f) continue;

                    var bufferedExtractedSlot = logisticsBuffer.TryExtract(new InventorySlot(slot.Held, bufferedAmountToExtract));
                    if (!bufferedExtractedSlot.IsEmpty)
                    {
                        TryInsert(bufferedExtractedSlot);
                        return;
                    }
                }

                return;
            }

            var availableOutput = producer.PeekOutput();
            if (availableOutput.IsEmpty || !CanHoldItem(availableOutput.Held)) return;

            var amountToExtract = WholeUnits(Mathf.Min(GetRemainingCapacity(availableOutput), TransferRate));
            if (amountToExtract <= 0f) return;

            var extractedSlot = producer.TryExtract(new InventorySlot(availableOutput.Held, amountToExtract));
            if (!extractedSlot.IsEmpty) TryInsert(extractedSlot);
        }

        private void ConfigureDefaultConnections()
        {
            for (var index = 0; index < Connections.Length; index++)
            {
                var neighborPos = GridPos + GridDirections.Cardinal[index];
                if (ParentTimeline.EntityAt(neighborPos) is not PipeEntity neighborPipe) continue;

                if (Connections[index] == PipeMode.None) Connections[index] = PipeMode.Neutral;

                var oppositeIndex = (index + 2) % 4;
                if (neighborPipe.Connections[oppositeIndex] == PipeMode.None) neighborPipe.Connections[oppositeIndex] = PipeMode.Neutral;
            }
        }

        private void BalanceWith(ITransport transport)
        {
            var neighborBuffer = transport.PeekBuffer();
            if (!neighborBuffer.IsEmpty && neighborBuffer.Held != Buffer.Held) return;

            var spaceAvailable = transport.GetRemainingCapacity(Buffer);

            var sourceWholeCount = Mathf.FloorToInt(Buffer.Count);
            var neighborWholeCount = Mathf.FloorToInt(neighborBuffer.Count);
            if (sourceWholeCount <= neighborWholeCount) return;

            var transferWhole = (sourceWholeCount - neighborWholeCount) / 2;
            if (transferWhole <= 0) return;

            var rateCap = Mathf.FloorToInt(TransferRate);
            var spaceCap = Mathf.FloorToInt(spaceAvailable);
            var amountToTransfer = Mathf.Min(Mathf.Min(rateCap, transferWhole), spaceCap);
            TransferTo(transport, amountToTransfer);
        }

        private void TransferTo(IConsumer consumer, float amountToMove)
        {
            amountToMove = WholeUnits(amountToMove);
            if (Buffer.IsEmpty || amountToMove <= 0f) return;

            var outgoingSlot = new InventorySlot(Buffer.Held, amountToMove);
            var remainder = consumer.TryInsert(outgoingSlot);
            var movedAmount = outgoingSlot.Count - remainder.Count;
            if (movedAmount > 0f) Buffer.Remove(movedAmount);
        }

        private static float WholeUnits(float amount)
        {
            return Mathf.Floor(Mathf.Max(0f, amount));
        }

        private string GetBufferLabelText()
        {
            if (Buffer.IsEmpty) return "Buffer: Empty";

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
            switch (currentMode)
            {
                case PipeMode.None:
                    return PipeMode.Push;
                case PipeMode.Push:
                    return PipeMode.Pull;
                case PipeMode.Pull:
                    return PipeMode.Neutral;
                default:
                    return PipeMode.None;
            }
        }
    }
}
