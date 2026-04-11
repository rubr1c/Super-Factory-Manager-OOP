using UnityEngine;

namespace GameItems
{
    [CreateAssetMenu(fileName = "UpgradeCardItem", menuName = "GameItems/Upgrade Card")]
    public class UpgradeCardItem : Item
    {
        [SerializeField] private float speedMultiplier = 1f;
        [SerializeField] private float energyMultiplier = 1f;
        [SerializeField] private float yieldMultiplier = 1f;

        public float SpeedMultiplier => speedMultiplier;
        public float EnergyMultiplier => energyMultiplier;
        public float YieldMultiplier => yieldMultiplier;
    }
}
