using Core;
using GameItems;
using Inventory;
using UI;
using UnityEngine;

namespace Entity.Machine
{
    public class IndustrialAssembler : UpgradableEntity, IConsumptionTickable, IProducer, IConsumer
    {
        private ItemContainer _inputs;
        private InventorySlot _output;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _inputs = new ItemContainer(9);
            _output = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnConsumptionTick()
        {
            if (CountItem(Items.IRON_PLATE) < 2f
                || CountItem(Items.COPPER_PLATE) < 2f
                || CountItem(Items.CIRCUIT_BOARD) < 1f)
            {
                return;
            }

            if (!_output.IsEmpty && _output.Held != Items.MACHINE_CHASSIS)
            {
                return;
            }

            ConsumeItem(Items.IRON_PLATE, 2f);
            ConsumeItem(Items.COPPER_PLATE, 2f);
            ConsumeItem(Items.CIRCUIT_BOARD, 1f);
            _output.Add(new InventorySlot(Items.MACHINE_CHASSIS, Mathf.Max(1f, Modifiers.Yield)));
        }

        public InventorySlot PeekOutput() => _output;

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _output.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            return _inputs.TryInsert(slot);
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

            _output = inventory.AddSlot(_output);
        }

        private float CountItem(Item item)
        {
            var total = 0f;
            for (var index = 0; index < _inputs.Capacity; index++)
            {
                var slot = _inputs.GetSlot(index);
                if (slot.IsEmpty || slot.Held != item)
                {
                    continue;
                }

                total += slot.Count;
            }

            return total;
        }

        private void ConsumeItem(Item item, float amount)
        {
            var remaining = amount;
            for (var index = 0; index < _inputs.Capacity && remaining > 0f; index++)
            {
                ref var slot = ref _inputs.GetSlot(index);
                if (slot.IsEmpty || slot.Held != item)
                {
                    continue;
                }

                var take = Mathf.Min(slot.Count, remaining);
                slot.Remove(take);
                remaining -= take;
            }
        }
    }
}
