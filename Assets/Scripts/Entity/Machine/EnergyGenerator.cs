using Core;
using UnityEngine;

namespace Entity.Machine
{
    public class EnergyGenerator : PlaceableGridEntity, IGenerator
    {
        [SerializeField] private float storedEnergy;
        [SerializeField] private int energyPerTick = 10;

        public override void Place(
            ItemData item, 
            Vector2Int pos, 
            float slotSize, 
            Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            storedEnergy = 0;
        }

        public void OnTick()
        {
            storedEnergy += energyPerTick;
            Debug.Log($"Total Energy: {storedEnergy}");
        }

        public ItemData ExtractOutput(float amount)
        {
            throw new System.NotImplementedException();        
        }

        public void OnInteract()
        {
            throw new System.NotImplementedException();
        }
    }
}