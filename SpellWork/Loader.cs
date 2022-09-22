using System;
using SpellWork.DBC;
using SpellWork.Spell;

namespace SpellWork
{
    class Loader
    {
        public Loader()
        {
            DBC.DBCStore.AreaGroup = DBCReader.ReadDBC<AreaGroupEntry>(null);
            DBC.DBCStore.AreaTable = DBCReader.ReadDBC<AreaTableEntry>(DBC.DBCStore.AreaStrings);
            DBC.DBCStore.OverrideSpellData = DBCReader.ReadDBC<OverrideSpellDataEntry>(null);
            DBC.DBCStore.ScreenEffect = DBCReader.ReadDBC<ScreenEffectEntry>(DBC.DBCStore.ScreenEffectStrings);
            DBC.DBCStore.SkillLine = DBCReader.ReadDBC<SkillLineEntry>(DBC.DBCStore.SkillLineStrings);
            DBC.DBCStore.SkillLineAbility = DBCReader.ReadDBC<SkillLineAbilityEntry>(null);
            DBC.DBCStore.Spell = DBCReader.ReadDBC<SpellEntry>(DBC.DBCStore.SpellStrings);
            DBC.DBCStore.SpellCastTimes = DBCReader.ReadDBC<SpellCastTimesEntry>(null);
            DBC.DBCStore.SpellDifficulty = DBCReader.ReadDBC<SpellDifficultyEntry>(null);
            DBC.DBCStore.SpellDuration = DBCReader.ReadDBC<SpellDurationEntry>(null);
            DBC.DBCStore.SpellRadius = DBCReader.ReadDBC<SpellRadiusEntry>(null);
            DBC.DBCStore.SpellRange = DBCReader.ReadDBC<SpellRangeEntry>(DBC.DBCStore.SpellRangeStrings);
            DBC.DBCStore.SpellMissile = DBCReader.ReadDBC<SpellMissileEntry>(null);
            DBC.DBCStore.SpellMissileMotion = DBCReader.ReadDBC<SpellMissileMotionEntry>(DBC.DBCStore.SpellMissileMotionStrings);
            DBC.DBCStore.SpellVisual = DBCReader.ReadDBC<SpellVisualEntry>(null);
            DBC.DBCStore.SummonProperties = DBCReader.ReadDBC<SummonPropertiesEntry>(null);

            DBC.DBCStore.Locale = DetectedLocale;
        }

        /// <exception cref="Exception"><c>Exception</c>.</exception>
        private static LocalesDBC DetectedLocale
        {
            get
            {
                byte locale = 0;
                while (DBC.DBCStore.Spell[DBC.DBCStore.SpellEntryForDetectLocale].GetName(locale) == String.Empty)
                {
                    ++locale;

                    if (locale >= DBC.DBCStore.MaxDbcLocale)
                        throw new Exception("Detected unknown locale index " + locale);
                }
                return (LocalesDBC)locale;
            }
        }
    }
}
