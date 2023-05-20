using System.Collections.Generic;
using UnityEngine;

namespace FireSystem
{
    /// <summary>
    /// A pool for Gameobjects associated to the fire VFX.
    /// </summary>
    public class FireVFXPool
    {
        private List<GameObject> pool = new List<GameObject>();
        private GameObject firePrefab;
        private Transform poolParent;

        public FireVFXPool(Transform poolParent, GameObject firePrefab, int initialPoolSize)
        {            
            this.firePrefab = firePrefab;
            this.poolParent = poolParent;

            for (int i = 0; i < initialPoolSize; i++)
            {
                CreateNewFireVFXGameobject(poolParent);
            }
        }

        /// <summary>
        /// Pops one gameobject from the pool. If there are not enough gameobjects, will create a new one.
        /// </summary>
        /// <returns></returns>
        public GameObject PopPool()
        {
            if (pool.Count == 0)
            {
                CreateNewFireVFXGameobject(poolParent);
            }

            GameObject res = pool[pool.Count - 1];
            pool.RemoveAt(pool.Count - 1);
            return res;
        }

        /// <summary>
        /// Disable the given fireVFX and adds it to the pool to be reused.
        /// </summary>
        /// <param name="fireVFX"></param>
        public void DisableAndPushToPool(GameObject fireVFX)
        {
            fireVFX.SetActive(false);
            pool.Add(fireVFX);
        }

        private void CreateNewFireVFXGameobject(Transform poolParent)
        {
            GameObject newFireVFX = Object.Instantiate(firePrefab,
                    new Vector3(0f, 0f, 0f),
                    Quaternion.identity,
                    poolParent.transform);

            newFireVFX.SetActive(false);
            pool.Add(newFireVFX);
        }
    }
}
