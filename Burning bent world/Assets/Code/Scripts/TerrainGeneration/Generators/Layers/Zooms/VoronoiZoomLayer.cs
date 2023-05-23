using System;
using Code.Scripts.TerrainGeneration.Layers.Optimization;
using Code.Scripts.TerrainGeneration;
using TerrainGeneration;
using UnityEngine;
using static Utils.Constants;

namespace Code.Scripts.TerrainGeneration.Layers
{
    public class VoronoiZoomLayer : TransformLayer
    {
        private const int ScaleFactor = 4;

        private const double Range = 3.6;
        private const int Odds = 1024;
        
        public override CellMap Apply()
        {
            return (x, z, width, height) =>
            {
                // Decoded and transformed from : 
                // https://github.com/eldariamc/client/blob/master/src/main/java/net/minecraft/world/gen/layer/GenLayerVoronoiZoom.java

                var pX = x / ScaleFactor;
                var pZ = z / ScaleFactor;
                
                var pWidth = (width / ScaleFactor) + ScaleFactor / 2;
                var pHeight = (height / ScaleFactor) + ScaleFactor / 2;
                
                var pMap = ParentMap(pX, pZ, pWidth, pHeight);
                
                var tempWidth = (pWidth - 1) * ScaleFactor;
                var tempHeight = (pHeight - 1) * ScaleFactor;
                var tempCells = MapAllocator.GetNew(tempWidth, tempHeight);
                
                for (var tX = 0; tX < pHeight - 1; ++tX)
                {
                    for (var tZ = 0; tZ < pWidth - 1; ++tZ)
                    {
                        InitChunkSeed((pX + tX) * ScaleFactor, (pZ + tZ) * ScaleFactor);
                        var p1Z = (NextInt(Odds) / (double)Odds - 0.5) * Range;
                        var p1X = (NextInt(Odds) / (double)Odds - 0.5) * Range;
                        
                        InitChunkSeed((pX + tX + 1) * ScaleFactor, (pZ + tZ) * ScaleFactor);
                        var p2Z = (NextInt(Odds) / (double)Odds - 0.5) * Range + ScaleFactor;
                        var p2X = (NextInt(Odds) / (double)Odds - 0.5) * Range;
                        
                        InitChunkSeed((pX + tX) * ScaleFactor, (pZ + tZ + 1) * ScaleFactor);
                        var p3Z = (NextInt(Odds) / (double)Odds - 0.5) * Range;
                        var p3X = (NextInt(Odds) / (double)Odds - 0.5) * Range + ScaleFactor;
                        
                        InitChunkSeed((pX + tX + 1) * ScaleFactor, (pZ + tZ + 1) * ScaleFactor);
                        var p4Z = (NextInt(Odds) / (double)Odds - 0.5) * Range + ScaleFactor;
                        var p4X = (NextInt(Odds) / (double)Odds - 0.5) * Range + ScaleFactor;

                        var cell1 = pMap[tX, tZ];
                        var cell2 = pMap[tX+1, tZ];
                        var cell3 = pMap[tX, tZ+1];
                        var cell4 = pMap[tX+1, tZ+1];

                        for (var dX = 0; dX < ScaleFactor; ++dX)
                        {
                            for (var dZ = 0; dZ < ScaleFactor; ++dZ)
                            {
                                var d1 = (dX - p1X) * (dX - p1X) + (dZ - p1Z) * (dZ - p1Z);
                                var d2 = (dX - p2X) * (dX - p2X) + (dZ - p2Z) * (dZ - p2Z);
                                var d3 = (dX - p3X) * (dX - p3X) + (dZ - p3Z) * (dZ - p3Z);
                                var d4 = (dX - p4X) * (dX - p4X) + (dZ - p4Z) * (dZ - p4Z);

                                if (d1 < d2 && d1 < d3 && d1 < d4)
                                {
                                    tempCells[
                                        tX * ScaleFactor + dX,
                                        tZ * ScaleFactor + dZ
                                    ] = cell1;
                                }
                                else if (d2 < d1 && d2 < d3 && d2 < d4)
                                {
                                    tempCells[
                                        tX * ScaleFactor + dX,
                                        tZ * ScaleFactor + dZ
                                    ] = cell3;
                                }
                                else if (d3 < d1 && d3 < d2 && d3 < d4)
                                {
                                    tempCells[
                                        tX * ScaleFactor + dX,
                                        tZ * ScaleFactor + dZ
                                    ] = cell2;
                                }
                                else
                                {
                                    tempCells[
                                        tX * ScaleFactor + dX,
                                        tZ * ScaleFactor + dZ
                                    ] = cell4;
                                }
                            }
                        }
                    }
                }
                
                var startX = x & 3;
                var startZ = z & 3;

                var resultCells = MapAllocator.GetNew(width, height);

                for (var rX = startX; rX < startX + width; rX++)
                {
                    for (var rZ = startZ; rZ < startZ + height; rZ++)
                    {
                        try
                        {
                            resultCells[rX - startX, rZ - startZ] = tempCells[rX, rZ];
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            Debug.LogError($"Res : [{width}, {height}] -> ({rX-startX}, {rZ-startZ})");
                            Debug.LogError($"Tmp : [{tempWidth}, {tempHeight}] -> ({rX}, {rZ})");
                            throw e;
                        }
                    }
                }

                return resultCells;
            };
        }

        public VoronoiZoomLayer(long baseSeed) : base(baseSeed) { }
    }
}