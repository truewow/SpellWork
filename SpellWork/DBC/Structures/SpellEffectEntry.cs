using System.Linq;
using System.Runtime.InteropServices;

namespace SpellWork.DBC.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public class SpellEffectEntry
    {
        public float EffectAmplitude;
        public float EffectBonusCoefficient;
        public float EffectChainAmplitude;
        public float EffectPointsPerResource;
        public float EffectRealPointsPerLevel;
        public uint[] EffectSpellClassMask;
        public float EffectPosFacing;
        public float BonusCoefficientFromAP;
        public uint ID;
        public uint DifficultyID;
        public uint Effect;
        public uint EffectAura;
        public uint EffectAuraPeriod;
        public int EffectBasePoints;
        public uint EffectChainTargets;
        public uint EffectDieSides;
        public uint EffectItemType;
        public uint EffectMechanic;
        public int[] EffectMiscValues;
        public int[] EffectRadiusIndex;
        public uint EffectTriggerSpell;
        public uint[] ImplicitTarget;
        public uint SpellID;
        public uint EffectIndex;
        public uint EffectAttributes;

        public SpellEffectScalingEntry SpellEffectScalingEntry { get; set; }

        public string MaxRadius
        {
            get
            {
                if (EffectRadiusIndex[1] == 0 || !DBC.SpellRadius.ContainsKey(EffectRadiusIndex[1]))
                    return string.Empty;

                return $"Max Radius (Id {EffectRadiusIndex[1]}) {DBC.SpellRadius[EffectRadiusIndex[1]].Radius:F}";
            }
        }

        public string Radius
        {
            get
            {
                if (EffectRadiusIndex[0] == 0 || !DBC.SpellRadius.ContainsKey(EffectRadiusIndex[0]))
                    return string.Empty;

                return $"Radius (Id {EffectRadiusIndex[0]}) {DBC.SpellRadius[EffectRadiusIndex[0]].Radius:F}";
            }
        }
    }
}
