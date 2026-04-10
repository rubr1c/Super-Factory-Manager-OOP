using Item;

namespace Core
{
    public interface ITickable
    {
        void OnTick();
    }

    public interface IInteractable
    {
        void OnInteract();
    }

    public interface IConsumer
    {
        bool TryConsume(ItemData item, float amount);
    }

    public interface IProducer
    {
        float ExtractOutput(ItemData item, float maxAmount);
    }
    
    public interface ITransport
    {
        bool Push(ItemData item, float amount);
        bool Pull(ItemData item, float amount);
        
        float GetItemCount(ItemData item);
        float GetRemainingCapacity(ItemData item);
    }

    public interface IGenerator : ITickable, IInteractable, IProducer { }
    public interface IMachine : ITickable, IInteractable, IProducer, IConsumer { }
}