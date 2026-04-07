using System.Collections;
using Core;
using UnityEngine;

namespace Managers
{
    public class TickManager : MonoBehaviour
    {
        public static TickManager Instance { get; private set; }

        [SerializeField] private float tickRateSeconds = 1.0f;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            StartCoroutine(TickRoutine());
        }

        private IEnumerator TickRoutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(tickRateSeconds);

                if (TimelineManager.Instance == null) continue;

                foreach (var timeline in TimelineManager.Instance.AllTimelines)
                {
                    foreach (var entity in timeline.GridEntities)
                    {
                        if (entity == null) continue;

                        if (entity is ITickable tickableEntity)
                        {
                            tickableEntity.OnTick();
                        }
                    }
                }
            }
        }
    }
}