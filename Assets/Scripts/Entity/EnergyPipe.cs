using Core;

namespace Entity
{
    public class EnergyPipe : PipeEntity
    {
        public override bool CanHoldItem(ItemData item)
        {
            return item.ItemName == "energy";
        }
    }
}