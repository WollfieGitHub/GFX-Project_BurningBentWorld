using System;
using System.Collections.Generic;
using Code.Scripts.TerrainGeneration;
using Code.Scripts.TerrainGeneration.Components;
using FireSystem;
using UnityEngine;

namespace Code.Scripts.FireSystem
{
    public class FireSystem : MonoBehaviour
    {
        [Header("Terrain synchronization")]
        [SerializeField] private GeneratedTerrain terrain;

        [Header("Configuration")]
        [SerializeField] private GameObject firePrefab;
        [Tooltip("Name given to the texture that must be updated in the fire VFX to indicate active cells")]
        [SerializeField] private string spawningTextureNameFireVFX = "ActiveCells";
        [SerializeField] private int initialVFXPoolSize = 20;

        [Header("Propagation parameters")]        
        [SerializeField] private float baseCellHp = 50f;
        [SerializeField] private float damageMultiplier = 1f;
        [Tooltip("Coefficient that will be applied to increase HP from the cell average temperature.")]
        [SerializeField] private float temperatureHpMultiplier = 0.5f;
        [Tooltip("Coefficient that will be applied to increase HP from the cell average humidity.")]
        [SerializeField] private float humidityHpMultiplier = 0.5f;
        [SerializeField] private float baseBurningLifetime = 10f;
        [SerializeField] private float burningLifetimeRandomizer = 0.2f;
        [SerializeField] public Vector3 windDirection = new(0, 0, 0);
        [SerializeField] public float windDamageMultiplier = 0.5f;

        //General
        private bool _active;
        private readonly Dictionary<(int, int), FireCell> _cells = new();

        //Cell update logic       
        private readonly Dictionary<(int, int), FireCell> _activeCells = new();
        private readonly List<FireCell> _newActiveCells = new();
        private readonly List<FireCell> _consumedActiveCells = new();
        private readonly HashSet<Chunk> _chunksToUpdateTextures = new();

        //VFX management        
        private readonly Dictionary<(int, int), FireChunk> _activeChunks = new();
        private readonly Dictionary<(int, int), FireChunk> _chunksToUpdateVFX = new();
        private FireVFXPool _fireVFXPool;

        // Start is called before the first frame update
        private void Start() { StartFireSystem(); }

        // Update is called once per frame
        private void Update()
        {            
            if (!_active) return;

            UpdateActiveCells();

            UpdateNewActiveCells();

            UpdateConsumedCells();

            UpdateModifiedChunks();
        }

        /// <summary>
        /// Starts the fire system. This method assumes that the terrain has already been initialized and generated.
        /// </summary>
        private void StartFireSystem()
        {
            _fireVFXPool = new FireVFXPool(transform, firePrefab, initialVFXPoolSize);

            _active = true;
        }

        /// <summary>
        /// Update each active cell in the dictionary, and its corresponding neighbors.
        /// </summary>
        private void UpdateActiveCells()
        {
            foreach (var cell in _activeCells.Values)
            {
                UpdateNeighbors(cell);
                cell.burningLifetime -= Time.deltaTime;
                if (cell.burningLifetime < 0) { _consumedActiveCells.Add(cell); }
            }
        }

        /// <summary>
        /// Update the cells that have just been set on fire and mark the corresponding chunks to be updated on VFX.
        /// Cannot be done in the same iteration that active cells are updated in (dict cannot be modified).
        /// </summary>
        private void UpdateNewActiveCells()
        {
            foreach (var cell in _newActiveCells) { _activeCells.TryAdd((cell.x, cell.z), cell); }

            _newActiveCells.Clear();
        }

        /// <summary>
        /// Remove consumed cells from active cells and mark the corresponding chunks to be updated both in mesh and VFX.
        /// Cannot be done in the same iteration that active cells are updated in (dict cannot be modified).
        /// </summary>
        private void UpdateConsumedCells()
        {
            foreach (var cell in _consumedActiveCells)
            {
                //Mark the chunk to update the terrain texture
                var terrainChunk = terrain.GetChunkAt(cell.x / Chunk.Size, cell.z / Chunk.Size);
                terrain.GetCellAt(cell.x, cell.z).Burnt = true;
                _chunksToUpdateTextures.Add(terrainChunk);

                //Mark the chunk to update the VFX texture (the value should always exist in the dictionnary)          
                if (_activeChunks.TryGetValue((terrainChunk.ChunkX, terrainChunk.ChunkZ), out var modifiedFireChunk))
                {
                    _chunksToUpdateVFX.TryAdd((modifiedFireChunk.X, modifiedFireChunk.Z), modifiedFireChunk);

                    modifiedFireChunk.ActiveCells.Remove((cell.x, cell.z));
                }
                else { throw new InvalidOperationException(); }
                
                _activeCells.Remove((cell.x, cell.z));
            }

            _consumedActiveCells.Clear();
        }

        private void UpdateModifiedChunks()
        {
            //Update chunks' textures
            foreach (var chunk in _chunksToUpdateTextures)
            {
                // ChunkRenderer cr = terrainRenderer.GetChunkRenderer(chunk.ChunkX, chunk.ChunkZ);
                //TODO: Waiting for L�o's modifications.
                //cr.GenerateTexture();
                try
                {
                    chunk.ChunkRenderer.UpdateBurntColor();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    throw e;
                }
            }

            _chunksToUpdateTextures.Clear();

            //Update chunks' VFX
            foreach (var fireChunk in _chunksToUpdateVFX.Values)
            {                
                if (fireChunk.ActiveCells.Count == 0)
                {
                    _fireVFXPool.DisableAndPushToPool(fireChunk.LinkedVFXGameObject);
                    _activeChunks.Remove((fireChunk.X, fireChunk.Z));                    
                }
                else { fireChunk.UpdateVFXTexture(spawningTextureNameFireVFX); }
            }

            _chunksToUpdateVFX.Clear();
        }

        /// <summary>
        /// Updates health from neighboring cells, removing hp as a function of time, wind and damage modifiers.
        /// If a neighboring cell's HP is below 0, no update happens to it.
        /// This function is only useful if the given cell is on fire.
        /// </summary>
        /// <param name="cell"></param>
        private void UpdateNeighbors(FireCell cell)
        {
            if (!cell.burning) { return; }

            var cellPos = new Vector3(cell.x, cell.height, cell.z);

            for (var deltaZ = -1; deltaZ < 2; deltaZ++)
            {
                for (var deltaX = -1; deltaX < 2; deltaX++)
                {
                    var neighborX = cell.x + deltaX;
                    var neighborZ = cell.z + deltaZ;
                    
                    if (neighborX == cell.x && neighborZ == cell.z) { return; }

                    // The cell we want to update is not loaded in the terrain and does not exist in _cells
                    if (!TryGet(neighborX, neighborZ, out var neighbor)) { continue; }
                    // The neighbour is already burning or about to be set on fire
                    if (_activeCells.ContainsKey((neighbor.x, neighbor.z)) || !(neighbor.HP > 0)) continue;
                    
                    neighbor.HP -= Time.deltaTime * (
                        damageMultiplier +
                        Math.Max(windDamageMultiplier * Vector3.Dot(
                            windDirection, 
                            new Vector3(neighbor.x, neighbor.height, neighbor.z) - cellPos
                        ), 0)
                    );

                    if (neighbor.HP < 0) { SetCellOnFire(neighbor); }
                }
            }
        }

        /// <summary>
        /// Set cell 'burning' boolean on and adds the cell to the list of new active cells.
        /// Marks the corresponding chunks to be updated on VFX.
        /// </summary>
        /// <param name="cell"></param>
        private void SetCellOnFire(FireCell cell)
        {
            cell.burning = true;
            _newActiveCells.Add(cell);

            var chunk = terrain.GetChunkAt(cell.x / Chunk.Size, cell.z / Chunk.Size);
            
            //Find out which chunks have been modified and if they were already active chunks.
            if (_activeChunks.TryGetValue((chunk.ChunkX, chunk.ChunkZ), out var fireChunk))
            {
                //Update active cell count in the chunk and update VFX accordingly.                
                fireChunk.ActiveCells.TryAdd((cell.x, cell.z), cell);
                fireChunk.UpdateVFXTexture(spawningTextureNameFireVFX);
                _chunksToUpdateVFX.TryAdd((fireChunk.X, fireChunk.Z), fireChunk);
            }
            else
            {
                //Create all necessary elements for the new FireChunk.
                var newChunk = terrain.GetChunkAt(cell.x / Chunk.Size, cell.z / Chunk.Size);
                var newFireChunk = new FireChunk(newChunk.ChunkX, newChunk.ChunkZ, _fireVFXPool.PopPool());
                
                newFireChunk.ActiveCells.TryAdd((cell.x, cell.z), cell);

                _activeChunks.TryAdd((newFireChunk.X, newFireChunk.Z), newFireChunk);
                _chunksToUpdateVFX.TryAdd((newFireChunk.X, newFireChunk.Z), newFireChunk);
            }
        }

        /// <summary>
        /// Sets the closest cell to the given vector on fire, assuming the position is inside the terrain (just simple floor rounding).
        /// </summary>
        /// <param name="position"></param>
        public void SetCellOnFire(Vector3 position)
        {
            Debug.Log($"Setting new cell on fire at {position}");
            var x = (int)position.x;
            var z = (int)position.z;

            Debug.Log($"Setting new cell on fire at {x}, {z}");

            if (!TryGet(x, z, out var fireCell)) { return; }
            
            SetCellOnFire(fireCell);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="fireCell"></param>
        /// <returns></returns>
        private bool TryGet(int x, int z, out FireCell fireCell)
        {
            if (_cells.TryGetValue((x, z), out fireCell)) { return true; }

            // If the cell from the terrain is not loaded, return false
            var terrainCell = terrain.GetCellAt(x, z);
            if (terrainCell == null) { return false; }
            if (terrainCell.Info.Ocean) { return false; }

            fireCell = CreateNewCellAt(x, z, terrainCell);
            _cells[(x, z)] = fireCell;
            return true;

        }

        /// <summary>
        /// Creates a new instance of a <see cref="FireCell"/> with
        /// the value of the properties of this fire system
        /// </summary>
        /// <param name="x">X Coordinate</param>
        /// <param name="z">Z Coordinate</param>
        /// <param name="cell">The cell</param>
        /// <returns>The FireCell</returns>
        private FireCell CreateNewCellAt(int x, int z, Cell cell)
        {
            return new FireCell(
                cell, x, z, temperatureHpMultiplier,
                humidityHpMultiplier, baseCellHp, baseBurningLifetime,
                burningLifetimeRandomizer
            );
        }

        public void ResetFires()
        {
            _activeCells.Clear();
            _newActiveCells.Clear();
        }


        private void OnDrawGizmosSelected()
        {
            float maxHeight = 0f;

            //Draw active cells with red cubes
            Gizmos.color = Color.red;
            foreach (var cell in _activeCells.Values)
            {
                Gizmos.DrawCube(new Vector3(cell.x, cell.height, cell.z), new Vector3(1f, 1f, 1f));
                maxHeight = maxHeight > cell.height ? maxHeight : cell.height;
            }

            //Draw active chunks with blue wire cubes
            Gizmos.color = Color.blue;
            foreach (var chunk in _activeChunks.Values)
            {
                Gizmos.DrawWireCube(new Vector3((chunk.X + 0.5f) * Chunk.Size, 0, (chunk.Z + 0.5f) * Chunk.Size), new Vector3(Chunk.Size, maxHeight, Chunk.Size));
            }
        }
    }
}
