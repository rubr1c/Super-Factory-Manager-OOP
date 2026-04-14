using Core;
using Data.Items;
using Gameplay.Entities;
using UnityEngine;

namespace Gameplay.World
{
    public class Timeline : MonoBehaviour
    {
        public int TimelineID { get; set; }

        [SerializeField] private int gridWidth = 30;
        [SerializeField] private int gridHeight = 30;
        [SerializeField] private float slotSize = 1.0f;
        [SerializeField] private float speedModifier = 1.0f;
        [SerializeField] private float energyUsageModifier = 1.0f;

        private PlaceableGridEntity[] _entityGrid;

        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public float SlotSize => slotSize;
        public PlaceableGridEntity[] GridEntities => _entityGrid;

        private void Awake()
        {
            _entityGrid = new PlaceableGridEntity[gridHeight * gridWidth];
        }

        public bool IsSlotEmpty(Vector2Int pos)
        {
            return IsValidGridPosition(pos) && _entityGrid[pos.y * gridWidth + pos.x] == null;
        }

        public PlaceableGridEntity EntityAt(Vector2Int pos)
        {
            if (!IsValidGridPosition(pos))
            {
                return null;
            }

            return _entityGrid[pos.y * gridWidth + pos.x];
        }

        public bool TryPlace(Item item, Vector2Int pos)
        {
            if (item is not IPlaceable placeableItem || placeableItem.Prefab == null || !IsSlotEmpty(pos))
            {
                return false;
            }

            var spawnedItemObject = Instantiate(placeableItem.Prefab, transform);
            if (!spawnedItemObject.TryGetComponent<PlaceableGridEntity>(out var itemEntity))
            {
                Destroy(spawnedItemObject);
                return false;
            }

            itemEntity.Place(item, pos, slotSize, this);
            _entityGrid[pos.y * gridWidth + pos.x] = itemEntity;
            return true;
        }

        public void ClearEntityAt(Vector2Int pos, PlaceableGridEntity entity)
        {
            if (!IsValidGridPosition(pos))
            {
                return;
            }

            var index = pos.y * gridWidth + pos.x;
            if (_entityGrid[index] == entity)
            {
                _entityGrid[index] = null;
            }
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPos)
        {
            var local = transform.InverseTransformPoint(worldPos);
            var x = Mathf.RoundToInt(local.x / slotSize);
            var y = Mathf.RoundToInt(local.y / slotSize);
            return new Vector2Int(x, y);
        }

        private bool IsValidGridPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
        }
    }
}
