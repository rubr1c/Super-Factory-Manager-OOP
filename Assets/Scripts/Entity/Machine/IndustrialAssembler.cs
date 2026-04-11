using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class IndustrialAssembler : PlaceableGridEntity, IConsumptionTickable, IProducer, IConsumer, IInteractable, IUpgradeable
    {
        private ItemContainer _inputs;
        private InventorySlot _output;

        public UpgradeSlots Upgrades { get; private set; }

        public MachineModifiers Modifiers => Upgrades.ComputeModifiers();

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _inputs = new ItemContainer(9);
            _output = InventorySlot.Empty;
            Upgrades = new UpgradeSlots(3);
        }

        public bool TryInstallUpgrade(UpgradeCardItem card) => Upgrades.TryInstall(card);

        public void OnConsumptionTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _output;

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
