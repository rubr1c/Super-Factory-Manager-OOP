using GameItems;

namespace Entity
{
    public class EnergyPipe : PipeEntity
    {
        public override bool CanHoldItem(Item item)
        {
            return item != null && item.Is(ItemTags.ENERGY);
        }
    }
}