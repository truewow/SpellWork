using SpellWork.Database;
using SpellWork.DBC.Structures;
using SpellWork.GameTables;
using SpellWork.GameTables.Structures;
using SpellWork.Properties;
using SpellWork.Spell;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SpellWork.DBC
{
    public static class DBC
    {
        public const string Version = "SpellWork 7.0.3 (21796)";
        public const uint MaxLevel = 110;

        public static readonly DB2Reader<AreaGroupMemberEntry> AreaGroupMember = new DB2Reader<AreaGroupMemberEntry>();
        public static DB2Reader<AreaTableEntry> AreaTable = new DB2Reader<AreaTableEntry>();
        public static DB2Reader<OverrideSpellDataEntry> OverrideSpellData = new DB2Reader<OverrideSpellDataEntry>();
        public static DB2Reader<ScreenEffectEntry> ScreenEffect = new DB2Reader<ScreenEffectEntry>();
        public static DB2Reader<SpellEntry> Spell = new DB2Reader<SpellEntry>();
        private static DB2Reader<SpellAuraOptionsEntry> SpellAuraOptions = new DB2Reader<SpellAuraOptionsEntry>();
        private static DB2Reader<SpellAuraRestrictionsEntry> SpellAuraRestrictions = new DB2Reader<SpellAuraRestrictionsEntry>();
        private static DB2Reader<SpellCastingRequirementsEntry> SpellCastingRequirements = new DB2Reader<SpellCastingRequirementsEntry>();
        public static DB2Reader<SpellCastTimesEntry> SpellCastTimes = new DB2Reader<SpellCastTimesEntry>();
        private static DB2Reader<SpellCategoriesEntry> SpellCategories = new DB2Reader<SpellCategoriesEntry>();
        private static DB2Reader<SpellClassOptionsEntry> SpellClassOptions = new DB2Reader<SpellClassOptionsEntry>();
        private static DB2Reader<SpellCooldownsEntry> SpellCooldowns = new DB2Reader<SpellCooldownsEntry>();
        public static DB2Reader<SpellDescriptionVariablesEntry> SpellDescriptionVariables = new DB2Reader<SpellDescriptionVariablesEntry>();
        public static DB2Reader<SpellDurationEntry> SpellDuration = new DB2Reader<SpellDurationEntry>();
        private static DB2Reader<SpellEffectEntry> SpellEffect = new DB2Reader<SpellEffectEntry>();
        public static DB2Reader<SpellEffectScalingEntry> SpellEffectScaling = new DB2Reader<SpellEffectScalingEntry>();
        private static DB2Reader<SpellMiscEntry> SpellMisc = new DB2Reader<SpellMiscEntry>();
        private static DB2Reader<SpellEquippedItemsEntry> SpellEquippedItems = new DB2Reader<SpellEquippedItemsEntry>();
        private static DB2Reader<SpellInterruptsEntry> SpellInterrupts = new DB2Reader<SpellInterruptsEntry>();
        private static DB2Reader<SpellLevelsEntry> SpellLevels = new DB2Reader<SpellLevelsEntry>();
        public static DB2Reader<SpellPowerEntry> SpellPower = new DB2Reader<SpellPowerEntry>();
        public static DB2Reader<SpellRadiusEntry> SpellRadius = new DB2Reader<SpellRadiusEntry>();
        public static DB2Reader<SpellRangeEntry> SpellRange = new DB2Reader<SpellRangeEntry>();
        private static DB2Reader<SpellScalingEntry> SpellScaling = new DB2Reader<SpellScalingEntry>();
        private static DB2Reader<SpellShapeshiftEntry> SpellShapeshift = new DB2Reader<SpellShapeshiftEntry>();
        private static DB2Reader<SpellTargetRestrictionsEntry> SpellTargetRestrictions = new DB2Reader<SpellTargetRestrictionsEntry>();
        private static DB2Reader<SpellTotemsEntry> SpellTotems = new DB2Reader<SpellTotemsEntry>();
        private static DB2Reader<SpellXSpellVisualEntry> SpellXSpellVisual = new DB2Reader<SpellXSpellVisualEntry>();
        public static DB2Reader<RandPropPointsEntry> RandPropPoints = new DB2Reader<RandPropPointsEntry>();
        private static DB2Reader<SpellProcsPerMinuteEntry> SpellProcsPerMinute = new DB2Reader<SpellProcsPerMinuteEntry>();

        public static DB2Reader<ItemEntry> Item = new DB2Reader<ItemEntry>();

        private static DB2Reader<SpellReagentsEntry> SpellReagents = new DB2Reader<SpellReagentsEntry>();
        public static DB2Reader<SpellMissileEntry> SpellMissile = new DB2Reader<SpellMissileEntry>();
        // public static DB2Reader<SpellMissileMotionEntry> SpellMissileMotion = new DB2Reader<SpellMissileMotionEntry>();
        // public static DB2Reader<SpellVisualEntry> SpellVisual = new DB2Reader<SpellVisualEntry>();

        //[DataStoreFileName("Item-sparse")]
        //public static DB2Reader<ItemSparseEntry> ItemSparse = new DB2Reader<ItemSparseEntry>();

        public static Dictionary<int, SkillLineAbilityEntry> SkilllLineAbilityStore = new Dictionary<int, SkillLineAbilityEntry>();
        public static Dictionary<int, SkillLineEntry> SkilllLineStore = new Dictionary<int, SkillLineEntry>();

        private static readonly IDictionary<uint, SpellInfoLoadData> SpellInfoLoadData = new ConcurrentDictionary<uint, SpellInfoLoadData>();
        public static readonly IDictionary<uint, ISet<uint>> SpellTriggerStore = new Dictionary<uint, ISet<uint>>();

        public static void Load()
        {
            foreach (var dbc in typeof(DBC).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!dbc.FieldType.IsGenericType || dbc.FieldType.GetGenericTypeDefinition() != typeof(DB2Reader<>))
                    continue;

                var name = dbc.Name;

                var attributes = dbc.GetCustomAttributes(typeof(DataStoreFileNameAttribute), false) as DataStoreFileNameAttribute[];
                if (attributes != null && attributes.Length == 1)
                    name = attributes[0].FileName;

                try
                {
                    dbc.FieldType.GetMethod("Open", new [] { typeof(string) }).Invoke(dbc.GetValue(null), new object[] { $@"{Settings.Default.DbcPath}\{name}.db2" });
                }
                catch (DirectoryNotFoundException)
                {
                }
                catch (TargetInvocationException tie)
                {
                    if (tie.InnerException is ArgumentException)
                        throw new ArgumentException($"Failed to load {dbc.Name}.db2: {tie.InnerException.Message}");

                    throw;
                }
            }

            // Open DBCs locally
            // This is faster because it avoids a truckload of reflection code later (ProcInfo, i'm looking at you)
            var skillLineAbility = new DB2Reader<SkillLineAbilityEntry>($@"{Settings.Default.DbcPath}\SkillLineAbility.db2");
            var skillLine = new DB2Reader<SkillLineEntry>($@"{Settings.Default.DbcPath}\SkillLine.db2");
            foreach (var kvp in skillLineAbility)
                SkilllLineAbilityStore[kvp.Key] = kvp.Value;

            foreach (var kvp in skillLine)
                SkilllLineStore[kvp.Key] = kvp.Value;

            skillLine.Clear();
            skillLineAbility.Clear();

            foreach (var spell in Spell)
                SpellInfoLoadData[spell.Value.ID] = new SpellInfoLoadData() { Spell = spell.Value };

            foreach (var effect in SpellInfoLoadData)
            {
                if (!SpellMisc.ContainsKey(effect.Value.Spell.MiscID))
                {
                    Console.WriteLine($"Spell {effect.Key} is referencing unknown SpellMisc {effect.Value.Spell.MiscID}, ignoring!");
                    continue;
                }

                effect.Value.Misc = SpellMisc[(int) effect.Value.Spell.MiscID];
            }

            foreach (var effect in SpellEffect)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"Spell effect {effect.Value.ID} is referencing unknown spell {effect.Value.SpellID}, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].Effects.Add(effect.Value);
                var triggerid = effect.Value.EffectTriggerSpell;
                if (triggerid != 0)
                {
                    if (SpellTriggerStore.ContainsKey(triggerid))
                        SpellTriggerStore[triggerid].Add(effect.Value.SpellID);
                    else
                        SpellTriggerStore.Add(triggerid, new SortedSet<uint> { effect.Value.SpellID });
                }
            }

            foreach (var effect in SpellTargetRestrictions)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellTargetRestrictions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].TargetRestrictions.Add(effect.Value);
            }

            foreach (var effect in SpellXSpellVisual)
            {
                if (effect.Value.DifficultyID != 0 || effect.Value.PlayerConditionID != 0)
                    continue;

                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellXSpellVisual: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].SpellXSpellVisual = effect.Value;
            }

            foreach (var effect in SpellScaling)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellScaling: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].Scaling = effect.Value;
            }

            foreach (var effect in SpellAuraOptions)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellAuraOptions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].AuraOptions = effect.Value;
                if (effect.Value.SpellProcsPerMinuteID != 0)
                    SpellInfoLoadData[effect.Value.SpellID].ProcsPerMinute = SpellProcsPerMinute[effect.Value.SpellProcsPerMinuteID];
            }

            foreach (var effect in SpellAuraRestrictions)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellAuraRestrictions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].AuraRestrictions = effect.Value;
            }

            foreach (var effect in SpellCategories)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellCategories: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].Categories = effect.Value;
            }

            foreach (var effect in SpellCastingRequirements)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellCastingRequirements: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    return;
                }

                SpellInfoLoadData[effect.Value.SpellID].CastingRequirements = effect.Value;
            }

            foreach (var effect in SpellClassOptions)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellClassOptions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].ClassOptions = effect.Value;
            }

            foreach (var effect in SpellCooldowns)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellCooldowns: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].Cooldowns = effect.Value;
            }

            foreach (var effect in SpellEffectScaling)
            {
                if (!SpellEffect.ContainsKey((int)effect.Value.SpellEffectId))
                {
                    Console.WriteLine($"SpellEffectScaling: Unknown spell effect {effect.Value.SpellEffectId} referenced, ignoring!");
                    continue;
                }

                SpellEffect[effect.Value.SpellEffectId].SpellEffectScalingEntry = effect.Value;
            }

            foreach (var effect in SpellInterrupts)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellInterrupts: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].Interrupts = effect.Value;
            }

            foreach (var effect in SpellEquippedItems)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellEquippedItems: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].EquippedItems = effect.Value;
            }

            foreach (var effect in SpellLevels)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellLevels: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].Levels = effect.Value;
            }

            foreach (var effect in SpellReagents)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellReagents: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].Reagents = effect.Value;
            }

            foreach (var effect in SpellShapeshift)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellShapeshift: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].Shapeshift = effect.Value;
            }

            foreach (var effect in SpellTotems)
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine($"SpellTotems: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    continue;
                }

                SpellInfoLoadData[effect.Value.SpellID].Totems = effect.Value;
            }

            /*OnLoadProgress?.Invoke($"Generating ItemTemplate store ...");

            foreach (var item in ItemSparse.Rows)
            {
                ItemTemplate.Add(new Item
                {
                    Entry = item.ID,
                    Name = item.Name,
                    Description = item.Description,
                    SpellId = new[] { 0, 0, 0, 0, 0 }
                });
            }*/

            foreach (var e in SpellInfoLoadData)
                _spellInfo.Add((int) e.Key, new SpellInfoHelper(e.Value));

            GameTable<GtSpellScalingEntry>.Open($@"{Settings.Default.GtPath}\SpellScaling.txt");
        }

        public static List<Item> ItemTemplate = new List<Item>();

        public static uint SelectedLevel = MaxLevel;
        public static uint SelectedItemLevel = 890;

        private static Dictionary<int, SpellInfoHelper> _spellInfo = new Dictionary<int, SpellInfoHelper>();
        public static Dictionary<int, SpellInfoHelper> SpellInfoStore => _spellInfo;
    }
}
