namespace SpellWork.DBC.Structures
{
    public sealed class ItemEffectEntry
    {
        public uint ItemID;
        public uint SpellID;
        public int Cooldown;
        public int CategoryCooldown;
        public short Charges;
        public ushort Category;
        public ushort ChrSpecializationID;
        public byte OrderIndex;
        public byte Trigger;
    }
}
