using System.Runtime.Serialization;
using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Layers;
using TerrainGeneration.Components;

namespace Code.Scripts.TerrainGeneration.Loaders.Serialization
{
    public class BiomeAttributeSerializer : ISerializationSurrogate
    {
        private const string AttributeBiomeLbl = "Biome";
        
        private const string AttributeHillLbl = "Hill";
        private const string AttributeOceanLbl = "Ocean";
        
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var attribute = (BiomeAttribute)obj;
            
            info.AddValue(AttributeBiomeLbl, attribute.Biome, typeof(Biome));
            
            info.AddValue(AttributeHillLbl, attribute.IsHill);
            info.AddValue(AttributeOceanLbl, attribute.IsOcean);
            
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var biome = (Biome)info.GetValue(AttributeBiomeLbl, typeof(Biome));

            var ocean = info.GetBoolean(AttributeOceanLbl);
            var hill = info.GetBoolean(AttributeHillLbl);

            return new BiomeAttribute
            {
                Biome = biome,
                IsHill = hill,
                IsOcean = ocean,
            };
        }
    }
}