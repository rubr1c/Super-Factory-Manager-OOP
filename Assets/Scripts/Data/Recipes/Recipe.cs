using UnityEngine;

namespace Data.Recipes
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "GameData/Recipe")]
    public class Recipe : ScriptableObject
    {
        [SerializeField] private string recipeId = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private MachineType machineType;
        [SerializeField] private RecipeStack[] itemInputs = new RecipeStack[0];
        [SerializeField] private RecipeStack[] itemOutputs = new RecipeStack[0];
        [SerializeField] private float energyCost;
        [SerializeField] private float waterCost;
        [SerializeField] private float processTimeSeconds = 1f;

        public string RecipeId => recipeId;
        public string DisplayName => string.IsNullOrWhiteSpace(displayName) ? name : displayName;
        public MachineType MachineType => machineType;
        public RecipeStack[] ItemInputs => itemInputs ?? new RecipeStack[0];
        public RecipeStack[] ItemOutputs => itemOutputs ?? new RecipeStack[0];
        public float EnergyCost => Mathf.Max(0f, energyCost);
        public float WaterCost => Mathf.Max(0f, waterCost);
        public float ProcessTimeSeconds => Mathf.Max(0.01f, processTimeSeconds);
    }
}
