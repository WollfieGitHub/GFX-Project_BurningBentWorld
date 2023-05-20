using System.Collections;
using System.Collections.Generic;
using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration.Components;
using UnityEngine;

namespace FireSystem
{
    /// <summary>
    /// Stores all information needed for the fire propagation system from each cell.
    /// </summary>
    public class FireCell
    {
        public float HP;
        public int x;
        public float height;
        public int z;
        public bool burning;
        public float burningLifetime;
        //TODO: Add a radius of effect maybe                

        ///<summary>
        /// The constructor of the fireCell will be responsible for determining the stats of a cell based on the informations
        /// given to it as parameters.
        /// </summary>
        /// <param name="c">The corresponding cell from the terrain.</param>
        /// <param name="x"></param>
        /// <param name="z">The cell y position.</param>
        /// <param name="averageTemperature"> of all cells.</param>
        /// <param name="averageHumidity"> of all cells.</param>
        /// <param name="temperatureHPMultiplier"></param>
        /// <param name="humidityHPMultiplier"></param>
        /// <param name="baseCellHP"></param>
        /// <param name="burningLifetimeRandomizer"></param>
        public FireCell(Cell c, int x, int z,
            float averageTemperature, float averageHumidity, float temperatureHPMultiplier, float humidityHPMultiplier,
            float baseCellHP, float baseBurningLifetime, float burningLifetimeRandomizer)
        {                        
            if (c.Info.Ocean || c.Info.Biome.IsRiver)
            {
                HP = -1;
                burningLifetime = baseBurningLifetime;
            }
            else
            {
                //TODO: Revise this calculation. For now it is very arbitrary, but the idea is that the higher the temperature, the lower 
                //the HP; and the higher the precipitation, the higher the HP.
                HP = averageTemperature / c.Info.Temperature * temperatureHPMultiplier + c.Info.Precipitation / averageHumidity * humidityHPMultiplier + baseCellHP;
                burningLifetime = baseBurningLifetime * Random.Range(1 - burningLifetimeRandomizer, 1 + burningLifetimeRandomizer);
            }

            this.x = x;
            height = c.Height;
            this.z = z;            
            burning = false;
        }
    }
}
