namespace SpellWork.DBC.Structures
{
    public class SpellInterruptsEntry
    {
        public uint SpellID;
        public uint[] AuraInterruptFlags;
        public uint[] ChannelInterruptFlags;
        public ushort InterruptFlags;
        public byte Difficulty;
    }
}
