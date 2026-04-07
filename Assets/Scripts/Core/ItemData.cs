
using UnityEngine;

namespace Core
{
    [CreateAssetMenu(fileName = "Item", menuName = "SFM/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string ItemName;
        public Sprite Icon;
        public bool IsPlaceable;
        
        [Tooltip("Only for placeable items")]
        public GameObject Prefab; 
    }
}