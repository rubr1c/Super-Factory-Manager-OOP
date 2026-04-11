namespace Core
{
    public struct MachineModifiers
    {
        public float Speed;
        public float Energy;
        public float Yield;

        public static MachineModifiers Default => new() { Speed = 1f, Energy = 1f, Yield = 1f };
    }
}
