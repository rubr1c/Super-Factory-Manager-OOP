using Application.Managers;
using Presentation.UI;
using Systems.Inventory;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Presentation.Input
{
    public class PlayerInput : MonoBehaviour
    {
        private Camera _camera;

        private void Start()
        {
            _camera = Camera.main;
        }

        private void Update()
        {
            if (!Mouse.current.leftButton.wasPressedThisFrame)
            {
                return;
            }

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            var currentTimeline = TimelineManager.Instance.ActiveTimeline;
            if (!currentTimeline || !_camera)
            {
                return;
            }

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
            if (!playerInventory)
            {
                return;
            }

            var selectedSlot = playerInventory.SelectedHotbarSlot;
            if (selectedSlot.IsEmpty || !currentTimeline.IsSlotEmpty(gridPos) || !currentTimeline.TryPlace(selectedSlot.Held, gridPos))
            {
                return;
            }

            playerInventory.TryConsumeSelectedHotbarItem(1);
        }
    }
}
