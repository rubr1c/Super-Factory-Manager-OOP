using Core;
using Entity;
using UnityEngine;

namespace Entity
{
    public abstract class PlaceableGridEntity : Entity
    {
        public virtual void Place(ItemData item, Vector2Int pos, float slotSize)
        {
            GridPos = pos;
            Item = item;
            transform.localPosition = new Vector3(pos.x * slotSize, pos.y * slotSize, 0f);
        }

        public virtual void Remove()
        {
            Destroy(gameObject);
        }
    }
}
