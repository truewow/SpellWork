using System.Runtime.InteropServices;

namespace SpellWork.DBC.Structures
{
    public class SpellReagentsEntry
    {
        public int SpellID;
        public uint[] Reagent;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public ushort[] ReagentCount;
    }
}
