using Data.Items;
using UnityEngine;

namespace Gameplay.Entities
{
    public abstract class GridEntity : MonoBehaviour
    {
        public Vector2Int GridPos { get; protected set; }
        public Item Held { get; protected set; }
    }
}
