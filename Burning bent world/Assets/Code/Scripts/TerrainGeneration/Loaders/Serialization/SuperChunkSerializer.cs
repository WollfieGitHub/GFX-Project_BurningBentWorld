using System.Runtime.Serialization;
using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration.Components;

namespace Code.Scripts.TerrainGeneration.Loaders.Serialization
{
    public class SuperChunkSerializer : ISerializationSurrogate
    {
        private const string SuperChunkXLbl = "X";
        private const string SuperChunkZLbl = "Z";

        private const string SuperChunkCellsLbl = "Cells";
        
        public void GetObjectData(object obj, SerializationInfo info, StreamingContext context)
        {
            var superChunk = (SuperChunk)obj;
            
            // First save chunk's coordinates
            info.AddValue(SuperChunkXLbl, superChunk.XOffset);
            info.AddValue(SuperChunkZLbl, superChunk.ZOffset);
            
            info.AddValue(SuperChunkCellsLbl, superChunk.Cells, typeof(Cell[,]));
            
        }

        public object SetObjectData(object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
        {
            var xOffset = info.GetInt32(SuperChunkXLbl);
            var zOffset = info.GetInt32(SuperChunkZLbl);

            var cells = (Cell[,]) info.GetValue(SuperChunkCellsLbl, typeof(Cell[,]));

            // Load super chunk data
            var superChunk = new SuperChunk();
            superChunk.Load(cells, xOffset, zOffset);
            
            return superChunk;
        }
    }
}