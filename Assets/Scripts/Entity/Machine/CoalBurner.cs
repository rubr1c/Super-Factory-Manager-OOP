using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class CoalBurner : PlaceableGridEntity, IProductionTickable, IProducer, IConsumer, IInteractable, IUpgradeable
    {
        private InventorySlot _fuelInput;
        private InventorySlot _energyOutput;

        public UpgradeSlots Upgrades { get; private set; }

        public MachineModifiers Modifiers => Upgrades.ComputeModifiers();

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _fuelInput = InventorySlot.Empty;
            _energyOutput = InventorySlot.Empty;
            Upgrades = new UpgradeSlots(3);
        }

        public bool TryInstallUpgrade(UpgradeCardItem card) => Upgrades.TryInstall(card);

        public void OnProductionTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _energyOutput;

        public InventorySlot TryExtract(InventorySlot request)
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteract()
        {
            throw new System.NotImplementedException();
        }
    }
}
