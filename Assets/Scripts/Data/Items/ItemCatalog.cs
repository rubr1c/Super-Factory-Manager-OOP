namespace Data.Items
{
    public static class ItemCatalog
    {
        public static Item ENERGY => Get("energy");
        public static Item WATER => Get("water");
        public static Item COAL => Get("coal");
        public static Item IRON_ORE => Get("iron_ore");
        public static Item COPPER_ORE => Get("copper_ore");
        public static Item QUARTZ => Get("quartz");
        public static Item IRON_PLATE => Get("iron_plate");
        public static Item COPPER_PLATE => Get("copper_plate");
        public static Item SILICON_WAFER => Get("silicon_wafer");
        public static Item STEEL_INGOT => Get("steel_ingot");
        public static Item COPPER_WIRE => Get("copper_wire");
        public static Item CIRCUIT_BOARD => Get("circuit_board");
        public static Item MACHINE_CHASSIS => Get("machine_chassis");
        public static Item CHRONOS_FRAGMENT => Get("chronos_fragment");

        public static Item OVERCLOCK_MODULE => Get("overclock_module");
        public static Item EFFICIENCY_MODULE => Get("efficiency_module");
        public static Item OPTIMIZATION_MODULE => Get("optimization_module");

        public static Item ENERGY_PIPE => Get("energy_pipe");
        public static Item FLUID_PIPE => Get("fluid_pipe");
        public static Item ITEM_PIPE => Get("item_pipe");

        public static Item STARTER_DRILL => Get("starter_drill");
        public static Item ENERGY_DRILL => Get("energy_drill");
        public static Item ENERGY_GENERATOR => Get("energy_generator");
        public static Item COAL_BURNER => Get("coal_burner");
        public static Item LIQUID_EXTRACTOR => Get("liquid_extractor");
        public static Item PLATE_COMPRESSOR => Get("plate_compressor");
        public static Item ARC_SMELTER => Get("arc_smelter");
        public static Item INDUSTRIAL_ASSEMBLER => Get("industrial_assembler");
        public static Item CIRCUIT_FABRICATOR => Get("circuit_fabricator");
        public static Item INVENTORY_UPLINK => Get("inventory_uplink");
        public static Item LOGISTICS_BUFFER => Get("logistics_buffer");
        public static Item INTER_TIMELINE_BRIDGE => Get("inter_timeline_bridge");

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
