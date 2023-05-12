using UnityEngine;

namespace Utils
{
    public class Singleton<T> : MonoBehaviour where T: MonoBehaviour
    {
        private static bool _init;
        
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_init) { return _instance; }
            
                var objs = FindObjectsOfType<T>();
                if (objs.Length > 0)
                {
                    _instance = objs[0];
                    _init = true;
                }
                if (objs.Length > 1)
                {
                    _init = true;
                    for (var i = 1; i < objs.Length; i++) { Destroy(objs[i]); }
                    Debug.LogWarning("There is more than one " + typeof(T).Name + " in the scene, destroying one of them.");
                }

                if (_init) { return _instance; }
            
                GameObject obj = new GameObject();
                obj.name = string.Format("_{0}", typeof(T).Name);
                _instance = obj.AddComponent<T>();
                _init = true;

                return _instance;
            }
        }

        protected virtual void OnDestroy()
        {
            _init = false;
            _instance = null;
        }
    }
}