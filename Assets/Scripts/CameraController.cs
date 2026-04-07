using Managers;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    private bool _isPanning;
    private Vector3 _lastTouchPos;

    private Timeline _trackedTimeline;

    private void LateUpdate()
    {
        if (Mouse.current == null || !_camera) return;

        var activeTimeline = TimelineManager.Instance.ActiveTimeline;
        if (!activeTimeline) return;

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
        var p = new Vector3(screenPixels.x, screenPixels.y, -_camera.transform.position.z);
        var w = _camera.ScreenToWorldPoint(p);
        w.z = _camera.transform.position.z;
        return w;
    }

    private void CenterOnTimeline(Timeline timeline)
    {
        var gridCenterX = (timeline.GridWidth * timeline.SlotSize) * 0.5f;
        var gridCenterY = (timeline.GridHeight * timeline.SlotSize) * 0.5f;
        var world = timeline.transform.TransformPoint(new Vector3(gridCenterX, gridCenterY, 0f));
        var camZ = _camera.transform.position.z;
        _camera.transform.position = new Vector3(world.x, world.y, camZ);
    }

    private void HandlePanning()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            _isPanning = true;
            _lastTouchPos = Mouse.current.position.ReadValue();
        }
        else if (Mouse.current.rightButton.wasReleasedThisFrame)
        {
            _isPanning = false;
        }

        if (!_isPanning || !Mouse.current.rightButton.isPressed) return;

        var currentMousePos = Mouse.current.position.ReadValue();
        var lastPos = ScreenToWorldOnPlayPlane(_lastTouchPos);
        var currentPos = ScreenToWorldOnPlayPlane(currentMousePos);
        _camera.transform.position += lastPos - currentPos;
        _lastTouchPos = currentMousePos;
    }

    private void ClampToGridBounds(Timeline timeline)
    {
        var halfSlotSize = timeline.SlotSize / 2f;
        var t = timeline.transform;
        var localBl = new Vector3(-halfSlotSize, -halfSlotSize, 0f);
        var localTr = new Vector3(
            ((timeline.GridWidth - 1) * timeline.SlotSize) + halfSlotSize,
            ((timeline.GridHeight - 1) * timeline.SlotSize) + halfSlotSize,
            0f);

        var wA = t.TransformPoint(localBl);
        var wB = t.TransformPoint(localTr);
        var gridMinX = Mathf.Min(wA.x, wB.x);
        var gridMaxX = Mathf.Max(wA.x, wB.x);
        var gridMinY = Mathf.Min(wA.y, wB.y);
        var gridMaxY = Mathf.Max(wA.y, wB.y);

        var cameraVerticalSize = _camera.orthographicSize;
        var cameraHorizontalSize = cameraVerticalSize * ((float)Screen.width / Screen.height);

        float minPosX = gridMinX + cameraHorizontalSize;
        float maxPosX = gridMaxX - cameraHorizontalSize;
        float minPosY = gridMinY + cameraVerticalSize;
        float maxPosY = gridMaxY - cameraVerticalSize;

        if (minPosX > maxPosX) minPosX = maxPosX = (gridMinX + gridMaxX) * 0.5f;
        if (minPosY > maxPosY) minPosY = maxPosY = (gridMinY + gridMaxY) * 0.5f;

        var cameraPos = _camera.transform.position;
        cameraPos.x = Mathf.Clamp(cameraPos.x, minPosX, maxPosX);
        cameraPos.y = Mathf.Clamp(cameraPos.y, minPosY, maxPosY);
        _camera.transform.position = cameraPos;
    }
}
