using System.Collections.Generic;
using Application.Managers;
using Core;
using Data.Items;
using Data.Recipes;
using Gameplay.Entities;
using Gameplay.World;
using Presentation.UI;
using Systems.Inventory;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gameplay.Machines.Base
{
    public abstract class RecipeMachineBase : UpgradableEntity, IProducer, IConsumer
    {
        private const int MaxUiSummaryEntries = 2;
        private const int InputSlotColumns = 4;

        [SerializeField] private MachineType machineType;
        [SerializeField] private Recipe[] allowedRecipes = new Recipe[0];
        [SerializeField] private Recipe defaultRecipe;
        [SerializeField] private int itemInputCapacity = 8;
        [SerializeField] private int outputCapacity = 4;

        private InventorySlot _energyInput;
        private InventorySlot _waterInput;
        private ItemContainer _itemInputs;
        private ItemContainer _outputBuffer;
        private Recipe _activeRecipe;
        private float _progressSeconds;
        private bool _usesItemInputs;
        private bool _usesEnergy;
        private bool _usesWater;
        private VisualElement[] _itemInputSlotIcons;
        private Label[] _itemInputSlotCounts;

        public override void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            InitUpgrades();
            InitializeRecipeState();
        }

        public InventorySlot PeekOutput() => _outputBuffer.GetFirst();

        public InventorySlot TryExtract(InventorySlot request)
        {
            return _outputBuffer.TryExtract(request);
        }

        public InventorySlot TryInsert(InventorySlot slot)
        {
            if (slot.IsEmpty)
            {
                return InventorySlot.Empty;
            }

            if (slot.Held == ItemCatalog.ENERGY && _usesEnergy)
            {
                return _energyInput.TryInsert(slot, item => item == ItemCatalog.ENERGY);
            }

            if (slot.Held == ItemCatalog.WATER && _usesWater)
            {
                return _waterInput.TryInsert(slot, item => item == ItemCatalog.WATER);
            }

            return AcceptsItem(slot.Held) ? _itemInputs.TryInsert(slot) : slot;
        }

        public override void BuildInfoPanel(EntityInfoPanel panel)
        {
            if (_usesItemInputs)
            {
                BuildItemInputGrid(panel);
            }

            panel.AddLiveLabel("stored-output", $"Output: {GetContainerSummary(_outputBuffer)}");

            if (_usesEnergy || _usesWater)
            {
                panel.AddLiveLabel("stored-utilities", GetUtilitiesSummary());
            }

            panel.AddButton("Collect", CollectOutput);
            if (_usesItemInputs)
            {
                RefreshItemInputGridVisuals();
            }
        }

        public override void RefreshInfoPanel(EntityInfoPanel panel)
        {
            panel.SetLiveLabelText("stored-output", $"Output: {GetContainerSummary(_outputBuffer)}");

            if (_usesEnergy || _usesWater)
            {
                panel.SetLiveLabelText("stored-utilities", GetUtilitiesSummary());
            }

            if (_usesItemInputs)
            {
                RefreshItemInputGridVisuals();
            }
        }

        private void BuildItemInputGrid(EntityInfoPanel panel)
        {
            var slotCount = _itemInputs != null ? _itemInputs.Capacity : Mathf.Max(1, itemInputCapacity);
            _itemInputSlotIcons = new VisualElement[slotCount];
            _itemInputSlotCounts = new Label[slotCount];

            var grid = new VisualElement();
            grid.AddToClassList("entity-buffer-grid");
            grid.AddToClassList("recipe-machine-input-grid");

            VisualElement row = null;
            for (var index = 0; index < slotCount; index++)
            {
                if (index % InputSlotColumns == 0)
                {
                    row = new VisualElement();
                    row.AddToClassList("entity-buffer-row");
                    grid.Add(row);
                }

                var slotRoot = new VisualElement();
                slotRoot.AddToClassList("entity-buffer-slot");

                var slotIndex = index;
                slotRoot.RegisterCallback<ClickEvent>(_ => OnItemInputSlotClicked(slotIndex));

                var icon = new VisualElement();
                icon.AddToClassList("slot-icon");
                slotRoot.Add(icon);

                var count = new Label();
                count.AddToClassList("slot-count");
                slotRoot.Add(count);

                row.Add(slotRoot);
                _itemInputSlotIcons[index] = icon;
                _itemInputSlotCounts[index] = count;
            }

            panel.AddContentChild(grid);
        }

        private void OnItemInputSlotClicked(int slotIndex)
        {
            var inventory = PlayerInventory.Instance;
            if (inventory == null || _itemInputs == null)
            {
                return;
            }

            if (slotIndex < 0 || slotIndex >= _itemInputs.Capacity)
            {
                return;
            }

            var selectedHotbarSlotIndex = inventory.SelectedHotbarSlotIndex;
            if (selectedHotbarSlotIndex >= 0 && selectedHotbarSlotIndex < PlayerInventory.HotbarSlotCount)
            {
                var selectedHotbarSlot = inventory.SelectedHotbarSlot;
                if (!selectedHotbarSlot.IsEmpty)
                {
                    InsertFromSelectedHotbar(inventory, selectedHotbarSlot);
                    RefreshItemInputGridVisuals();
                    return;
                }
            }

            ExtractToPlayerInventory(slotIndex, inventory);
            RefreshItemInputGridVisuals();
        }

        private void InsertFromSelectedHotbar(PlayerInventory inventory, InventorySlot selectedHotbarSlot)
        {
            if (selectedHotbarSlot.IsEmpty || !AcceptsItem(selectedHotbarSlot.Held))
            {
                return;
            }

            var wholeItemCount = Mathf.FloorToInt(selectedHotbarSlot.Count);
            if (wholeItemCount <= 0)
            {
                return;
            }

            for (var index = 0; index < wholeItemCount; index++)
            {
                if (!inventory.TryConsumeSelectedHotbarItem(1))
                {
                    return;
                }

                var remainder = _itemInputs.TryInsert(new InventorySlot(selectedHotbarSlot.Held, 1f));
                if (!remainder.IsEmpty)
                {
                    inventory.AddSlot(remainder);
                    return;
                }
            }
        }

        private void ExtractToPlayerInventory(int slotIndex, PlayerInventory inventory)
        {
            ref var machineSlot = ref _itemInputs.GetSlot(slotIndex);
            if (machineSlot.IsEmpty)
            {
                return;
            }

            machineSlot = inventory.AddSlot(machineSlot);
        }

        private void RefreshItemInputGridVisuals()
        {
            if (_itemInputs == null || _itemInputSlotIcons == null || _itemInputSlotCounts == null)
            {
                return;
            }

            var slotCount = Mathf.Min(_itemInputs.Capacity, _itemInputSlotIcons.Length);
            for (var index = 0; index < slotCount; index++)
            {
                var slot = _itemInputs.GetSlot(index);
                var hasItem = !slot.IsEmpty;

                _itemInputSlotIcons[index].style.display = hasItem ? DisplayStyle.Flex : DisplayStyle.None;
                _itemInputSlotCounts[index].style.display = hasItem ? DisplayStyle.Flex : DisplayStyle.None;
                if (hasItem && slot.Held.Icon != null)
                {
                    _itemInputSlotIcons[index].style.backgroundImage = new StyleBackground(slot.Held.Icon);
                }
                else
                {
                    _itemInputSlotIcons[index].style.backgroundImage = StyleKeyword.None;
                }

                _itemInputSlotCounts[index].text = hasItem ? $"{slot.Count:0.##}" : string.Empty;
            }
        }

        protected void RunRecipeTick()
        {
            var recipe = FindCraftableRecipe();
            if (recipe == null)
            {
                _activeRecipe ??= GetPreferredRecipe();
                return;
            }

            if (_activeRecipe != recipe)
            {
                _activeRecipe = recipe;
                _progressSeconds = 0f;
            }

            _progressSeconds += TickManager.Instance != null ? TickManager.Instance.TickRateSeconds : 1f;
            if (_progressSeconds < GetCraftDuration(recipe))
            {
                return;
            }

            ConsumeRecipe(recipe);
            ProduceRecipe(recipe);
            _progressSeconds = 0f;
        }

        private void InitializeRecipeState()
        {
            if (allowedRecipes == null)
            {
                allowedRecipes = new Recipe[0];
            }

            _energyInput = InventorySlot.Empty;
            _waterInput = InventorySlot.Empty;
            _itemInputs = new ItemContainer(Mathf.Max(1, itemInputCapacity));
            _outputBuffer = new ItemContainer(Mathf.Max(1, outputCapacity));

            _usesItemInputs = false;
            _usesEnergy = false;
            _usesWater = false;
            for (var index = 0; index < allowedRecipes.Length; index++)
            {
                var recipe = allowedRecipes[index];
                if (recipe == null || recipe.MachineType != machineType)
                {
                    continue;
                }

                if (recipe.EnergyCost > 0f)
                {
                    _usesEnergy = true;
                }

                if (recipe.WaterCost > 0f)
                {
                    _usesWater = true;
                }

                if (!_usesItemInputs)
                {
                    var inputs = recipe.ItemInputs;
                    for (var inputIndex = 0; inputIndex < inputs.Length; inputIndex++)
                    {
                        if (inputs[inputIndex].IsValid)
                        {
                            _usesItemInputs = true;
                            break;
                        }
                    }
                }
            }

            _activeRecipe = GetPreferredRecipe();
            _progressSeconds = 0f;
        }

        private void CollectOutput()
        {
            var inventory = PlayerInventory.Instance;
            if (inventory == null)
            {
                return;
            }

            inventory.Add(_outputBuffer);
        }

        private Recipe GetPreferredRecipe()
        {
            if (IsAllowedRecipe(defaultRecipe))
            {
                return defaultRecipe;
            }

            for (var index = 0; index < allowedRecipes.Length; index++)
            {
                if (IsAllowedRecipe(allowedRecipes[index]))
                {
                    return allowedRecipes[index];
                }
            }

            return null;
        }

        private Recipe FindCraftableRecipe()
        {
            Recipe bestRecipe = null;
            var bestInputTypeCount = -1;
            var bestInputTotalAmount = -1f;

            for (var index = 0; index < allowedRecipes.Length; index++)
            {
                var recipe = allowedRecipes[index];
                if (!CanCraft(recipe))
                {
                    continue;
                }

                GetRecipeInputMetrics(recipe, out var inputTypeCount, out var inputTotalAmount);
                var isBetter = inputTypeCount > bestInputTypeCount
                    || (inputTypeCount == bestInputTypeCount && inputTotalAmount > bestInputTotalAmount)
                    || (bestRecipe != null
                        && inputTypeCount == bestInputTypeCount
                        && Mathf.Approximately(inputTotalAmount, bestInputTotalAmount)
                        && recipe == _activeRecipe);

                if (isBetter)
                {
                    bestRecipe = recipe;
                    bestInputTypeCount = inputTypeCount;
                    bestInputTotalAmount = inputTotalAmount;
                }
            }

            return bestRecipe;
        }

        private static void GetRecipeInputMetrics(Recipe recipe, out int inputTypeCount, out float inputTotalAmount)
        {
            inputTypeCount = 0;
            inputTotalAmount = 0f;
            if (recipe == null)
            {
                return;
            }

            var inputs = recipe.ItemInputs;
            for (var index = 0; index < inputs.Length; index++)
            {
                if (!inputs[index].IsValid)
                {
                    continue;
                }

                inputTypeCount++;
                inputTotalAmount += inputs[index].Amount;
            }
        }

        private bool CanCraft(Recipe recipe)
        {
            if (!IsAllowedRecipe(recipe))
            {
                return false;
            }

            if (_energyInput.Count < GetEffectiveEnergyCost(recipe) || _waterInput.Count < recipe.WaterCost)
            {
                return false;
            }

            var inputs = recipe.ItemInputs;
            for (var index = 0; index < inputs.Length; index++)
            {
                var input = inputs[index];
                if (!input.IsValid || CountStoredItem(input.Item) < input.Amount)
                {
                    return false;
                }
            }

            return HasOutputSpace(recipe);
        }

        private bool IsAllowedRecipe(Recipe recipe)
        {
            if (recipe == null || recipe.MachineType != machineType)
            {
                return false;
            }

            for (var index = 0; index < allowedRecipes.Length; index++)
            {
                if (allowedRecipes[index] == recipe)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HasOutputSpace(Recipe recipe)
        {
            var simulated = new InventorySlot[_outputBuffer.Capacity];
            for (var index = 0; index < _outputBuffer.Capacity; index++)
            {
                simulated[index] = _outputBuffer.GetSlot(index);
            }

            var yieldMultiplier = Mathf.Max(1f, EffectiveModifiers.Yield);
            var outputs = recipe.ItemOutputs;
            for (var index = 0; index < outputs.Length; index++)
            {
                var output = outputs[index];
                if (!output.IsValid)
                {
                    return false;
                }

                var remaining = new InventorySlot(output.Item, output.Amount * yieldMultiplier);
                for (var slotIndex = 0; slotIndex < simulated.Length; slotIndex++)
                {
                    remaining = simulated[slotIndex].TryInsert(remaining);
                    if (remaining.IsEmpty)
                    {
                        break;
                    }
                }

                if (!remaining.IsEmpty)
                {
                    return false;
                }
            }

            return true;
        }

        private bool AcceptsItem(Item item)
        {
            if (item == null)
            {
                return false;
            }

            for (var recipeIndex = 0; recipeIndex < allowedRecipes.Length; recipeIndex++)
            {
                var recipe = allowedRecipes[recipeIndex];
                if (recipe == null || recipe.MachineType != machineType)
                {
                    continue;
                }

                var inputs = recipe.ItemInputs;
                for (var inputIndex = 0; inputIndex < inputs.Length; inputIndex++)
                {
                    if (inputs[inputIndex].Item == item)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private float CountStoredItem(Item item)
        {
            var total = 0f;
            for (var index = 0; index < _itemInputs.Capacity; index++)
            {
                var slot = _itemInputs.GetSlot(index);
                if (!slot.IsEmpty && slot.Held == item)
                {
                    total += slot.Count;
                }
            }

            return total;
        }

        private void ConsumeRecipe(Recipe recipe)
        {
            _energyInput.Remove(GetEffectiveEnergyCost(recipe));
            _waterInput.Remove(recipe.WaterCost);

            var inputs = recipe.ItemInputs;
            for (var index = 0; index < inputs.Length; index++)
            {
                ConsumeStoredItem(inputs[index].Item, inputs[index].Amount);
            }
        }

        private void ConsumeStoredItem(Item item, float amount)
        {
            var remaining = amount;
            for (var index = 0; index < _itemInputs.Capacity && remaining > 0f; index++)
            {
                ref var slot = ref _itemInputs.GetSlot(index);
                if (slot.IsEmpty || slot.Held != item)
                {
                    continue;
                }

                var take = Mathf.Min(slot.Count, remaining);
                slot.Remove(take);
                remaining -= take;
            }
        }

        private void ProduceRecipe(Recipe recipe)
        {
            var yieldMultiplier = Mathf.Max(1f, EffectiveModifiers.Yield);
            var outputs = recipe.ItemOutputs;
            for (var index = 0; index < outputs.Length; index++)
            {
                var output = outputs[index];
                if (output.IsValid)
                {
                    _outputBuffer.TryInsert(new InventorySlot(output.Item, output.Amount * yieldMultiplier));
                }
            }
        }

        private float GetCraftDuration(Recipe recipe)
        {
            return recipe.ProcessTimeSeconds / Mathf.Max(0.01f, EffectiveModifiers.Speed);
        }

        private float GetEffectiveEnergyCost(Recipe recipe)
        {
            return recipe.EnergyCost * Mathf.Max(0f, EffectiveModifiers.Energy);
        }

        private static string GetSlotSummary(InventorySlot slot)
        {
            if (slot.IsEmpty)
            {
                return "Empty";
            }

            return $"{slot.Held.DisplayName} x{slot.Count:0.##}";
        }

        private string GetUtilitiesSummary()
        {
            if (_usesEnergy && _usesWater)
            {
                return $"Energy: {GetSlotSummary(_energyInput)} | Water: {GetSlotSummary(_waterInput)}";
            }

            if (_usesEnergy)
            {
                return $"Energy: {GetSlotSummary(_energyInput)}";
            }

            if (_usesWater)
            {
                return $"Water: {GetSlotSummary(_waterInput)}";
            }

            return string.Empty;
        }

        private static string GetContainerSummary(ItemContainer container)
        {
            var parts = new List<string>();
            var hiddenCount = 0;

            for (var index = 0; index < container.Capacity; index++)
            {
                var slot = container.GetSlot(index);
                if (slot.IsEmpty)
                {
                    continue;
                }

                if (parts.Count < MaxUiSummaryEntries)
                {
                    parts.Add($"{slot.Held.DisplayName} x{slot.Count:0.##}");
                    continue;
                }

                hiddenCount++;
            }

            if (parts.Count == 0)
            {
                return "Empty";
            }

            var summary = string.Join(", ", parts);
            if (hiddenCount > 0)
            {
                summary += $", +{hiddenCount} more";
            }

            return summary;
        }
    }
}
