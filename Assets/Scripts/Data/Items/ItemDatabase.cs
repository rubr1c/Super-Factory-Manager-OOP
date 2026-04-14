using System.Collections.Generic;
using UnityEngine;

namespace Data.Items
{
    public class ItemDatabase : MonoBehaviour
    {
        public static ItemDatabase Instance { get; private set; }

        [SerializeField] private List<Item> items = new();

        public IReadOnlyList<Item> Items => items;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public Item Get(string registryName)
        {
            if (TryGet(registryName, out var item))
            {
                return item;
            }

            throw new UnityException($"Item '{registryName}' is not in the ItemDatabase.");
        }

        public bool TryGet(string registryName, out Item item)
        {
            for (var i = 0; i < items.Count; i++)
            {
                var currentItem = items[i];
                if (currentItem == null)
                {
                    continue;
                }

                if (currentItem.RegistryName == registryName)
                {
                    item = currentItem;
                    return true;
                }
            }

            item = null;
            return false;
        }
    }
}
