using System;
using Core;
using UnityEngine;


public class Timeline : MonoBehaviour
{
    public int TimelineID { get; set; }
    
    // grid properties
    [SerializeField] private int gridWidth  = 30;
    [SerializeField] private int gridHeight = 30;
    [SerializeField] private float slotSize = 1.0f;
    
    // timeline modifiers
    [SerializeField] private float speedModifier       = 1.0f;
    [SerializeField] private float energyUsageModifier = 1.0f;

    private GridEntity[] _entityGrid;

    private void Awake()
    {
        _entityGrid = new GridEntity[gridHeight * gridWidth];
    }

    public bool IsSlotEmpty(Vector2Int pos)
    {
        if (!IsValidGridPosition(pos)) return false;
        return _entityGrid[pos.y * gridWidth + pos.x] == null;
    }

    private bool IsValidGridPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth && pos.y >= 0 && pos.y < gridHeight;
    }
    
    public GridEntity EntityAt(Vector2Int pos)
    {
        if (!IsValidGridPosition(pos)) return null;
        return _entityGrid[pos.y * gridWidth + pos.x];
    }

    public bool TryPlace(ItemData item, Vector2Int pos)
    {
        if (!IsSlotEmpty(pos)) return false;

        var spawnedItemObject = Instantiate(item.Prefab, transform);

        if (!spawnedItemObject.TryGetComponent<GridEntity>(out var itemEntity))
        {
            Debug.LogError(
                $"Prefab for '{item.ItemName}' needs a component that inherits GridEntity (e.g. PlaceableGridItem).",
                spawnedItemObject);
            Destroy(spawnedItemObject);
            return false;
        }

        itemEntity.Place(item, pos, slotSize);

        _entityGrid[pos.y * gridWidth + pos.x] = itemEntity;
        return true;
    }
    
    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        var local = transform.InverseTransformPoint(worldPos);
        var x = Mathf.RoundToInt(local.x / slotSize);
        var y = Mathf.RoundToInt(local.y / slotSize);
        return new Vector2Int(x, y);
    }
}
