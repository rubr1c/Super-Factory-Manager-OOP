using Entity;
using UnityEngine;
using UnityEngine.UIElements;

namespace UI
{
    [RequireComponent(typeof(UIDocument))]
    public class EntityInfoPanel : MonoBehaviour
    {
        public static EntityInfoPanel Instance { get; private set; }

        private VisualElement _panel;
        private Label _title;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            var root = GetComponent<UIDocument>().rootVisualElement;
            _panel = root.Q<VisualElement>("entity-info-panel");
            _title = root.Q<Label>("panel-title");

            SetPanelVisible(false);
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Show(PlaceableGridEntity entity)
        {
            if (!entity) return;

            _title.text = entity.Held ? entity.Held.DisplayName : "Unknown";
            SetPanelVisible(true);
        }

        public void Hide()
        {
            SetPanelVisible(false);
        }

        private void SetPanelVisible(bool visible)
        {
            _panel.style.display = visible ? DisplayStyle.Flex : DisplayStyle.None;
        }
    }
}
