using Core;
using Unity.Mathematics;
using UnityEngine;

namespace Entity
{
    public abstract class Entity : MonoBehaviour
    {
        public Vector2Int GridPos { get; protected set; }
        public ItemData Item { get; protected set; }
    }
}