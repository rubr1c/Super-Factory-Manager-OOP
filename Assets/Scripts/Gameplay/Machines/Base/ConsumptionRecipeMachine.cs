using Core;

namespace Gameplay.Machines.Base
{
    public abstract class ConsumptionRecipeMachine : RecipeMachineBase, IConsumptionTickable
    {
        public void OnConsumptionTick()
        {
            RunRecipeTick();
        }
    }
}
