using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class InterTimelineBridge : PlaceableGridEntity, ILogisticsTickable, IProducer, IConsumer, IInteractable, IUpgradeable
    {
        private InventorySlot _buffer;

        [SerializeField] private InterTimelineBridge linkedBridge;

        public UpgradeSlots Upgrades { get; private set; }

        public MachineModifiers Modifiers => Upgrades.ComputeModifiers();

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _buffer = InventorySlot.Empty;
            Upgrades = new UpgradeSlots(3);
        }

        public bool TryInstallUpgrade(UpgradeCardItem card) => Upgrades.TryInstall(card);

        public void OnLogisticsTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput() => _buffer;

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
