using System;
using Application.Managers;
using Core;
using Data.Items;
using Data.Recipes;
using Gameplay.Entities;
using Gameplay.World;
using Presentation.UI;
using Systems.Inventory;
using UnityEngine;

namespace Gameplay.Machines.Base
{
    public abstract class RecipeMachineBase : UpgradableEntity, IProducer, IConsumer
    {
        [SerializeField] private MachineType machineType;
        [SerializeField] private Recipe[] allowedRecipes = Array.Empty<Recipe>();
        [SerializeField] private Recipe defaultRecipe;
        [SerializeField] private int itemInputCapacity = 8;
        [SerializeField] private int outputCapacity = 4;

        private InventorySlot _energyInput;
        private InventorySlot _waterInput;
        private ItemContainer _itemInputs;
        private ItemContainer _outputBuffer;
        private Recipe _activeRecipe;
        private float _progressSeconds;
        private bool _usesEnergy;
        private bool _usesWater;

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
            panel.AddLiveLabel("active-recipe", $"Recipe: {ActiveRecipeUiName}");
            panel.AddLiveLabel("recipe-progress", $"Progress: {ProgressUiText}");
            panel.AddButton("Collect", CollectOutput);
        }

        public override void RefreshInfoPanel(EntityInfoPanel panel)
        {
            panel.SetLiveLabelText("active-recipe", $"Recipe: {ActiveRecipeUiName}");
            panel.SetLiveLabelText("recipe-progress", $"Progress: {ProgressUiText}");
        }

        protected void RunRecipeTick()
        {
            var recipe = FindCraftableRecipe();
            if (recipe == null)
            {
                _activeRecipe = GetPreferredRecipe();
                _progressSeconds = 0f;
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

        private string ActiveRecipeUiName => _activeRecipe != null ? _activeRecipe.DisplayName : "Idle";

        private string ProgressUiText
        {
            get
            {
                if (_activeRecipe == null)
                {
                    return "0%";
                }

                var duration = GetCraftDuration(_activeRecipe);
                var ratio = duration <= 0f ? 1f : Mathf.Clamp01(_progressSeconds / duration);
                return $"{ratio * 100f:0}%";
            }
        }

        private void InitializeRecipeState()
        {
            allowedRecipes ??= Array.Empty<Recipe>();
            _energyInput = InventorySlot.Empty;
            _waterInput = InventorySlot.Empty;
            _itemInputs = new ItemContainer(Mathf.Max(1, itemInputCapacity));
            _outputBuffer = new ItemContainer(Mathf.Max(1, outputCapacity));

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
            if (CanCraft(_activeRecipe))
            {
                return _activeRecipe;
            }

            for (var index = 0; index < allowedRecipes.Length; index++)
            {
                var recipe = allowedRecipes[index];
                if (CanCraft(recipe))
                {
                    return recipe;
                }
            }

            return null;
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

            var outputs = recipe.ItemOutputs;
            for (var index = 0; index < outputs.Length; index++)
            {
                var output = outputs[index];
                if (!output.IsValid)
                {
                    return false;
                }

                var remaining = new InventorySlot(output.Item, output.Amount);
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
            var outputs = recipe.ItemOutputs;
            for (var index = 0; index < outputs.Length; index++)
            {
                var output = outputs[index];
                if (output.IsValid)
                {
                    _outputBuffer.TryInsert(new InventorySlot(output.Item, output.Amount));
                }
            }
        }

        private float GetCraftDuration(Recipe recipe)
        {
            return recipe.ProcessTimeSeconds / Mathf.Max(0.01f, Modifiers.Speed);
        }

        private float GetEffectiveEnergyCost(Recipe recipe)
        {
            return recipe.EnergyCost * Mathf.Max(0f, Modifiers.Energy);
        }
    }
}
