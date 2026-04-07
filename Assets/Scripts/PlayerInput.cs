using Core;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public ItemData TestItem;

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (TestItem == null) return;

            var currentTimeline = TimelineManager.Instance.ActiveTimeline;
            if (currentTimeline == null) return;

            var cam = Camera.main;
            if (cam == null) return;

            var screen = (Vector3)Mouse.current.position.ReadValue();
            screen.z = -cam.transform.position.z;
            var mousePos = cam.ScreenToWorldPoint(screen);
            mousePos.z = 0f;

            var gridPos = currentTimeline.WorldToGridPosition(mousePos);

            if (currentTimeline.IsSlotEmpty(gridPos))
            {
                currentTimeline.TryPlace(TestItem, gridPos);
            }
        }
    }
}
