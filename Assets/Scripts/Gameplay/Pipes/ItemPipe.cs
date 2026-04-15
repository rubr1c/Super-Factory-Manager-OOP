using Core;
using Data.Items;
using Gameplay.Entities;

namespace Gameplay.Pipes
{
    public class ItemPipe : PipeEntity
    {
        private const string WaterRegistryName = "water";

        protected override bool UseIntegerTransfers => true;

        public override bool CanHoldItem(Item item)
        {
            if (item.RegistryName == WaterRegistryName)
            {
                return false;
            }

            return item is not IEnergyItem;
        }
    }
}
