using Core;
using Data.Items;
using Gameplay.Entities;
using Systems.Inventory;
using Gameplay.World;
using Presentation.UI;
using UnityEngine;

namespace Gameplay.Machines
{
    public class CoalBurner : UpgradableEntity, IProductionTickable, IProducer, IConsumer
    {
        private InventorySlot _fuelInput;
        private InventorySlot _energyOutput;

        [SerializeField] private float energyPerFuelUnit = 1000f;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _fuelInput = InventorySlot.Empty;
            _energyOutput = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnProductionTick()
        {
            if (_fuelInput.IsEmpty || _fuelInput.Held is not IFuel fuel)
            {
                return;
            }

            var producedEnergy = Mathf.Max(0f, fuel.BurnTime * energyPerFuelUnit * Modifiers.Yield);
            if (producedEnergy <= 0f)
            {
                return;
            }

            _fuelInput.Remove(1f);
            _energyOutput.Add(new InventorySlot(ItemCatalog.ENERGY, producedEnergy));
        }

        public InventorySlot PeekOutput() => _energyOutput;

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _energyOutput.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            return _fuelInput.TryInsert(slot, item => item is IFuel);
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

            _energyOutput = inventory.AddSlot(_energyOutput);
        }
    }
}
