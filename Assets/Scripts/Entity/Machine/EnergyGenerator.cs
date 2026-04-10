using Core;
using Item;
using UnityEngine;

namespace Entity.Machine
{
    public class EnergyGenerator : PlaceableGridEntity, IGenerator
    {
        private float _storedEnergy;
        [SerializeField] private int energyPerTick = 10;

        public override void Place(
            ItemData item, 
            Vector2Int pos, 
            float slotSize, 
            Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _storedEnergy = 0;
        }

        public void OnTick()
        {
            _storedEnergy += energyPerTick;
            Debug.Log($"Total Energy: {_storedEnergy}");
        }

        public float ExtractOutput(ItemData item, float maxAmount)
        {
            throw new System.NotImplementedException();
        }

        public void OnInteract()
        {
            throw new System.NotImplementedException();
        }
    }
}