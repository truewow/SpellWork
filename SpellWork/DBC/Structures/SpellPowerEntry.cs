namespace SpellWork.DBC.Structures
{
    public sealed class SpellPowerEntry
    {
        public uint Id;
        public uint PowerType;
        public uint ManaCost;
        public uint ManaCostPerlevel;
        public uint ManaPerSecond;
        public uint ManaPerSecondPerLevel;
        public uint PowerDisplayId;
        public float ManaCostPercentage;
        public float UnkMop1; // Pandaria
        public uint RequiredAura; // Pandaria
        public float UnkMop2; // Pandaria
        public uint UnkWod1;
        public uint UnkWod2;
        public uint UnkWod3;
    }
}
