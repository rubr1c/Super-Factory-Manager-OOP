using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    public class StarterDrill : PlaceableGridEntity, IProductionTickable, IProducer, IInteractable
    {
        private InventorySlot _output;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _output = InventorySlot.Empty;
        }

        public void OnProductionTick()
        {
            throw new System.NotImplementedException();
        }

        public InventorySlot PeekOutput()
        {
            return _output;
        }

        public InventorySlot TryExtract(InventorySlot request)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteract()
        {
            throw new System.NotImplementedException();
        }
    }
}
