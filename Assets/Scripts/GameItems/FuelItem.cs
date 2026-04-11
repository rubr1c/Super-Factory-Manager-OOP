using Core;
using UnityEngine;

namespace GameItems
{
    [CreateAssetMenu(fileName = "FuelItem", menuName = "GameItems/Fuel Item")]
    public class FuelItem : Item, IFuel
    {
        [SerializeField] private float burnTime = 1f;

        public float BurnTime => Mathf.Max(0f, burnTime);
    }
}
