using Inventory;
using Managers;
using UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            var currentTimeline = TimelineManager.Instance.ActiveTimeline;
            if (!currentTimeline) return;

            if (!_camera) return;

            Vector3 screen = Mouse.current.position.ReadValue();
            screen.z = -_camera.transform.position.z;
            var mousePos = _camera.ScreenToWorldPoint(screen);
            mousePos.z = 0f;

            var gridPos = currentTimeline.WorldToGridPosition(mousePos);

            var existingEntity = currentTimeline.EntityAt(gridPos);
            if (existingEntity)
            {
                existingEntity.OnInteract();
                return;
            }

            EntityInfoPanel.Instance?.Hide();

            var playerInventory = PlayerInventory.Instance;
            if (!playerInventory) return;

            var selectedSlot = playerInventory.SelectedHotbarSlot;
            if (selectedSlot.IsEmpty) return;

            if (!currentTimeline.IsSlotEmpty(gridPos)) return;
            if (!currentTimeline.TryPlace(selectedSlot.Held, gridPos)) return;

            playerInventory.TryConsumeSelectedHotbarItem(1);
        }
    }
}
