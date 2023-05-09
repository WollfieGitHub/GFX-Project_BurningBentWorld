using System;
using TerrainGeneration.Components;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Components
{
    public struct BiomeAttribute
    {
        private const float HillAmplitudeModifier = 2f;
        
        /** The biome concerned by this attribute */
        public Biome Biome;
        
        /** True if the biome is an ocean, false otherwise */
        public bool IsOcean;
        /** True if the biome is a hill, false otherwise */
        public bool IsHill;

        public bool IsShore => Biome.Shore.Equals(Biome);

        public float FBm(int x, int y)
        {
            // Between 0 and 1
            var baseFbm = Biome.FBm.Apply(x, y);

            if (IsShore) { baseFbm = 0; }
            if (IsOcean) { baseFbm *= -1; }
            if (IsHill) { baseFbm *= HillAmplitudeModifier; }

            return baseFbm;
        }

        public bool Equals(BiomeAttribute other)
        {
            return Equals(Biome, other.Biome)
                   && IsOcean == other.IsOcean
                   && IsHill == other.IsHill;
        }

        public override bool Equals(object obj)
        {
            return obj is BiomeAttribute other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Biome, IsOcean, IsHill);
        }
    }
}