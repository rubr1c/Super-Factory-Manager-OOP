using Core;
using GameItems;
using UnityEngine;

namespace Entity.Machine
{
    public class WaterPump : PlaceableGridEntity, IMachine
    {
        private InventorySlot _energyInput;
        private InventorySlot _waterOutput;

        [SerializeField] private float waterPerTick = 10f;
        
        [SerializeField] private float energyNeededPerTick = 100f;


        public override void Place(
            Item item,
            Vector2Int pos,
            float slotSize,
            Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _waterOutput = InventorySlots.Empty;
            _energyInput = InventorySlots.Empty;
        }

        public void OnConsumptionTick()
        {
            if (InventorySlots.IsEmpty(_energyInput) || !_energyInput.Definition.Is(ItemTags.ENERGY))
            {
                return;
            }

            if (_energyInput.Count < energyNeededPerTick)
            {
                return;
            }

            var waterItem = Items.WATER;
            _energyInput = InventorySlots.Remove(_energyInput, energyNeededPerTick);
            _waterOutput = InventorySlots.Add(_waterOutput, waterItem, waterPerTick);
        }

        public void OnInteract()
        {
            Debug.Log($"Energy: {_energyInput.Count}, Water: {_waterOutput.Count}");
        }

        public InventorySlot PeekOutput()
        {
            return _waterOutput;
        }

        public InventorySlot TryExtract(Item item, float maxAmount)
        {
            if (InventorySlots.IsEmpty(_waterOutput) || _waterOutput.Definition != item || maxAmount <= 0f)
            {
                return InventorySlots.Empty;
            }

            var extractedAmount = Mathf.Min(_waterOutput.Count, maxAmount);
            _waterOutput = InventorySlots.Remove(_waterOutput, extractedAmount);
            return new InventorySlot(item, extractedAmount);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            if (InventorySlots.IsEmpty(slot) || !slot.Definition.Is(ItemTags.ENERGY) || !InventorySlots.CanAdd(_energyInput, slot.Definition))
            {
                return slot;
            }

            var maxCapacity = slot.Definition.MaxStackSize;
            var spaceAvailable = InventorySlots.IsEmpty(_energyInput) || _energyInput.Definition == slot.Definition
                ? maxCapacity - _energyInput.Count
                : 0f;
            if (spaceAvailable <= 0f)
            {
                return slot;
            }

            var movedAmount = Mathf.Min(spaceAvailable, slot.Count);
            _energyInput = InventorySlots.Add(_energyInput, slot.Definition, movedAmount);
            return InventorySlots.Remove(slot, movedAmount);
        }
    }
}