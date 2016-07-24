using System.Linq;
using System.Text;
using SpellWork.DBC.Structures;
using SpellWork.Extensions;
using System.Collections.Generic;

namespace SpellWork.Spell
{
    public sealed class SpellInfoHelper
    {
        public uint ID;
        public uint Category;
        public uint Dispel;
        public uint Mechanic;
        public uint Attributes;
        public uint AttributesEx;
        public uint AttributesEx2;
        public uint AttributesEx3;
        public uint AttributesEx4;
        public uint AttributesEx5;
        public uint AttributesEx6;
        public uint AttributesEx7;
        public uint AttributesEx8;
        public uint AttributesEx9;
        public uint AttributesEx10;
        public uint AttributesEx11;
        public uint AttributesEx12;
        public uint AttributesEx13;
        public ulong Stances;
        public ulong StancesNot;
        public uint RequiresSpellFocus;
        public uint CasterAuraState;
        public uint TargetAuraState;
        public uint CasterAuraStateNot;
        public uint TargetAuraStateNot;
        public uint CasterAuraSpell;
        public uint TargetAuraSpell;
        public uint ExcludeCasterAuraSpell;
        public uint ExcludeTargetAuraSpell;
        public uint CastingTimeIndex;
        public uint RecoveryTime;
        public uint CategoryRecoveryTime;
        public uint InterruptFlags;
        public uint[] AuraInterruptFlags;
        public uint[] ChannelInterruptFlags;
        public uint ProcFlags;
        public uint ProcChance;
        public uint ProcCharges;
        public uint MaxLevel;
        public uint BaseLevel;
        public uint SpellLevel;
        public uint DurationIndex;
        public uint RangeIndex;
        public float Speed;
        public uint ModalNextSpell;
        public uint StackAmount;
        public uint[] Totem;
        public uint[] Reagent;
        public uint[] ReagentCount;
        public uint EquippedItemClass;
        public uint EquippedItemSubClassMask;
        public uint EquippedItemInventoryTypeMask;
        public List<SpellEffectEntry> Effects = new List<SpellEffectEntry>();
        public uint[] SpellVisual = new uint[2];
        public uint SpellIconID;
        public uint ActiveIconID;
        public string SpellName;
        public string Rank;
        public string _Description;
        public string ToolTip;
        public uint StartRecoveryCategory;
        public uint StartRecoveryTime;
        public uint SpellFamilyName;
        public uint[] SpellFamilyFlags;
        public uint DmgClass;
        public uint PreventionType;
        public int AreaGroupId;
        public uint SchoolMask;
        public uint SpellDescriptionVariableID;

        public SpellScalingEntry Scaling;
        public List<SpellTargetRestrictionsEntry> SpellTargetRestrictions;
        public float ProcPPMRate;
        public byte ProcPPMFlags;

        public string CastTime
        {
            get
            {
                if (CastingTimeIndex != 0)
                {
                    var level = DBC.DBC.SelectedLevel;
                    if (Scaling != null && level > Scaling.MaxScalingLevel)
                        level = Scaling.MaxScalingLevel;

                    if (((SpellAtributeEx13) AttributesEx13).HasFlag(SpellAtributeEx13.SPELL_ATTR13_UNK17))
                        level *= 5;

                    if (Scaling != null && level > Scaling.MaxScalingLevel)
                        level = Scaling.MaxScalingLevel;

                    if (BaseLevel != 0)
                        level -= BaseLevel;

                    if (Scaling != null && level < Scaling.MinScalingLevel)
                        level = Scaling.MinScalingLevel;

                    var castTimeEntry = DBC.DBC.SpellCastTimes[(int)CastingTimeIndex];
                    var castTime = castTimeEntry.CastTime + castTimeEntry.CastTimePerLevel*level;
                    if (castTime < castTimeEntry.MinCastTime)
                        castTime = castTimeEntry.MinCastTime;

                    return $"Cast Time (Id #{CastingTimeIndex}) = {castTime}";
                }

                return string.Empty;
            }
        }

        public string Duration => DBC.DBC.SpellDuration.ContainsKey((int)DurationIndex) ? DBC.DBC.SpellDuration[(int)DurationIndex].ToString() : string.Empty;

        public string ProcInfo
        {
            get
            {
                var i = 0;
                var sb = new StringBuilder();
                var proc = ProcFlags;
                while (proc != 0)
                {
                    if ((proc & 1) != 0)
                        sb.AppendFormatLine("  {0}", SpellEnums.ProcFlagDesc[i]);
                    ++i;
                    proc >>= 1;
                }

                return sb.ToString();
            }
        }

        public string Range
        {
            get
            {
                if (RangeIndex == 0 || !DBC.DBC.SpellRange.ContainsKey((int)RangeIndex))
                    return string.Empty;

                var range = DBC.DBC.SpellRange[(int)RangeIndex];
                var sb = new StringBuilder();
                sb.AppendFormatLine("SpellRange: (Id {0}) \"{1}\":", RangeIndex, range.DisplayName);
                sb.AppendFormatLine("    MinRangeNegative = {0}, MinRangePositive = {1}", range.MinRange[0], range.MinRange[1]);
                sb.AppendFormatLine("    MaxRangeNegative = {0}, MaxRangePositive = {1}", range.MaxRange[0], range.MaxRange[1]);

                return sb.ToString();
            }
        }

        public SpellSchoolMask School => (SpellSchoolMask)SchoolMask;

        public string SpellNameRank => string.IsNullOrEmpty(Rank) ? SpellName : $"{SpellName} ({Rank})";

        public string ScalingText => Scaling != null ? $" (Level {DBC.DBC.SelectedLevel})" : string.Empty;

        public string Description
        {
            get
            {
                var sb = new StringBuilder();
                if (!string.IsNullOrEmpty(_Description))
                    sb.AppendFormatLine("Description: {0}", _Description);

                if (SpellDescriptionVariableID == 0 || !DBC.DBC.SpellDescriptionVariables.ContainsKey(SpellDescriptionVariableID))
                    return sb.ToString();

                var sdesc = DBC.DBC.SpellDescriptionVariables[SpellDescriptionVariableID];
                sb.AppendFormatLine("Description variable Id: {0}", SpellDescriptionVariableID);
                sb.AppendFormatLine("Description variable: {0}", sdesc.Variables);
                return sb.ToString();
            }
        }

        public SpellInfoHelper(SpellInfoLoadData dbcData)
        {
            ID = dbcData.Spell.ID;
            var miscs = dbcData.Misc;
            if (miscs != null)
            {
                Attributes = dbcData.Misc.Attributes[0];
                AttributesEx = dbcData.Misc.Attributes[1];
                AttributesEx2 = dbcData.Misc.Attributes[2];
                AttributesEx3 = dbcData.Misc.Attributes[3];
                AttributesEx4 = dbcData.Misc.Attributes[4];
                AttributesEx5 = dbcData.Misc.Attributes[5];
                AttributesEx6 = dbcData.Misc.Attributes[6];
                AttributesEx7 = dbcData.Misc.Attributes[7];
                AttributesEx8 = dbcData.Misc.Attributes[8];
                AttributesEx9 = dbcData.Misc.Attributes[9];
                AttributesEx10 = dbcData.Misc.Attributes[10];
                AttributesEx11 = dbcData.Misc.Attributes[11];
                AttributesEx12 = dbcData.Misc.Attributes[12];
                AttributesEx13 = dbcData.Misc.Attributes[13];
                CastingTimeIndex = dbcData.Misc.CastingTimeIndex;
                DurationIndex = dbcData.Misc.DurationIndex;
                //PowerType = dbcData.MiscEntry.PowerType;
                RangeIndex = dbcData.Misc.RangeIndex;
                Speed = dbcData.Misc.Speed;
                SpellIconID = dbcData.Misc.SpellIconID;
                ActiveIconID = dbcData.Misc.ActiveIconID;
                SchoolMask = dbcData.Misc.SchoolMask;
            }
            SpellName = dbcData.Spell.Name;
            _Description = dbcData.Spell.Description;
            ToolTip = dbcData.Spell.AuraDescription;
            // SpellMissileID = dbcData.SpellMissileID;
            // SpellDescriptionVariableID = dbcData.SpellDescriptionVariableID;
            // SpellDifficultyId = dbcData.SpellDifficultyId;

            // SpellCategories.dbc
            var cat = dbcData.Categories;
            if (cat != null)
            {
                Category = cat.Category;
                Dispel = cat.DispelType;
                Mechanic = cat.Mechanic;
                StartRecoveryCategory = cat.StartRecoveryCategory;
                DmgClass = cat.DefenseType;
                PreventionType = cat.PreventionType;
            }

            // SpellShapeshift.dbc
            var shapeshift = dbcData.Shapeshift;
            if (shapeshift != null)
            {
                Stances = (shapeshift.ShapeshiftMask[0] << 32) | shapeshift.ShapeshiftMask[1];
                StancesNot = (shapeshift.ShapeshiftExclude[0] << 32) | shapeshift.ShapeshiftExclude[1];
            }

            // SpellTargetRestrictions.dbc
            SpellTargetRestrictions = dbcData.TargetRestrictions.ToList();

            // SpellCastingRequirements.dbc
            var castingRequirements = dbcData.CastingRequirements;
            if (castingRequirements != null)
            {
                RequiresSpellFocus = castingRequirements.RequiresSpellFocus;
                AreaGroupId = castingRequirements.RequiredAreasID;
            }

            // SpellAuraRestrictions.dbc
            var auraRestrictions = dbcData.AuraRestrictions;
            if (auraRestrictions != null)
            {
                CasterAuraState = auraRestrictions.CasterAuraState;
                TargetAuraState = auraRestrictions.TargetAuraState;
                CasterAuraStateNot = auraRestrictions.ExcludeCasterAuraState;
                TargetAuraStateNot = auraRestrictions.ExcludeTargetAuraState;
                CasterAuraSpell = auraRestrictions.CasterAuraSpell;
                TargetAuraSpell = auraRestrictions.TargetAuraSpell;
                ExcludeCasterAuraSpell = auraRestrictions.ExcludeCasterAuraSpell;
                ExcludeTargetAuraSpell = auraRestrictions.ExcludeTargetAuraSpell;
            }

            // SpellCooldowns.dbc
            var cooldowns = dbcData.Cooldowns;
            if (cooldowns != null)
            {
                RecoveryTime = cooldowns.RecoveryTime;
                CategoryRecoveryTime = cooldowns.CategoryRecoveryTime;
                StartRecoveryTime = cooldowns.StartRecoveryTime;
            }

            // SpellInterrupts.dbc
            var interrupts = dbcData.Interrupts;
            if (interrupts != null)
            {
                InterruptFlags = interrupts.InterruptFlags;
                AuraInterruptFlags = (uint[]) interrupts.AuraInterruptFlags.Clone();
                ChannelInterruptFlags = (uint[]) interrupts.ChannelInterruptFlags.Clone();
            }
            else
            {
                AuraInterruptFlags = new uint[2];
                ChannelInterruptFlags = new uint[2];
            }

            // SpellAuraOptions.dbc
            var options = dbcData.AuraOptions;
            if (options != null)
            {
                ProcFlags = options.ProcTypeMask;
                ProcChance = options.ProcChance;
                ProcCharges = options.ProcCharges;
                StackAmount = options.ProcCharges;
                if (dbcData.ProcsPerMinute != null)
                {
                    ProcPPMRate = dbcData.ProcsPerMinute.BaseProcRate;
                    ProcPPMFlags = dbcData.ProcsPerMinute.Flags;
                }
            }

            // SpellLevels.dbc
            var levels = dbcData.Levels;
            if (levels != null)
            {
                MaxLevel = levels.MaxLevel;
                BaseLevel = levels.BaseLevel;
                SpellLevel = levels.SpellLevel;
            }

            // SpellPowerList = dbcData.SpellPowerList;


            // SpellClassOptions
            var classOptions = dbcData.ClassOptions;
            if (classOptions != null)
            {
                ModalNextSpell = classOptions.ModalNextSpell;
                SpellFamilyName = classOptions.SpellClassSet;
                SpellFamilyFlags = (uint[])classOptions.SpellFamilyFlags.Clone();
            }
            else
                SpellFamilyFlags = new uint[4];

            // SpellTotems.db2
            var totems = dbcData.Totems;
            if (totems != null)
            {
                Totem = (uint[])totems.Totem.Clone();
            }
            else
            {
                Totem = new uint[2];
            }

            // SpellReagents.dbc
            var reagents = dbcData.Reagents;
            if (reagents != null)
            {
                Reagent = (uint[])reagents.Reagent.Clone();
                ReagentCount = (uint[])reagents.ReagentCount.Clone();
            }
            else
            {
                Reagent = new uint[8];
                ReagentCount = new uint[8];
            }

            // SpellEquippedItems.dbc
            var equippedItems = dbcData.EquippedItems;
            if (equippedItems != null)
            {
                EquippedItemClass = equippedItems.EquippedItemClass;
                EquippedItemSubClassMask = equippedItems.EquippedItemSubClassMask;
                EquippedItemInventoryTypeMask = equippedItems.EquippedItemInventoryTypeMask;
            }

            /*var visuals = dbcData.Visuals;
            if (visuals != null)
            {
                SpellVisual[0] = visuals.SpellVisual[0];
                SpellVisual[1] = visuals.SpellVisual[1];
            }*/

            Scaling = dbcData.Scaling;
            Effects = new List<SpellEffectEntry>();

            Effects = dbcData.Effects.ToList();
        }

        public bool HasEffect(SpellEffects effect)
        {
            return Effects.Any(eff => eff != null && eff.Effect == (uint)effect);
        }

        public bool HasAura(AuraType aura)
        {
            return Effects.Any(eff => eff != null && eff.EffectAura == (uint)aura);
        }

        public bool HasTargetA(Targets target)
        {
            return Effects.Any(eff => eff != null && eff.ImplicitTarget[0] == (uint)target);
        }

        public bool HasTargetB(Targets target)
        {
            return Effects.Any(eff => eff != null && eff.ImplicitTarget[1] == (uint)target);
        }
    }
}
