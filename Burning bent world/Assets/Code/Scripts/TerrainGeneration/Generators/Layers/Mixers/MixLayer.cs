using Code.Scripts.TerrainGeneration;
using Code.Scripts.TerrainGeneration.Generators.Layers;
using TerrainGeneration;

namespace Code.Scripts.TerrainGeneration.Layers
{
    /// <summary>
    /// A layer which takes two map as input and return as output a new map using
    /// the inputs' information
    /// </summary>
    public abstract class MixLayer : TransformLayer
    {
        protected TransformLayer Filter;

        protected CellMap FilterMap => Filter.Apply();

        public void SetFilter(TransformLayer filter) => Filter = filter;

        protected MixLayer(long baseSeed) : base(baseSeed) { }

        public override void InitWorldGenSeed(long seed)
        {
            // Also init the filter's seed
            base.InitWorldGenSeed(seed);
            Filter?.InitWorldGenSeed(seed);
        }
        
    }

    public static class MixLayerExtension
    {
        public static T Mix<T>(
            this T mixLayer, GenerationStack parent, GenerationStack filter
        ) where T : MixLayer {
            mixLayer.SetParent(parent.GetLast());
            mixLayer.SetFilter(filter.GetLast());
            return mixLayer;
        }
    }
}