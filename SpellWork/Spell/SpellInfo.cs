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
        private readonly RichTextBox m_spellInfoLog;
        private SpellEntry m_spellInfo;

        private const string _line = "=================================================";

        public SpellInfo(RichTextBox rtb, SpellEntry spell)
        {
            m_spellInfoLog = rtb;
            m_spellInfo = spell;

            ProcInfo.SpellProc = spell;

            ViewSpellInfo();
        }

        private void ViewSpellInfo()
        {
            // Base spell details
            m_spellInfoLog.Clear();
            m_spellInfoLog.SetBold();
            m_spellInfoLog.AppendFormatLine("ID - {0} {1}", m_spellInfo.ID, m_spellInfo.SpellNameRank);
            m_spellInfoLog.SetDefaultStyle();

            m_spellInfoLog.AppendFormatLine(_line);
            m_spellInfoLog.AppendFormatLineIfNotNull("Description: {0}", m_spellInfo.Description);
            m_spellInfoLog.AppendFormatLineIfNotNull("ToolTip: {0}", m_spellInfo.ToolTip);
            m_spellInfoLog.AppendFormatLineIfNotNull("Modal Next Spell: {0}", m_spellInfo.ModalNextSpell);
            if (!m_spellInfo.Description.IsEmpty() && !m_spellInfo.ToolTip.IsEmpty() && m_spellInfo.ModalNextSpell != 0)
                m_spellInfoLog.AppendFormatLine(_line);

            //m_spellInfoLog.AppendFormatLine(_line);
            m_spellInfoLog.AppendFormatLine("Family {0}, flag [0] 0x{1:X8} [1] 0x{2:X8} [2] 0x{3:X8}",
                (SpellFamilyNames)m_spellInfo.SpellFamilyName,
                m_spellInfo.SpellFamilyFlags[0],
                m_spellInfo.SpellFamilyFlags[1],
                m_spellInfo.SpellFamilyFlags[2]
            );
            m_spellInfoLog.AppendFormatLine(_line);
            m_spellInfoLog.AppendFormatLine("Category = {0}, SpellIconID = {1}, activeIconID = {2}, SpellVisual = ({3},{4}), SpellPriority = {5}",
                m_spellInfo.Category,
                m_spellInfo.SpellIconID,
                m_spellInfo.ActiveIconID,
                m_spellInfo.SpellVisual[0],
                m_spellInfo.SpellVisual[1],
                m_spellInfo.SpellPriority
            );

            // SchoolMask, DamageClass, PeventionType
            m_spellInfoLog.AppendLine();
            m_spellInfoLog.AppendFormatLine("SpellSchoolMask = {0} ({1})", m_spellInfo.SchoolMask, m_spellInfo.School);
            m_spellInfoLog.AppendFormatLine("DamageClass = {0} ({1})", m_spellInfo.DmgClass, (SpellDmgClass)m_spellInfo.DmgClass);
            m_spellInfoLog.AppendFormatLine("PreventionType = {0} ({1})", m_spellInfo.PreventionType, (SpellPreventionType)m_spellInfo.PreventionType);

            // Spell Attributes
            if (m_spellInfo.Attributes != 0 || m_spellInfo.AttributesEx != 0 || m_spellInfo.AttributesEx2 != 0 || m_spellInfo.AttributesEx3 != 0
                || m_spellInfo.AttributesEx4 != 0 || m_spellInfo.AttributesEx5 != 0 || m_spellInfo.AttributesEx6 != 0 || m_spellInfo.AttributesEx7 != 0)
                m_spellInfoLog.AppendLine(_line);

            if (m_spellInfo.Attributes != 0)
                m_spellInfoLog.AppendFormatLine("Attributes: 0x{0:X8} ({1})", m_spellInfo.Attributes, (SpellAtribute)m_spellInfo.Attributes);
            if (m_spellInfo.AttributesEx != 0)
                m_spellInfoLog.AppendFormatLine("AttributesEx1: 0x{0:X8} ({1})", m_spellInfo.AttributesEx, (SpellAtributeEx)m_spellInfo.AttributesEx);
            if (m_spellInfo.AttributesEx2 != 0)
                m_spellInfoLog.AppendFormatLine("AttributesEx2: 0x{0:X8} ({1})", m_spellInfo.AttributesEx2, (SpellAtributeEx2)m_spellInfo.AttributesEx2);
            if (m_spellInfo.AttributesEx3 != 0)
                m_spellInfoLog.AppendFormatLine("AttributesEx3: 0x{0:X8} ({1})", m_spellInfo.AttributesEx3, (SpellAtributeEx3)m_spellInfo.AttributesEx3);
            if (m_spellInfo.AttributesEx4 != 0)
                m_spellInfoLog.AppendFormatLine("AttributesEx4: 0x{0:X8} ({1})", m_spellInfo.AttributesEx4, (SpellAtributeEx4)m_spellInfo.AttributesEx4);
            if (m_spellInfo.AttributesEx5 != 0)
                m_spellInfoLog.AppendFormatLine("AttributesEx5: 0x{0:X8} ({1})", m_spellInfo.AttributesEx5, (SpellAtributeEx5)m_spellInfo.AttributesEx5);
            if (m_spellInfo.AttributesEx6 != 0)
                m_spellInfoLog.AppendFormatLine("AttributesEx6: 0x{0:X8} ({1})", m_spellInfo.AttributesEx6, (SpellAtributeEx6)m_spellInfo.AttributesEx6);
            if (m_spellInfo.AttributesEx7 != 0)
                m_spellInfoLog.AppendFormatLine("AttributesEx7: 0x{0:X8} ({1})", m_spellInfo.AttributesEx7, (SpellAtributeEx7)m_spellInfo.AttributesEx7);

            // Custom spell attributes (server side)
            if (DBC.DBCStore._spellCustomAttributes.ContainsKey(m_spellInfo.ID))
                m_spellInfoLog.AppendFormatLine("AttributesCu: 0x{0:X8} ({1})", DBC.DBCStore._spellCustomAttributes[m_spellInfo.ID], (SpellCustomAttributes)(DBC.DBCStore._spellCustomAttributes[m_spellInfo.ID]));

            m_spellInfoLog.AppendLine(_line);
            if (m_spellInfo.Targets != 0)
                m_spellInfoLog.AppendFormatLine("Targets Mask = 0x{0:X8} ({1})", m_spellInfo.Targets, (SpellCastTargetFlags)m_spellInfo.Targets);

            if (m_spellInfo.TargetCreatureType != 0)
                m_spellInfoLog.AppendFormatLine("Creature Type Mask = 0x{0:X8} ({1})", m_spellInfo.TargetCreatureType, (CreatureTypeMask)m_spellInfo.TargetCreatureType);

            if (m_spellInfo.Stances != 0)
                m_spellInfoLog.AppendFormatLine("Stances: {0}", (ShapeshiftFormMask)m_spellInfo.Stances);

            if (m_spellInfo.StancesNot != 0)
                m_spellInfoLog.AppendFormatLine("Stances Not: {0}", (ShapeshiftFormMask)m_spellInfo.StancesNot);

            AppendSkillLine();

            // reagents
            {
                var printedHeader = false;
                for (var i = 0; i < m_spellInfo.Reagent.Length; ++i)
                {
                    if (m_spellInfo.Reagent[i] == 0)
                        continue;

                    if (!printedHeader)
                    {
                        m_spellInfoLog.AppendLine();
                        m_spellInfoLog.Append("Reagents:");
                        printedHeader = true;
                    }

                    m_spellInfoLog.AppendFormat("  {0} x{1}", m_spellInfo.Reagent[i], m_spellInfo.ReagentCount[i]);
                }

                if (printedHeader)
                    m_spellInfoLog.AppendLine();
            }

            m_spellInfoLog.AppendFormatLine("Spell Level = {0}, base {1}, max {2}, maxTarget {3}",
                m_spellInfo.SpellLevel, m_spellInfo.BaseLevel, m_spellInfo.MaxLevel, m_spellInfo.MaxTargetLevel);

            if (m_spellInfo.EquippedItemClass != -1)
            {
                m_spellInfoLog.AppendFormatLine("EquippedItemClass = {0} ({1})", m_spellInfo.EquippedItemClass, (ItemClass)m_spellInfo.EquippedItemClass);

                if (m_spellInfo.EquippedItemSubClassMask != 0)
                {
                    switch ((ItemClass)m_spellInfo.EquippedItemClass)
                    {
                        case ItemClass.WEAPON:
                            m_spellInfoLog.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                m_spellInfo.EquippedItemSubClassMask, (ItemSubClassWeaponMask)m_spellInfo.EquippedItemSubClassMask);
                            break;
                        case ItemClass.ARMOR:
                            m_spellInfoLog.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                m_spellInfo.EquippedItemSubClassMask, (ItemSubClassArmorMask)m_spellInfo.EquippedItemSubClassMask);
                            break;
                        case ItemClass.MISC:
                            m_spellInfoLog.AppendFormatLine("    SubClass mask 0x{0:X8} ({1})",
                                m_spellInfo.EquippedItemSubClassMask, (ItemSubClassMiscMask)m_spellInfo.EquippedItemSubClassMask);
                            break;
                    }
                }

                if (m_spellInfo.EquippedItemInventoryTypeMask != 0)
                    m_spellInfoLog.AppendFormatLine("    InventoryType mask = 0x{0:X8} ({1})",
                        m_spellInfo.EquippedItemInventoryTypeMask, (InventoryTypeMask)m_spellInfo.EquippedItemInventoryTypeMask);
            }

            m_spellInfoLog.AppendLine();
            m_spellInfoLog.AppendFormatLine("Category = {0}", m_spellInfo.Category);
            m_spellInfoLog.AppendFormatLine("DispelType = {0} ({1})", m_spellInfo.Dispel, (DispelType)m_spellInfo.Dispel);
            m_spellInfoLog.AppendFormatLine("Mechanic = {0} ({1})", m_spellInfo.Mechanic, (Mechanics)m_spellInfo.Mechanic);

            m_spellInfoLog.AppendLine(m_spellInfo.Range);

            m_spellInfoLog.AppendFormatLineIfNotNull("Speed {0:F}", m_spellInfo.Speed);
            m_spellInfoLog.AppendFormatLineIfNotNull("Stackable up to {0}", m_spellInfo.StackAmount);

            m_spellInfoLog.AppendLine(m_spellInfo.CastTime);

            if (m_spellInfo.RecoveryTime != 0 || m_spellInfo.CategoryRecoveryTime != 0 || m_spellInfo.StartRecoveryCategory != 0)
            {
                m_spellInfoLog.AppendFormatLine("RecoveryTime: {0} ms, CategoryRecoveryTime: {1} ms", m_spellInfo.RecoveryTime, m_spellInfo.CategoryRecoveryTime);
                m_spellInfoLog.AppendFormatLine("StartRecoveryCategory = {0}, StartRecoveryTime = {1:F} ms", m_spellInfo.StartRecoveryCategory, m_spellInfo.StartRecoveryTime);
            }

            m_spellInfoLog.AppendLine(m_spellInfo.Duration);

            if (m_spellInfo.ManaCost != 0 || m_spellInfo.ManaCostPercentage != 0 || m_spellInfo.PowerType != 0 ||
                m_spellInfo.ManaCostPerlevel != 0 || m_spellInfo.ManaPerSecond != 0 || m_spellInfo.ManaPerSecondPerLevel != 0)
            {
                m_spellInfoLog.AppendFormat("Power {0}, Cost {1}", (Powers)m_spellInfo.PowerType, m_spellInfo.ManaCost == 0 ? m_spellInfo.ManaCostPercentage + " %" : m_spellInfo.ManaCost.ToString());
                m_spellInfoLog.AppendFormatIfNotNull(" + lvl * {0}", m_spellInfo.ManaCostPerlevel);
                m_spellInfoLog.AppendFormatIfNotNull(" + {0} Per Second", m_spellInfo.ManaPerSecond);
                m_spellInfoLog.AppendFormatIfNotNull(" + lvl * {0}", m_spellInfo.ManaPerSecondPerLevel);
                m_spellInfoLog.AppendLine();
            }

            m_spellInfoLog.AppendLine();

            if (m_spellInfo.InterruptFlags != 0)
            {
                if ((m_spellInfo.InterruptFlags & (uint)SpellInterruptFlags.SPELL_INTERRUPT_FLAG_ALL) != 0 || m_spellInfo.InterruptFlags > (uint)SpellInterruptFlags.SPELL_INTERRUPT_FLAG_ALL)
                    m_spellInfoLog.AppendFormatLine("Casting interrupt flags: 0x{0:X8} ({1})", m_spellInfo.InterruptFlags, (SpellInterruptFlags)m_spellInfo.InterruptFlags);
                else
                    m_spellInfoLog.AppendFormatLine("Casting interrupt flags: 0x{0:X8} ({1})", m_spellInfo.InterruptFlags, (SpellAuraInterruptFlags)m_spellInfo.InterruptFlags);
            }
            else
                m_spellInfoLog.AppendFormatLine("Casting interrupt Flags: 0");

            m_spellInfoLog.AppendLine();

            if (m_spellInfo.AuraInterruptFlags != 0)
                m_spellInfoLog.AppendFormatLine("Aura interrupt flags: 0x{0:X8} ({1})", m_spellInfo.AuraInterruptFlags, (SpellAuraInterruptFlags)m_spellInfo.AuraInterruptFlags);
            else
                m_spellInfoLog.AppendFormatLine("Aura interrupt Flags: 0");

            m_spellInfoLog.AppendLine();

            if (m_spellInfo.ChannelInterruptFlags != 0)
            {
                var interruptFlag = m_spellInfo.ChannelInterruptFlags;
                if ((interruptFlag &~ (uint)SpellChannelInterruptFlags.CHANNEL_INTERRUPT_FLAG_ALL) == 0)
                    m_spellInfoLog.AppendFormatLine("Channel interrupt flags: 0x{0:X8} ({1})", interruptFlag, (SpellChannelInterruptFlags)interruptFlag);
                else
                    m_spellInfoLog.AppendFormatLine("Channel interrupt flags: 0x{0:X8} ({1})", interruptFlag, (SpellAuraInterruptFlags)interruptFlag);
            }
            else
                m_spellInfoLog.AppendFormatLine("Channel interrupt Flags: 0");

            if (m_spellInfo.CasterAuraState != 0)
                m_spellInfoLog.AppendFormatLine("Requires caster CasterAuraState state = {0} ({1})", m_spellInfo.CasterAuraState, (AuraState)m_spellInfo.CasterAuraState);

            if (m_spellInfo.TargetAuraState != 0)
                m_spellInfoLog.AppendFormatLine("Requires target TargetAuraState state = {0} ({1})", m_spellInfo.TargetAuraState, (AuraState)m_spellInfo.TargetAuraState);

            if (m_spellInfo.CasterAuraStateNot != 0)
                m_spellInfoLog.AppendFormatLine("Requires caster to not have CasterAuraStateNot state = {0} ({1})", m_spellInfo.CasterAuraStateNot, (AuraState)m_spellInfo.CasterAuraStateNot);

            if (m_spellInfo.TargetAuraStateNot != 0)
                m_spellInfoLog.AppendFormatLine("Requires target to not have TargetAuraStateNot state = {0} ({1})", m_spellInfo.TargetAuraStateNot, (AuraState)m_spellInfo.TargetAuraStateNot);

            if (m_spellInfo.MaxAffectedTargets != 0)
                m_spellInfoLog.AppendFormatLine("MaxAffectedTargets = {0}", m_spellInfo.MaxAffectedTargets);

            FillSpellAuraInfo();
            FillSpellAreaInfo();

            m_spellInfoLog.AppendFormatLineIfNotNull("Requires Spell Focus {0}", m_spellInfo.RequiresSpellFocus);

            if (m_spellInfo.ProcFlags != 0)
            {
                m_spellInfoLog.SetBold();
                m_spellInfoLog.AppendFormatLine("Proc flag 0x{0:X8}, chance = {1}, charges - {2}", m_spellInfo.ProcFlags, m_spellInfo.ProcChance, m_spellInfo.ProcCharges);
                m_spellInfoLog.SetDefaultStyle();
                m_spellInfoLog.AppendFormatLine(_line);
                m_spellInfoLog.AppendText(m_spellInfo.ProcInfo);
            }
            else
            {
                m_spellInfoLog.AppendFormatLine("Chance = {0}, charges - {1}", m_spellInfo.ProcChance, m_spellInfo.ProcCharges);
            }

            FillSpellEffectInfo();
            AppendItemInfo();
            AppendDifficultyInfo();
            AppendSpellVisualInfo();
        }

        private void AppendSpellVisualInfo()
        {
            SpellVisualEntry visualData;
            if (!DBC.DBCStore.SpellVisual.TryGetValue(m_spellInfo.SpellVisual[0], out visualData))
                return;

            SpellMissileEntry missileEntry;
            SpellMissileMotionEntry missileMotionEntry;
            var hasMissileEntry = DBC.DBCStore.SpellMissile.TryGetValue(visualData.MissileModel, out missileEntry);
            var hasMissileMotion = DBC.DBCStore.SpellMissileMotion.TryGetValue(visualData.MissileMotionId, out missileMotionEntry);

            if (!hasMissileEntry && !hasMissileMotion)
                return;

            m_spellInfoLog.AppendLine(_line);
            m_spellInfoLog.SetBold();
            m_spellInfoLog.AppendLine("Missile data");
            m_spellInfoLog.SetDefaultStyle();

            // Missile Model Data.
            if (hasMissileEntry)
            {
                m_spellInfoLog.AppendFormatLine("Missile Model ID: {0}", visualData.MissileModel);
                m_spellInfoLog.AppendFormatLine("Missile attachment: {0}", visualData.MissileAttachment);
                m_spellInfoLog.AppendFormatLine("Missile cast offset: X:{0} Y:{1} Z:{2}", visualData.MissileCastOffsetX, visualData.MissileCastOffsetY, visualData.MissileCastOffsetZ);
                m_spellInfoLog.AppendFormatLine("Missile impact offset: X:{0} Y:{1} Z:{2}", visualData.MissileImpactOffsetX, visualData.MissileImpactOffsetY, visualData.MissileImpactOffsetZ);
                m_spellInfoLog.AppendFormatLine("MissileEntry ID: {0}", missileEntry.Id);
                m_spellInfoLog.AppendFormatLine("Collision Radius: {0}", missileEntry.collisionRadius);
                m_spellInfoLog.AppendFormatLine("Default Pitch: {0} - {1}", missileEntry.defaultPitchMin, missileEntry.defaultPitchMax);
                m_spellInfoLog.AppendFormatLine("Random Pitch: {0} - {1}", missileEntry.randomizePitchMax, missileEntry.randomizePitchMax);
                m_spellInfoLog.AppendFormatLine("Default Speed: {0} - {1}", missileEntry.defaultSpeedMin, missileEntry.defaultSpeedMax);
                m_spellInfoLog.AppendFormatLine("Randomize Speed: {0} - {1}", missileEntry.randomizeSpeedMin, missileEntry.randomizeSpeedMax);
                m_spellInfoLog.AppendFormatLine("Gravity: {0}", missileEntry.gravity);
                m_spellInfoLog.AppendFormatLine("Maximum duration:", missileEntry.maxDuration);
                m_spellInfoLog.AppendLine("");
            }

            // Missile Motion Data.
            if (hasMissileMotion)
            {
                m_spellInfoLog.AppendFormatLine("Missile motion: {0}", missileMotionEntry.Name);
                m_spellInfoLog.AppendFormatLine("Missile count: {0}", missileMotionEntry.MissileCount);
                m_spellInfoLog.AppendLine("Missile Script body:");
                m_spellInfoLog.AppendText(missileMotionEntry.Script);
            }
        }

        private void AppendSkillLine()
        {
            var query = from skillLineAbility in DBC.DBCStore.SkillLineAbility
                        join skillLine in DBC.DBCStore.SkillLine
                        on skillLineAbility.Value.SkillId equals skillLine.Key
                        where skillLineAbility.Value.SpellId == m_spellInfo.ID
                        select new
                        {
                            skillLineAbility,
                            skillLine
                        };

            if (query.Count() == 0)
                return;

            var skill = query.First().skillLineAbility.Value;
            var line =  query.First().skillLine.Value;

            m_spellInfoLog.AppendFormatLine("Skill (Id {0}) \"{1}\"", skill.SkillId, line.Name);
            m_spellInfoLog.AppendFormat("    ReqSkillValue {0}", skill.ReqSkillValue);

            m_spellInfoLog.AppendFormat(", Forward Spell = {0}, MinMaxValue ({1}, {2})", skill.ForwardSpellid, skill.MinValue, skill.MaxValue);
            m_spellInfoLog.AppendFormat(", CharacterPoints ({0}, {1})", skill.CharacterPoints[0], skill.CharacterPoints[1]);
        }

        private int CalculateBasePoints(int effectIndex)
        {
            int basePoints = m_spellInfo.EffectBasePoints[effectIndex];
            switch (m_spellInfo.EffectDieSides[effectIndex])
            {
                case 0: break;
                case 1: basePoints += 1; break;
                default: break;
            }

            return basePoints;
        }

        private void FillSpellEffectInfo()
        {
            m_spellInfoLog.AppendLine(_line);

            for (var effectIndex = 0; effectIndex < DBC.DBCStore.MaxEffectIndex; ++effectIndex)
            {
                m_spellInfoLog.SetBold();

                int basePoints = CalculateBasePoints(effectIndex);
                if ((SpellEffects)m_spellInfo.Effect[effectIndex] == SpellEffects.NO_SPELL_EFFECT)
                {
                    m_spellInfoLog.AppendFormatLine("Effect {0}:  NO EFFECT", effectIndex);
                    if (basePoints != 0)
                    {
                        m_spellInfoLog.AppendFormat("BasePoints = {0}", basePoints);
                        m_spellInfoLog.AppendLine();
                    }

                    m_spellInfoLog.AppendLine();
                    continue;
                }

                m_spellInfoLog.AppendFormatLine("Effect {0}: Id {1} ({2})", effectIndex, m_spellInfo.Effect[effectIndex], (SpellEffects)m_spellInfo.Effect[effectIndex]);
                m_spellInfoLog.SetDefaultStyle();
                m_spellInfoLog.AppendFormat("BasePoints = {0}", basePoints);

                if (m_spellInfo.EffectRealPointsPerLevel[effectIndex] != 0)
                    m_spellInfoLog.AppendFormat(" + Level * {0:F}", m_spellInfo.EffectRealPointsPerLevel[effectIndex]);

                int randomPoints = m_spellInfo.EffectDieSides[effectIndex];
                if (randomPoints != 0 && randomPoints != 1)
                {
                    m_spellInfoLog.AppendFormat(" to {0}", basePoints + randomPoints);
                    if (m_spellInfo.EffectRealPointsPerLevel[effectIndex] != 0)
                        m_spellInfoLog.AppendFormat(" + Level * {0:F}", m_spellInfo.EffectRealPointsPerLevel[effectIndex]);
                }

                m_spellInfoLog.AppendFormatIfNotNull(" + combo * {0:F}", m_spellInfo.EffectPointsPerComboPoint[effectIndex]);

                if (m_spellInfo.DmgMultiplier[effectIndex] != 1.0f)
                    m_spellInfoLog.AppendFormat(" x {0:F}", m_spellInfo.DmgMultiplier[effectIndex]);

                m_spellInfoLog.AppendFormatIfNotNull("  Multiple = {0:F}", m_spellInfo.EffectMultipleValue[effectIndex]);
                m_spellInfoLog.AppendFormatIfNotNull(" Bonus = {0:F}", m_spellInfo.DamageCoeficient[effectIndex]);
                m_spellInfoLog.AppendLine();

                m_spellInfoLog.AppendFormatLine("Targets ({0}, {1}) ({2}, {3})",
                    m_spellInfo.EffectImplicitTargetA[effectIndex],
                    m_spellInfo.EffectImplicitTargetB[effectIndex],
                    (Targets)m_spellInfo.EffectImplicitTargetA[effectIndex],
                    (Targets)m_spellInfo.EffectImplicitTargetB[effectIndex]
                );

                FillExtraEffectInfo(effectIndex);

                var classMask = new uint[3];

                switch (effectIndex)
                {
                    case 0:
                        classMask = m_spellInfo.EffectSpellClassMaskA;
                        break;
                    case 1:
                        classMask = m_spellInfo.EffectSpellClassMaskB;
                        break;
                    case 2:
                        classMask = m_spellInfo.EffectSpellClassMaskC;
                        break;
                }

                if (classMask[0] != 0 || classMask[1] != 0 || classMask[2] != 0)
                {
                    m_spellInfoLog.AppendFormatLine("SpellClassMask = {0:X8} {1:X8} {2:X8}", classMask[0], classMask[1], classMask[2]);

                    var query = from spell in DBC.DBCStore.Spell.Values
                                where spell.SpellFamilyName == m_spellInfo.SpellFamilyName && spell.SpellFamilyFlags.ContainsElement(classMask)
                                join sk in DBC.DBCStore.SkillLineAbility on spell.ID equals sk.Value.SpellId into temp
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
                            m_spellInfoLog.SelectionColor = Color.Blue;
                            m_spellInfoLog.AppendFormatLine("\t+ {0} - {1}",  row.SpellID, row.SpellName);
                        }
                        else
                        {
                            m_spellInfoLog.SelectionColor = Color.Red;
                            m_spellInfoLog.AppendFormatLine("\t- {0} - {1}", row.SpellID, row.SpellName);
                        }
                        m_spellInfoLog.SelectionColor = Color.Black;
                    }
                }

                m_spellInfoLog.AppendFormatLineIfNotNull("{0}", m_spellInfo.GetRadius(effectIndex));

                // append trigger spell
                var trigger = m_spellInfo.EffectTriggerSpell[effectIndex];
                if (trigger != 0)
                {
                    if (DBC.DBCStore.Spell.ContainsKey(trigger))
                    {
                        var triggerSpell = DBC.DBCStore.Spell[trigger];
                        m_spellInfoLog.SetStyle(Color.Blue, FontStyle.Bold);
                        m_spellInfoLog.AppendFormatLine("   Trigger spell ({0}) {1}. Chance = {2}", trigger, triggerSpell.SpellNameRank, m_spellInfo.ProcChance);
                        m_spellInfoLog.AppendFormatLineIfNotNull("   Description: {0}", triggerSpell.Description);
                        m_spellInfoLog.AppendFormatLineIfNotNull("   ToolTip: {0}", triggerSpell.ToolTip);
                        m_spellInfoLog.SetDefaultStyle();

                        if (triggerSpell.ProcFlags != 0)
                        {
                            m_spellInfoLog.AppendFormatLine("Charges - {0}", triggerSpell.ProcCharges);
                            m_spellInfoLog.AppendLine(_line);
                            m_spellInfoLog.AppendLine(triggerSpell.ProcInfo);
                            m_spellInfoLog.AppendLine(_line);
                        }
                    }
                    else
                    {
                        m_spellInfoLog.AppendFormatLine("Trigger spell ({0}) Not found, Chance = {1}", trigger, m_spellInfo.ProcChance);
                    }
                }

                m_spellInfoLog.AppendFormatLineIfNotNull("EffectChainTarget = {0}", m_spellInfo.EffectChainTarget[effectIndex]);
                m_spellInfoLog.AppendFormatLineIfNotNull("EffectItemType = {0}", m_spellInfo.EffectItemType[effectIndex]);

                if ((Mechanics)m_spellInfo.EffectMechanic[effectIndex] != Mechanics.MECHANIC_NONE)
                    m_spellInfoLog.AppendFormatLine("Effect Mechanic = {0} ({1})", m_spellInfo.EffectMechanic[effectIndex], (Mechanics)m_spellInfo.EffectMechanic[effectIndex]);

                if (m_spellInfo.EffectImplicitTargetA[effectIndex] == (uint)Targets.TARGET_DEST_DB || m_spellInfo.EffectImplicitTargetB[effectIndex] == (uint)Targets.TARGET_DEST_DB)
                {
                    uint mapId = 0;
                    float x = 0;
                    float y = 0;
                    float z = 0;
                    float o = 0;

                    m_spellInfoLog.AppendLine();
                    m_spellInfoLog.AppendLine("spell_target_position DB data:");
                    if (SqlConnection.GetDBSpellPosition(m_spellInfo.ID, (uint)effectIndex, ref mapId, ref x, ref y, ref z, ref o))
                        m_spellInfoLog.AppendFormatLine("mapId: {0}, x: {1:F}, y {2:F}, z {3:F}, o: {4:F}", mapId, x, y, z, o);
                    else
                        m_spellInfoLog.AppendFormatLine("NO data");
                }
                m_spellInfoLog.AppendLine();
            }
        }

        private void FillSpellAuraInfo()
        {
            if (m_spellInfo.CasterAuraSpell != 0)
            {
                if (DBC.DBCStore.Spell.ContainsKey(m_spellInfo.CasterAuraSpell))
                    m_spellInfoLog.AppendFormatLine("  Caster Aura Spell ({0}) {1}", m_spellInfo.CasterAuraSpell, DBC.DBCStore.Spell[m_spellInfo.CasterAuraSpell].SpellName);
                else
                    m_spellInfoLog.AppendFormatLine("  Caster Aura Spell ({0}) ?????", m_spellInfo.CasterAuraSpell);
            }

            if (m_spellInfo.TargetAuraSpell != 0)
            {
                if (DBC.DBCStore.Spell.ContainsKey(m_spellInfo.TargetAuraSpell))
                    m_spellInfoLog.AppendFormatLine("  Target Aura Spell ({0}) {1}", m_spellInfo.TargetAuraSpell, DBC.DBCStore.Spell[m_spellInfo.TargetAuraSpell].SpellName);
                else
                    m_spellInfoLog.AppendFormatLine("  Target Aura Spell ({0}) ?????", m_spellInfo.TargetAuraSpell);
            }

            if (m_spellInfo.ExcludeCasterAuraSpell != 0)
            {
                if (DBC.DBCStore.Spell.ContainsKey(m_spellInfo.ExcludeCasterAuraSpell))
                    m_spellInfoLog.AppendFormatLine("  Ex Caster Aura Spell ({0}) {1}", m_spellInfo.ExcludeCasterAuraSpell, DBC.DBCStore.Spell[m_spellInfo.ExcludeCasterAuraSpell].SpellName);
                else
                    m_spellInfoLog.AppendFormatLine("  Ex Caster Aura Spell ({0}) ?????", m_spellInfo.ExcludeCasterAuraSpell);
            }

            // ReSharper disable InvertIf
            if (m_spellInfo.ExcludeTargetAuraSpell != 0)
            {
                if (DBC.DBCStore.Spell.ContainsKey(m_spellInfo.ExcludeTargetAuraSpell))
                    m_spellInfoLog.AppendFormatLine("  Ex Target Aura Spell ({0}) {1}", m_spellInfo.ExcludeTargetAuraSpell, DBC.DBCStore.Spell[m_spellInfo.ExcludeTargetAuraSpell].SpellName);
                else
                    m_spellInfoLog.AppendFormatLine("  Ex Target Aura Spell ({0}) ?????", m_spellInfo.ExcludeTargetAuraSpell);
            }
            // ReSharper enable InvertIf
        }

        private void FillExtraEffectInfo(int index)
        {
            var applyAuraName       = (AuraType)m_spellInfo.EffectApplyAuraName[index];
            var miscValueA          = m_spellInfo.EffectMiscValue[index];
            var miscValueB          = m_spellInfo.EffectMiscValueB[index];
            var effectAplitude      = m_spellInfo.EffectAmplitude[index];

            // ////////////////////////
            // Spell effects
            // ////////////////////////

            if (applyAuraName == AuraType.SPELL_AURA_NONE)
            {
                // Append value next to MiscValueA
                if (miscValueA != 0)
                {
                    m_spellInfoLog.AppendFormat("EffectMiscValueA = {0}", miscValueA);
                    switch ((SpellEffects)m_spellInfo.Effect[index])
                    {
                        case SpellEffects.SPELL_EFFECT_ACTIVATE_OBJECT:
                            m_spellInfoLog.AppendFormat(" (Action: {0})", (GameObjectActions)miscValueA);
                            break;
                        case SpellEffects.SPELL_EFFECT_SUMMON:
                            m_spellInfoLog.AppendFormat(" (Object entry: {0})", miscValueA);
                            break;
                        case SpellEffects.SPELL_EFFECT_RESURRECT_NEW:
                            m_spellInfoLog.AppendFormat(" (Health {0}, Mana {1})", miscValueA, miscValueB);
                            break;
                        default:
                            break;
                    }

                    m_spellInfoLog.AppendLine();
                }

                m_spellInfoLog.AppendLine();
                m_spellInfoLog.AppendFormatLineIfNotNull("EffectAmplitude = {0}", effectAplitude);

                // Append extra info after all effects data
                switch ((SpellEffects)m_spellInfo.Effect[index])
                {
                    case SpellEffects.SPELL_EFFECT_DISPEL:
                    case SpellEffects.SPELL_EFFECT_STEAL_BENEFICIAL_BUFF:
                        if (miscValueA != 0)
                        {
                            m_spellInfoLog.AppendFormat("Effected dispel type: {0}", miscValueA);
                        }
                        break;
                    case SpellEffects.SPELL_EFFECT_DISPEL_MECHANIC:
                        if (miscValueA != 0)
                        {
                            m_spellInfoLog.Append("Effected mechanic types: ");
                            bool isFirst = true;
                            //DispelType
                            for (int x = (int)Mechanics.MECHANIC_CHARM; x < StaticConstants.MAX_MECHANIC_TYPES; ++x)
                            {
                                if ((miscValueA & (1 << (x - 1))) != 0)
                                {
                                    if (!isFirst)
                                        m_spellInfoLog.Append(", ");
                                    else
                                        isFirst = false;

                                    m_spellInfoLog.AppendFormat("{0}", (Mechanics)x);
                                }
                            }
                        }
                        break;
                    case SpellEffects.SPELL_EFFECT_SUMMON:
                        {
                            if (DBC.DBCStore.SummonProperties.ContainsKey((uint)miscValueB))
                            {
                                m_spellInfoLog.AppendFormat("Summon property (entry: {0}) category - {1}, type - {2}",
                                                            miscValueB,
                                                            (SummonCategory)DBC.DBCStore.SummonProperties[(uint)miscValueB].Category,
                                                            (SummonType)DBC.DBCStore.SummonProperties[(uint)miscValueB].Type);
                            }
                        }
                        break;
                    default:
                        break;
                }
                m_spellInfoLog.AppendLine();
                return;
            }

            // ////////////////////////
            // Aura effects
            // ////////////////////////

            m_spellInfoLog.AppendFormat("Aura Id {0:D} ({0})", applyAuraName);
            m_spellInfoLog.AppendFormat(", value = {0}", m_spellInfo.EffectBasePoints[index] + 1);
            m_spellInfoLog.AppendFormat(", misc = {0} (", miscValueA);

            switch (applyAuraName)
            {
                case AuraType.SPELL_AURA_MOD_STAT:
                    m_spellInfoLog.Append((UnitMods)miscValueA);
                    break;
                case AuraType.SPELL_AURA_MOD_RATING:
                    m_spellInfoLog.Append((CombatRating)miscValueA);
                    break;
                case AuraType.SPELL_AURA_ADD_FLAT_MODIFIER:
                case AuraType.SPELL_AURA_ADD_PCT_MODIFIER:
                    m_spellInfoLog.Append((SpellModOp)miscValueA);
                    break;
                // todo: more case
                default:
                    m_spellInfoLog.Append(miscValueA);
                    break;
            }

            m_spellInfoLog.AppendFormat("), miscB = {0}", miscValueB);
            m_spellInfoLog.AppendFormatLine(", periodic = {0}", m_spellInfo.EffectAmplitude[index]);

            switch (applyAuraName)
            {
                case AuraType.SPELL_AURA_OVERRIDE_SPELLS:
                    if (!DBC.DBCStore.OverrideSpellData.ContainsKey((uint)miscValueA))
                    {
                        m_spellInfoLog.SetStyle(Color.Red, FontStyle.Bold);
                        m_spellInfoLog.AppendFormatLine("Cannot find key {0} in OverrideSpellData.dbc", (uint)miscValueA);
                    }
                    else
                    {
                        m_spellInfoLog.AppendLine();
                        m_spellInfoLog.SetStyle(Color.DarkRed, FontStyle.Bold);
                        m_spellInfoLog.AppendLine("Overriding Spells:");
                        var @override = DBC.DBCStore.OverrideSpellData[(uint)miscValueA];
                        for (var i = 0; i < 10; ++i)
                        {
                            if (@override.Spells[i] == 0)
                                continue;

                            m_spellInfoLog.SetStyle(Color.DarkBlue, FontStyle.Regular);
                            m_spellInfoLog.AppendFormatLine("\t - #{0} ({1}) {2}", i + 1, @override.Spells[i],
                                DBC.DBCStore.Spell.ContainsKey(@override.Spells[i]) ? DBC.DBCStore.Spell[@override.Spells[i]].SpellName : "?????");
                        }
                        m_spellInfoLog.AppendLine();
                    }
                    break;
                case AuraType.SPELL_AURA_SCREEN_EFFECT:
                    m_spellInfoLog.SetStyle(Color.DarkBlue, FontStyle.Bold);
                    m_spellInfoLog.AppendFormatLine("ScreenEffect: {0}",
                        DBC.DBCStore.ScreenEffect.ContainsKey((uint)miscValueA) ? DBC.DBCStore.ScreenEffect[(uint)miscValueA].Name : "?????");
                    break;
                case AuraType.SPELL_AURA_MECHANIC_IMMUNITY_MASK:
                    {
                        m_spellInfoLog.Append("Effected mechanic immunities: ");
                        bool isFirst = true;
                        for (int x = (int)Mechanics.MECHANIC_CHARM; x < StaticConstants.MAX_MECHANIC_TYPES; ++x)
                        {
                            if ((miscValueA & (1 << (x - 1))) != 0)
                            {
                                if (!isFirst)
                                    m_spellInfoLog.Append(", ");
                                else
                                    isFirst = false;

                                m_spellInfoLog.AppendFormat("{0}", (Mechanics)x);
                            }
                        }
                        m_spellInfoLog.AppendLine();
                    }
                    break;
                case AuraType.SPELL_AURA_MOD_DAMAGE_DONE_VERSUS_AURASTATE:
                    m_spellInfoLog.AppendFormatLine("Effected aura state: {0}", (AuraState)miscValueA);
                    break;
                case AuraType.SPELL_AURA_MOD_MECHANIC_DAMAGE_TAKEN_PERCENT:
                    m_spellInfoLog.AppendFormatLine("Effected mechanic: {0}", (Mechanics)miscValueA);
                    break;
                case AuraType.SPELL_AURA_MOD_DAMAGE_DONE:
                case AuraType.SPELL_AURA_MOD_DAMAGE_TAKEN:
                    {
                        m_spellInfoLog.Append("Effected spell schools: ");
                        m_spellInfoLog.SetStyle(Color.Chocolate, FontStyle.Bold);
                        bool isFirst = true;
                        for (int x = (int)SpellSchools.SPELL_SCHOOL_NORMAL; x < StaticConstants.MAX_SPELL_SCHOOLS; ++x)
                        {
                            if ((miscValueA & (1 << x)) != 0)
                            {
                                if (!isFirst)
                                    m_spellInfoLog.Append(", ");
                                else
                                    isFirst = false;

                                m_spellInfoLog.AppendFormat("{0}", (SpellSchools)x);
                            }
                        }
                        m_spellInfoLog.AppendLine();
                    }
                    break;
                default:
                    break;
            }
        }

        private void AppendDifficultyInfo()
        {
            var difficultyId = m_spellInfo.SpellDifficultyId;
            if (difficultyId == 0)
                return;

            if (!DBC.DBCStore.SpellDifficulty.ContainsKey(difficultyId))
            {
                m_spellInfoLog.AppendFormatLine("Cannot find difficulty overrides for id {0} in SpellDifficulty.dbc!", difficultyId);
                return;
            }

            var modeNames = new[]
            {
                "Normal 10 (5 man normal)",
                "Normal 25 (5 man heroic)",
                "Heroic 10",
                "Heroic 25",
            };

            m_spellInfoLog.SetBold();
            m_spellInfoLog.AppendLine("Spell difficulty Ids:");

            var difficulty = DBC.DBCStore.SpellDifficulty[difficultyId];
            for (var i = 0; i < 4; ++i)
            {
                if (difficulty.SpellId[i] <= 0)
                    continue;

                m_spellInfoLog.AppendFormatLine("{0}: {1}", modeNames[i], difficulty.SpellId[i]);
            }
        }

        private void FillSpellAreaInfo()
        {
            if (m_spellInfo.AreaGroupId <= 0)
                return;

            var areaGroupId = (uint)m_spellInfo.AreaGroupId;
            if (!DBC.DBCStore.AreaGroup.ContainsKey(areaGroupId))
            {
                m_spellInfoLog.AppendFormatLine("Cannot find area group id {0} in AreaGroup.dbc!", m_spellInfo.AreaGroupId);
                return;
            }

            m_spellInfoLog.AppendLine();
            m_spellInfoLog.SetBold();
            m_spellInfoLog.AppendLine("Allowed areas:");
            while (DBC.DBCStore.AreaGroup.ContainsKey(areaGroupId))
            {
                var groupEntry = DBC.DBCStore.AreaGroup[areaGroupId];
                for (var i = 0; i < 6; ++i)
                {
                    var areaId = groupEntry.AreaId[i];
                    if (DBC.DBCStore.AreaTable.ContainsKey(areaId))
                    {
                        var areaEntry = DBC.DBCStore.AreaTable[areaId];
                        m_spellInfoLog.AppendFormatLine("{0} - {1} (Map: {2})", areaId, areaEntry.Name, areaEntry.MapId);
                    }
                }


                if (groupEntry.NextGroup == 0)
                    break;

                // Try search in next group
                areaGroupId = groupEntry.NextGroup;
            }

            m_spellInfoLog.AppendLine();
        }

        private void AppendItemInfo()
        {
            if (!SqlConnection.Connected)
                return;

            var items = from item in DBC.DBCStore.ItemTemplate
                        where  item.SpellId.ContainsElement((int)m_spellInfo.ID)
                        select item;

            if (items.Count() == 0)
                return;

            m_spellInfoLog.AppendLine(_line);
            m_spellInfoLog.SetStyle(Color.Blue, FontStyle.Bold);
            m_spellInfoLog.AppendLine("Items using this spell:");
            m_spellInfoLog.SetDefaultStyle();

            foreach (var item in items)
            {
                var name = ((int)DBC.DBCStore.Locale == 0 || string.IsNullOrEmpty(item.LocalesName)) ? item.Name : item.LocalesName;
                var desc = ((int)DBC.DBCStore.Locale == 0 || string.IsNullOrEmpty(item.LocalesDescription)) ? item.Description : item.LocalesDescription;

                desc = string.IsNullOrEmpty(desc) ? string.Empty : string.Format(" - \"{0}\"", desc);

                m_spellInfoLog.AppendFormatLine(@"   {0}: {1} {2}", item.Entry, name, desc);
            }
        }
    }
}
