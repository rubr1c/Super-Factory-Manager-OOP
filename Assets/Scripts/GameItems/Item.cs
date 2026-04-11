using UnityEngine;

namespace GameItems
{
    public abstract class Item : ScriptableObject
    {
        [SerializeField] private string registryName = string.Empty;
        [SerializeField] private string displayName = string.Empty;
        [SerializeField] private Sprite icon;
        [SerializeField] private float maxStackSize = 100f;
        [SerializeField] private string[] tagIds = { };

        public string RegistryName => registryName;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public float MaxStackSize => Mathf.Max(0f, maxStackSize);
        public string[] TagIds => tagIds;

        public bool Is(ItemTag tag)
        {
            if (string.IsNullOrWhiteSpace(tag.Id))
            {
                return false;
            }

            for (var index = 0; index < tagIds.Length; index++)
            {
                if (tagIds[index] == tag.Id)
                {
                    return true;
                }
            }

            return false;
        }

        public bool Is(Item item)
        {
            return Equals(item);
        }
    }
}
