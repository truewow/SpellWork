using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Windows.Forms;
using SpellWorkLib.DBC;
using SpellWorkLib.Extensions;

namespace SpellWorkLib.Spell
{
    public class NamedId
    {
        public ulong Id { get; set; }
        public string Name { get; set; }

        public static NamedId Build<T>(ulong id) where T : struct, IConvertible
        {
            string name;

            try
            {
                name = Enum.GetName(typeof(T), id);

            }
            catch (ArgumentException)
            {
                name = "Error";
            }

            return new NamedId { Id = id, Name = name };
        }

        public static NamedId Build(ulong id, string name)
        {
            return new NamedId { Id = id, Name = name };
        }
    }

    public class SpellInfo
    {
        private SpellEntry _spell;

        public SpellInfo(SpellEntry spell)
        {
            _spell = spell;

            ViewSpellInfo();
        }

        private void ViewSpellInfo()
        {
            dynamic obj = new ExpandoObject();

            obj.Id = new ExpandoObject();
            obj.Id.Entry = _spell.ID;
            obj.Id.Rank = _spell.SpellNameRank;

            obj.Description = _spell.Description;
            obj.ToolTip = _spell.ToolTip;
            obj.ModalNextSpell = _spell.ModalNextSpell;
            obj.Category = _spell.Category;
            obj.SpellIconID = _spell.SpellIconID;
            obj.ActiveIconID = _spell.ActiveIconID;
            obj.SpellVisual = _spell.SpellVisual;

            obj.Family = new ExpandoObject();
            obj.Family.Name = NamedId.Build<SpellFamilyNames>(_spell.SpellFamilyName);
            obj.Family.Flags = _spell.SpellFamilyFlags;

            obj.SchoolMask = NamedId.Build<SpellSchoolMask>(_spell.SchoolMask);
            obj.DmgClass = NamedId.Build<SpellDmgClass>(_spell.DmgClass);
            obj.PreventionType = NamedId.Build<SpellPreventionType>(_spell.PreventionType);

            obj.Attributes = NamedId.Build<SpellAtribute>(_spell.Attributes);
            obj.AttributesEx = NamedId.Build<SpellAtributeEx>(_spell.AttributesEx);
            obj.AttributesEx2 = NamedId.Build<SpellAtributeEx2>(_spell.AttributesEx2);
            obj.AttributesEx3 = NamedId.Build<SpellAtributeEx3>(_spell.AttributesEx3);
            obj.AttributesEx4 = NamedId.Build<SpellAtributeEx4>(_spell.AttributesEx4);
            obj.AttributesEx5 = NamedId.Build<SpellAtributeEx5>(_spell.AttributesEx5);
            obj.AttributesEx6 = NamedId.Build<SpellAtributeEx6>(_spell.AttributesEx6);
            obj.AttributesEx7 = NamedId.Build<SpellAtributeEx7>(_spell.AttributesEx7);

            obj.TargetsMask = NamedId.Build<SpellCastTargetFlags>(_spell.Targets);
            obj.CreatureTypeMask = NamedId.Build<CreatureTypeMask>(_spell.TargetCreatureType);
            obj.Stances = NamedId.Build<ShapeshiftFormMask>(_spell.Stances);
            obj.StancesNot = NamedId.Build<ShapeshiftFormMask>(_spell.StancesNot);

            AppendSkillLine(obj);

            obj.Reagents = _spell.Reagent.Select((r, idx) => new {Id = r, Count = _spell.ReagentCount[idx]})
                .Where(r => r.Id != 0);
            
            obj.Level = new ExpandoObject();
            obj.Level.SpellLevel = _spell.SpellLevel;
            obj.Level.BaseLevel = _spell.BaseLevel;
            obj.Level.MaxLevel = _spell.MaxLevel;
            obj.Level.MaxTargetLevel = _spell.MaxTargetLevel;


            if (_spell.EquippedItemClass != -1)
            {
                obj.EquippedItem = new ExpandoObject();

                obj.EquippedItem.Class = NamedId.Build<ItemClass>((ulong)_spell.EquippedItemClass);


                if (_spell.EquippedItemSubClassMask != 0)
                {
                    switch ((ItemClass) _spell.EquippedItemClass)
                    {
                        case ItemClass.WEAPON:
                            obj.EquippedItem.SubClass = NamedId.Build<ItemSubClassWeaponMask>((ulong) _spell.EquippedItemSubClassMask);
                            break;
                        case ItemClass.ARMOR:
                            obj.EquippedItem.SubClass = NamedId.Build<ItemSubClassArmorMask>((ulong)_spell.EquippedItemSubClassMask);
                            break;
                        case ItemClass.MISC:
                            obj.EquippedItem.SubClass = NamedId.Build<ItemSubClassMiscMask>((ulong)_spell.EquippedItemSubClassMask);
                            break;
                    }
                }

                if (_spell.EquippedItemInventoryTypeMask != 0)
                    obj.EquippedItem.EquippedItemInventoryTypeMask = NamedId.Build<InventoryTypeMask>((ulong) _spell.EquippedItemInventoryTypeMask);
            }

            obj.DispelType = NamedId.Build<DispelType>(_spell.Dispel);
            obj.Mechanic = NamedId.Build<Mechanics>(_spell.Mechanic);

            obj.Range = _spell.RangeObj;

            if (_spell.Speed != 0.0f)
                obj.Speed = _spell.Speed;
            if (_spell.StackAmount != 0)
                obj.StackAmount = _spell.StackAmount;

            obj.CastTime = _spell.CastTimeObj;

            if (_spell.RecoveryTime != 0 || _spell.CategoryRecoveryTime != 0 || _spell.StartRecoveryCategory != 0)
            {
                obj.Recovery = new ExpandoObject();
                obj.Recovery.Time = _spell.RecoveryTime;
                obj.Recovery.CategoryTime = _spell.CategoryRecoveryTime;
                obj.Recovery.StartCategory = _spell.StartRecoveryCategory;
                obj.Recovery.StartTime = _spell.StartRecoveryTime;
            }

            obj.Duration = _spell.Duration;

            if (_spell.ManaCost != 0 || _spell.ManaCostPercentage != 0 || _spell.PowerType != 0 ||
                _spell.ManaCostPerlevel != 0 || _spell.ManaPerSecond != 0 || _spell.ManaPerSecondPerLevel != 0)
            {
                obj.Power = (Powers) _spell.PowerType;
                if (_spell.ManaCost != 0)
                    obj.ManaCost = _spell.ManaCost;
                else
                    obj.ManaCostPercentage = _spell.ManaCostPercentage;

                if (_spell.ManaCostPerlevel != 0)
                    obj.ManaCostPerlevel = _spell.ManaCostPerlevel;

                if (_spell.ManaPerSecond != 0)
                    obj.ManaPerSecond = _spell.ManaPerSecond;

                if (_spell.ManaPerSecondPerLevel != 0)
                    obj.ManaPerSecondPerLevel = _spell.ManaPerSecondPerLevel;
            }

            obj.Interrupt = new ExpandoObject();
            obj.Interrupt.Flags = _spell.InterruptFlags;
            obj.Interrupt.AuraFlags = _spell.AuraInterruptFlags;
            obj.Interrupt.ChannelFlags = _spell.ChannelInterruptFlags;

            if (_spell.CasterAuraState != 0)
                obj.CasterAuraState = NamedId.Build<AuraState>(_spell.CasterAuraState);

            if (_spell.TargetAuraState != 0)
                obj.TargetAuraState = NamedId.Build<AuraState>(_spell.TargetAuraState);

            if (_spell.CasterAuraStateNot != 0)
                obj.CasterAuraStateNot = NamedId.Build<AuraState>(_spell.CasterAuraStateNot);

            if (_spell.TargetAuraStateNot != 0)
                obj.TargetAuraStateNot = NamedId.Build<AuraState>(_spell.TargetAuraStateNot);

            if (_spell.MaxAffectedTargets != 0)
                obj.MaxAffectedTargets = _spell.MaxAffectedTargets;

            AppendSpellAura(obj);

            AppendAreaInfo(obj);

            if (_spell.RequiresSpellFocus != 0)
                obj.RequiresSpellFocus = _spell.RequiresSpellFocus;

            obj.Proc = new ExpandoObject();
            obj.Proc.Chance = _spell.ProcChance;
            obj.Proc.Charges = _spell.ProcCharges;

            if (_spell.ProcFlags != 0)
            {
                obj.Proc.Flags = _spell.ProcFlags;
                obj.Proc.Info = _spell.ProcInfo;
            }

            AppendSpellEffectInfo(obj);
            AppendDifficultyInfo(obj);

            AppendSpellVisualInfo(obj);
        }

        private void AppendSpellVisualInfo(dynamic obj)
        {
            SpellVisualEntry visualData;
            if (!DBC.DBC.SpellVisual.TryGetValue(_spell.SpellVisual[0], out visualData))
                return;

            SpellMissileEntry missileEntry;
            SpellMissileMotionEntry missileMotionEntry;
            var hasMissileEntry = DBC.DBC.SpellMissile.TryGetValue(visualData.MissileModel, out missileEntry);
            var hasMissileMotion = DBC.DBC.SpellMissileMotion.TryGetValue(visualData.MissileMotionId, out missileMotionEntry);

            if (!hasMissileEntry && !hasMissileMotion)
                return;

            // Missile Model Data.
            if (hasMissileEntry)
            {
                obj.MissileModel = new ExpandoObject();
                obj.MissileModel.Id = visualData.MissileModel;
                obj.MissileModel.Attachment = visualData.MissileAttachment;
                obj.MissileModel.CastOffset = new ExpandoObject();
                obj.MissileModel.CastOffset.X = visualData.MissileCastOffsetX;
                obj.MissileModel.CastOffset.Y = visualData.MissileCastOffsetY;
                obj.MissileModel.CastOffset.Z = visualData.MissileCastOffsetZ;
                obj.MissileModel.ImpactOffset = new ExpandoObject();
                obj.MissileModel.ImpactOffset.X = visualData.MissileImpactOffsetX;
                obj.MissileModel.ImpactOffset.Y = visualData.MissileImpactOffsetY;
                obj.MissileModel.ImpactOffset.Z = visualData.MissileImpactOffsetZ;
                obj.MissileModel.MissileEntry = new ExpandoObject();
                obj.MissileModel.MissileEntry.Id = missileEntry.Id;
                obj.MissileModel.MissileEntry.CollisionRadius = missileEntry.collisionRadius;
                obj.MissileModel.MissileEntry.DefaultPitch = new ExpandoObject();
                obj.MissileModel.MissileEntry.DefaultPitch.Min = missileEntry.defaultPitchMin;
                obj.MissileModel.MissileEntry.DefaultPitch.Max = missileEntry.defaultPitchMax;
                obj.MissileModel.MissileEntry.RandomPitch = new ExpandoObject();
                obj.MissileModel.MissileEntry.RandomPitch.Min = missileEntry.randomizePitchMax;
                obj.MissileModel.MissileEntry.RandomPitch.Max = missileEntry.randomizePitchMax;
                obj.MissileModel.MissileEntry.DefaultSpeed = new ExpandoObject();
                obj.MissileModel.MissileEntry.DefaultSpeed.Min = missileEntry.defaultSpeedMin;
                obj.MissileModel.MissileEntry.DefaultSpeed.Max = missileEntry.defaultSpeedMax;
                obj.MissileModel.MissileEntry.RandomizeSpeed = new ExpandoObject();
                obj.MissileModel.MissileEntry.RandomizeSpeed.Min = missileEntry.randomizeSpeedMin;
                obj.MissileModel.MissileEntry.RandomizeSpeed.Max = missileEntry.randomizeSpeedMax;
                obj.MissileModel.Gravity = missileEntry.gravity;
                obj.MissileModel.MaximumDuration = missileEntry.maxDuration;
            }

            // Missile Motion Data.
            if (hasMissileMotion)
            {
                obj.MissileMotion = new ExpandoObject();
                obj.MissileMotion.Name = missileMotionEntry.Name;
                obj.MissileMotion.Count = missileMotionEntry.MissileCount;
                obj.MissileMotion.Script = missileMotionEntry.Script;
            }
        }

        private void AppendSkillLine(dynamic obj)
        {
            var query = (from skillLineAbility in DBC.DBC.SkillLineAbility
                        join skillLine in DBC.DBC.SkillLine
                        on skillLineAbility.Value.SkillId equals skillLine.Key
                        where skillLineAbility.Value.SpellId == _spell.ID
                        select new
                        {
                            skillLineAbility,
                            skillLine
                        }).FirstOrDefault();

            if (query == null)
                return;

            var skill = query.skillLineAbility.Value;
            var line = query.skillLine.Value;

            obj.Skill = new ExpandoObject();
            obj.Skill.Id = skill.SkillId;
            obj.Skill.Name = line.Name;
            obj.Skill.ReqSkillValue = skill.ReqSkillValue;
            obj.Skill.ForwardSpell = skill.ForwardSpellid;
            obj.Skill.MinValue = skill.MinValue;
            obj.Skill.MaxValue = skill.MaxValue;
            obj.Skill.CharacterPoints = skill.CharacterPoints;
        }

        private void AppendSpellEffectInfo(dynamic obj)
        {
            var effects = new List<dynamic>();
            for (var effectIndex = 0; effectIndex < DBC.DBC.MaxEffectIndex; effectIndex++)
            {
                dynamic effect = new ExpandoObject();
                if ((SpellEffects)_spell.Effect[effectIndex] == SpellEffects.NO_SPELL_EFFECT)
                {
                    effect.Effect = "NO EFFECT";
                    effects.Add(effect);
                    continue;
                }

                effect.Effect = NamedId.Build<SpellEffects>(_spell.Effect[effectIndex]);
                effect.BasePoints = _spell.EffectBasePoints[effectIndex] + 1;

                if (_spell.EffectRealPointsPerLevel[effectIndex] != 0)
                    effect.RealPointsPerLevel = _spell.EffectRealPointsPerLevel[effectIndex];

                if (_spell.EffectDieSides[effectIndex] != 0)
                    effect.DieSides = _spell.EffectDieSides[effectIndex];

                if (_spell.EffectPointsPerComboPoint[effectIndex] != 0.0f)
                    effect.PointsPerComboPoint = _spell.EffectPointsPerComboPoint[effectIndex];

                if (_spell.DmgMultiplier[effectIndex] != 1.0f)
                    effect.DmgMultiplier = _spell.DmgMultiplier[effectIndex];

                if (_spell.EffectMultipleValue[effectIndex] != 0.0f)
                    effect.MultipleValue = _spell.EffectMultipleValue[effectIndex];

                effect.TargetA = NamedId.Build<Targets>(_spell.EffectImplicitTargetA[effectIndex]);
                effect.TargetB = NamedId.Build<Targets>(_spell.EffectImplicitTargetB[effectIndex]);

                AuraModTypeName(effect, effectIndex);

                var classMask = new uint[3];

                switch (effectIndex)
                {
                    case 0: classMask = _spell.EffectSpellClassMaskA; break;
                    case 1: classMask = _spell.EffectSpellClassMaskB; break;
                    case 2: classMask = _spell.EffectSpellClassMaskC; break;
                }

                if (classMask[0] != 0 || classMask[1] != 0 || classMask[2] != 0)
                {
                    effect.SpellClassMask = new[] {classMask[0], classMask[1], classMask[2]};

                    var query = from spell in DBC.DBC.Spell.Values
                                where spell.SpellFamilyName == _spell.SpellFamilyName && spell.SpellFamilyFlags.ContainsElement(classMask)
                                join sk in DBC.DBC.SkillLineAbility on spell.ID equals sk.Value.SpellId into temp
                                from skill in temp.DefaultIfEmpty()
                                select new
                                {
                                    SpellID = spell.ID,
                                    SpellName = spell.SpellNameRank,
                                    skill.Value.SkillId
                                };

                    var skillRows = new List<dynamic>();
                    foreach (var row in query)
                    {
                        dynamic skillRow = new ExpandoObject();
                        skillRow.SkillId = row.SkillId;
                        skillRow.Spell = NamedId.Build(row.SpellID, row.SpellName);
                        skillRow.SpellName = row.SpellName;
                        skillRows.Add(skillRow);
                    }

                    effect.SkillRows = skillRows;
                }

                var radius = _spell.GetRadiusObj(effectIndex);
                if (radius != null)
                    effect.Radius = radius;

                // append trigger spell
                var trigger = _spell.EffectTriggerSpell[effectIndex];
                if (trigger != 0)
                {
                    SpellEntry triggerSpell;
                    if (DBC.DBC.Spell.TryGetValue(trigger, out triggerSpell))
                    {
                        _rtb.SetStyle(Color.Blue, FontStyle.Bold);
                        _rtb.AppendFormatLine("   Trigger spell ({0}) {1}. Chance = {2}", trigger, triggerSpell.SpellNameRank, _spell.ProcChance);
                        _rtb.AppendFormatLineIfNotNull("   Description: {0}", triggerSpell.Description);
                        _rtb.AppendFormatLineIfNotNull("   ToolTip: {0}", triggerSpell.ToolTip);
                        _rtb.SetDefaultStyle();
                        if (triggerSpell.ProcFlags != 0)
                        {
                            _rtb.AppendFormatLine("Charges - {0}", triggerSpell.ProcCharges);
                            _rtb.AppendLine(triggerSpell.ProcInfo);
                        }
                    }
                    else
                    {
                        _rtb.AppendFormatLine("Trigger spell ({0}) Not found, Chance = {1}", trigger, _spell.ProcChance);
                    }
                }

                _rtb.AppendFormatLineIfNotNull("EffectChainTarget = {0}", _spell.EffectChainTarget[effectIndex]);
                _rtb.AppendFormatLineIfNotNull("EffectItemType = {0}", _spell.EffectItemType[effectIndex]);

                if ((Mechanics)_spell.EffectMechanic[effectIndex] != Mechanics.MECHANIC_NONE)
                    _rtb.AppendFormatLine("Effect Mechanic = {0} ({1})", _spell.EffectMechanic[effectIndex], (Mechanics)_spell.EffectMechanic[effectIndex]);
            }
        }

        private void AppendSpellAura(dynamic obj)
        {
            if (_spell.CasterAuraSpell != 0)
            {
                SpellEntry spell;
                if (DBC.DBC.Spell.TryGetValue(_spell.CasterAuraSpell, out spell))
                    obj.CasterAuraSpell = NamedId.Build(_spell.CasterAuraSpell, spell.SpellName);
                else
                    obj.CasterAuraSpell = _spell.CasterAuraSpell;
            }

            if (_spell.TargetAuraSpell != 0)
            {
                SpellEntry spell;
                if (DBC.DBC.Spell.TryGetValue(_spell.TargetAuraSpell, out spell))
                    obj.TargetAuraSpell = NamedId.Build(_spell.TargetAuraSpell, spell.SpellName);
                else
                    obj.TargetAuraSpell = _spell.TargetAuraSpell;
            }

            if (_spell.ExcludeCasterAuraSpell != 0)
            {
                SpellEntry spell;
                if (DBC.DBC.Spell.TryGetValue(_spell.ExcludeCasterAuraSpell, out spell))
                    obj.ExcludeCasterAuraSpell = NamedId.Build(_spell.ExcludeCasterAuraSpell, spell.SpellName);
                else
                    obj.ExcludeCasterAuraSpell = _spell.ExcludeCasterAuraSpell;
            }

            if (_spell.ExcludeTargetAuraSpell != 0)
            {
                SpellEntry spell;
                if (DBC.DBC.Spell.TryGetValue(_spell.ExcludeTargetAuraSpell, out spell))
                    obj.ExcludeTargetAuraSpell = NamedId.Build(_spell.ExcludeTargetAuraSpell, spell.SpellName);
                else
                    obj.ExcludeTargetAuraSpell = _spell.ExcludeTargetAuraSpell;
            }
        }

        private void AuraModTypeName(dynamic effect, int index)
        {
            if (_spell.EffectApplyAuraName[index] == 0)
            {
                if (_spell.EffectMiscValue[index] != 0.0f)
                    effect.EffectMiscValueA = _spell.EffectMiscValue[index];

                if (_spell.EffectMiscValueB[index] != 0.0f)
                    effect.EffectMiscValueB = _spell.EffectMiscValueB[index];

                if (_spell.EffectAmplitude[index] != 0)
                    effect.EffectAmplitude = _spell.EffectAmplitude[index];
                return;
            }

            effect.Aura = NamedId.Build<AuraType>(_spell.EffectApplyAuraName[index]);
            effect.Value = _spell.EffectBasePoints[index] + 1;

            switch ((AuraType)_spell.EffectApplyAuraName[index])
            {
                case AuraType.SPELL_AURA_MOD_STAT:
                    effect.Misc = NamedId.Build<UnitMods>((ulong) _spell.EffectMiscValue[index]);
                    break;
                case AuraType.SPELL_AURA_MOD_RATING:
                    effect.Misc = NamedId.Build<CombatRating>((ulong)_spell.EffectMiscValue[index]);
                    break;
                case AuraType.SPELL_AURA_ADD_FLAT_MODIFIER:
                case AuraType.SPELL_AURA_ADD_PCT_MODIFIER:
                    effect.Misc = NamedId.Build<SpellModOp>((ulong)_spell.EffectMiscValue[index]);
                    break;
                case AuraType.SPELL_AURA_OVERRIDE_SPELLS:
                    OverrideSpellDataEntry overr;
                    if (DBC.DBC.OverrideSpellData.TryGetValue((uint)_spell.EffectMiscValue[index], out overr))
                    {
                        var overrSpells = new List<dynamic>();
                        for (var i = 0; i < 10; ++i)
                        {
                            if (overr.Spells[i] == 0)
                                continue;

                            dynamic overrSpell = new ExpandoObject();
                            overrSpell.Index = i + 1;

                            SpellEntry spell;
                            if (DBC.DBC.Spell.TryGetValue(overr.Spells[i], out spell))
                                overrSpell.Spell = NamedId.Build(overr.Spells[i], spell.SpellName);
                            else
                                overrSpell.Spell = overr.Spells[i];
                            overrSpells.Add(overrSpell);
                        }
                        effect.OverrideSpells = overrSpells.ToArray();
                    }
                    break;
                case AuraType.SPELL_AURA_SCREEN_EFFECT:
                    ScreenEffectEntry screenEffect;
                    if (DBC.DBC.ScreenEffect.TryGetValue((uint)_spell.EffectMiscValue[index], out screenEffect))
                        effect.ScreenEffect = NamedId.Build((ulong)_spell.EffectMiscValue[index], screenEffect.Name);
                    else
                        effect.ScreenEffect = _spell.EffectMiscValue[index];
                    break;
                default:
                    effect.Misc = _spell.EffectMiscValue[index];
                    break;
            }

            effect.MiscB = _spell.EffectMiscValueB[index];
            effect.Periodic = _spell.EffectAmplitude[index];
        }

        private void AppendDifficultyInfo(dynamic obj)
        {
            var difficultyId = _spell.SpellDifficultyId;
            if (difficultyId == 0)
                return;

            SpellDifficultyEntry difficulty;
            if (!DBC.DBC.SpellDifficulty.TryGetValue(difficultyId, out difficulty))
            {
                var modeNames = new[]
                {
                    "Normal 10",
                    "Normal 25",
                    "Heroic 10",
                    "Heroic 25",
                };

                var difficulties = new List<dynamic>();

                for (var i = 0; i < 4; ++i)
                {
                    if (difficulty.SpellId[i] == 0)
                        continue;

                    dynamic diff = new ExpandoObject();
                    diff.Mode = modeNames[i];
                    diff.SpellId = difficulty.SpellId[i];
                    difficulties.Add(diff);
                }

                if (difficulties.Count > 0)
                    obj.Difficulties = difficulties.ToArray();
            }
        }

        private void AppendAreaInfo(dynamic obj)
        {
            if (_spell.AreaGroupId <= 0)
                return;

            var areaGroupId = (uint)_spell.AreaGroupId;
            if (!DBC.DBC.AreaGroup.ContainsKey(areaGroupId))
                return;

            AreaGroupEntry groupEntry;
            var allowedAreas = new List<dynamic>();
            while (DBC.DBC.AreaGroup.TryGetValue(areaGroupId, out groupEntry))
            {
                for (var i = 0; i < 6; ++i)
                {
                    var areaId = groupEntry.AreaId[i];
                    AreaTableEntry areaEntry;
                    if (DBC.DBC.AreaTable.TryGetValue(areaId, out areaEntry))
                    {
                        dynamic area = new ExpandoObject();
                        area.Id = areaId;
                        area.Name = areaEntry.Name;
                        area.Map = areaEntry.MapId;
                        allowedAreas.Add(area);
                    }
                }

                if (groupEntry.NextGroup == 0)
                    break;

                // Try search in next group
                areaGroupId = groupEntry.NextGroup;
            }

            if (allowedAreas.Count > 0)
                obj.AllowedAreas = allowedAreas.ToArray();
        }
    }
}
