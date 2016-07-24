namespace SpellWork.GameTables.Structures
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class GtSpellScalingEntry : GameTableRecord
    {
        // ReSharper disable MemberCanBePrivate.Global
        public uint ID;
        public uint Rogue;
        public uint Druid;
        public uint Hunter;
        public uint Mage;
        public uint Paladin;
        public uint Priest;
        public uint Shaman;
        public uint Warlock;
        public uint Warrior;
        public uint DeathKnight;
        public uint Monk;
        public uint DemonHunter;
        public uint Item;
        public uint Consumable;
        public uint Gem1;
        public uint Gem2;
        public uint Gem3;
        public uint Health;
        // ReSharper restore MemberCanBePrivate.Global

        public uint GetColumnForClass(int columnIndex)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (columnIndex)
            {
                case 0: return ID;
                case 1: return Rogue;
                case 2: return Druid;
                case 3: return Hunter;
                case 4: return Mage;
                case 5: return Paladin;
                case 6: return Priest;
                case 7: return Shaman;
                case 8: return Warlock;
                case 9: return Warrior;
                case 10: return DeathKnight;
                case 11: return Monk;
                case 12: return DemonHunter;
                case 13: return Item;
                case 14: return Consumable;
                case 15: return Gem1;
                case 16: return Gem2;
                case 17: return Gem3;
                case 18: return Health;
            }
            return 0; // Shut up, compiler
        }
    }
}
