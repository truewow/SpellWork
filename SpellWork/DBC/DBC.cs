using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DBFilesClient.NET;
using SpellWork.Database;
using SpellWork.DBC.Structures;
using SpellWork.Spell;
using SpellWork.Properties;

namespace SpellWork.DBC
{
    public static class DBC
    {
        public const string Version = "SpellWork 6.2.3 (20726)";
        public const uint MaxLevel  = 100;

        public const int MaxDbcLocale                 = 16;
        //public const int MaxEffectIndex               = 32;
        public const int SpellEntryForDetectLocale    = 1;

        public static DB2Storage<AreaGroupMemberEntry> AreaGroupMember = new DB2Storage<AreaGroupMemberEntry>();
        public static DBCStorage<AreaTableEntry> AreaTable = new DBCStorage<AreaTableEntry>();
        public static DBCStorage<gtSpellScalingEntry> gtSpellScaling = new DBCStorage<gtSpellScalingEntry>();
        public static DB2Storage<OverrideSpellDataEntry> OverrideSpellData = new DB2Storage<OverrideSpellDataEntry>();
        public static DBCStorage<RandPropPointsEntry> RandPropPoints = new DBCStorage<RandPropPointsEntry>();
        public static DBCStorage<ScreenEffectEntry> ScreenEffect = new DBCStorage<ScreenEffectEntry>();
        public static DBCStorage<SkillLineAbilityEntry> SkillLineAbility = new DBCStorage<SkillLineAbilityEntry>();
        public static DBCStorage<SkillLineEntry> SkillLine = new DBCStorage<SkillLineEntry>();
        public static DBCStorage<SpellEntry> Spell = new DBCStorage<SpellEntry>();
        public static DBCStorage<SpellAuraOptionsEntry> SpellAuraOptions = new DBCStorage<SpellAuraOptionsEntry>();
        public static DB2Storage<SpellAuraRestrictionsEntry> SpellAuraRestrictions = new DB2Storage<SpellAuraRestrictionsEntry>();
        public static DB2Storage<SpellCastingRequirementsEntry> SpellCastingRequirements = new DB2Storage<SpellCastingRequirementsEntry>();
        public static DB2Storage<SpellCastTimesEntry> SpellCastTimes = new DB2Storage<SpellCastTimesEntry>();
        public static DBCStorage<SpellCategoriesEntry> SpellCategories = new DBCStorage<SpellCategoriesEntry>();
        public static DB2Storage<SpellClassOptionsEntry> SpellClassOptions = new DB2Storage<SpellClassOptionsEntry>();
        public static DBCStorage<SpellCooldownsEntry> SpellCooldowns = new DBCStorage<SpellCooldownsEntry>();
        public static DBCStorage<SpellDescriptionVariablesEntry> SpellDescriptionVariables = new DBCStorage<SpellDescriptionVariablesEntry>();
        public static DB2Storage<SpellDurationEntry> SpellDuration = new DB2Storage<SpellDurationEntry>();
        public static DBCStorage<SpellEffectEntry> SpellEffect = new DBCStorage<SpellEffectEntry>();
        public static DBCStorage<SpellEffectScalingEntry> SpellEffectScaling = new DBCStorage<SpellEffectScalingEntry>();
        public static DB2Storage<SpellMiscEntry> SpellMisc = new DB2Storage<SpellMiscEntry>();
        public static DBCStorage<SpellEquippedItemsEntry> SpellEquippedItems = new DBCStorage<SpellEquippedItemsEntry>();
        public static DBCStorage<SpellInterruptsEntry> SpellInterrupts = new DBCStorage<SpellInterruptsEntry>();
        public static DBCStorage<SpellLevelsEntry> SpellLevels = new DBCStorage<SpellLevelsEntry>();
        public static DB2Storage<SpellPowerEntry> SpellPower = new DB2Storage<SpellPowerEntry>();
        //public static Dictionary<uint, List<SpellPowerEntry>> _spellPower = new Dictionary<uint, List<SpellPowerEntry>>();
        public static DB2Storage<SpellRadiusEntry> SpellRadius = new DB2Storage<SpellRadiusEntry>();
        public static DB2Storage<SpellRangeEntry> SpellRange = new DB2Storage<SpellRangeEntry>();
        public static DB2Storage<SpellRuneCostEntry> SpellRuneCost = new DB2Storage<SpellRuneCostEntry>();
        public static DBCStorage<SpellScalingEntry> SpellScaling = new DBCStorage<SpellScalingEntry>();
        public static DBCStorage<SpellShapeshiftEntry> SpellShapeshift = new DBCStorage<SpellShapeshiftEntry>();
        public static DBCStorage<SpellTargetRestrictionsEntry> SpellTargetRestrictions = new DBCStorage<SpellTargetRestrictionsEntry>();
        public static Dictionary<uint, List<SpellTargetRestrictionsEntry>> _spellTargetRestrictions = new Dictionary<uint, List<SpellTargetRestrictionsEntry>>();
        public static DB2Storage<SpellTotemsEntry> SpellTotems = new DB2Storage<SpellTotemsEntry>();
        public static DB2Storage<SpellXSpellVisualEntry> SpellXSpellVisual = new DB2Storage<SpellXSpellVisualEntry>();

        public static DB2Storage<ItemEntry> Item = new DB2Storage<ItemEntry>();
        public static DB2Storage<ItemEffectEntry> ItemEffect = new DB2Storage<ItemEffectEntry>();
        public static DB2Storage<SpellReagentsEntry> SpellReagents = new DB2Storage<SpellReagentsEntry>();
        public static DB2Storage<SpellMissileEntry> SpellMissile = new DB2Storage<SpellMissileEntry>();
        public static DB2Storage<SpellMissileMotionEntry> SpellMissileMotion = new DB2Storage<SpellMissileMotionEntry>();
        public static DB2Storage<SpellVisualEntry> SpellVisual = new DB2Storage<SpellVisualEntry>();

        [DataStoreFileName("Item-sparse")]
        public static DB2Storage<ItemSparseEntry> ItemSparse = new DB2Storage<ItemSparseEntry>();

        public static Dictionary<uint, SpellInfoHelper> SpellInfoStore = new Dictionary<uint, SpellInfoHelper>();
        public static Dictionary<uint, List<SpellEffectEntry>> SpellEffectLists = new Dictionary<uint, List<SpellEffectEntry>>();
        public static Dictionary<uint, List<uint>> SpellTriggerStore = new Dictionary<uint, List<uint>>();
        public static Dictionary<uint, SpellXSpellVisualEntry> SpellVisualsBySpell = new Dictionary<uint, SpellXSpellVisualEntry>();

        public static ICollection<ItemEffectEntry> ItemEffectLists = new List<ItemEffectEntry>();

        public static void Load()
        {
            if (!Directory.Exists(Settings.Default.DbcPath))
            {
                System.Windows.Forms.FolderBrowserDialog browserDialog = new System.Windows.Forms.FolderBrowserDialog();
                if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    Settings.Default.DbcPath = browserDialog.SelectedPath;
                    Settings.Default.Save();
                }
            }

            foreach (var dbc in typeof(DBC).GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                if (!dbc.FieldType.IsGenericType)
                    continue;

                string extension;
                if (dbc.FieldType.GetGenericTypeDefinition() == typeof(DBCStorage<>))
                    extension = "dbc";
                else if (dbc.FieldType.GetGenericTypeDefinition() == typeof(DB2Storage<>))
                    extension = "db2";
                else
                    continue;

                string name = dbc.Name;

                DataStoreFileNameAttribute[] attributes = dbc.GetCustomAttributes(typeof(DataStoreFileNameAttribute), false) as DataStoreFileNameAttribute[];
                if (attributes.Length == 1)
                    name = attributes[0].FileName;

                try
                {
                    using (var strm = new FileStream($"{Settings.Default.DbcPath}\\{name}.{extension}", FileMode.Open))
                        dbc.FieldType.GetMethod("Load", new Type[] { typeof(FileStream) }).Invoke(dbc.GetValue(null), new object[] { strm });
                }
                catch (DirectoryNotFoundException)
                {
                    throw new DirectoryNotFoundException($"Could not open {dbc.Name}.{extension}!");
                }
                catch (TargetInvocationException tie)
                {
                    if (tie.InnerException is ArgumentException)
                        throw new ArgumentException($"Failed to load {dbc.Name}.{extension}: {tie.InnerException.Message}");

                    throw;
                }
            }

            // this is to speedup spelleffect lookups
            foreach (var effect in SpellEffect)
            {
                uint EffectSpellId = effect.SpellId;
                uint effIdx = effect.Index;
                Difficulty diff = (Difficulty)effect.Difficulty;
                if (!SpellEffectLists.ContainsKey(EffectSpellId))
                {
                    List<SpellEffectEntry> temp = new List<SpellEffectEntry>();
                    SpellEffectLists.Add(EffectSpellId, temp);
                }
                SpellEffectLists[EffectSpellId].Add(effect);

                // triggered spell store
                uint triggerid = effect.TriggerSpell;
                if (SpellTriggerStore.ContainsKey(triggerid))
                {
                    SpellTriggerStore[triggerid].Add(EffectSpellId);
                }
                else
                {
                    List<uint> ids = new List<uint>();
                    ids.Add(EffectSpellId);
                    SpellTriggerStore.Add(triggerid, ids);
                }
            }

            /*foreach (var sp in DBC.SpellPower)
            {
                if (!DBC._spellPower.ContainsKey(sp.SpellId))
                {
                    List<SpellPowerEntry> spl = new List<SpellPowerEntry>();
                    DBC._spellPower.Add(sp.SpellId, spl);
                }
                DBC._spellPower[sp.SpellId].Add(sp);
            }
            DBC.SpellPower.Clear();*/

            foreach (var tr in SpellTargetRestrictions)
            {
                if (!_spellTargetRestrictions.ContainsKey(tr.SpellId))
                {
                    List<SpellTargetRestrictionsEntry> str = new List<SpellTargetRestrictionsEntry>();
                    _spellTargetRestrictions.Add(tr.SpellId, str);
                }
                _spellTargetRestrictions[tr.SpellId].Add(tr);
            }
            SpellTargetRestrictions.Clear();

            foreach (var v in SpellXSpellVisual)
            {
                if (v.DifficultyID != 0 || v.PlayerConditionID != 0)
                    continue;

                if (!SpellVisualsBySpell.ContainsKey(v.SpellID))
                    SpellVisualsBySpell[v.SpellID] = v;
            }

            foreach (var dbcInfo in Spell.Records)
                SpellInfoStore.Add(dbcInfo.Id, new SpellInfoHelper(dbcInfo));

            ItemEffectLists = ItemEffect;
            var itemEffects = ItemEffectLists.GroupBy(itemEffect => itemEffect.ItemID)
                .ToDictionary(group => group.Key, group => group.ToList());

            foreach (var item in ItemSparse)
            {
                int[] itemSpell = new int[5];

                List<ItemEffectEntry> itemEffectList;
                if (itemEffects.TryGetValue(item.Id, out itemEffectList))
                    foreach (var itemEffect in itemEffectList)
                        itemSpell[itemEffect.OrderIndex] = (int)itemEffect.SpellID;

                ItemTemplate.Add(new Item
                {
                    Entry = item.Id,
                    Name = item.Name,
                    Description = item.Description,
                    SpellId = itemSpell
                });
            }
        }

        // DB
        public static List<Item> ItemTemplate = new List<Item>();

        public static uint SelectedLevel = MaxLevel;
        public static uint SelectedItemLevel = 710;
    }
}
