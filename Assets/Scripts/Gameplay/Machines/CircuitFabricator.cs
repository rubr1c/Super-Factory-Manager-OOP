using Core;
using Data.Items;
using Gameplay.Entities;
using Systems.Inventory;
using Gameplay.World;
using Presentation.UI;
using UnityEngine;

namespace Gameplay.Machines
{
    public class CircuitFabricator : UpgradableEntity, IConsumptionTickable, IProducer, IConsumer
    {
        private InventorySlot _waferInput;
        private InventorySlot _wireInput;
        private InventorySlot _boardOutput;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _waferInput = InventorySlot.Empty;
            _wireInput = InventorySlot.Empty;
            _boardOutput = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnConsumptionTick()
        {
            if (_waferInput.IsEmpty || _waferInput.Held != ItemCatalog.SILICON_WAFER || _waferInput.Count < 1f)
            {
                return;
            }

            if (_wireInput.IsEmpty || _wireInput.Held != ItemCatalog.COPPER_WIRE || _wireInput.Count < 2f)
            {
                return;
            }

            if (!_boardOutput.IsEmpty && _boardOutput.Held != ItemCatalog.CIRCUIT_BOARD)
            {
                return;
            }

            _waferInput.Remove(1f);
            _wireInput.Remove(2f);
            _boardOutput.Add(new InventorySlot(ItemCatalog.CIRCUIT_BOARD, Mathf.Max(1f, Modifiers.Yield)));
        }

        public InventorySlot PeekOutput() => _boardOutput;

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _boardOutput.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            if (slot.IsEmpty)
            {
                return InventorySlot.Empty;
            }

            if (slot.Held == ItemCatalog.SILICON_WAFER)
            {
                return _waferInput.TryInsert(slot, item => item == ItemCatalog.SILICON_WAFER);
            }

            if (slot.Held == ItemCatalog.COPPER_WIRE)
            {
                return _wireInput.TryInsert(slot, item => item == ItemCatalog.COPPER_WIRE);
            }

            return slot;
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            panel.AddButton("Collect", CollectOutput);
        }

        private void CollectOutput()
        {
            var inventory = PlayerInventory.Instance;
            if (inventory == null)
            {
                return;
            }

            _boardOutput = inventory.AddSlot(_boardOutput);
        }
    }
}
