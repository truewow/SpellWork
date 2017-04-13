namespace SpellWork.DBC.Structures
{
    public sealed class SkillLineEntry
    {
        public string DisplayName;
        public string Description;
        public string AlternateVerb;
        public ushort Flags;
        public byte CategoryID;
        public byte CanLink;
        public uint IconFileDataID;
        public uint ParentSkillLineID;
    }
}
