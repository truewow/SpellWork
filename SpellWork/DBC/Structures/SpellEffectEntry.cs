using System.Runtime.InteropServices;

namespace SpellWork.DBC.Structures
{
    [StructLayout(LayoutKind.Sequential)]
    public class SpellEffectEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] EffectSpellClassMask;
        public uint ID;
        public int SpellID;
        public uint Effect;
        public uint EffectAura;
        public int EffectBasePoints;
        public uint EffectIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] EffectMiscValues;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] EffectRadiusIndex;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] ImplicitTarget;
        public uint DifficultyID;
        public float EffectAmplitude;
        public uint EffectAuraPeriod;
        public float EffectBonusCoefficient;
        public float EffectChainAmplitude;
        public uint EffectChainTargets;
        public int EffectDieSides;
        public uint EffectItemType;
        public uint EffectMechanic;
        public float EffectPointsPerResource;
        public float EffectRealPointsPerLevel;
        public uint EffectTriggerSpell;
        public float EffectPosFacing;
        public uint EffectAttributes;
        public float BonusCoefficientFromAP;
        public float PvPMultiplier;

        public SpellEffectScalingEntry SpellEffectScalingEntry;

        public string MaxRadius
        {
            get
            {
                if (EffectRadiusIndex[1] == 0 || !DBC.SpellRadius.ContainsKey((int)EffectRadiusIndex[1]))
                    return string.Empty;

                return $"Max Radius (Id {EffectRadiusIndex[1]}) {DBC.SpellRadius[(int)EffectRadiusIndex[1]].Radius:F}" +
                       $" (Min: {DBC.SpellRadius[(int)EffectRadiusIndex[1]].RadiusMin:F} Max: {DBC.SpellRadius[(int)EffectRadiusIndex[1]].MaxRadius:F})";
            }
        }

        public string Radius
        {
            get
            {
                if (EffectRadiusIndex[0] == 0 || !DBC.SpellRadius.ContainsKey((int)EffectRadiusIndex[0]))
                    return string.Empty;

                return $"Radius (Id {EffectRadiusIndex[0]}) {DBC.SpellRadius[(int)EffectRadiusIndex[0]].Radius:F}" +
                       $" (Min: {DBC.SpellRadius[(int)EffectRadiusIndex[0]].RadiusMin:F} Max: {DBC.SpellRadius[(int)EffectRadiusIndex[0]].MaxRadius:F})";
            }
        }
    }
}
