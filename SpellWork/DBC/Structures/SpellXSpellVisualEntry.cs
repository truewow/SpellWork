namespace SpellWork.DBC.Structures
{
    public sealed class SpellXSpellVisualEntry
    {
        public int SpellID;
        public uint SpellVisualID;
        public uint ID;
        public float Chance;
        public ushort CasterPlayerConditionID;
        public ushort CasterUnitConditionID;
        public ushort PlayerConditionID;
        public ushort UnitConditionID;
        public uint IconFileDataID;
        public uint ActiveIconFileDataID;
        public byte Flags;
        public byte DifficultyID;
        public byte Priority;
    }
}
