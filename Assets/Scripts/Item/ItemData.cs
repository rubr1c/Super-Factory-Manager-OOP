using UnityEngine;

namespace Item
{
    [CreateAssetMenu(fileName = "Item", menuName = "SFM/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string ItemName;
        public Sprite Icon;
        public bool IsPlaceable;
        public GameObject Prefab; 
    }
}