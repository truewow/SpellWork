using DBFilesClient.NET;

namespace SpellWork.DBC.Structures
{
    public sealed class RandPropPointsEntry
    {
        public uint ID;
        [StoragePresence(StoragePresenceOption.Include, ArraySize = 5)]
        public uint[] Epic;
        [StoragePresence(StoragePresenceOption.Include, ArraySize = 5)]
        public uint[] Superior;
        [StoragePresence(StoragePresenceOption.Include, ArraySize = 5)]
        public uint[] Good;
    }
}
