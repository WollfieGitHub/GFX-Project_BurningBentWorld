using Code.Scripts.TerrainGeneration.Components;
using Code.Scripts.TerrainGeneration.Generators.Layers.Optimization;
using TerrainGeneration;

namespace Code.Scripts.TerrainGeneration.Generators.Layers.Zooms
{
    public class Zoom2Layer : TransformLayer
    {
        private const int ScaleFactor = 2;
        
        public Zoom2Layer(long seed) : base(seed) { }
        
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                var parentX = x / ScaleFactor;
                var parentZ = z / ScaleFactor;

                var parentWidth = width / ScaleFactor + 2;
                var parentHeight = height / ScaleFactor + 2;

                var parentArray = ParentMap(parentX, parentZ, parentWidth, parentHeight);

                var tempWidth = (parentWidth - 1) * ScaleFactor;
                var tempHeight = (parentHeight - 1) * ScaleFactor;

                var tempArray = MapAllocator.GetNew(tempWidth, tempHeight);

                for (var rX = 0; rX < parentWidth - 1; rX++)
                {
                    for (var rZ = 0; rZ < parentHeight - 1; rZ++)
                    {
                        // Init chunk seed 
                        InitChunkSeed((parentX + rX) * 2, (parentZ + rZ) * 2);
                        tempArray[rX * ScaleFactor, rZ * ScaleFactor] = parentArray[rX, rZ];

                        tempArray[rX * ScaleFactor + 1, rZ * ScaleFactor] =
                            SelectRandom(new[] { parentArray[rX, rZ], parentArray[rX + 1, rZ] });

                        tempArray[rX * ScaleFactor, rZ * ScaleFactor + 1] =
                            SelectRandom(new[] { parentArray[rX, rZ], parentArray[rX, rZ + 1] });

                        tempArray[rX * ScaleFactor + 1, rZ * ScaleFactor + 1] = LastSelection(
                            parentArray[rX, rZ],
                            parentArray[rX + 1, rZ],
                            parentArray[rX, rZ + 1],
                            parentArray[rX + 1, rZ + 1]
                        );
                    }
                }

                var startX = x & 1;
                var startZ = z & 1;

                var resultArray = MapAllocator.GetNew(width, height);

                for (var rx = startX; rx < startX + width; rx++)
                {
                    for (var rZ = startZ; rZ < startZ + height; rZ++)
                    {
                        resultArray[rx - startX, rZ - startZ] = tempArray[rx, rZ];
                    }
                }
                
                return resultArray;
            };
        }

        /** Finds the cell which is represented in majority in a square and select at random if no majority */
        protected virtual CellInfo LastSelection(CellInfo c1, CellInfo c2, CellInfo c3, CellInfo c4)
        {
            if (c1.Equals(c2) && c2.Equals(c3)) { return c1; }
            if (c2.Equals(c3) && c3.Equals(c4)) { return c2; }
            if (c3.Equals(c4) && c4.Equals(c1)) { return c3; }
            
            if (c1.Equals(c2) && !c3.Equals(c4)) { return c1; }
            if (c1.Equals(c3) && !c2.Equals(c4)) { return c1; }
            if (c1.Equals(c4) && !c2.Equals(c3)) { return c1; }
            
            if (c2.Equals(c3) && !c1.Equals(c4)) { return c2; }
            if (c2.Equals(c4) && !c1.Equals(c3)) { return c2; }
            
            if (c3.Equals(c4) && !c1.Equals(c2)) { return c3; }

            // I know I could have written a function but it's fun to write it like that
            return SelectRandom(new[] { c1, c2, c3, c4 });
        }
    }

    public class FuzzyZoom2Layer : Zoom2Layer
    {
        public FuzzyZoom2Layer(long seed) : base(seed) { }

        protected override CellInfo LastSelection(CellInfo c1, CellInfo c2, CellInfo c3, CellInfo c4)
        {
            return SelectRandom(new[] { c1, c2, c3, c4 });
        }
    }

    // Perfect zoom
    public class DebugZoom2Layer : Zoom2Layer
    {
        public DebugZoom2Layer(long seed) : base(seed) { }

        protected override CellInfo LastSelection(CellInfo c1, CellInfo c2, CellInfo c3, CellInfo c4) => c1;

        protected override T SelectRandom<T>(T[] collection) => collection[0];
    }
}