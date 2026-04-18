using UnityEngine;

namespace Core
{
    public static class GridDirections
    {
        public static readonly Vector2Int[] Cardinal =
        {
            new(0, 1),
            new(1, 0),
            new(0, -1),
            new(-1, 0)
        };
    }
}
