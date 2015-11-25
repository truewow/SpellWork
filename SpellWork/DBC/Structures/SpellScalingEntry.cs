using DBFilesClient.NET;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellScalingEntry
    {
        public uint Id;
        public int CastTimeMin;
        public int CastTimeMax;
        public uint CastTimeMaxLevel;   // player level at which cast time reaches max value
        public int Class;
        public float NerfFactor;
        public uint NerfMaxLevel;
        public uint MaxScalingLevel;
        public uint ScalesFromItemLevel;
    }
}
