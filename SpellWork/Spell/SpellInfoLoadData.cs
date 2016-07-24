using System.Collections.Generic;
using SpellWork.DBC.Structures;

namespace SpellWork.Spell
{
    public class SpellInfoLoadData
    {
        public SpellEntry Spell { get; set; }
        public SpellAuraOptionsEntry AuraOptions { get; set; }
        public SpellAuraRestrictionsEntry AuraRestrictions { get; set; }
        public SpellCastingRequirementsEntry CastingRequirements { get; set; }
        public SpellCategoriesEntry Categories { get; set; }
        public SpellClassOptionsEntry ClassOptions { get; set; }
        public SpellCooldownsEntry Cooldowns { get; set; }
        public SpellEquippedItemsEntry EquippedItems { get; set; }
        public SpellInterruptsEntry Interrupts { get; set; }
        public SpellLevelsEntry Levels { get; set; }
        public SpellMiscEntry Misc { get; set; }
        public SpellReagentsEntry Reagents { get; set; }
        public SpellScalingEntry Scaling { get; set; }
        public SpellShapeshiftEntry Shapeshift { get; set; }
        public SynchronizedCollection<SpellTargetRestrictionsEntry> TargetRestrictions { get; } = new SynchronizedCollection<SpellTargetRestrictionsEntry>();
        public SpellTotemsEntry Totems { get; set; }
        public SpellXSpellVisualEntry SpellXSpellVisual { get; set; }
        public SynchronizedCollection<SpellEffectEntry> Effects { get; } = new SynchronizedCollection<SpellEffectEntry>(40);
        public SpellProcsPerMinuteEntry ProcsPerMinute { get; set; }
    }
}
