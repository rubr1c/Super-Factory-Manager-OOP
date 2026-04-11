namespace GameItems
{
    public static class Items
    {
        public static Item ENERGY => Get("energy");
        public static Item WATER => Get("water");
        public static Item COAL => Get("coal");
        public static Item PIPE => Get("pipe");
        public static Item ENERGY_GENERATOR => Get("energy_generator");
        public static Item WATER_PUMP => Get("water_pump");

        public static Item Get(string registryName)
        {
            return ItemDatabase.Instance.Get(registryName);
        }

        public static bool TryGet(string registryName, out Item item)
        {
            return ItemDatabase.Instance.TryGet(registryName, out item);
        }
    }
}
