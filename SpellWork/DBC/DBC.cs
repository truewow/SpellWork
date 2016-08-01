using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using SpellWork.Database;
using SpellWork.DBC.Structures;
using SpellWork.GameTables.Structures;
using SpellWork.Spell;
using SpellWork.Properties;
using SpellWork.GameTables;
using System.Collections.Concurrent;

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
        private static DB2Reader<SpellEntry> Spell = new DB2Reader<SpellEntry>();
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

        private static readonly Dictionary<uint, SpellInfoLoadData> SpellInfoLoadData = new Dictionary<uint, SpellInfoLoadData>();

        public static void Load()
        {
            if (!Directory.Exists(Settings.Default.DbcPath))
            {
                var browserDialog = new System.Windows.Forms.FolderBrowserDialog {
                    Description = "Select the path to the directory containing your DB2 files."
                };
                if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Settings.Default.DbcPath = browserDialog.SelectedPath;
                    Settings.Default.Save();
                }
            }

            if (!Directory.Exists(Settings.Default.GtPath))
            {
                var browserDialog = new System.Windows.Forms.FolderBrowserDialog {
                    Description = "Select the path to the directory containing your GameTable files."
                };
                if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Settings.Default.GtPath = browserDialog.SelectedPath;
                    Settings.Default.Save();
                }
            }

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
                    OnLoadProgress?.Invoke(0, $"Loading {dbc.Name}.db2 ...");
                    dbc.FieldType.GetMethod("Open", new [] { typeof(string) }).Invoke(dbc.GetValue(null), new object[] { $@"{Settings.Default.DbcPath}\{name}.db2" });
                }
                catch (DirectoryNotFoundException)
                {
                    OnLoadProgress?.Invoke(0, $"Could not open {dbc.Name}.db2 ...");
                }
                catch (TargetInvocationException tie)
                {
                    OnLoadProgress?.Invoke(0, $"Could not open {dbc.Name}.db2 ...");
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

            // Use ConcurrentQueue to enable safe enqueueing from multiple threads.
            var exceptions = new ConcurrentQueue<Exception>();

            OnLoadProgress?.Invoke(0, "Generating SpellInfo store ...");

            foreach (var spell in Spell)
                SpellInfoLoadData[spell.Value.ID] = new SpellInfoLoadData() {
                    Spell = spell.Value
                };

            Parallel.ForEach(SpellInfoLoadData, effect =>
            {
                try
                {
                    if (!SpellMisc.ContainsKey(effect.Value.Spell.MiscID))
                    {
                        Console.WriteLine(
                            $"Spell {effect.Key} is referencing unknown SpellMisc {effect.Value.Spell.MiscID}, ignoring!");
                        return;
                    }
                    effect.Value.Misc = SpellMisc[(int) effect.Value.Spell.MiscID];
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading spell effects ...");

            Parallel.ForEach(SpellEffect, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"Spell effect {effect.Value.ID} is referencing unknown spell {effect.Value.SpellID}, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].Effects.Add(effect.Value);
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading spell target restrictions ...");

            Parallel.ForEach(SpellTargetRestrictions, effect =>
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine(
                        $"SpellTargetRestrictions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    return;
                }
                SpellInfoLoadData[effect.Value.SpellID].TargetRestrictions.Add(effect.Value);
            });

            OnLoadProgress?.Invoke(0, "Loading spell visuals ...");

            Parallel.ForEach(SpellXSpellVisual, effect =>
            {
                if (effect.Value.DifficultyID != 0 || effect.Value.PlayerConditionID != 0)
                    return;

                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellXSpellVisual: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].SpellXSpellVisual = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading spell scaling parameters ...");

            Parallel.ForEach(SpellScaling, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellScaling: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].Scaling = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading aura options ...");

            Parallel.ForEach(SpellAuraOptions, effect =>
            {
                try
                {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellAuraOptions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].AuraOptions = effect.Value;
                    if (effect.Value.SpellProcsPerMinuteID != 0)
                        SpellInfoLoadData[effect.Value.SpellID].ProcsPerMinute = SpellProcsPerMinute[effect.Value.SpellProcsPerMinuteID];
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            SpellProcsPerMinute.Clear();

            OnLoadProgress?.Invoke(0, "Loading aura restrictions ...");

            Parallel.ForEach(SpellAuraRestrictions, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellAuraRestrictions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].AuraRestrictions = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading spell categories ...");

            Parallel.ForEach(SpellCategories, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellCategories: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].Categories = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading casting requirements ...");

            Parallel.ForEach(SpellCastingRequirements, effect =>
            {
                if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                {
                    Console.WriteLine(
                        $"SpellCastingRequirements: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                    return;
                }
                SpellInfoLoadData[effect.Value.SpellID].CastingRequirements = effect.Value;
            });

            OnLoadProgress?.Invoke(0, "Loading spell class options ...");

            Parallel.ForEach(SpellClassOptions, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellClassOptions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].ClassOptions = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading cooldowns ...");

            Parallel.ForEach(SpellCooldowns, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellCooldowns: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].Cooldowns = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            Parallel.ForEach(SpellEffectScaling, effect =>
            {
                try
                {
                    if (!SpellEffect.ContainsKey((int)effect.Value.SpellEffectId))
                    {
                        Console.WriteLine(
                            $"SpellEffectScaling: Unknown spell effect {effect.Value.SpellEffectId} referenced, ignoring!");
                        return;
                    }

                    SpellEffect[effect.Value.SpellEffectId].SpellEffectScalingEntry = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading interrupts ...");

            Parallel.ForEach(SpellInterrupts, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellInterrupts: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].Interrupts = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading equipped items ...");

            Parallel.ForEach(SpellEquippedItems, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellEquippedItems: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].EquippedItems = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading spell levels ...");

            Parallel.ForEach(SpellLevels, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellLevels: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].Levels = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading spell reagents costs ...");

            Parallel.ForEach(SpellReagents, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellReagents: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].Reagents = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading spell shapeshift conditions ...");

            Parallel.ForEach(SpellShapeshift, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellShapeshift: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].Shapeshift = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

            OnLoadProgress?.Invoke(0, "Loading spell totem cost ...");

            Parallel.ForEach(SpellTotems, effect =>
            {
                try {
                    if (!SpellInfoLoadData.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellTotems: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }
                    SpellInfoLoadData[effect.Value.SpellID].Totems = effect.Value;
                }
                catch (Exception e)
                {
                    exceptions.Enqueue(e);
                }
            });

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

            OnLoadProgress?.Invoke(0, "Generating spell structure ...");

            foreach (var e in SpellInfoLoadData)
            // Parallel.ForEach(SpellInfoLoadData, e => {
                _spellInfo.Add((int) e.Key, new SpellInfoHelper(e.Value));
            // });

            if (!exceptions.IsEmpty)
                throw new Exception("Debug me!");

            OnLoadProgress?.Invoke(0, "Cleaning leftover data ...");

            foreach(var dbc in typeof(DBC).GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                if (!dbc.FieldType.IsGenericType || dbc.FieldType.GetGenericTypeDefinition() != typeof(DB2Reader<>))
                    continue;

                if (dbc.FieldType.GetGenericArguments()[0].IsValueType)
                    dbc.FieldType.GetMethod("Clear").Invoke(dbc.GetValue(null), new object[0]);
            }

            OnLoadProgress?.Invoke(0, "Loading GameTables ...");

            GameTable<GtSpellScalingEntry>.Open($@"{Settings.Default.GtPath}\SpellScaling.txt");

            OnLoadProgress?.Invoke(100, "Done!");
        }

        public static List<Item> ItemTemplate = new List<Item>();

        public static uint SelectedLevel = MaxLevel;

        private static Dictionary<int, SpellInfoHelper> _spellInfo = new Dictionary<int, SpellInfoHelper>();
        public static Dictionary<int, SpellInfoHelper> SpellInfoStore => _spellInfo;

        public static event Action<int, string> OnLoadProgress;
    }
}
