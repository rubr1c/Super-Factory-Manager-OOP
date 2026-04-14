using Data.Items;
using Systems.Inventory;
using UnityEngine;

namespace Core
{
    public interface IProductionTickable
    {
        void OnProductionTick();
    }

    public interface ILogisticsTickable
    {
        void OnLogisticsTick();
    }

    public interface IConsumptionTickable
    {
        void OnConsumptionTick();
    }

    public interface IInteractable
    {
        void OnInteract();
    }

    public interface IConsumer
    {
        InventorySlot TryInsert(InventorySlot slot);
    }

    public interface IProducer
    {
        InventorySlot PeekOutput();
        InventorySlot TryExtract(InventorySlot request);
    }

    public interface ITransport : IProducer, IConsumer
    {
        InventorySlot PeekBuffer();
        float GetRemainingCapacity(InventorySlot typeSlot);
    }

    public interface IPlaceable
    {
        GameObject Prefab { get; }
    }

    public interface IUpgradeable
    {
        UpgradeSlots Upgrades { get; }
        MachineModifiers Modifiers { get; }
        bool TryInstallUpgrade(UpgradeCardItem card);
    }

    public interface IFuel
    {
        float BurnTime { get; }
    }

    public interface IEnergyItem
    {
    }

    public interface IResourceItem
    {
    }
}
