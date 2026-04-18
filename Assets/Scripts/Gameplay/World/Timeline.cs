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
        [SerializeField] private Vector2 speedModifierRange = new(1.1f, 2.5f);
        [SerializeField] private Vector2 energyUsageModifierRange = new(0.5f, 1.5f);

        private PlaceableGridEntity[] _entityGrid;

        public int GridWidth => gridWidth;
        public int GridHeight => gridHeight;
        public float SlotSize => slotSize;
        public PlaceableGridEntity[] GridEntities => _entityGrid;
        public float SpeedModifier => speedModifier;
        public float EnergyUsageModifier => energyUsageModifier;

        private void Awake()
        {
            _entityGrid = new PlaceableGridEntity[gridHeight * gridWidth];
        }

        public void Initialize(int timelineId)
        {
            TimelineID = timelineId;

            if (timelineId == 0)
            {
                speedModifier = 1f;
                energyUsageModifier = 1f;
            }
            else
            {
                speedModifier = GenerateRandomModifier(speedModifierRange);
                energyUsageModifier = GenerateRandomModifier(energyUsageModifierRange);
            }
        }

        public bool IsSlotEmpty(Vector2Int pos)
        {
            return IsValidGridPosition(pos) && _entityGrid[pos.y * gridWidth + pos.x] == null;
        }

        public PlaceableGridEntity EntityAt(Vector2Int pos)
        {
            return IsValidGridPosition(pos)
                ? _entityGrid[pos.y * gridWidth + pos.x]
                : null;
        }

        public bool TryPlace(Item item, Vector2Int pos)
        {
            if (item is not IPlaceable placeableItem || placeableItem.Prefab == null || !IsSlotEmpty(pos)) return false;

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
            if (!IsValidGridPosition(pos)) return;

            var index = pos.y * gridWidth + pos.x;
            if (_entityGrid[index] == entity) _entityGrid[index] = null;
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPos)
        {
            var localPosition = transform.InverseTransformPoint(worldPos);
            var gridX = Mathf.RoundToInt(localPosition.x / slotSize);
            var gridY = Mathf.RoundToInt(localPosition.y / slotSize);
            return new Vector2Int(gridX, gridY);
        }

        private bool IsValidGridPosition(Vector2Int pos)
        {
            return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
        }

        private static float GenerateRandomModifier(Vector2 range)
        {
            var min = Mathf.Min(range.x, range.y);
            var max = Mathf.Max(range.x, range.y);
            if (Mathf.Approximately(min, max)) return Mathf.Max(0f, min);

            var rawValue = Random.Range(min, max);
            return Mathf.Max(0f, Mathf.Round(rawValue * 100f) / 100f);
        }
    }
}
