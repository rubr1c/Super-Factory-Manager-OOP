using UnityEngine;

namespace Data.Items
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] private string registryName = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private Sprite icon;
        [SerializeField] private float maxStackSize = 100f;

        public string RegistryName => registryName;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public float MaxStackSize => Mathf.Max(0f, maxStackSize);
    }
}
