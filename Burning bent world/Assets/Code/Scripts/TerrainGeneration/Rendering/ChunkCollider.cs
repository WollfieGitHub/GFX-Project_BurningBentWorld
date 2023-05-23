using System.ComponentModel;
using System.Linq;
using Code.Scripts.TerrainGeneration.Components;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Rendering
{
    public class ChunkCollider : MonoBehaviour
    {
        [SerializeField][DefaultValue(false)] private bool colliderEnabled; 

        private Transform _transform;
        private Chunk _chunk;

        private Transform _cameraTransform;

        public static float Threshold;

        private void OnValidate()
        {
            if (colliderEnabled && !_collidersLoaded) { EnableCollider(); }
            if (!colliderEnabled && _collidersLoaded) { DisableCollider(); }
        }

        private void Awake()
        {
            colliderEnabled = false;
            _collidersLoaded = false;
            
            _transform = transform;
            _chunk = GetComponent<Chunk>();

            _cameraTransform = Camera.main.transform;
        }

        public bool ColliderEnabled
        {
            get => colliderEnabled;
            set {
                if (value) { EnableCollider(); }
                else { DisableCollider(); }
            }
        }

        private Vector3 _currentPos;
        private Vector3 _currentCamPos;
        private float _currentDist;
        
        private void Update()
        {
            _currentPos = _transform.position;
            _currentCamPos = _cameraTransform.position;

            _currentDist = Mathf.Sqrt(
                (_currentPos.x - _currentCamPos.x) * (_currentPos.x - _currentCamPos.x) +
                (_currentPos.z - _currentCamPos.z) * (_currentPos.z - _currentCamPos.z)
            );

            switch (colliderEnabled)
            {
                case false when _currentDist <= Threshold: EnableCollider(); break;
                case true when _currentDist > Threshold: DisableCollider(); break;
            }
        }

        private readonly GameObject[,] _colliders = new GameObject[Chunk.Size, Chunk.Size];
        private bool _collidersLoaded;

        private void EnableCollider()
        {
            Debug.Log("WTF1");
            if (colliderEnabled && _collidersLoaded) { return; }
            Debug.Log("WTF2");

            colliderEnabled = true;
            _collidersLoaded = true;

            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var z = 0; z < Chunk.Size; z++)
                {
                    var go = new GameObject($"ChunkCollider#{x}_{z}") {
                        transform = {
                            position = new Vector3(
                                _chunk.ChunkX * Chunk.Size + x,
                                0,
                                _chunk.ChunkZ * Chunk.Size + z
                            )
                        }
                    };
                    
                    go.transform.SetParent(_transform);
                    var boxCollider = go.AddComponent<BoxCollider>();
                    _colliders[x, z] = go;

                    var cellHeight = _chunk.GetHeightAt(x, z);

                    var maxDiff = new []
                    {
                        cellHeight - _chunk.GetHeightAtOrDefault(x, z + 1, 1),
                        cellHeight - _chunk.GetHeightAtOrDefault(x, z - 1, 1),
                        cellHeight - _chunk.GetHeightAtOrDefault(x - 1, z, 1),
                        cellHeight - _chunk.GetHeightAtOrDefault(x + 1, z, 1),
                    }.Max();
                    
                    var centerY = Mathf.Lerp(
                        0.5f, cellHeight - maxDiff/2.0f, 0.5f
                    );

                    boxCollider.center = new Vector3(0, centerY, 0);
                    boxCollider.size = new Vector3(1f, maxDiff,1f);
                }
            } 
        }

        private void DisableCollider()
        {
            if (!colliderEnabled && !_collidersLoaded) { return; }
            
            colliderEnabled = false;
            _collidersLoaded = false;
            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var z = 0; z < Chunk.Size; z++)
                {
                    Destroy(_colliders[x, z]);
                }
            }
        }

        /// <summary>
        /// Recalculate the colliders of this chunk if they
        /// are enabled (If they are not, they will be recalculated in next enabling)
        /// </summary>
        public void Recalculate()
        {
            if (ColliderEnabled)
            {
                DisableCollider();
                EnableCollider();
            }
        }
    }
}