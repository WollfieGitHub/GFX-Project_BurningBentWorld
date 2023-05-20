using System;
using UnityEngine;
using static Utils.Utils;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

namespace Code.Scripts.TerrainGeneration.Loaders
{
    public class ChunkManager
    {
        /// <summary>
        /// The desired position in number of chunks of the center chunk
        /// </summary>
        private Vector3 _position;
        /** <inheritdoc cref="_position"/> */
        public Vector3 Position
        {
            get => _position;
            set {
                var prev = _position;
                _position = new Vector3(
                    Mathf.RoundToInt(value.x),
                    Mathf.RoundToInt(value.y),
                    Mathf.RoundToInt(value.z)
                );
                OnPositionChanged(prev, _position);
            }
        }

        /// <summary>
        /// The number of chunks the player can see in front of them
        /// </summary>
        private int _renderingDistance;
        /** <inheritdoc cref="_renderingDistance"/> */
        public int RenderingDistance
        {
            get => _renderingDistance;
            set {
                if (_renderingDistance != value)
                {
                    _renderingDistance = value;
                    OnSizeChanged();
                }
            }
        }

        /** Whether the chunk at coordinate (x,z) is loaded */
        private bool[,] _loaded;

        private readonly Action<int,int> _loadChunkAt;
        private readonly Action<int,int> _unloadChunkAt;

        public ChunkManager(
            Action<int, int> loadChunkAt,    
            Action<int, int> unloadChunkAt,
            int initialDistance,
            Vector3 initialPosition
        )
        {
            _loadChunkAt = loadChunkAt;
            _unloadChunkAt = unloadChunkAt;
            _renderingDistance = initialDistance;
            
            _position = new Vector3(
                Mathf.RoundToInt(initialPosition.x),
                Mathf.RoundToInt(initialPosition.y),
                Mathf.RoundToInt(initialPosition.z)
            );
            
            _loaded = new bool[
                initialDistance * 2 + 1,
                initialDistance * 2 + 1
            ];
            
            LoadNecessaryChunks();
        }

        /// <summary>
        /// The size of the loaded chunks array changed, update accordingly
        /// </summary>
        private void OnSizeChanged()
        {
            var prevArray = _loaded;
            
            var prevRadius = (prevArray.GetLength(0) - 1) / 2;
            var nextRadius = _renderingDistance;

            // Two radius + center
            _loaded = new bool[nextRadius * 2 + 1, nextRadius * 2 + 1];
            
            for (var x = -prevRadius; x <=prevRadius; x++)
            {
                for (var z = -prevRadius; z <= prevRadius; z++)
                {
                    var newX = nextRadius + x;
                    var newZ = nextRadius + z;

                    var oldX = prevRadius + x;
                    var oldZ = prevRadius + z;

                    if (IsInBounds(newX, newZ, nextRadius*2+1, nextRadius*2+1))
                    {
                        // Chunk is moved, its old position is no longer the one of a loaded
                        // chunk, the new one is
                        _loaded[newX, newZ] = prevArray[oldX, oldZ];
                    }
                    else
                    {
                        // Chunk should be unloaded because it goes out of range
                        _unloadChunkAt(
                            Mathf.RoundToInt(Position.x) + x,
                            Mathf.RoundToInt(Position.z) + z
                        );
                    }
                }
            }
            LoadNecessaryChunks();
        }

        /// <summary>
        /// Check if the position of the center chunk should be changed
        /// </summary>
        private void OnPositionChanged(Vector3 prev, Vector3 next)
        {
            var diff = next - prev;
            var diffX = Mathf.RoundToInt(diff.x);
            var diffZ = Mathf.RoundToInt(diff.z);
            
            Debug.Log($"Diff = {diff}");
            
            if (diffX != 0 || diffZ != 0) { MoveTowards(diffX, diffZ); }
        }

        /// <summary>
        /// Move the center chunk by the specified delta coordinates
        /// </summary>
        /// <param name="dX">Difference in X coordinate</param>
        /// <param name="dZ">Difference in Z coordinate</param>
        /// <remarks>
        /// Moving towards a direction means moving all
        /// loaded chunk contrary to this direction
        /// </remarks>
        private void MoveTowards(int dX, int dZ)
        {
            var width = _loaded.GetLength(0);
            var height = _loaded.GetLength(1);

            var prevArray = _loaded;
            _loaded = new bool[width, height];

            var traversalDirX = Math.Sign(dX) != 0 ? Math.Sign(dX) : 1;
            var traversalDirZ = Math.Sign(dZ) != 0 ? Math.Sign(dZ) : 1;
            
            for (
                var x = traversalDirX > 0 ? 0 : width-1;
                traversalDirX > 0 ? x < width : x >= 0 ; 
                x += traversalDirX
            ) {
                for (
                    var z = traversalDirZ > 0 ? 0 : height-1;
                     traversalDirZ > 0 ? z < height : z >= 0; 
                    z += traversalDirZ
                ) {
                    var rX = x - dX;
                    var rZ = z - dZ;
                    
                    if (IsInBounds(rX, rZ, width, height))
                    {
                        // Chunk is moved, its old position is no longer the one of a loaded
                        // chunk, the new one is now loaded
                        _loaded[rX, rZ] = prevArray[x, z];
                    }
                    else
                    {
                        // Chunk should be unloaded because it goes out of range
                        _unloadChunkAt(
                            Mathf.RoundToInt(Position.x) + rX - _renderingDistance,
                            Mathf.RoundToInt(Position.z) + rZ - _renderingDistance
                        );
                    }
                }
            }
            
            LoadNecessaryChunks();
        }

        /// <summary>
        /// Loads all chunks which are annotated as "not loaded"
        /// by the <see cref="_loaded"/> array
        /// </summary>
        private void LoadNecessaryChunks()
        {
            var width = _loaded.GetLength(0);
            var height = _loaded.GetLength(1);
            
            for (var x = 0; x < width; x++)
            {
                for (var z = 0; z < height; z++)
                {
                    if (!_loaded[x, z])
                    {
                        _loadChunkAt(
                            Mathf.RoundToInt(Position.x) + x - _renderingDistance,
                            Mathf.RoundToInt(Position.z) + z - _renderingDistance
                        );
                        _loaded[x, z] = true;
                    }
                }
            }
        }
    }
}