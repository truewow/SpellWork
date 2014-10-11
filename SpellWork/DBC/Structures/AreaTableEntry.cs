using DBFilesClient.NET;

namespace SpellWork.DBC.Structures
{
    public sealed class AreaTableEntry
    {
        public uint Id;
        public uint MapId;

        [StoragePresence(StoragePresenceOption.Include, ArraySize = 11)]
        public uint[] PlaceHolder1;

        /*public uint ZoneId;
        public uint ExploreFlag;
        public uint Flags; // m_flags[2]
        public uint Flags2;
        public uint SoundPreferences;
        public uint SoundPreferencesUnderwater;
        public uint SoundAmbience;
        public uint ZoneMusic;
        public string Name2;
        public uint ZoneIntroMusicTable;
        public int Level;*/
        public string Name;
        /*public uint FactionFlags;
        public uint LiquidType1;
        public uint LiquidType2;
        public uint LiquidType3;
        public uint LiquidType4;
        public float MinElevation;
        public float AmbientMultiplier;
        public uint Light;
        public uint m_mountFlags;
        public uint m_uwIntroSound;
        public uint m_uwZoneMusic;
        public uint m_uwAmbience;
        public uint m_world_pvp_id;
        public uint m_pvpCombatWorldStateID;
        //public uint m_wildBattlePetLevelMin;
        //public uint m_wildBattlePetLevelMax;
        public uint m_windSettingsID;*/

        [StoragePresence(StoragePresenceOption.Include, ArraySize = 15)]
        public uint[] PlaceHolder2;
    }
}
