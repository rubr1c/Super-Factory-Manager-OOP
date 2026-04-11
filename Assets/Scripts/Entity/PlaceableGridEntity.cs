using GameItems;
using UnityEngine;

namespace Entity
{
    public abstract class PlaceableGridEntity : Entity
    {
        public Timeline ParentTimeline { get; protected set; }
        
        public virtual void Place(
            Item item, 
            Vector2Int pos, 
            float slotSize,
            Timeline timeline)
        {
            GridPos = pos;
            Definition = item;
            ParentTimeline = timeline;
            transform.localPosition = new Vector3(pos.x * slotSize, pos.y * slotSize, 0f);
        }

        public virtual void Remove()
        {
            if (ParentTimeline != null)
            {
                ParentTimeline.ClearEntityAt(GridPos, this);
            }

            Destroy(gameObject);
        }
    }
}
