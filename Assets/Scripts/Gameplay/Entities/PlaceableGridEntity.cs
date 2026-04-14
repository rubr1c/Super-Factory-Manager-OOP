using Core;
using Data.Items;
using Gameplay.World;
using Presentation.UI;
using UnityEngine;

namespace Gameplay.Entities
{
    public abstract class PlaceableGridEntity : GridEntity, IInteractable
    {
        public Timeline ParentTimeline { get; protected set; }

        public virtual void Place(Item item, Vector2Int pos, float slotSize, Timeline timeline)
        {
            GridPos = pos;
            Held = item;
            ParentTimeline = timeline;
            transform.localPosition = new Vector3(pos.x * slotSize, pos.y * slotSize, 0f);
        }

        public virtual void Remove()
        {
            if (ParentTimeline != null)
            {
                ParentTimeline.ClearEntityAt(GridPos, this);
            }

            Destroy(gameObject);
        }

        public virtual void OnInteract()
        {
            EntityInfoPanel.Instance?.Show(this);
        }

        public virtual void BuildInfoPanel(EntityInfoPanel panel)
        {
        }

        public virtual void RefreshInfoPanel(EntityInfoPanel panel)
        {
        }
    }
}
