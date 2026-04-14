using Core;
using Data.Items;
using Gameplay.Entities;
using Systems.Inventory;
using Gameplay.World;
using Presentation.UI;
using UnityEngine;

namespace Gameplay.Machines
{
    public class PlateCompressor : UpgradableEntity, IConsumptionTickable, IProducer, IConsumer
    {
        private InventorySlot _oreInput;
        private InventorySlot _plateOutput;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _oreInput = InventorySlot.Empty;
            _plateOutput = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnConsumptionTick()
        {
            if (_oreInput.IsEmpty || !TryGetCompressedOutput(_oreInput.Held, out var outputItem))
            {
                return;
            }

            if (!_plateOutput.IsEmpty && _plateOutput.Held != outputItem)
            {
                return;
            }

            var producedAmount = Mathf.Max(1f, Modifiers.Yield);
            _oreInput.Remove(1f);
            _plateOutput.Add(new InventorySlot(outputItem, producedAmount));
        }

        public InventorySlot PeekOutput() => _plateOutput;

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _plateOutput.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            return _oreInput.TryInsert(slot, item => item == ItemCatalog.IRON_ORE || item == ItemCatalog.COPPER_ORE);
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

            _plateOutput = inventory.AddSlot(_plateOutput);
        }

        private static bool TryGetCompressedOutput(Item input, out Item output)
        {
            if (input == ItemCatalog.IRON_ORE)
            {
                output = ItemCatalog.IRON_PLATE;
                return true;
            }

            if (input == ItemCatalog.COPPER_ORE)
            {
                output = ItemCatalog.COPPER_PLATE;
                return true;
            }

            output = null;
            return false;
        }
    }
}
