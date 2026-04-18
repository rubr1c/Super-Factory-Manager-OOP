using Data.Items;
using Gameplay.Entities;

namespace Gameplay.Pipes
{
    public class FluidPipe : PipeEntity
    {
        private const string WaterRegistryName = "water";

        public override bool CanHoldItem(Item item)
        {
            return item.RegistryName == WaterRegistryName;
        }
    }
}
