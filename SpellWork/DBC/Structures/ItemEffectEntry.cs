namespace SpellWork.DBC.Structures
{
    public sealed class ItemEffectEntry
    {
        public uint Id;
        public uint ItemID;
        public uint OrderIndex;
        public uint SpellID;
        public uint Trigger;
        public int  Charges;
        public int  Cooldown;
        public uint Category;
        public int  CategoryCooldown;
        public uint ChrSpecializationID;
    }
}
