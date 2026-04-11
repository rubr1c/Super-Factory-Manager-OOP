using Core;
using GameItems;
using UnityEngine;

namespace Entity.Machine
{
    public class EnergyGenerator : PlaceableGridEntity, IGenerator
    {
        private InventorySlot _fuelInput;
        private InventorySlot _energyOutput;

        [SerializeField] private float fallbackBurnTime = 10f;

        public override void Place(
            Item item,
            Vector2Int pos,
            float slotSize,
            Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _fuelInput = InventorySlots.Empty;
            _energyOutput = InventorySlots.Empty;
        }

        public void OnConsumptionTick()
        {
            if (InventorySlots.IsEmpty(_fuelInput) || !_fuelInput.Definition.Is(ItemTags.FUEL))
            {
                return;
            }

            var energyItem = Items.ENERGY;
            var burnTime = fallbackBurnTime;
            if (_fuelInput.Definition is IFuel fuel)
            {
                burnTime = fuel.BurnTime;
            }

            _fuelInput = InventorySlots.Remove(_fuelInput, 1f);
            _energyOutput = InventorySlots.Add(_energyOutput, energyItem, burnTime);
        }

        public InventorySlot PeekOutput()
        {
            return _energyOutput;
        }

        public InventorySlot TryExtract(Item item, float maxAmount)
        {
            if (InventorySlots.IsEmpty(_energyOutput) || _energyOutput.Definition != item || maxAmount <= 0f)
            {
                return InventorySlots.Empty;
            }

            var extractedAmount = Mathf.Min(_energyOutput.Count, maxAmount);
            _energyOutput = InventorySlots.Remove(_energyOutput, extractedAmount);
            return new InventorySlot(item, extractedAmount);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            if (InventorySlots.IsEmpty(slot) || !slot.Definition.Is(ItemTags.FUEL) || !InventorySlots.CanAdd(_fuelInput, slot.Definition))
            {
                return slot;
            }

            var maxCapacity = slot.Definition.MaxStackSize;
            var spaceAvailable = InventorySlots.IsEmpty(_fuelInput) || _fuelInput.Definition == slot.Definition
                ? maxCapacity - _fuelInput.Count
                : 0f;
            if (spaceAvailable <= 0f)
            {
                return slot;
            }

            var movedAmount = Mathf.Min(spaceAvailable, slot.Count);
            _fuelInput = InventorySlots.Add(_fuelInput, slot.Definition, movedAmount);
            return InventorySlots.Remove(slot, movedAmount);
        }

        public void OnInteract()
        {
            Debug.Log($"Fuel: {_fuelInput.Count}, Energy: {_energyOutput.Count}");
        }
    }
}