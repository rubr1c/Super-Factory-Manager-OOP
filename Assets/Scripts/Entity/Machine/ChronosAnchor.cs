using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class ChronosAnchor : UpgradableEntity, IConsumptionTickable, IConsumer
    {
        private ItemContainer _inputs;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _inputs = new ItemContainer(6);
            InitUpgrades();
        }

        public void OnConsumptionTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            throw new System.NotImplementedException();
        }
    }
}
