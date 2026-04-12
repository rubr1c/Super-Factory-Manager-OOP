using Core;
using GameItems;
using Inventory;
using UnityEngine;

namespace Entity.Machine
{
    
    public class StarterDrill : PlaceableGridEntity, IProductionTickable, IProducer
    {
        private ItemContainer _output;

        [SerializeField] private Item[] drillOutputs;

        public override void Place(
            Item item, 
            Vector2Int pos, 
            float slotSize, 
            Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _output = new ItemContainer(4);
        }

        public void OnProductionTick()
        {
            if (drillOutputs == null || drillOutputs.Length == 0)
            {
                return;
            }

            var resource = drillOutputs[Random.Range(0, drillOutputs.Length)];
            if (resource == null)
            {
                return;
            }

            if (_output.TryAdd(new InventorySlot(resource, 1f)))
            {
                Debug.Log(
                    $"[StarterDrill] +1 {resource.DisplayName} ({resource.RegistryName}) @ {GridPos}");
            }
        }

        public InventorySlot PeekOutput()
        {
            return _output.GetFirst();
        }

        public InventorySlot TryExtract(InventorySlot request)
        {
            if (request.IsEmpty)
            {
                return InventorySlot.Empty;
            }

            for (var i = 0; i < _output.Capacity; i++)
            {
                ref var slot = ref _output.GetSlot(i);
                if (!slot.CanConsume() || slot.Held != request.Held)
                {
                    continue;
                }

                var extractedAmount = Mathf.Min(slot.Count, request.Count);
                if (extractedAmount <= 0f)
                {
                    return InventorySlot.Empty;
                }

                slot.Remove(extractedAmount);
                return new InventorySlot(request.Held, extractedAmount);
            }

            return InventorySlot.Empty;
        }
    }
}
