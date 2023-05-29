using System.Collections.Generic;
using UnityEngine;

namespace FireSystem
{
    /// <summary>
    /// A pool for Gameobjects associated to the fire VFX.
    /// </summary>
    public class FireVFXPool
    {
        private readonly List<GameObject> _pool = new ();
        private readonly GameObject _firePrefab;
        private readonly Transform _poolParent;

        public FireVFXPool(Transform poolParent, GameObject firePrefab, int initialPoolSize)
        {            
            _firePrefab = firePrefab;
            _poolParent = poolParent;

            for (var i = 0; i < initialPoolSize; i++)
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
            if (_pool.Count == 0)
            {
                CreateNewFireVFXGameobject(_poolParent);
            }

            GameObject res = _pool[_pool.Count - 1];
            _pool.RemoveAt(_pool.Count - 1);
            return res;
        }

        /// <summary>
        /// Disable the given fireVFX and adds it to the pool to be reused.
        /// </summary>
        /// <param name="fireVFX"></param>
        public void DisableAndPushToPool(GameObject fireVFX)
        {
            fireVFX.SetActive(false);
            _pool.Add(fireVFX);
        }

        private void CreateNewFireVFXGameobject(Transform poolParent)
        {
            var newFireVFX = Object.Instantiate(_firePrefab,
                    new Vector3(0f, 0f, 0f),
                    Quaternion.identity,
                    poolParent.transform
            );
            
            newFireVFX.SetActive(false);
            _pool.Add(newFireVFX);
        }
    }
}
