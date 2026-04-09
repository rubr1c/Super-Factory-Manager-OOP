using Core;
using UnityEngine;

namespace Entity
{
    public enum PipeMode { None, Push, Pull, Neutral }

    public abstract class PipeEntity : PlaceableGridEntity, ITransport, ITickable
    {
        protected float ItemCount;
        protected float MaxCapacity;
        
        public float TransferRate = 10f; 

        public PipeMode[] Connections = new PipeMode[4]; 

        public override void Place(
            ItemData item, 
            Vector2Int pos, 
            float slotSize, 
            Timeline timeline)
        {
            Place(item, pos, slotSize, timeline, 1000);
        }

        public void Place(
            ItemData item, 
            Vector2Int pos, 
            float slotSize, 
            Timeline timeline, 
            float maxCapacity)
        {
            base.Place(item, pos, slotSize, timeline);
            MaxCapacity = maxCapacity;
            ItemCount = 0;
            
            // Default all sides to Neutral for now
            for(int i=0; i<4; i++) Connections[i] = PipeMode.Neutral;
        }

        public virtual bool CanHoldItem(ItemData item) { return true; }
        
        public bool Push(ItemData item, float amount)
        {
            if (!CanHoldItem(item)) return false;
            if (ItemCount + amount > MaxCapacity) return false;

            ItemCount += amount;
            return true;
        }

        public bool Pull(ItemData item, float amount)
        {
            if (!CanHoldItem(item)) return false;
            if (amount > ItemCount) return false;

            ItemCount -= amount;
            return true;
        }

        public float GetItemCount(ItemData item)
        {
            if (Item == item) return ItemCount;
            return 0f;
        }

        public float GetRemainingCapacity(ItemData item)
        {
            if (!CanHoldItem(item)) return 0f;
            return MaxCapacity - ItemCount;
        }

        public void OnTick()
        {
            if (ItemCount <= 0) return; 

            Vector2Int[] directions =
            {
                new (0, 1),
                new (1, 0),
                new (0, -1),
                new (-1, 0)
            };

            for (int i = 0; i < 4; i++)
            {
                if (Connections[i] == PipeMode.Push)
                {
                    var neighborPos = GridPos + directions[i];

                    var neighbor = ParentTimeline.EntityAt(neighborPos);

                    if (neighbor != null)
                    {
                        float amountToMove = Mathf.Min(ItemCount, TransferRate);

                        if (neighbor is IConsumer consumer)
                        {
                            if (consumer.TryConsume(Item, amountToMove))
                            {
                                ItemCount -= amountToMove;
                            }
                        }
                        else if (neighbor is ITransport transport)
                        {
                            var neighborCount = transport.GetItemCount(this.Item);
                            
                            if (ItemCount > neighborCount)
                            {
                                var volumeDifferance = ItemCount - neighborCount;
                                var transferAmountNeeded = volumeDifferance / 2f;

                                var spaceAvailable = transport.GetRemainingCapacity(Item);
                                var amountToTransfer = Mathf.Min(Mathf.Min(TransferRate, transferAmountNeeded), spaceAvailable);

                                if (amountToTransfer > 0 && transport.Push(Item, amountToTransfer))
                                {
                                    ItemCount -= amountToTransfer;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}