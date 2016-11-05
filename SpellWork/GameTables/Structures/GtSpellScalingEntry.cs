namespace SpellWork.GameTables.Structures
{
    // ReSharper disable once ClassNeverInstantiated.Global
    internal class GtSpellScalingEntry : GameTableRecord
    {
        // ReSharper disable MemberCanBePrivate.Global
        public uint ID { get; set; }
        public float Rogue { get; set; }
        public float Druid { get; set; }
        public float Hunter { get; set; }
        public float Mage { get; set; }
        public float Paladin { get; set; }
        public float Priest { get; set; }
        public float Shaman { get; set; }
        public float Warlock { get; set; }
        public float Warrior { get; set; }
        public float DeathKnight { get; set; }
        public float Monk { get; set; }
        public float DemonHunter { get; set; }
        public float Item { get; set; }
        public float Consumable { get; set; }
        public float Gem1 { get; set; }
        public float Gem2 { get; set; }
        public float Gem3 { get; set; }
        public float Health { get; set; }
        // ReSharper restore MemberCanBePrivate.Global

        public object GetColumnForClass(int columnIndex)
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
