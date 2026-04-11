using GameItems;
using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] private string selectedItemId = "pipe";
    private Camera _camera;

    private void Start()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            var currentTimeline = TimelineManager.Instance.ActiveTimeline;
            if (!currentTimeline) return;

            if (!_camera) return;

            if (!Items.TryGet(selectedItemId, out var selectedItem))
            {
                return;
            }

            Vector3 screen = Mouse.current.position.ReadValue();
            screen.z = -_camera.transform.position.z;
            var mousePos = _camera.ScreenToWorldPoint(screen);
            mousePos.z = 0f;

            var gridPos = currentTimeline.WorldToGridPosition(mousePos);

            if (currentTimeline.IsSlotEmpty(gridPos))
            {
                currentTimeline.TryPlace(selectedItem, gridPos);
            }
        }
    }
}
