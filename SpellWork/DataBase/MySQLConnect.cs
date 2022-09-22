﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using SpellWork.DBC;
using SpellWork.Extensions;
using SpellWork.Properties;

namespace SpellWork.Database
{
    public static class SqlConnection
    {
        private static MySqlConnection _conn;
        private static MySqlCommand _command;

        public static bool Connected { get; private set; }
        public static List<string> Dropped = new List<string>();
        public static List<SpellProcEventEntry> SpellProcEvent = new List<SpellProcEventEntry>();

        private static String ConnectionString
        {
            get
            {
                if (Settings.Default.Host == ".")
                    return String.Format("Server=localhost;Pipe={0};UserID={1};Password={2};Database={3};CharacterSet=utf8;ConnectionTimeout=5;ConnectionProtocol=Pipe;",
                        Settings.Default.PortOrPipe, Settings.Default.User, Settings.Default.Pass, Settings.Default.WorldDbName);

                return String.Format("Server={0};Port={1};UserID={2};Password={3};Database={4};CharacterSet=utf8;ConnectionTimeout=5;",
                    Settings.Default.Host, Settings.Default.PortOrPipe, Settings.Default.User, Settings.Default.Pass, Settings.Default.WorldDbName);
            }
        }

        private static String GetSpellName(uint id)
        {
            if (DBC.DBCStore.Spell.ContainsKey(id))
                return DBC.DBCStore.Spell[id].SpellNameRank;

            Dropped.Add(String.Format("DELETE FROM `spell_proc_event` WHERE `entry` IN ({0});\r\n", id.ToUInt32()));
            return String.Empty;
        }

        public static void LoadSpellsDBCFromDB()
        {
            using (_conn = new MySqlConnection(ConnectionString))
            {
                string query = @"SELECT Id,Dispel,Mechanic,Attributes,AttributesEx,AttributesEx2,AttributesEx3,AttributesEx4,AttributesEx5,AttributesEx6,AttributesEx7,Stances,StancesNot,Targets,CastingTimeIndex,AuraInterruptFlags,ProcFlags,ProcChance,ProcCharges,MaxLevel,
                    BaseLevel,SpellLevel,DurationIndex,RangeIndex,StackAmount,EquippedItemClass,EquippedItemSubClassMask,EquippedItemInventoryTypeMask,Effect1,Effect2,Effect3,EffectDieSides1,EffectDieSides2,EffectDieSides3,EffectRealPointsPerLevel1,
                    EffectRealPointsPerLevel2,EffectRealPointsPerLevel3,EffectBasePoints1,EffectBasePoints2,EffectBasePoints3,EffectMechanic1,EffectMechanic2,EffectMechanic3,EffectImplicitTargetA1,EffectImplicitTargetA2,EffectImplicitTargetA3,EffectImplicitTargetB1,
                    EffectImplicitTargetB2,EffectImplicitTargetB3,EffectRadiusIndex1,EffectRadiusIndex2,EffectRadiusIndex3,EffectApplyAuraName1,EffectApplyAuraName2,EffectApplyAuraName3,EffectAmplitude1,EffectAmplitude2,EffectAmplitude3,
                    EffectMultipleValue1,EffectMultipleValue2,EffectMultipleValue3,EffectItemType1,EffectItemType2,EffectItemType3,EffectMiscValue1,EffectMiscValue2,EffectMiscValue3,EffectMiscValueB1,EffectMiscValueB2,EffectMiscValueB3,
                    EffectTriggerSpell1,EffectTriggerSpell2,EffectTriggerSpell3,EffectSpellClassMaskA1,EffectSpellClassMaskA2,EffectSpellClassMaskA3,EffectSpellClassMaskB1,EffectSpellClassMaskB2,EffectSpellClassMaskB3,
                    EffectSpellClassMaskC1,EffectSpellClassMaskC2,EffectSpellClassMaskC3,MaxTargetLevel,SpellFamilyName,SpellFamilyFlags1,SpellFamilyFlags2,SpellFamilyFlags3,MaxAffectedTargets,DmgClass,PreventionType,DmgMultiplier1,DmgMultiplier2,DmgMultiplier3,
                    AreaGroupId,SchoolMask,SpellName FROM `spell_dbc` ORDER BY Id ASC;";
                DBC.DBCStore.SpellStringsFromDB.Clear();

                _command = new MySqlCommand(query, _conn);
                _conn.Open();

                using (var reader = _command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int index = 0;
                        uint spellId = reader.GetUInt32(index++);
                        if (DBC.DBCStore.Spell.ContainsKey(spellId))
                            continue;

                        DBC.DBCStore.Spell.Add(spellId, new SpellEntry
                        {
                            ID = spellId,
                            Dispel = reader.GetUInt32(index++),
                            Mechanic = reader.GetUInt32(index++),
                            Attributes = reader.GetUInt32(index++),
                            AttributesEx = reader.GetUInt32(index++),
                            AttributesEx2 = reader.GetUInt32(index++),
                            AttributesEx3 = reader.GetUInt32(index++),
                            AttributesEx4 = reader.GetUInt32(index++),
                            AttributesEx5 = reader.GetUInt32(index++),
                            AttributesEx6 = reader.GetUInt32(index++),
                            AttributesEx7 = reader.GetUInt32(index++),
                            Stances = reader.GetUInt32(index++),
                            StancesNot = reader.GetUInt32(index++),
                            Targets = reader.GetUInt32(index++),
                            CastingTimeIndex = reader.GetUInt32(index++),
                            AuraInterruptFlags = reader.GetUInt32(index++),
                            ProcFlags = reader.GetUInt32(index++),
                            ProcChance = reader.GetUInt32(index++),
                            ProcCharges = reader.GetUInt32(index++),
                            MaxLevel = reader.GetUInt32(index++),
                            BaseLevel = reader.GetUInt32(index++),
                            SpellLevel = reader.GetUInt32(index++),
                            DurationIndex = reader.GetUInt32(index++),
                            RangeIndex = reader.GetUInt32(index++),
                            StackAmount = reader.GetUInt32(index++),
                            EquippedItemClass = reader.GetInt32(index++),
                            EquippedItemSubClassMask = reader.GetInt32(index++),
                            EquippedItemInventoryTypeMask = reader.GetInt32(index++),
                            Effect = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectDieSides = new[]
                            {
                                reader.GetInt32(index++),
                                reader.GetInt32(index++),
                                reader.GetInt32(index++),
                            },
                            EffectRealPointsPerLevel = new[]
                            {
                                reader.GetFloat(index++),
                                reader.GetFloat(index++),
                                reader.GetFloat(index++),
                            },
                            EffectBasePoints = new[]
                            {
                                reader.GetInt32(index++),
                                reader.GetInt32(index++),
                                reader.GetInt32(index++),
                            },
                            EffectMechanic = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectImplicitTargetA = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectImplicitTargetB = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectRadiusIndex = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectApplyAuraName = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectAmplitude = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectMultipleValue = new[]
                            {
                                reader.GetFloat(index++),
                                reader.GetFloat(index++),
                                reader.GetFloat(index++),
                            },
                            EffectItemType = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectMiscValue = new[]
                            {
                                reader.GetInt32(index++),
                                reader.GetInt32(index++),
                                reader.GetInt32(index++),
                            },
                            EffectMiscValueB = new[]
                            {
                                reader.GetInt32(index++),
                                reader.GetInt32(index++),
                                reader.GetInt32(index++),
                            },
                            EffectTriggerSpell = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectSpellClassMaskA = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectSpellClassMaskB = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            EffectSpellClassMaskC = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            MaxTargetLevel = reader.GetUInt32(index++),
                            SpellFamilyName = reader.GetUInt32(index++),
                            SpellFamilyFlags = new[]
                            {
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                                reader.GetUInt32(index++),
                            },
                            MaxAffectedTargets = reader.GetUInt32(index++),
                            DmgClass = reader.GetUInt32(index++),
                            PreventionType = reader.GetUInt32(index++),
                            DmgMultiplier = new[]
                            {
                                reader.GetFloat(index++),
                                reader.GetFloat(index++),
                                reader.GetFloat(index++),
                            },
                            AreaGroupId = reader.GetInt32(index++),
                            SchoolMask = reader.GetUInt32(index++),
                            SpellVisual = new uint[2],
                            Reagent = new int[DBC.DBCStore.MaxReagentCount],
                            ReagentCount = new uint[DBC.DBCStore.MaxReagentCount],
                            EffectPointsPerComboPoint = new float[DBC.DBCStore.MaxEffectIndex],
                            EffectChainTarget = new uint[DBC.DBCStore.MaxEffectIndex],
                            DamageCoeficient = new float[DBC.DBCStore.MaxEffectIndex]
                        });

                        string DBName = reader.GetString(index);
                        DBName += " (SERVERSIDE)";
                        DBC.DBCStore.SpellStringsFromDB.Add(spellId, DBName);
                    }
                }

                var sortedList = new List<KeyValuePair<uint, SpellEntry>>(DBC.DBCStore.Spell);
                sortedList.Sort(
                    delegate (KeyValuePair<uint, SpellEntry> firstPair,
                    KeyValuePair<uint, SpellEntry> nextPair)
                    {
                        return firstPair.Key.CompareTo(nextPair.Key);
                    });

                DBC.DBCStore.Spell.Clear();
                foreach (KeyValuePair<uint, SpellEntry> pair in sortedList)
                    DBC.DBCStore.Spell.Add(pair.Key, pair.Value);

                _conn.Close();
            }
        }

        public static void LoadSpellDifficultyFromDB()
        {
            using (_conn = new MySqlConnection(ConnectionString))
            {
                var query = @"SELECT id, spellid0, spellid1, spellid2, spellid3 FROM spelldifficulty_dbc ORDER BY id ASC";
                _command = new MySqlCommand(query, _conn);
                _conn.Open();

                using (var reader = _command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint spellId = reader.GetUInt32(0);

                        if (!DBC.DBCStore.Spell.ContainsKey(spellId) || DBC.DBCStore.Spell[spellId].SpellDifficultyId != 0)
                            continue;

                        if (DBC.DBCStore.SpellDifficulty.ContainsKey(spellId))
                            continue;

                        SpellDifficultyEntry diffData = new SpellDifficultyEntry { Id = spellId, SpellId = new uint[4] };

                        for (int i = 0; i < 4; ++i)
                        {
                            diffData.SpellId[i] = reader.GetUInt32(i + 1);

                            // Set spell difficulty entry to related spells
                            if (!DBC.DBCStore.Spell.ContainsKey(diffData.SpellId[i]))
                                continue;

                            SpellEntry entry = DBC.DBCStore.Spell[diffData.SpellId[i]];
                            entry.SpellDifficultyId = spellId;
                            DBC.DBCStore.Spell[diffData.SpellId[i]] = entry;
                        }

                        // Insert difficulty data
                        DBC.DBCStore.SpellDifficulty.Add(spellId, diffData);
                    }
                }

                _conn.Close();
            }
        }

        public static void LoadCustomSpellAttributes()
        {
            using (_conn = new MySqlConnection(ConnectionString))
            {
                var query = @"SELECT entry, attributes FROM spell_custom_attr ORDER BY entry ASC";
                DBC.DBCStore._spellCustomAttributes.Clear();
                _command = new MySqlCommand(query, _conn);
                _conn.Open();

                using (var reader = _command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint spellId = reader.GetUInt32(0);
                        if (!DBC.DBCStore.Spell.ContainsKey(spellId))
                            continue;

                        uint attributes = reader.GetUInt32(1);
                        if (attributes == 0)
                            continue;

                        DBC.DBCStore._spellCustomAttributes.Add(spellId, attributes);
                    }
                }
            }
        }

        public static void SelectProc(string query)
        {
            using (_conn = new MySqlConnection(ConnectionString))
            {
                _command = new MySqlCommand(query, _conn);
                _conn.Open();
                SpellProcEvent.Clear();

                using (var reader = _command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        uint spellId = reader.GetUInt32(0);
                        SpellProcEvent.Add(new SpellProcEventEntry
                        {
                            Id                  = spellId,
                            SpellName           = GetSpellName(spellId),
                            SchoolMask          = reader.GetByte(1),
                            SpellFamilyName     = reader.GetUInt16(2),
                            SpellFamilyMask     = new[]
                            {
                                reader.GetUInt32(3),
                                reader.GetUInt32(4),
                                reader.GetUInt32(5)
                            },
                            ProcFlags           = reader.GetUInt32(6),
                            ProcEx              = reader.GetUInt32(7),
                            PpmRate             = reader.GetFloat(8),
                            CustomChance        = reader.GetFloat(9),
                            Cooldown            = reader.GetUInt32(10)
                        });
                    }
                }
            }
        }

        public static void Insert(string query)
        {
            _conn    = new MySqlConnection(ConnectionString);
            _command = new MySqlCommand(query, _conn);
            _conn.Open();
            _command.ExecuteNonQuery();
            _command.Connection.Close();
        }

        public static List<Item> SelectItems()
        {
            var items = DBC.DBCStore.ItemTemplate;
            // In order to reduce the search time, we make the first selection of all items that have spellid
            var query = String.Format(
                @"SELECT    t.entry,
                            t.name,
                            t.description,
                            l.Name,
                            l.Description,
                            t.spellid_1,
                            t.spellid_2,
                            t.spellid_3,
                            t.spellid_4,
                            t.spellid_5
                FROM
                    `item_template` t
                LEFT JOIN
                    `item_template_locale` l
                ON
                    t.entry = l.ID && (l.locale = '{0}' || l.locale IS NULL)
                WHERE
                    (t.spellid_1 > 0 || t.spellid_2 > 0 || t.spellid_3 > 0 || t.spellid_4 > 0 || t.spellid_5 > 0);",
                Enum.GetName(typeof(Spell.LocalesDBC), DBC.DBCStore.Locale));

            using (_conn = new MySqlConnection(ConnectionString))
            {
                _command = new MySqlCommand(query, _conn);
                _conn.Open();

                using (var reader = _command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new Item
                        {
                            Entry               = reader.GetUInt32(0),
                            Name                = reader.GetString(1),
                            Description         = reader.GetString(2),
                            LocalesName         = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                            LocalesDescription  = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                            SpellId             = new[]
                            {
                                reader.GetInt32(5),
                                reader.GetInt32(6),
                                reader.GetInt32(7),
                                reader.GetInt32(8),
                                reader.GetInt32(9)
                            }
                        });
                    }
                }
             }
            return items;
        }

        public static void TestConnect()
        {
            if (!Settings.Default.UseDbConnect)
            {
                Connected = false;
                return;
            }

            try
            {
                _conn = new MySqlConnection(ConnectionString);
                _conn.Open();
                _conn.Close();
                Connected = true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(string.Format("Errno {0}{1}{2}", ex.Number, Environment.NewLine, ex.Message));
                Connected = false;
            }
            catch
            {
                Connected = false;
            }
        }
    }
}
