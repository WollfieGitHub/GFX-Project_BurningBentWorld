using System;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Components
{
    public class Cell
    {
        public float Height => Info.Height;

        public readonly CellInfo Info;

        public Cell(CellInfo cellInfo) { Info = cellInfo; }

        public bool Burnt = false;
    }
}