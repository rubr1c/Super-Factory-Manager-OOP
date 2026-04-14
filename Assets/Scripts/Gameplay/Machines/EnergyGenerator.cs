using Core;
using Data.Items;
using Gameplay.Entities;
using Systems.Inventory;
using Gameplay.World;
using Presentation.UI;
using UnityEngine;

namespace Gameplay.Machines
{
    public class EnergyGenerator : UpgradableEntity, IProductionTickable, IProducer
    {
        private InventorySlot _energyOutput;
        [SerializeField] private float energyPerTick = 100.0f;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _energyOutput = InventorySlot.Empty;
            InitUpgrades();
        }

        public void OnProductionTick()
        {
            var generatedAmount = energyPerTick * Modifiers.Energy;
            if (generatedAmount <= 0f)
            {
                return;
            }

            if (_energyOutput.IsEmpty)
            {
                _energyOutput = new InventorySlot(ItemCatalog.ENERGY, 0f);
            }

            if (_energyOutput.Held != ItemCatalog.ENERGY)
            {
                return;
            }

            _energyOutput.Add(new InventorySlot(ItemCatalog.ENERGY, generatedAmount));
        }

        public InventorySlot PeekOutput() => _energyOutput;

        public InventorySlot TryExtract(InventorySlot request)
        {
            if (request.IsEmpty || _energyOutput.IsEmpty || _energyOutput.Held != request.Held)
            {
                return InventorySlot.Empty;
            }

            var extractedAmount = Mathf.Min(_energyOutput.Count, request.Count);
            if (extractedAmount <= 0f)
            {
                return InventorySlot.Empty;
            }

            _energyOutput.Remove(extractedAmount);
            return new InventorySlot(request.Held, extractedAmount);
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            panel.AddLiveLabel("stored-energy", $"Stored Energy: {_energyOutput.Count:0.##}");
        }

        public override void RefreshInfoPanel(EntityInfoPanel panel)
        {
            panel.SetLiveLabelText("stored-energy", $"Stored Energy: {_energyOutput.Count:0.##}");
        }
    }
}
