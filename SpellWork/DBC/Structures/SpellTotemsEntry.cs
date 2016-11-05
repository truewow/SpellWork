using System.Runtime.InteropServices;

namespace SpellWork.DBC.Structures
{
    public class SpellTotemsEntry
    {
        public int SpellID;
        public uint[] Totem;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ushort[] RequiredTotemCategoryID;
    }
}
