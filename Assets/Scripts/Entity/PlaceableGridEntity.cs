using Item;
using UnityEngine;

namespace Entity
{
    public abstract class PlaceableGridEntity : Entity
    {
        public Timeline ParentTimeline { get; protected set; }
        
        public virtual void Place(
            ItemData item, 
            Vector2Int pos, 
            float slotSize,
            Timeline timeline)
        {
            GridPos = pos;
            Item = item;
            ParentTimeline = timeline;
            transform.localPosition = new Vector3(pos.x * slotSize, pos.y * slotSize, 0f);
        }

        public virtual void Remove()
        {
            Destroy(gameObject);
        }
    }
}
