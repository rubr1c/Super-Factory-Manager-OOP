using Core;
using GameItems;

namespace Entity
{
    public abstract class UpgradableEntity : PlaceableGridEntity, IUpgradeable
    {
        public UpgradeSlots Upgrades { get; private set; }

        public MachineModifiers Modifiers => Upgrades.ComputeModifiers();

        public bool TryInstallUpgrade(UpgradeCardItem card) => Upgrades.TryInstall(card);

        protected void InitUpgrades(int slotCount = 3)
        {
            Upgrades = new UpgradeSlots(slotCount);
        }
    }
}
