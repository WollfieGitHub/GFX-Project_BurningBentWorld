﻿using System;
using UnityEngine;

namespace TerrainGeneration.Components
{
    /**
     * 2D Cell in the terrain, its underlay/overlay field represent anything we could
     * want to display under/on the cell's level
     *
     * TODO List:
     * <ul style="padding: 10px">
     *  <li>Rivers which go from sea to inside the land</li>
     *  <li></li>
     *  <li></li>
     * </ul>
     */
    public class Cell
    {
        public float Height => Info.Height;

        public readonly CellInfo Info;

        public Cell(CellInfo cellInfo) { Info = cellInfo; }

        private MonoBehaviour _overlay;
        private MonoBehaviour _underlay;        

        public Boolean burnt = false;
    }
}