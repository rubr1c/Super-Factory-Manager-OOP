using Data.Items;
using UnityEngine;

namespace Data.Recipes
{
    [System.Serializable]
    public struct RecipeStack
    {
        [SerializeField] private Item item;
        [SerializeField] private float amount;

        public Item Item => item;
        public float Amount => Mathf.Max(0f, amount);
        public bool IsValid => item != null && Amount > 0f;
    }
}
