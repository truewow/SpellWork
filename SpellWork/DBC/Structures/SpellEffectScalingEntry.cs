using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SpellWork.DBC.Structures
{
    public sealed class SpellEffectScalingEntry
    {
        public uint Id;                                                // 1       Id
        public float Coefficient;                                      // 2
        public float Variance;                                         // 3
        public float ResourceCoefficient;                              // 4
        public uint SpellEffectId;                                     // 5
    }
}
