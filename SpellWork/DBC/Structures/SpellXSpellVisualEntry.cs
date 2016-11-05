namespace SpellWork.DBC.Structures
{
    public sealed class SpellXSpellVisualEntry
    {
        public int SpellID;
        public float Chance;
        public ushort SpellVisualID;
        public ushort ViolentSpellVisualID;
        public ushort PlayerConditionID;
        public ushort UnitConditionID;
        public byte Flags;
        public byte DifficultyID;
        public byte Priority;
        public uint ID;
    }
}
