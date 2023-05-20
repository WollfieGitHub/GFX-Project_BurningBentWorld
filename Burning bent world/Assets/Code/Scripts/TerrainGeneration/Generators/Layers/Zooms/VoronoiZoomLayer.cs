using Code.Scripts.TerrainGeneration.Layers.Optimization;
using Code.Scripts.TerrainGeneration;
using TerrainGeneration;
using static Utils.Constants;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class VoronoiZoomLayer : TransformLayer
    {
        private const int ScaleFactor = 2;

        private const double Range = 3.6;
        private const int Odds = 1024;
        
        public override CellMap Apply()
        {
            return (x, y, width, height) =>
            {
                // Decoded and transformed from : 
                // https://github.com/eldariamc/client/blob/master/src/main/java/net/minecraft/world/gen/layer/GenLayerVoronoiZoom.java

                var pX = x / ScaleFactor;
                var pY = y / ScaleFactor;
                
                var pWidth = (width / ScaleFactor) + ScaleFactor / 2;
                var pHeight = (height / ScaleFactor) + ScaleFactor / 2;
                
                var pMap = ParentMap(pX, pY, pWidth, pHeight);
                
                var tempWidth = (pWidth - 1) * ScaleFactor;
                var tempHeight = (pHeight - 1) * ScaleFactor;
                var tempCells = MapAllocator.GetNew(tempWidth, tempHeight);
                
                for (var tX = 0; tX < pHeight - 1; ++tX)
                {
                    for (var tY = 0; tY < pWidth - 1; ++tY)
                    {
                        var p1Y = (URandom.Next(Odds) / (double)Odds - 0.5) * Range;
                        var p1X = (URandom.Next(Odds) / (double)Odds - 0.5) * Range;
                        
                        var p2Y = (URandom.Next(Odds) / (double)Odds - 0.5) * Range + ScaleFactor;
                        var p2X = (URandom.Next(Odds) / (double)Odds - 0.5) * Range;
                        
                        var p3Y = (URandom.Next(Odds) / (double)Odds - 0.5) * Range;
                        var p3X = (URandom.Next(Odds) / (double)Odds - 0.5) * Range + ScaleFactor;
                        
                        var p4Y = (URandom.Next(Odds) / (double)Odds - 0.5) * Range + ScaleFactor;
                        var p4X = (URandom.Next(Odds) / (double)Odds - 0.5) * Range + ScaleFactor;

                        var cell1 = pMap[tX, tY];
                        var cell2 = pMap[tX+1, tY];
                        var cell3 = pMap[tX, tY+1];
                        var cell4 = pMap[tX+1, tY+1];

                        for (var dX = 0; dX < ScaleFactor; ++dX)
                        {
                            for (var dY = 0; dY < ScaleFactor; ++dY)
                            {
                                var d1 = (dX - p1X) * (dX - p1X) + (dY - p1Y) * (dY - p1Y);
                                var d2 = (dX - p2X) * (dX - p2X) + (dY - p2Y) * (dY - p2Y);
                                var d3 = (dX - p3X) * (dX - p3X) + (dY - p3Y) * (dY - p3Y);
                                var d4 = (dX - p4X) * (dX - p4X) + (dY - p4Y) * (dY - p4Y);

                                if (d1 < d2 && d1 < d3 && d1 < d4)
                                {
                                    tempCells[
                                        tX * ScaleFactor + dX,
                                        tY * ScaleFactor + dY
                                    ] = cell1;
                                }
                                else if (d2 < d1 && d2 < d3 && d2 < d4)
                                {
                                    tempCells[
                                        tX * ScaleFactor + dX,
                                        tY * ScaleFactor + dY
                                    ] = cell3;
                                }
                                else if (d3 < d1 && d3 < d2 && d3 < d4)
                                {
                                    tempCells[
                                        tX * ScaleFactor + dX,
                                        tY * ScaleFactor + dY
                                    ] = cell2;
                                }
                                else
                                {
                                    tempCells[
                                        tX * ScaleFactor + dX,
                                        tY * ScaleFactor + dY
                                    ] = cell4;
                                }
                            }
                        }
                    }
                }
                
                var startX = x & 3;
                var startY = x & 3;

                var resultCells = MapAllocator.GetNew(width, height);

                for (var rx = startX; rx < startX + width; rx++)
                {
                    for (var ry = startY; ry < startY + height; ry++)
                    {
                        resultCells[rx - startX, ry - startY] = tempCells[rx, ry];
                    }
                }

                return resultCells;
            };
        }

        public VoronoiZoomLayer(long baseSeed) : base(baseSeed) { }
    }
}