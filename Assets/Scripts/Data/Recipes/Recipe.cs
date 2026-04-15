using System;
using UnityEngine;

namespace Data.Recipes
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "GameData/Recipe")]
    public class Recipe : ScriptableObject
    {
        [SerializeField] private string recipeId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private MachineType machineType;
        [SerializeField] private RecipeStack[] itemInputs = Array.Empty<RecipeStack>();
        [SerializeField] private RecipeStack[] itemOutputs = Array.Empty<RecipeStack>();
        [SerializeField] private float energyCost;
        [SerializeField] private float waterCost;
        [SerializeField] private float processTimeSeconds = 1f;

        public string RecipeId => recipeId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public MachineType MachineType => machineType;
        public RecipeStack[] ItemInputs => itemInputs ?? Array.Empty<RecipeStack>();
        public RecipeStack[] ItemOutputs => itemOutputs ?? Array.Empty<RecipeStack>();
        public float EnergyCost => Mathf.Max(0f, energyCost);
        public float WaterCost => Mathf.Max(0f, waterCost);
        public float ProcessTimeSeconds => Mathf.Max(0.01f, processTimeSeconds);
    }
}
