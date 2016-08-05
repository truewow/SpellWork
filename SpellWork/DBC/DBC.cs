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
using DBFilesClient.NET;
using System.Threading.Tasks;

namespace SpellWork.DBC
{
    public static class DBC
    {
        public const string Version = "SpellWork 7.0.3 (21796)";
        public const uint MaxLevel = 110;

        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable CollectionNeverUpdated.Global
        public static Storage<AreaGroupMemberEntry>             AreaGroupMember { get; set; }
        public static Storage<AreaTableEntry>                   AreaTable { get; set; }
        public static Storage<OverrideSpellDataEntry>           OverrideSpellData { get; set; }
        public static Storage<ScreenEffectEntry>                ScreenEffect { get; set; }
        public static Storage<SpellEntry>                       Spell { get; set; }
        public static Storage<SpellAuraOptionsEntry>            SpellAuraOptions { get; set; }
        public static Storage<SpellAuraRestrictionsEntry>       SpellAuraRestrictions { get; set; }
        public static Storage<SpellCastingRequirementsEntry>    SpellCastingRequirements { get; set; }
        public static Storage<SpellCastTimesEntry>              SpellCastTimes { get; set; }
        public static Storage<SpellCategoriesEntry>             SpellCategories { get; set; }
        public static Storage<SpellClassOptionsEntry>           SpellClassOptions { get; set; }
        public static Storage<SpellCooldownsEntry>              SpellCooldowns { get; set; }
        public static Storage<SpellDescriptionVariablesEntry>   SpellDescriptionVariables { get; set; }
        public static Storage<SpellDurationEntry>               SpellDuration { get; set; }
        public static Storage<SpellEffectEntry>                 SpellEffect { get; set; }
        public static Storage<SpellEffectScalingEntry>          SpellEffectScaling { get; set; }
        public static Storage<SpellMiscEntry>                   SpellMisc { get; set; }
        public static Storage<SpellEquippedItemsEntry>          SpellEquippedItems { get; set; }
        public static Storage<SpellInterruptsEntry>             SpellInterrupts { get; set; }
        public static Storage<SpellLevelsEntry>                 SpellLevels { get; set; }
        public static Storage<SpellPowerEntry>                  SpellPower { get; set; }
        public static Storage<SpellRadiusEntry>                 SpellRadius { get; set; }
        public static Storage<SpellRangeEntry>                  SpellRange { get; set; }
        public static Storage<SpellScalingEntry>                SpellScaling { get; set; }
        public static Storage<SpellShapeshiftEntry>             SpellShapeshift { get; set; }
        public static Storage<SpellTargetRestrictionsEntry>     SpellTargetRestrictions { get; set; }
        public static Storage<SpellTotemsEntry>                 SpellTotems { get; set; }
        public static Storage<SpellXSpellVisualEntry>           SpellXSpellVisual { get; set; }
        public static Storage<RandPropPointsEntry>              RandPropPoints { get; set; }
        public static Storage<SpellProcsPerMinuteEntry>         SpellProcsPerMinute { get; set; }

        public static Storage<SkillLineAbilityEntry>            SkillLineAbility { get; set; }
        public static Storage<SkillLineEntry>                   SkillLine { get; set; }

        public static Storage<ItemEntry>                        Item { get; set; }
        public static Storage<ItemSparseEntry>                  ItemSparse { get; set; }

        public static Storage<SpellReagentsEntry>               SpellReagents { get; set; }
        public static Storage<SpellMissileEntry>                SpellMissile { get; set; }
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore CollectionNeverUpdated.Global

        // public static Storage<SpellMissileMotionEntry>         SpellMissileMotion { get; public set; }
        // public static Storage<SpellVisualEntry>                SpellVisual { get; public set; }

        public static readonly IDictionary<int, SpellInfo> SpellInfoStore = new ConcurrentDictionary<int, SpellInfo>();
        public static readonly IDictionary<int, ISet<int>> SpellTriggerStore = new Dictionary<int, ISet<int>>();

        public static async void Load()
        {
            Parallel.ForEach(
                typeof (DBC).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic), dbc =>
                {
                    if (!dbc.PropertyType.IsGenericType ||
                        dbc.PropertyType.GetGenericTypeDefinition() != typeof (Storage<>))
                        return;

                    var name = dbc.Name;

                    var fileNameAttr =
                        dbc.PropertyType.GetGenericArguments()[0].GetCustomAttribute<DBFileNameAttribute>();
                    if (fileNameAttr != null)
                        name = fileNameAttr.FileName;

                    try
                    {
                        dbc.SetValue(dbc.GetValue(null),
                            Activator.CreateInstance(dbc.PropertyType, $@"{Settings.Default.DbcPath}\{name}.db2"));
                    }
                    catch (DirectoryNotFoundException)
                    {
                    }
                    catch (TargetInvocationException tie)
                    {
                        if (tie.InnerException is ArgumentException)
                            throw new ArgumentException($"Failed to load {name}.db2: {tie.InnerException.Message}");
                        throw;
                    }
                });

            foreach (var spell in Spell)
                SpellInfoStore[spell.Value.ID] = new SpellInfo { Spell = spell.Value};

            await Task.WhenAll(Task.Run(() =>
            {
                foreach (var effect in SpellInfoStore)
                {
                    if (!SpellMisc.ContainsKey(effect.Value.Spell.MiscID))
                        continue;

                    effect.Value.Misc = SpellMisc[effect.Value.Spell.MiscID];

                    if (SpellDuration.ContainsKey(effect.Value.Misc.DurationIndex))
                        effect.Value.DurationEntry = SpellDuration[effect.Value.Misc.DurationIndex];
                    if (SpellRange.ContainsKey(effect.Value.Misc.RangeIndex))
                        effect.Value.Range = SpellRange[effect.Value.Misc.RangeIndex];
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellEffect)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"Spell effect {effect.Value.ID} is referencing unknown spell {effect.Value.SpellID}, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].Effects.Add(effect.Value);
                    var triggerid = effect.Value.EffectTriggerSpell;
                    if (triggerid != 0)
                    {
                        if (SpellTriggerStore.ContainsKey((int) triggerid))
                            SpellTriggerStore[(int) triggerid].Add((int) effect.Value.SpellID);
                        else
                            SpellTriggerStore.Add((int) triggerid, new SortedSet<int> {(int) effect.Value.SpellID});
                    }
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellTargetRestrictions)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellTargetRestrictions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].TargetRestrictions.Add(effect.Value);
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellXSpellVisual)
                {
                    if (effect.Value.DifficultyID != 0 || effect.Value.PlayerConditionID != 0)
                        continue;

                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellXSpellVisual: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].SpellXSpellVisual = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellScaling)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellScaling: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].Scaling = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellAuraOptions)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellAuraOptions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].AuraOptions = effect.Value;
                    if (effect.Value.SpellProcsPerMinuteID != 0)
                        SpellInfoStore[effect.Value.SpellID].ProcsPerMinute = SpellProcsPerMinute[effect.Value.SpellProcsPerMinuteID];
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellAuraRestrictions)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellAuraRestrictions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].AuraRestrictions = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellCategories)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellCategories: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].Categories = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellCastingRequirements)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellCastingRequirements: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        return;
                    }

                    SpellInfoStore[effect.Value.SpellID].CastingRequirements = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellClassOptions)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellClassOptions: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].ClassOptions = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellCooldowns)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellCooldowns: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].Cooldowns = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellEffectScaling)
                {
                    if (!SpellEffect.ContainsKey(effect.Value.SpellEffectId))
                    {
                        Console.WriteLine(
                            $"SpellEffectScaling: Unknown spell effect {effect.Value.SpellEffectId} referenced, ignoring!");
                        continue;
                    }

                    SpellEffect[effect.Value.SpellEffectId].SpellEffectScalingEntry = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellInterrupts)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellInterrupts: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].Interrupts = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellEquippedItems)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellEquippedItems: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].EquippedItems = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellLevels)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine($"SpellLevels: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].Levels = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellReagents)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellReagents: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].Reagents = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellShapeshift)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine(
                            $"SpellShapeshift: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].Shapeshift = effect.Value;
                }
            }), Task.Run(() =>
            {
                foreach (var effect in SpellTotems)
                {
                    if (!SpellInfoStore.ContainsKey(effect.Value.SpellID))
                    {
                        Console.WriteLine($"SpellTotems: Unknown spell {effect.Value.SpellID} referenced, ignoring!");
                        continue;
                    }

                    SpellInfoStore[effect.Value.SpellID].Totems = effect.Value;
                }
            }));

            /*foreach (var item in ItemSparse.Rows)
            {
                ItemTemplate.Add(new Item
                {
                    Entry = item.ID,
                    Name = item.Name,
                    Description = item.Description,
                    SpellId = new[] { 0, 0, 0, 0, 0 }
                });
            }*/

            GameTable<GtSpellScalingEntry>.Open($@"{Settings.Default.GtPath}\SpellScaling.txt");
        }

        public static List<Item> ItemTemplate = new List<Item>();

        public static uint SelectedLevel = MaxLevel;
        public static uint SelectedItemLevel = 890;
    }
}
