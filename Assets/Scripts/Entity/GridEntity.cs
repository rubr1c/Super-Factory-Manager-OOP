using GameItems;
using UnityEngine;

namespace Entity
{
    public abstract class Entity : MonoBehaviour
    {
        public Vector2Int GridPos { get; protected set; }
        public Item Held { get; protected set; }
    }
}