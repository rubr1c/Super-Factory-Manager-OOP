using System.Collections.Generic;
using System;
using Data.Items;
using Gameplay.World;
using Systems.Inventory;
using UnityEngine;

namespace Application.Managers
{
    public class TimelineManager : MonoBehaviour
    {
        public static TimelineManager Instance { get; private set; }

        [SerializeField] private GameObject timelinePrefab;

        public Timeline ActiveTimeline { get; private set; }

        [SerializeField] private List<Timeline> timelines = new();
        public IReadOnlyList<Timeline> AllTimelines => timelines;

        public event Action<Timeline> ActiveTimelineChanged;
        public event Action TimelinesChanged;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            CreateNewTimeline();
        }

        public float GetNewTimelineCost()
        {
            var purchasedTimelineCount = Mathf.Max(0, timelines.Count - 1);
            return Mathf.Pow(2f, purchasedTimelineCount);
        }

        public Timeline CreateNewTimeline()
        {
            var newTimelineObj = Instantiate(timelinePrefab);
            if (!newTimelineObj.TryGetComponent<Timeline>(out var newTimeline))
            {
                Destroy(newTimelineObj);
                return null;
            }

            newTimeline.Initialize(timelines.Count);
            timelines.Add(newTimeline);
            TimelinesChanged?.Invoke();

            SwitchToTimeline(newTimeline.TimelineID);
            return newTimeline;
        }

        public bool TryBuyNewTimeline()
        {
            var inventory = PlayerInventory.Instance;
            if (inventory == null) return false;

            if (!ItemCatalog.TryGet("chronos_fragment", out var chronosFragment)) return false;

            var cost = GetNewTimelineCost();
            if (inventory.CountItem(chronosFragment) < cost) return false;

            if (!inventory.TryConsumeItem(chronosFragment, cost)) return false;

            return CreateNewTimeline() != null;
        }

        public void SwitchToTimeline(int id)
        {
            if (id < 0 || id >= timelines.Count) return;

            foreach (var timeline in timelines)
            {
                timeline.gameObject.SetActive(false);
            }

            ActiveTimeline = timelines[id];
            ActiveTimeline.gameObject.SetActive(true);
            ActiveTimelineChanged?.Invoke(ActiveTimeline);
        }
    }
}
