namespace SpellWork.DBC.Structures
{
    public sealed class SpellDurationEntry
    {
        public int Duration;
        public int MaxDuration;
        public ushort DurationPerLevel;

        public override string ToString()
        {
            return $"{Duration}, {DurationPerLevel}, {MaxDuration}";
        }
    }
}
