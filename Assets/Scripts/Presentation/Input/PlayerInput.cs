using Application.Managers;
using Data.Items;
using Presentation.UI;
using Systems.Inventory;
using Gameplay.Entities;
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
            if (!TryGetTapPosition(out var screenPosition))
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

            Vector3 screen = screenPosition;
            screen.z = -_camera.transform.position.z;
            var mousePos = _camera.ScreenToWorldPoint(screen);
            mousePos.z = 0f;

            var gridPos = currentTimeline.WorldToGridPosition(mousePos);
            var existingEntity = currentTimeline.EntityAt(gridPos);
            var playerInventory = PlayerInventory.Instance;
            if (existingEntity)
            {
                if (playerInventory != null
                    && playerInventory.SelectedHotbarSlotIndex >= 0
                    && existingEntity is UpgradableEntity upgradable)
                {
                    var selected = playerInventory.SelectedHotbarSlot;
                    if (!selected.IsEmpty
                        && selected.Held is UpgradeCardItem upgradeCard
                        && selected.Count >= 1f
                        && upgradable.TryInstallUpgrade(upgradeCard, out var installedSlot))
                    {
                        if (!playerInventory.TryConsumeSelectedHotbarItem(1))
                        {
                            upgradable.Upgrades.TryRemove(installedSlot);
                        }
                        else
                        {
                            EntityInfoPanel.Instance?.Show(existingEntity);
                        }

                        return;
                    }
                }

                existingEntity.OnInteract();
                return;
            }

            EntityInfoPanel.Instance?.Hide();

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

        private static bool TryGetTapPosition(out Vector2 screenPosition)
        {
            if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
            {
                screenPosition = Pointer.current.position.ReadValue();
                return true;
            }

            screenPosition = default;
            return false;
        }
    }
}
