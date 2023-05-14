using System.Collections;
using System.Collections.Generic;
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
        public float burningLifetime;
        public GameObject linkedGameobject;
        //TODO: Add a radius of effect maybe

        public FireCell(int x, float height, int z, float HP, float burningLifetime)
        {
            this.HP = HP;
            this.x = x;
            this.height = height;
            this.z = z;
            this.burningLifetime = burningLifetime;
        }
    }
}
