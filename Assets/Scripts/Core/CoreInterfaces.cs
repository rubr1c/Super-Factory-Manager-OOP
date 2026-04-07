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
        bool TryConsume(ItemData item, int amount);
    }

    public interface IProducer
    {
        ItemData ExtractOutput(float amount);
    }

    public interface IGenerator : ITickable, IInteractable, IProducer { }
    public interface IMachine : ITickable, IInteractable, IProducer, IConsumer { }
}