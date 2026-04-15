using Application.Managers;
using Gameplay.World;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Presentation.Cameras
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private UnityEngine.Camera cameraComponent;

        private bool _isPanning;
        private Vector3 _lastTouchPos;
        private Timeline _trackedTimeline;

        private void LateUpdate()
        {
            if (!cameraComponent)
            {
                return;
            }

            var activeTimeline = TimelineManager.Instance.ActiveTimeline;
            if (!activeTimeline)
            {
                return;
            }

            if (_trackedTimeline != activeTimeline)
            {
                CenterOnTimeline(activeTimeline);
                _trackedTimeline = activeTimeline;
            }

            HandlePanning();
            ClampToGridBounds(activeTimeline);
        }

        private Vector3 ScreenToWorldOnPlayPlane(Vector2 screenPixels)
        {
            var point = new Vector3(screenPixels.x, screenPixels.y, -cameraComponent.transform.position.z);
            var worldPoint = cameraComponent.ScreenToWorldPoint(point);
            worldPoint.z = cameraComponent.transform.position.z;
            return worldPoint;
        }

        private void CenterOnTimeline(Timeline timeline)
        {
            var gridCenterX = (timeline.GridWidth * timeline.SlotSize) * 0.5f;
            var gridCenterY = (timeline.GridHeight * timeline.SlotSize) * 0.5f;
            var world = timeline.transform.TransformPoint(new Vector3(gridCenterX, gridCenterY, 0f));
            var cameraZ = cameraComponent.transform.position.z;
            cameraComponent.transform.position = new Vector3(world.x, world.y, cameraZ);
        }

        private void HandlePanning()
        {
            if (Pointer.current == null)
            {
                return;
            }

            if (Pointer.current.press.wasPressedThisFrame)
            {
                _isPanning = true;
                _lastTouchPos = Pointer.current.position.ReadValue();
            }
            else if (Pointer.current.press.wasReleasedThisFrame)
            {
                _isPanning = false;
            }

            if (!_isPanning || !Pointer.current.press.isPressed)
            {
                return;
            }

            var currentMousePos = Pointer.current.position.ReadValue();
            var lastPos = ScreenToWorldOnPlayPlane(_lastTouchPos);
            var currentPos = ScreenToWorldOnPlayPlane(currentMousePos);
            cameraComponent.transform.position += lastPos - currentPos;
            _lastTouchPos = currentMousePos;
        }

        private void ClampToGridBounds(Timeline timeline)
        {
            var halfSlotSize = timeline.SlotSize / 2f;
            var timelineTransform = timeline.transform;
            var localBottomLeft = new Vector3(-halfSlotSize, -halfSlotSize, 0f);
            var localTopRight = new Vector3(
                ((timeline.GridWidth - 1) * timeline.SlotSize) + halfSlotSize,
                ((timeline.GridHeight - 1) * timeline.SlotSize) + halfSlotSize,
                0f);

            var worldA = timelineTransform.TransformPoint(localBottomLeft);
            var worldB = timelineTransform.TransformPoint(localTopRight);
            var gridMinX = Mathf.Min(worldA.x, worldB.x);
            var gridMaxX = Mathf.Max(worldA.x, worldB.x);
            var gridMinY = Mathf.Min(worldA.y, worldB.y);
            var gridMaxY = Mathf.Max(worldA.y, worldB.y);

            var cameraVerticalSize = cameraComponent.orthographicSize;
            var cameraHorizontalSize = cameraVerticalSize * ((float)Screen.width / Screen.height);

            var minPosX = gridMinX + cameraHorizontalSize;
            var maxPosX = gridMaxX - cameraHorizontalSize;
            var minPosY = gridMinY + cameraVerticalSize;
            var maxPosY = gridMaxY - cameraVerticalSize;

            if (minPosX > maxPosX)
            {
                minPosX = maxPosX = (gridMinX + gridMaxX) * 0.5f;
            }

            if (minPosY > maxPosY)
            {
                minPosY = maxPosY = (gridMinY + gridMaxY) * 0.5f;
            }

            var cameraPos = cameraComponent.transform.position;
            cameraPos.x = Mathf.Clamp(cameraPos.x, minPosX, maxPosX);
            cameraPos.y = Mathf.Clamp(cameraPos.y, minPosY, maxPosY);
            cameraComponent.transform.position = cameraPos;
        }
    }
}
