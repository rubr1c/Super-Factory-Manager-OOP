using Core;
using Data.Items;
using Gameplay.Entities;

namespace Gameplay.Pipes
{
    public class EnergyPipe : PipeEntity
    {
        public override bool CanHoldItem(Item item)
        {
            return item is IEnergyItem;
        }
    }
}