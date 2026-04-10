using Core;
using Item;
using UnityEngine;

namespace Entity.Machine
{
    public class WaterPump : PlaceableGridEntity, IMachine
    {
        private float _storedWater;
        [SerializeField] private int waterPerTick = 10;
        
        private float _storedEnergy;
        [SerializeField] private int energyNeededPerTick = 100;


        public override void Place(
            ItemData item,
            Vector2Int pos,
            float slotSize,
            Timeline timeline)
        {
            base.Place(item, pos, slotSize, timeline);
            _storedWater = 0;
            _storedEnergy = 0;
        }

        public void OnTick()
        {
            if (!(_storedEnergy >= energyNeededPerTick)) return;
            
            _storedWater += waterPerTick;
            _storedEnergy -= energyNeededPerTick;
        }

        public void OnInteract()
        {
            throw new System.NotImplementedException();
        }

        public float ExtractOutput(ItemData item, float maxAmount)
        {
            throw new System.NotImplementedException();
        }

        public bool TryConsume(ItemData item, float amount)
        {
            throw new System.NotImplementedException();
        }
    }
}