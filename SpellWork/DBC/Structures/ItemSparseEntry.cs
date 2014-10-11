using DBFilesClient.NET;

namespace SpellWork.DBC.Structures
{
    public sealed class ItemSparseEntry
    {
        public uint Id;

        [StoragePresence(StoragePresenceOption.Include, ArraySize = 70)]
        public int[] PlaceHolder1;

        public string Name;
        public string Name2;
        public string Name3;
        public string Name4;
        public string Description;

        [StoragePresence(StoragePresenceOption.Include, ArraySize = 26)]
        public int[] PlaceHolder2;
    }
}
