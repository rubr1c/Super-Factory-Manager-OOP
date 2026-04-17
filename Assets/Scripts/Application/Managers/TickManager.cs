using System.Collections;
using Core;
using UnityEngine;

namespace Application.Managers
{
    public class TickManager : MonoBehaviour
    {
        public static TickManager Instance { get; private set; }

        [SerializeField] private float tickRateSeconds = 1.0f;

        public float TickRateSeconds => tickRateSeconds;

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

                RunPhase<IProductionTickable>(RunProductionPhase);
                RunPhase<ILogisticsTickable>(RunLogisticsPhase);
                RunPhase<IConsumptionTickable>(RunConsumptionPhase);
            }
        }

        private static void RunProductionPhase(IProductionTickable phaseEntity)
        {
            phaseEntity.OnProductionTick();
        }

        private static void RunLogisticsPhase(ILogisticsTickable phaseEntity)
        {
            phaseEntity.OnLogisticsTick();
        }

        private static void RunConsumptionPhase(IConsumptionTickable phaseEntity)
        {
            phaseEntity.OnConsumptionTick();
        }

        private static void RunPhase<TPhase>(System.Action<TPhase> phaseAction)
        {
            if (TimelineManager.Instance == null) return;

            foreach (var timeline in TimelineManager.Instance.AllTimelines)
            {
                foreach (var entity in timeline.GridEntities)
                {
                    if (entity is TPhase phaseEntity) phaseAction(phaseEntity);
                }
            }
        }
    }
}
