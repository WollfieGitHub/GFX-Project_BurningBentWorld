using System.Runtime.Serialization;
using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration.Components;

namespace Code.Scripts.TerrainGeneration.Loaders.Serialization
{
    public class BiomeSerializer : ISerializationSurrogate
    {
        private const string BiomeIdLbl = "Id";
        
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            info.AddValue(BiomeIdLbl, ((Biome)obj).Id);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return Biome.FromId(info.GetInt32(BiomeIdLbl));
        }
    }
}