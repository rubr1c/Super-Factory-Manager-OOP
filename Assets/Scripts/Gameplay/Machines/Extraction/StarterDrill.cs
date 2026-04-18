using Core;
using Data.Items;
using Gameplay.Entities;
using Gameplay.World;
using Presentation.UI;
using Systems.Inventory;
using UnityEngine;

namespace Gameplay.Machines.Extraction
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
            if (drillOutputs == null || drillOutputs.Length == 0) return;

            var resource = drillOutputs[Random.Range(0, drillOutputs.Length)];
            if (resource == null) return;

            _output.TryAdd(new InventorySlot(resource, 1f));
        }

        public InventorySlot PeekOutput()
        {
            return _output.GetFirst();
        }

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _output.TryExtract(request);
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            panel.AddButton("Collect", CollectOutput);
        }

        private void CollectOutput()
        {
            var inventory = PlayerInventory.Instance;
            if (inventory == null) return;

            inventory.Add(_output);
        }
    }
}
