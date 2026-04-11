using GameItems;
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
        InventorySlot TryExtract(Item item, float maxAmount);
    }

    public interface ITransport : IProducer, IConsumer
    {
        InventorySlot PeekBuffer();
        float GetRemainingCapacity(Item item);
    }

    public interface IPlaceable
    {
        GameObject Prefab { get; }
    }

    public interface IFuel
    {
        float BurnTime { get; }
    }

    public interface IGenerator : IConsumptionTickable, IInteractable, IProducer, IConsumer { }
    public interface IMachine : IConsumptionTickable, IInteractable, IProducer, IConsumer { }
}