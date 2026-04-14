using Core;
using Data.Items;

namespace Gameplay.Entities
{
    public abstract class UpgradableEntity : PlaceableGridEntity, IUpgradeable
    {
        public UpgradeSlots Upgrades { get; private set; }
        public MachineModifiers Modifiers => Upgrades.ComputeModifiers();

        public bool TryInstallUpgrade(UpgradeCardItem card)
        {
            return Upgrades.TryInstall(card);
        }

        protected void InitUpgrades(int slotCount = 3)
        {
            Upgrades = new UpgradeSlots(slotCount);
        }
    }
}
