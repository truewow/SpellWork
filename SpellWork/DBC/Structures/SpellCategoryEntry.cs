using DBFileReaderLib.Attributes;

namespace SpellWork.DBC.Structures
{
    public class SpellCategoryEntry
    {
        [Index(true)]
        public uint ID;
        public string Name;
        public sbyte Flags;
        public byte UsesPerWeek;
        public sbyte MaxCharges;
        public int ChargeRecoveryTime;
        public int TypeMask;
    };
}
