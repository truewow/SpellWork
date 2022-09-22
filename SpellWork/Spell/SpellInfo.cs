using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using SpellWork.Database;
using SpellWork.DBC;
using SpellWork.Extensions;

namespace SpellWork.Spell
{
    class SpellInfo
    {
        private readonly RichTextBox _spellInfoLog;
        private SpellEntry _spell;

        private const string _line = "=================================================";

        public SpellInfo(RichTextBox rtb, SpellEntry spell)
        {
            _spellInfoLog = rtb;
            _spell = spell;

            ProcInfo.SpellProc = spell;

            ViewSpellInfo();
        }

        private void ViewSpellInfo()
        {
            _spellInfoLog.Clear();
            _spellInfoLog.SetBold();
            _spellInfoLog.AppendFormatLine("ID - {0} {1}", _spell.ID, _spell.SpellNameRank);
            _spellInfoLog.SetDefaultStyle();

            _spellInfoLog.AppendFormatLine(_line);
            _spellInfoLog.AppendFormatLineIfNotNull("Description: {0}", _spell.Description);
            _spellInfoLog.AppendFormatLineIfNotNull("ToolTip: {0}", _spell.ToolTip);
            _spellInfoLog.AppendFormatLineIfNotNull("Modal Next Spell: {0}", _spell.ModalNextSpell);
            if (_spell.Description != string.Empty && _spell.ToolTip != string.Empty && _spell.ModalNextSpell != 0)
                _spellInfoLog.AppendFormatLine(_line);

            _spellInfoLog.AppendFormatLine("Category = {0}, SpellIconID = {1}, activeIconID = {2}, SpellVisual = ({3},{4}), SpellPriority = {5}",
                _spell.Category, _spell.SpellIconID, _spell.ActiveIconID, _spell.SpellVisual[0], _spell.SpellVisual[1], _spell.SpellPriority);

            _spellInfoLog.AppendFormatLine("Family {0}, flag [0] 0x{1:X8} [1] 0x{2:X8} [2] 0x{3:X8}",
                (SpellFamilyNames)_spell.SpellFamilyName, _spell.SpellFamilyFlags[0], _spell.SpellFamilyFlags[1], _spell.SpellFamilyFlags[2]);

            _spellInfoLog.AppendLine();

            _spellInfoLog.AppendFormatLine("SpellSchoolMask = {0} ({1})", _spell.SchoolMask, _spell.School);
            _spellInfoLog.AppendFormatLine("DamageClass = {0} ({1})", _spell.DmgClass, (SpellDmgClass)_spell.DmgClass);
            _spellInfoLog.AppendFormatLine("PreventionType = {0} ({1})", _spell.PreventionType, (SpellPreventionType)_spell.PreventionType);

            if (_spell.Attributes != 0 || _spell.AttributesEx != 0 || _spell.AttributesEx2 != 0 || _spell.AttributesEx3 != 0
                || _spell.AttributesEx4 != 0 || _spell.AttributesEx5 != 0 || _spell.AttributesEx6 != 0 || _spell.AttributesEx7 != 0)
                _spellInfoLog.AppendLine(_line);

            if (_spell.Attributes != 0)
                _spellInfoLog.AppendFormatLine("Attributes: 0x{0:X8} ({1})", _spell.Attributes, (SpellAtribute)_spell.Attributes);
            if (_spell.AttributesEx != 0)
                _spellInfoLog.AppendFormatLine("AttributesEx1: 0x{0:X8} ({1})", _spell.AttributesEx, (SpellAtributeEx)_spell.AttributesEx);
            if (_spell.AttributesEx2 != 0)
                _spellInfoLog.AppendFormatLine("AttributesEx2: 0x{0:X8} ({1})", _spell.AttributesEx2, (SpellAtributeEx2)_spell.AttributesEx2);
            if (_spell.AttributesEx3 != 0)
                _spellInfoLog.AppendFormatLine("AttributesEx3: 0x{0:X8} ({1})", _spell.AttributesEx3, (SpellAtributeEx3)_spell.AttributesEx3);
            if (_spell.AttributesEx4 != 0)
                _spellInfoLog.AppendFormatLine("AttributesEx4: 0x{0:X8} ({1})", _spell.AttributesEx4, (SpellAtributeEx4)_spell.AttributesEx4);
            if (_spell.AttributesEx5 != 0)
                _spellInfoLog.AppendFormatLine("AttributesEx5: 0x{0:X8} ({1})", _spell.AttributesEx5, (SpellAtributeEx5)_spell.AttributesEx5);
            if (_spell.AttributesEx6 != 0)
                _spellInfoLog.AppendFormatLine("AttributesEx6: 0x{0:X8} ({1})", _spell.AttributesEx6, (SpellAtributeEx6)_spell.AttributesEx6);
            if (_spell.AttributesEx7 != 0)
                _spellInfoLog.AppendFormatLine("AttributesEx7: 0x{0:X8} ({1})", _spell.AttributesEx7, (SpellAtributeEx7)_spell.AttributesEx7);

            if (DBC.DBC._spellCustomAttributes.ContainsKey(_spell.ID))
                _spellInfoLog.AppendFormatLine("AttributesCu: 0x{0:X8} ({1})", DBC.DBC._spellCustomAttributes[_spell.ID], (SpellCustomAttributes)(DBC.DBC._spellCustomAttributes[_spell.ID]));

            _spellInfoLog.AppendLine(_line);

            if (_spell.Targets != 0)
                _spellInfoLog.AppendFormatLine("Targets Mask = 0x{0:X8} ({1})", _spell.Targets, (SpellCastTargetFlags)_spell.Targets);

            if (_spell.TargetCreatureType != 0)
                _spellInfoLog.AppendFormatLine("Creature Type Mask = 0x{0:X8} ({1})",
                    _spell.TargetCreatureType, (CreatureTypeMask)_spell.TargetCreatureType);

            if (_spell.Stances != 0)
                _spellInfoLog.AppendFormatLine("Stances: {0}", (ShapeshiftFormMask)_spell.Stances);

            if (_spell.StancesNot != 0)
                _spellInfoLog.AppendFormatLine("Stances Not: {0}", (ShapeshiftFormMask)_spell.StancesNot);

            AppendSkillLine();

            // reagents
            {
                var printedHeader = false;
                for (var i = 0; i < _spell.Reagent.Length; ++i)
                {
                    if (_spell.Reagent[i] == 0)
                        continue;

                    if (!printedHeader)
                    {
                        _spellInfoLog.AppendLine();
                        _spellInfoLog.Append("Reagents:");
                        printedHeader = true;
                    }

                    _spellInfoLog.AppendFormat("  {0} x{1}", _spell.Reagent[i], _spell.ReagentCount[i]);
                }

                if (printedHeader)
                    _spellInfoLog.AppendLine();
            }

            _spellInfoLog.AppendFormatLine("Spell Level = {0}, base {1}, max {2}, maxTarget {3}",
                _spell.SpellLevel, _spell.BaseLevel, _spell.MaxLevel, _spell.MaxTargetLevel);

            if (_spell.EquippedItemClass != -1)
            {
                _spellInfoLog.AppendFormatLine("EquippedItemClass = {0} ({1})", _spell.EquippedItemClass, (ItemClass)_spell.EquippedItemClass);

                if (_spell.EquippedItemSubClassMask != 0)
                {
                    switch ((ItemClass)_spell.EquippedItemClass)
                    {
                        case ItemClass.WEAPON:
                            _spellInfoLog.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                _spell.EquippedItemSubClassMask, (ItemSubClassWeaponMask)_spell.EquippedItemSubClassMask);
                            break;
                        case ItemClass.ARMOR:
                            _spellInfoLog.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                _spell.EquippedItemSubClassMask, (ItemSubClassArmorMask)_spell.EquippedItemSubClassMask);
                            break;
                        case ItemClass.MISC:
                            _spellInfoLog.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                _spell.EquippedItemSubClassMask, (ItemSubClassMiscMask)_spell.EquippedItemSubClassMask);
                            break;
                    }
                }

                if (_spell.EquippedItemInventoryTypeMask != 0)
                    _spellInfoLog.AppendFormatLine("    InventoryType mask = 0x{0:X8} ({1})",
                        _spell.EquippedItemInventoryTypeMask, (InventoryTypeMask)_spell.EquippedItemInventoryTypeMask);
            }

            _spellInfoLog.AppendLine();
            _spellInfoLog.AppendFormatLine("Category = {0}", _spell.Category);
            _spellInfoLog.AppendFormatLine("DispelType = {0} ({1})", _spell.Dispel, (DispelType)_spell.Dispel);
            _spellInfoLog.AppendFormatLine("Mechanic = {0} ({1})", _spell.Mechanic, (Mechanics)_spell.Mechanic);

            _spellInfoLog.AppendLine(_spell.Range);

            _spellInfoLog.AppendFormatLineIfNotNull("Speed {0:F}", _spell.Speed);
            _spellInfoLog.AppendFormatLineIfNotNull("Stackable up to {0}", _spell.StackAmount);

            _spellInfoLog.AppendLine(_spell.CastTime);

            if (_spell.RecoveryTime != 0 || _spell.CategoryRecoveryTime != 0 || _spell.StartRecoveryCategory != 0)
            {
                _spellInfoLog.AppendFormatLine("RecoveryTime: {0} ms, CategoryRecoveryTime: {1} ms", _spell.RecoveryTime, _spell.CategoryRecoveryTime);
                _spellInfoLog.AppendFormatLine("StartRecoveryCategory = {0}, StartRecoveryTime = {1:F} ms", _spell.StartRecoveryCategory, _spell.StartRecoveryTime);
            }

            _spellInfoLog.AppendLine(_spell.Duration);

            if (_spell.ManaCost != 0 || _spell.ManaCostPercentage != 0 || _spell.PowerType != 0 ||
                _spell.ManaCostPerlevel != 0 || _spell.ManaPerSecond != 0 || _spell.ManaPerSecondPerLevel != 0)
            {
                _spellInfoLog.AppendFormat("Power {0}, Cost {1}",
                    (Powers)_spell.PowerType, _spell.ManaCost == 0 ? _spell.ManaCostPercentage + " %" : _spell.ManaCost.ToString());
                _spellInfoLog.AppendFormatIfNotNull(" + lvl * {0}", _spell.ManaCostPerlevel);
                _spellInfoLog.AppendFormatIfNotNull(" + {0} Per Second", _spell.ManaPerSecond);
                _spellInfoLog.AppendFormatIfNotNull(" + lvl * {0}", _spell.ManaPerSecondPerLevel);
                _spellInfoLog.AppendLine();
            }

            _spellInfoLog.AppendLine();

            if (_spell.InterruptFlags != 0)
            {
                if ((_spell.InterruptFlags & (uint)SpellInterruptFlags.SPELL_INTERRUPT_FLAG_ALL) != 0 || _spell.InterruptFlags > (uint)SpellInterruptFlags.SPELL_INTERRUPT_FLAG_ALL)
                    _spellInfoLog.AppendFormatLine("Casting interrupt flags: 0x{0:X8} ({1})", _spell.InterruptFlags, (SpellInterruptFlags)_spell.InterruptFlags);
                else
                    _spellInfoLog.AppendFormatLine("Casting interrupt flags: 0x{0:X8} ({1})", _spell.InterruptFlags, (SpellAuraInterruptFlags)_spell.InterruptFlags);
            }
            else
                _spellInfoLog.AppendFormatLine("Casting interrupt Flags: 0");
            _spellInfoLog.AppendLine();

            if (_spell.AuraInterruptFlags != 0)
                _spellInfoLog.AppendFormatLine("Aura interrupt flags: 0x{0:X8} ({1})", _spell.AuraInterruptFlags, (SpellAuraInterruptFlags)_spell.AuraInterruptFlags);
            else
                _spellInfoLog.AppendFormatLine("Aura interrupt Flags: 0");

            _spellInfoLog.AppendLine();

            if (_spell.ChannelInterruptFlags != 0)
            {
                var interruptFlag = _spell.ChannelInterruptFlags;
                if ((interruptFlag &~ (uint)SpellChannelInterruptFlags.CHANNEL_INTERRUPT_FLAG_ALL) == 0)
                    _spellInfoLog.AppendFormatLine("Channel interrupt flags: 0x{0:X8} ({1})", interruptFlag, (SpellChannelInterruptFlags)interruptFlag);
                else
                    _spellInfoLog.AppendFormatLine("Channel interrupt flags: 0x{0:X8} ({1})", interruptFlag, (SpellAuraInterruptFlags)interruptFlag);
            }
            else
                _spellInfoLog.AppendFormatLine("Channel interrupt Flags: 0");

            if (_spell.CasterAuraState != 0)
                _spellInfoLog.AppendFormatLine("CasterAuraState = {0} ({1})", _spell.CasterAuraState, (AuraState)_spell.CasterAuraState);

            if (_spell.TargetAuraState != 0)
                _spellInfoLog.AppendFormatLine("TargetAuraState = {0} ({1})", _spell.TargetAuraState, (AuraState)_spell.TargetAuraState);

            if (_spell.CasterAuraStateNot != 0)
                _spellInfoLog.AppendFormatLine("CasterAuraStateNot = {0} ({1})", _spell.CasterAuraStateNot, (AuraState)_spell.CasterAuraStateNot);

            if (_spell.TargetAuraStateNot != 0)
                _spellInfoLog.AppendFormatLine("TargetAuraStateNot = {0} ({1})", _spell.TargetAuraStateNot, (AuraState)_spell.TargetAuraStateNot);

            if (_spell.MaxAffectedTargets != 0)
                _spellInfoLog.AppendFormatLine("MaxAffectedTargets = {0}", _spell.MaxAffectedTargets);

            AppendSpellAura();

            AppendAreaInfo();

            _spellInfoLog.AppendFormatLineIfNotNull("Requires Spell Focus {0}", _spell.RequiresSpellFocus);

            if (_spell.ProcFlags != 0)
            {
                _spellInfoLog.SetBold();
                _spellInfoLog.AppendFormatLine("Proc flag 0x{0:X8}, chance = {1}, charges - {2}",
                _spell.ProcFlags, _spell.ProcChance, _spell.ProcCharges);
                _spellInfoLog.SetDefaultStyle();
                _spellInfoLog.AppendFormatLine(_line);
                _spellInfoLog.AppendText(_spell.ProcInfo);
            }
            else
            {
                _spellInfoLog.AppendFormatLine("Chance = {0}, charges - {1}", _spell.ProcChance, _spell.ProcCharges);
            }

            AppendSpellEffectInfo();
            AppendItemInfo();
            AppendDifficultyInfo();

            AppendSpellVisualInfo();
        }

        private void AppendSpellVisualInfo()
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

            _spellInfoLog.AppendLine(_line);
            _spellInfoLog.SetBold();
            _spellInfoLog.AppendLine("Missile data");
            _spellInfoLog.SetDefaultStyle();

            // Missile Model Data.
            if (hasMissileEntry)
            {
                _spellInfoLog.AppendFormatLine("Missile Model ID: {0}", visualData.MissileModel);
                _spellInfoLog.AppendFormatLine("Missile attachment: {0}", visualData.MissileAttachment);
                _spellInfoLog.AppendFormatLine("Missile cast offset: X:{0} Y:{1} Z:{2}", visualData.MissileCastOffsetX, visualData.MissileCastOffsetY, visualData.MissileCastOffsetZ);
                _spellInfoLog.AppendFormatLine("Missile impact offset: X:{0} Y:{1} Z:{2}", visualData.MissileImpactOffsetX, visualData.MissileImpactOffsetY, visualData.MissileImpactOffsetZ);
                _spellInfoLog.AppendFormatLine("MissileEntry ID: {0}", missileEntry.Id);
                _spellInfoLog.AppendFormatLine("Collision Radius: {0}", missileEntry.collisionRadius);
                _spellInfoLog.AppendFormatLine("Default Pitch: {0} - {1}", missileEntry.defaultPitchMin, missileEntry.defaultPitchMax);
                _spellInfoLog.AppendFormatLine("Random Pitch: {0} - {1}", missileEntry.randomizePitchMax, missileEntry.randomizePitchMax);
                _spellInfoLog.AppendFormatLine("Default Speed: {0} - {1}", missileEntry.defaultSpeedMin, missileEntry.defaultSpeedMax);
                _spellInfoLog.AppendFormatLine("Randomize Speed: {0} - {1}", missileEntry.randomizeSpeedMin, missileEntry.randomizeSpeedMax);
                _spellInfoLog.AppendFormatLine("Gravity: {0}", missileEntry.gravity);
                _spellInfoLog.AppendFormatLine("Maximum duration:", missileEntry.maxDuration);
                _spellInfoLog.AppendLine("");
            }

            // Missile Motion Data.
            if (hasMissileMotion)
            {
                _spellInfoLog.AppendFormatLine("Missile motion: {0}", missileMotionEntry.Name);
                _spellInfoLog.AppendFormatLine("Missile count: {0}", missileMotionEntry.MissileCount);
                _spellInfoLog.AppendLine("Missile Script body:");
                _spellInfoLog.AppendText(missileMotionEntry.Script);
            }
        }

        private void AppendSkillLine()
        {
            var query = from skillLineAbility in DBC.DBC.SkillLineAbility
                        join skillLine in DBC.DBC.SkillLine
                        on skillLineAbility.Value.SkillId equals skillLine.Key
                        where skillLineAbility.Value.SpellId == _spell.ID
                        select new
                        {
                            skillLineAbility,
                            skillLine
                        };

            if (query.Count() == 0)
                return;

            var skill = query.First().skillLineAbility.Value;
            var line =  query.First().skillLine.Value;

            _spellInfoLog.AppendFormatLine("Skill (Id {0}) \"{1}\"", skill.SkillId, line.Name);
            _spellInfoLog.AppendFormat("    ReqSkillValue {0}", skill.ReqSkillValue);

            _spellInfoLog.AppendFormat(", Forward Spell = {0}, MinMaxValue ({1}, {2})", skill.ForwardSpellid, skill.MinValue, skill.MaxValue);
            _spellInfoLog.AppendFormat(", CharacterPoints ({0}, {1})", skill.CharacterPoints[0], skill.CharacterPoints[1]);
        }

        private int CalculateBasePoints(int effectIndex)
        {
            int basePoints = _spell.EffectBasePoints[effectIndex];
            switch (_spell.EffectDieSides[effectIndex])
            {
                case 0: break;
                case 1: basePoints += 1; break;
                default: break;
            }

            return basePoints;
        }

        private void AppendSpellEffectInfo()
        {
            _spellInfoLog.AppendLine(_line);

            for (var effectIndex = 0; effectIndex < DBC.DBC.MaxEffectIndex; effectIndex++)
            {
                _spellInfoLog.SetBold();

                int basePoints = CalculateBasePoints(effectIndex);
                if ((SpellEffects)_spell.Effect[effectIndex] == SpellEffects.NO_SPELL_EFFECT)
                {
                    _spellInfoLog.AppendFormatLine("Effect {0}:  NO EFFECT", effectIndex);
                    if (basePoints != 0)
                    {
                        _spellInfoLog.AppendFormat("BasePoints = {0}", basePoints);
                        _spellInfoLog.AppendLine();
                    }

                    _spellInfoLog.AppendLine();
                    continue;
                }

                _spellInfoLog.AppendFormatLine("Effect {0}: Id {1} ({2})", effectIndex, _spell.Effect[effectIndex], (SpellEffects)_spell.Effect[effectIndex]);
                _spellInfoLog.SetDefaultStyle();
                _spellInfoLog.AppendFormat("BasePoints = {0}", basePoints);

                if (_spell.EffectRealPointsPerLevel[effectIndex] != 0)
                    _spellInfoLog.AppendFormat(" + Level * {0:F}", _spell.EffectRealPointsPerLevel[effectIndex]);

                int randomPoints = _spell.EffectDieSides[effectIndex];
                if (randomPoints != 0 && randomPoints != 1)
                {
                    _spellInfoLog.AppendFormat(" to {0}", basePoints + randomPoints);
                    if (_spell.EffectRealPointsPerLevel[effectIndex] != 0)
                        _spellInfoLog.AppendFormat(" + Level * {0:F}", _spell.EffectRealPointsPerLevel[effectIndex]);
                }

                _spellInfoLog.AppendFormatIfNotNull(" + combo * {0:F}", _spell.EffectPointsPerComboPoint[effectIndex]);

                if (_spell.DmgMultiplier[effectIndex] != 1.0f)
                    _spellInfoLog.AppendFormat(" x {0:F}", _spell.DmgMultiplier[effectIndex]);

                _spellInfoLog.AppendFormatIfNotNull("  Multiple = {0:F}", _spell.EffectMultipleValue[effectIndex]);
                _spellInfoLog.AppendFormatIfNotNull(" Bonus = {0:F}", _spell.DamageCoeficient[effectIndex]);
                _spellInfoLog.AppendLine();

                _spellInfoLog.AppendFormatLine("Targets ({0}, {1}) ({2}, {3})",
                    _spell.EffectImplicitTargetA[effectIndex], _spell.EffectImplicitTargetB[effectIndex],
                    (Targets)_spell.EffectImplicitTargetA[effectIndex], (Targets)_spell.EffectImplicitTargetB[effectIndex]);

                AuraModTypeName(effectIndex);

                var classMask = new uint[3];

                switch (effectIndex)
                {
                    case 0: classMask = _spell.EffectSpellClassMaskA; break;
                    case 1: classMask = _spell.EffectSpellClassMaskB; break;
                    case 2: classMask = _spell.EffectSpellClassMaskC; break;
                }

                if (classMask[0] != 0 || classMask[1] != 0 || classMask[2] != 0)
                {
                    _spellInfoLog.AppendFormatLine("SpellClassMask = {0:X8} {1:X8} {2:X8}", classMask[0], classMask[1], classMask[2]);

                    var query = from spell in DBC.DBC.Spell.Values
                                where spell.SpellFamilyName == _spell.SpellFamilyName && spell.SpellFamilyFlags.ContainsElement(classMask)
                                join sk in DBC.DBC.SkillLineAbility on spell.ID equals sk.Value.SpellId into temp
                                from skill in temp.DefaultIfEmpty()
                                select new
                                {
                                    SpellID   = spell.ID,
                                    SpellName = spell.SpellNameRank, skill.Value.SkillId
                                };

                    foreach (var row in query)
                    {
                        if (row.SkillId > 0)
                        {
                            _spellInfoLog.SelectionColor = Color.Blue;
                            _spellInfoLog.AppendFormatLine("\t+ {0} - {1}",  row.SpellID, row.SpellName);
                        }
                        else
                        {
                            _spellInfoLog.SelectionColor = Color.Red;
                            _spellInfoLog.AppendFormatLine("\t- {0} - {1}", row.SpellID, row.SpellName);
                        }
                        _spellInfoLog.SelectionColor = Color.Black;
                    }
                }

                _spellInfoLog.AppendFormatLineIfNotNull("{0}", _spell.GetRadius(effectIndex));

                // append trigger spell
                var trigger = _spell.EffectTriggerSpell[effectIndex];
                if (trigger != 0)
                {
                    if (DBC.DBC.Spell.ContainsKey(trigger))
                    {
                        var triggerSpell = DBC.DBC.Spell[trigger];
                        _spellInfoLog.SetStyle(Color.Blue, FontStyle.Bold);
                        _spellInfoLog.AppendFormatLine("   Trigger spell ({0}) {1}. Chance = {2}", trigger, triggerSpell.SpellNameRank, _spell.ProcChance);
                        _spellInfoLog.AppendFormatLineIfNotNull("   Description: {0}", triggerSpell.Description);
                        _spellInfoLog.AppendFormatLineIfNotNull("   ToolTip: {0}", triggerSpell.ToolTip);
                        _spellInfoLog.SetDefaultStyle();
                        if (triggerSpell.ProcFlags != 0)
                        {
                            _spellInfoLog.AppendFormatLine("Charges - {0}", triggerSpell.ProcCharges);
                            _spellInfoLog.AppendLine(_line);
                            _spellInfoLog.AppendLine(triggerSpell.ProcInfo);
                            _spellInfoLog.AppendLine(_line);
                        }
                    }
                    else
                    {
                        _spellInfoLog.AppendFormatLine("Trigger spell ({0}) Not found, Chance = {1}", trigger, _spell.ProcChance);
                    }
                }

                _spellInfoLog.AppendFormatLineIfNotNull("EffectChainTarget = {0}", _spell.EffectChainTarget[effectIndex]);
                _spellInfoLog.AppendFormatLineIfNotNull("EffectItemType = {0}", _spell.EffectItemType[effectIndex]);

                if((Mechanics)_spell.EffectMechanic[effectIndex] != Mechanics.MECHANIC_NONE)
                    _spellInfoLog.AppendFormatLine("Effect Mechanic = {0} ({1})", _spell.EffectMechanic[effectIndex], (Mechanics)_spell.EffectMechanic[effectIndex]);

                _spellInfoLog.AppendLine();
            }
        }

        private void AppendSpellAura()
        {
            if (_spell.CasterAuraSpell != 0)
            {
                if (DBC.DBC.Spell.ContainsKey(_spell.CasterAuraSpell))
                    _spellInfoLog.AppendFormatLine("  Caster Aura Spell ({0}) {1}", _spell.CasterAuraSpell, DBC.DBC.Spell[_spell.CasterAuraSpell].SpellName);
                else
                    _spellInfoLog.AppendFormatLine("  Caster Aura Spell ({0}) ?????", _spell.CasterAuraSpell);
            }

            if (_spell.TargetAuraSpell != 0)
            {
                if (DBC.DBC.Spell.ContainsKey(_spell.TargetAuraSpell))
                    _spellInfoLog.AppendFormatLine("  Target Aura Spell ({0}) {1}", _spell.TargetAuraSpell, DBC.DBC.Spell[_spell.TargetAuraSpell].SpellName);
                else
                    _spellInfoLog.AppendFormatLine("  Target Aura Spell ({0}) ?????", _spell.TargetAuraSpell);
            }

            if (_spell.ExcludeCasterAuraSpell != 0)
            {
                if (DBC.DBC.Spell.ContainsKey(_spell.ExcludeCasterAuraSpell))
                    _spellInfoLog.AppendFormatLine("  Ex Caster Aura Spell ({0}) {1}", _spell.ExcludeCasterAuraSpell, DBC.DBC.Spell[_spell.ExcludeCasterAuraSpell].SpellName);
                else
                    _spellInfoLog.AppendFormatLine("  Ex Caster Aura Spell ({0}) ?????", _spell.ExcludeCasterAuraSpell);
            }

            // ReSharper disable InvertIf
            if (_spell.ExcludeTargetAuraSpell != 0)
            {
                if (DBC.DBC.Spell.ContainsKey(_spell.ExcludeTargetAuraSpell))
                    _spellInfoLog.AppendFormatLine("  Ex Target Aura Spell ({0}) {1}", _spell.ExcludeTargetAuraSpell, DBC.DBC.Spell[_spell.ExcludeTargetAuraSpell].SpellName);
                else
                    _spellInfoLog.AppendFormatLine("  Ex Target Aura Spell ({0}) ?????", _spell.ExcludeTargetAuraSpell);
            }
            // ReSharper enable InvertIf
        }

        private void AuraModTypeName(int index)
        {
            var applyAuraName       = (AuraType)_spell.EffectApplyAuraName[index];
            var miscValueA          = _spell.EffectMiscValue[index];
            var miscValueB          = _spell.EffectMiscValueB[index];
            var effectAplitude      = _spell.EffectAmplitude[index];

            if (_spell.EffectApplyAuraName[index] == 0)
            {
                if (_spell.EffectMiscValue[index] != 0)
                {
                    _spellInfoLog.AppendFormat("EffectMiscValueA = {0}", _spell.EffectMiscValue[index]);
                    switch ((SpellEffects)_spell.Effect[index])
                    {
                        case SpellEffects.SPELL_EFFECT_ACTIVATE_OBJECT:
                            _spellInfoLog.AppendFormat(" ({0})", (GameObjectActions)miscValueA);
                            break;
                        case SpellEffects.SPELL_EFFECT_SUMMON:
                            _spellInfoLog.AppendFormat(" (Object entry: {0})", miscValueA);
                            break;
                        case SpellEffects.SPELL_EFFECT_RESURRECT_NEW:
                            _spellInfoLog.AppendFormat(" (Health {0}, Mana {1})", miscValueA, miscValueB);
                            break;
                        default:
                            break;
                    }
                    _spellInfoLog.AppendLine();
                }

                _spellInfoLog.AppendFormat("EffectMiscValueB = {0}", miscValueB);
                if ((SpellEffects)_spell.Effect[index] == SpellEffects.SPELL_EFFECT_SUMMON && DBC.DBC.SummonProperties.ContainsKey((uint)miscValueB))
                {
                    _spellInfoLog.AppendFormat(", summon property category - {0}, type - {1}", (SummonCategory)DBC.DBC.SummonProperties[(uint)miscValueB].Category, (SummonType)DBC.DBC.SummonProperties[(uint)miscValueB].Type);
                }

                _spellInfoLog.AppendLine();
                _spellInfoLog.AppendFormatLineIfNotNull("EffectAmplitude = {0}", effectAplitude);

                return;
            }

            _spellInfoLog.AppendFormat("Aura Id {0:D} ({0})", applyAuraName);
            _spellInfoLog.AppendFormat(", value = {0}", _spell.EffectBasePoints[index] + 1);
            _spellInfoLog.AppendFormat(", misc = {0} (", miscValueA);

            switch (applyAuraName)
            {
                case AuraType.SPELL_AURA_MOD_STAT:
                    _spellInfoLog.Append((UnitMods)miscValueA);
                    break;
                case AuraType.SPELL_AURA_MOD_RATING:
                    _spellInfoLog.Append((CombatRating)miscValueA);
                    break;
                case AuraType.SPELL_AURA_ADD_FLAT_MODIFIER:
                case AuraType.SPELL_AURA_ADD_PCT_MODIFIER:
                    _spellInfoLog.Append((SpellModOp)miscValueA);
                    break;
                // todo: more case
                default:
                    _spellInfoLog.Append(miscValueA);
                    break;
            }

            _spellInfoLog.AppendFormat("), miscB = {0}", miscValueB);
            _spellInfoLog.AppendFormatLine(", periodic = {0}", _spell.EffectAmplitude[index]);

            switch (applyAuraName)
            {
                case AuraType.SPELL_AURA_OVERRIDE_SPELLS:
                    if (!DBC.DBC.OverrideSpellData.ContainsKey((uint)miscValueA))
                    {
                        _spellInfoLog.SetStyle(Color.Red, FontStyle.Bold);
                        _spellInfoLog.AppendFormatLine("Cannot find key {0} in OverrideSpellData.dbc", (uint)miscValueA);
                    }
                    else
                    {
                        _spellInfoLog.AppendLine();
                        _spellInfoLog.SetStyle(Color.DarkRed, FontStyle.Bold);
                        _spellInfoLog.AppendLine("Overriding Spells:");
                        var @override = DBC.DBC.OverrideSpellData[(uint)miscValueA];
                        for (var i = 0; i < 10; ++i)
                        {
                            if (@override.Spells[i] == 0)
                                continue;

                            _spellInfoLog.SetStyle(Color.DarkBlue, FontStyle.Regular);
                            _spellInfoLog.AppendFormatLine("\t - #{0} ({1}) {2}", i + 1, @override.Spells[i],
                                DBC.DBC.Spell.ContainsKey(@override.Spells[i]) ? DBC.DBC.Spell[@override.Spells[i]].SpellName : "?????");
                        }
                        _spellInfoLog.AppendLine();
                    }
                    break;
                case AuraType.SPELL_AURA_SCREEN_EFFECT:
                    _spellInfoLog.SetStyle(Color.DarkBlue, FontStyle.Bold);
                    _spellInfoLog.AppendFormatLine("ScreenEffect: {0}",
                        DBC.DBC.ScreenEffect.ContainsKey((uint)miscValueA) ? DBC.DBC.ScreenEffect[(uint)miscValueA].Name : "?????");
                    break;
                default:
                    break;
            }
        }

        private void AppendDifficultyInfo()
        {
            var difficultyId = _spell.SpellDifficultyId;
            if (difficultyId == 0)
                return;

            if (!DBC.DBC.SpellDifficulty.ContainsKey(difficultyId))
            {
                _spellInfoLog.AppendFormatLine("Cannot find difficulty overrides for id {0} in SpellDifficulty.dbc!", difficultyId);
                return;
            }

            var modeNames = new[]
            {
                "Normal 10",
                "Normal 25",
                "Heroic 10",
                "Heroic 25",
            };

            _spellInfoLog.SetBold();
            _spellInfoLog.AppendLine("Spell difficulty Ids:");

            var difficulty = DBC.DBC.SpellDifficulty[difficultyId];
            for (var i = 0; i < 4; ++i)
            {
                if (difficulty.SpellId[i] <= 0)
                    continue;

                _spellInfoLog.AppendFormatLine("{0}: {1}", modeNames[i], difficulty.SpellId[i]);
            }
        }

        private void AppendAreaInfo()
        {
            if (_spell.AreaGroupId <= 0)
                return;

            var areaGroupId = (uint)_spell.AreaGroupId;
            if (!DBC.DBC.AreaGroup.ContainsKey(areaGroupId))
            {
                _spellInfoLog.AppendFormatLine("Cannot find area group id {0} in AreaGroup.dbc!", _spell.AreaGroupId);
                return;
            }

            _spellInfoLog.AppendLine();
            _spellInfoLog.SetBold();
            _spellInfoLog.AppendLine("Allowed areas:");
            while (DBC.DBC.AreaGroup.ContainsKey(areaGroupId))
            {
                var groupEntry = DBC.DBC.AreaGroup[areaGroupId];
                for (var i = 0; i < 6; ++i)
                {
                    var areaId = groupEntry.AreaId[i];
                    if (DBC.DBC.AreaTable.ContainsKey(areaId))
                    {
                        var areaEntry = DBC.DBC.AreaTable[areaId];
                        _spellInfoLog.AppendFormatLine("{0} - {1} (Map: {2})", areaId, areaEntry.Name, areaEntry.MapId);
                    }
                }


                if (groupEntry.NextGroup == 0)
                    break;

                // Try search in next group
                areaGroupId = groupEntry.NextGroup;
            }

            _spellInfoLog.AppendLine();
        }

        private void AppendItemInfo()
        {
            if (!SqlConnection.Connected)
                return;

            var items = from item in DBC.DBC.ItemTemplate
                        where  item.SpellId.ContainsElement((int)_spell.ID)
                        select item;

            if (items.Count() == 0)
                return;

            _spellInfoLog.AppendLine(_line);
            _spellInfoLog.SetStyle(Color.Blue, FontStyle.Bold);
            _spellInfoLog.AppendLine("Items using this spell:");
            _spellInfoLog.SetDefaultStyle();

            foreach (var item in items)
            {
                var name = ((int)DBC.DBC.Locale == 0 || string.IsNullOrEmpty(item.LocalesName)) ? item.Name : item.LocalesName;
                var desc = ((int)DBC.DBC.Locale == 0 || string.IsNullOrEmpty(item.LocalesDescription)) ? item.Description : item.LocalesDescription;

                desc = string.IsNullOrEmpty(desc) ? string.Empty : string.Format(" - \"{0}\"", desc);

                _spellInfoLog.AppendFormatLine(@"   {0}: {1} {2}", item.Entry, name, desc);
            }
        }
    }
}
