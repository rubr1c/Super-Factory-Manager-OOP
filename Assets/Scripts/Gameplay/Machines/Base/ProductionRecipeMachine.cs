using Core;

namespace Gameplay.Machines.Base
{
    public abstract class ProductionRecipeMachine : RecipeMachineBase, IProductionTickable
    {
        public void OnProductionTick()
        {
            RunRecipeTick();
        }
    }
}
