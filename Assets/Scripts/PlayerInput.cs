using Item;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    public ItemData testItem;
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (!testItem) return;

            var currentTimeline = TimelineManager.Instance.ActiveTimeline;
            if (!currentTimeline) return;

            if (!_camera) return;

            Vector3 screen = Mouse.current.position.ReadValue();
            screen.z = -_camera.transform.position.z;
            var mousePos = _camera.ScreenToWorldPoint(screen);
            mousePos.z = 0f;

            var gridPos = currentTimeline.WorldToGridPosition(mousePos);

            if (currentTimeline.IsSlotEmpty(gridPos))
            {
                currentTimeline.TryPlace(testItem, gridPos);
            }
        }
    }
}
