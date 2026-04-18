using UnityEngine;

namespace Core
{
    public struct MachineModifiers
    {
        public float Speed;
        public float Energy;
        public float Yield;

        public static MachineModifiers Default => new() { Speed = 1f, Energy = 1f, Yield = 1f };

        public MachineModifiers Scaled(float speedMultiplier, float energyMultiplier, float yieldMultiplier = 1f)
        {
            return new MachineModifiers
            {
                Speed = Speed * speedMultiplier,
                Energy = Energy * energyMultiplier,
                Yield = Yield * yieldMultiplier
            };
        }

        public MachineModifiers Clamped(
            float minSpeed = 0.1f, float maxSpeed = 10f,
            float minEnergy = 0.1f, float maxEnergy = 5f,
            float minYield = 1f, float maxYield = 5f)
        {
            return new MachineModifiers
            {
                Speed = Mathf.Clamp(Speed, minSpeed, maxSpeed),
                Energy = Mathf.Clamp(Energy, minEnergy, maxEnergy),
                Yield = Mathf.Clamp(Yield, minYield, maxYield)
            };
        }
    }
}
