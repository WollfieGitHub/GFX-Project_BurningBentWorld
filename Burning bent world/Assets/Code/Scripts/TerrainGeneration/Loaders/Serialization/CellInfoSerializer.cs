using System.Runtime.Serialization;
using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration;

namespace Code.Scripts.TerrainGeneration.Loaders.Serialization
{
    public class CellInfoSerializer : ISerializationSurrogate
    {
        private const string CellInfoTemperatureLbl = "Temperature";
        private const string CellInfoPrecipitationLbl = "Precipitation";
        
        private const string CellInfoHeightLbl = "Height";

        private const string CellInfoBiomeAttributeLbl = "Attribute";
        
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var cellInfo = (CellInfo)obj;
            
            info.AddValue(CellInfoTemperatureLbl, cellInfo.Temperature);
            info.AddValue(CellInfoPrecipitationLbl, cellInfo.Precipitation);
            
            info.AddValue(CellInfoHeightLbl, cellInfo.Height);
            
            info.AddValue(CellInfoBiomeAttributeLbl, cellInfo.BiomeAttribute);
            
            
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            return new CellInfo
            {
                BiomeAttribute = (BiomeAttribute)info.GetValue(CellInfoBiomeAttributeLbl, typeof(BiomeAttribute)),
                Height = info.GetInt32(CellInfoHeightLbl),
                Precipitation = info.GetSingle(CellInfoPrecipitationLbl),
                Temperature = info.GetSingle(CellInfoTemperatureLbl),
            };
        }
    }
}