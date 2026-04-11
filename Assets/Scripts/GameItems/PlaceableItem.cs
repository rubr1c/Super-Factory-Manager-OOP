using Core;
using UnityEngine;

namespace GameItems
{
    [CreateAssetMenu(fileName = "PlaceableItem", menuName = "GameItems/Placeable Item")]
    public class PlaceableItem : Item, IPlaceable
    {
        [SerializeField] private GameObject prefab;

        public GameObject Prefab => prefab;
    }
}
