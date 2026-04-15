using Core;
using Data.Items;
using Gameplay.Entities;
using Systems.Inventory;
using Gameplay.World;
using Presentation.UI;
using UnityEngine;

namespace Gameplay.Machines.Extraction
{
    public class EnergyDrill : UpgradableEntity, IProductionTickable, IProducer, IConsumer
    {
        private InventorySlot _energyInput;
        private ItemContainer _output;

        [SerializeField] private Item[] drillOutputs;
        [SerializeField] private float energyCostPerItem = 2000f;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _energyInput = InventorySlot.Empty;
            _output = new ItemContainer(4);
            InitUpgrades();
        }

        public void OnProductionTick()
        {
            if (drillOutputs == null || drillOutputs.Length == 0)
            {
                return;
            }

            var requiredEnergy = Mathf.Max(0f, energyCostPerItem * EffectiveModifiers.Energy);
            if (requiredEnergy > 0f)
            {
                if (_energyInput.IsEmpty || _energyInput.Held != ItemCatalog.ENERGY || _energyInput.Count < requiredEnergy)
                {
                    return;
                }
            }

            var resource = drillOutputs[Random.Range(0, drillOutputs.Length)];
            if (resource == null)
            {
                return;
            }

            var yieldAmount = Mathf.Max(1f, EffectiveModifiers.Yield) * Mathf.Max(1f, EffectiveModifiers.Speed);
            if (!_output.TryAdd(new InventorySlot(resource, yieldAmount)))
            {
                return;
            }

            if (requiredEnergy > 0f)
            {
                _energyInput.Remove(requiredEnergy);
            }
        }

        public InventorySlot PeekOutput() => _output.GetFirst();

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _output.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            return _energyInput.TryInsert(slot, item => item == ItemCatalog.ENERGY);
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            panel.AddButton("Collect", CollectOutput);
        }

        private void CollectOutput()
        {
            var inventory = PlayerInventory.Instance;
            if (inventory == null) return;

            inventory.Add(_output);
        }
    }
}
