using System.Runtime.Serialization;
using Code.Scripts.TerrainGeneration.Components;

namespace Code.Scripts.TerrainGeneration.Loaders.Serialization
{
    /// <summary>
    /// TODO : Add additional cell data such as fire, etc...
    /// </summary>
    public class CellSerializer : ISerializationSurrogate
    {
        private const string CellInfoLbl = "Info";
        
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var cell = (Cell)obj;
            
            info.AddValue(CellInfoLbl, cell.Info);
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var cellInfo = (CellInfo)info.GetValue(CellInfoLbl, typeof(CellInfo));

            return new Cell(cellInfo);
        }
    }
}