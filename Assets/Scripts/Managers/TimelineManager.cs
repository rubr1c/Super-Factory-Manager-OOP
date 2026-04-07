using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class TimelineManager : MonoBehaviour
    {
        public static TimelineManager Instance { get; private set; }
        
        [SerializeField] public GameObject timeline;
        
        public Timeline ActiveTimeline { get; private set; }
        
        [SerializeField] private List<Timeline> timelines = new();
        public List<Timeline> AllTimelines => timelines;
        
        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }
        
        private void Start()
        {
            CreateNewTimeline();
        }
        
        public void CreateNewTimeline()
        {
            var newTimelineObj = Instantiate(timeline);
            var newTimeline = newTimelineObj.GetComponent<Timeline>();
            
            newTimeline.TimelineID = timelines.Count;
            
            timelines.Add(newTimeline);

            SwitchToTimeline(newTimeline.TimelineID);
        }

        public void SwitchToTimeline(int id)
        {
            if (id < 0 || id >= timelines.Count) return;

            foreach (var tl in timelines)
            {
                tl.gameObject.SetActive(false);
            }

            ActiveTimeline = timelines[id];
            ActiveTimeline.gameObject.SetActive(true);
        }
    }
}