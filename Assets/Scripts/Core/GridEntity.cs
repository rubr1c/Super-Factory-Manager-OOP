using Unity.Mathematics;
using UnityEngine;

namespace Core
{
    public abstract class GridEntity : MonoBehaviour
    {
        public Vector2Int GridPos { get; protected set; }
        public ItemData Item { get; protected set; }

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