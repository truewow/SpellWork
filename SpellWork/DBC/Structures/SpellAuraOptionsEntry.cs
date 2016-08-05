namespace SpellWork.DBC.Structures
{
    public class SpellAuraOptionsEntry
    {
        public int SpellID;
        public uint ProcCharges;
        public uint ProcTypeMask;
        public uint ProcCategoryRecovery;
        public ushort CumulativeAura;
        public byte DifficultyID;
        public byte ProcChance;
        public byte SpellProcsPerMinuteID;
    }
}
