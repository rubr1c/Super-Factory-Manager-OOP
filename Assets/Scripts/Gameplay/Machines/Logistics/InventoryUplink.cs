using Core;
using Data.Items;
using Gameplay.Entities;
using Gameplay.World;
using Systems.Inventory;
using UnityEngine;

namespace Gameplay.Machines.Logistics
{
    public class InventoryUplink : PlaceableGridEntity, IConsumer
    {
        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            var inventory = PlayerInventory.Instance;
            if (inventory == null) return slot;

            return inventory.AddSlot(slot);
        }
    }
}
