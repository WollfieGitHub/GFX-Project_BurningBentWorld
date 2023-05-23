using System.Collections.Generic;
using TerrainGeneration.Rendering;
using TerrainGeneration.Components;
using UnityEngine;
using TerrainGeneration.Generators;
using UnityEngine.VFX;

namespace FireSystem
{
    public class FireSystem : MonoBehaviour
    {
        [Header("Terrain synchronization")]
        [SerializeField] private TerrainRenderer terrainRenderer;
        [SerializeField] private TerrainGenerator terrainGenerator;        

        [Header("Configuration")]
        [SerializeField] private GameObject firePrefab;
        [Tooltip("Name given to the texture that must be updated in the fire VFX to indicate active cells")]
        [SerializeField] private string SpawningTextureNameFireVFX = "ActiveCells";
        [SerializeField] private int initialVFXPoolSize = 20;
        [SerializeField] private int initialCell_x = 0;
        [SerializeField] private int initialCell_z = 0;        

        [Header("Propagation parameters")]        
        [SerializeField] private float baseCellHP = 50f;
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private float averageTemperature = 20f;
        [Tooltip("Coefficient that will be applied to increase HP from the cell average temperature.")]
        [SerializeField] private float temperatureHPMultiplier = 0.5f;
        [SerializeField] private float averageHumidity = 100f;
        [Tooltip("Coefficient that will be applied to increase HP from the cell average humidity.")]
        [SerializeField] private float humidityHPMultiplier = 0.5f;
        [SerializeField] private float baseBurningLifetime = 10f;
        [SerializeField] private float burningLifetimeRandomizer = 0.2f;
        [SerializeField] private Vector3 windDirection = new Vector3(0, 0, 0);
        [SerializeField] private float windDamageMultiplier = 0.5f;

        //General
        private TerrainGeneration.Components.Terrain terrain;
        private bool active = false;
        private FireCell[,] cells;

        //Cell update logic       
        private Dictionary<int, FireCell> activeCells = new Dictionary<int, FireCell>();
        private List<FireCell> newActiveCells = new List<FireCell>();
        private List<FireCell> consumedActiveCells = new List<FireCell>();
        private HashSet<Chunk> chunksToUpdateTextures = new HashSet<Chunk>();

        //VFX management        
        private Dictionary<int, FireChunk> activeChunks = new Dictionary<int, FireChunk>();
        private Dictionary<int, FireChunk> chunksToUpdateVFX = new Dictionary<int, FireChunk>();
        private FireVFXPool fireVFXPool;

        // Start is called before the first frame update
        void Start()
        {
            terrainRenderer.OnGenerationFinished += StartFireSystem;
        }

        // Update is called once per frame
        void Update()
        {            
            if (!active) return;

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
            terrain = terrainRenderer.Terrain;
            cells = new FireCell[terrain.Width, terrain.Height];

            for (int x = 0; x < terrain.Width; x++)
            {
                for (int y = 0; y < terrain.Height; y++)
                {
                    Cell c = terrain.GetCellAt(x, y);
                    cells[x, y] = new FireCell(c, x, y,
                        averageTemperature, averageHumidity, temperatureHPMultiplier, humidityHPMultiplier,
                        baseCellHP, baseBurningLifetime, burningLifetimeRandomizer);
                }
            }

            fireVFXPool = new FireVFXPool(this.transform, firePrefab, initialVFXPoolSize);

            active = true;

            //TODO: Arbitrarly starting a fire here. Will remove later.            
            SetCellOnFire(cells[initialCell_x, initialCell_z]);
        }

        /// <summary>
        /// Update each active cell in the dictionnary, and its corresponding neighbors.
        /// </summary>
        private void UpdateActiveCells()
        {
            foreach (var cell in activeCells.Values)
            {
                UpdateNeighbors(cell);
                cell.burningLifetime -= Time.deltaTime;
                if (cell.burningLifetime < 0) { consumedActiveCells.Add(cell); }
            }
        }

        /// <summary>
        /// Update the cells that have just been set on fire and mark the corresponding chunks to be updated on VFX.
        /// Cannot be done in the same iteration that active cells are updated in (dict cannot be modified).
        /// </summary>
        private void UpdateNewActiveCells()
        {
            foreach (var cell in newActiveCells)
            {
                activeCells.Add(GetKeyFromCell(cell), cell);
            }

            newActiveCells.Clear();
        }

        /// <summary>
        /// Remove consumed cells from active cells and mark the corresponding chunks to be updated both in mesh and VFX.
        /// Cannot be done in the same iteration that active cells are updated in (dict cannot be modified).
        /// </summary>
        private void UpdateConsumedCells()
        {
            foreach (var cell in consumedActiveCells)
            {
                //Mark the chunk to update the terrain texture
                Chunk terrainChunk = terrain.GetChunkAt(cell.x, cell.z, true);
                terrain.GetCellAt(cell.x, cell.z).burnt = true;
                chunksToUpdateTextures.Add(terrainChunk);

                //Mark the chunk to update the VFX texture (the value should always exist in the dictionnary)          
                activeChunks.TryGetValue(GetKeyFromChunk(terrainChunk), out var modifiedFireChunk);
                chunksToUpdateVFX.TryAdd(GetKeyFromChunk(modifiedFireChunk), modifiedFireChunk);
                
                modifiedFireChunk.activeCells.Remove(GetKeyFromCell(cell));

                activeCells.Remove(GetKeyFromCell(cell));
            }

            consumedActiveCells.Clear();
        }

        private void UpdateModifiedChunks()
        {
            //Update chunks' textures
            foreach (var chunk in chunksToUpdateTextures)
            {
                ChunkRenderer cr = terrainRenderer.GetChunkRenderer(chunk.ChunkX, chunk.ChunkZ);
                //TODO: Waiting for L�o's modifications.
                //cr.GenerateTexture();
            }

            chunksToUpdateTextures.Clear();

            //Update chunks' VFX
            foreach (var fireChunk in chunksToUpdateVFX.Values)
            {                
                if (fireChunk.activeCells.Count == 0)
                {
                    fireVFXPool.DisableAndPushToPool(fireChunk.linkedVFXGameObject);
                    activeChunks.Remove(GetKeyFromChunk(fireChunk));                    
                }
                else
                {                    
                    fireChunk.UpdateVFXTexture(SpawningTextureNameFireVFX);
                }
            }

            chunksToUpdateVFX.Clear();
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

            for (int deltaz = -1; deltaz < 2; deltaz++)
            {
                for (int deltax = -1; deltax < 2; deltax++)
                {
                    int neighborX = cell.x + deltax;
                    int neighborZ = cell.z + deltaz;

                    if (neighborX >= 0 && neighborZ >= 0
                        && neighborX < terrain.Width && neighborZ < terrain.Height
                        && (neighborX != cell.x || neighborZ != cell.z))
                    {
                        FireCell neighbor = cells[neighborX, neighborZ];

                        if (!activeCells.ContainsKey(GetKeyFromCell(neighbor))
                            && neighbor.HP > 0)
                        {
                            neighbor.HP -= Time.deltaTime * (damageMultiplier +
                                windDamageMultiplier * Vector3.Dot(windDirection,
                                new Vector3(neighbor.x, neighbor.height, neighbor.z) - new Vector3(cell.x, cell.height, cell.z)));

                            if (neighbor.HP < 0) { SetCellOnFire(neighbor); }
                        }
                    }
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
            newActiveCells.Add(cell);

            //Find out which chunks have been modified and if they were already active chunks.
            if (activeChunks.TryGetValue(GetKeyFromChunk(terrain.GetChunkAt(cell.x, cell.z, true)), out var fireChunk))
            {
                //Update active cell count in the chunk and update VFX accordingly.                
                fireChunk.activeCells.Add(GetKeyFromCell(cell), cell);
                fireChunk.UpdateVFXTexture(SpawningTextureNameFireVFX);
                chunksToUpdateVFX.TryAdd(GetKeyFromChunk(fireChunk), fireChunk);
            }
            else
            {
                //Create all necessary elements for the new FireChunk.
                Chunk newChunk = terrain.GetChunkAt(cell.x, cell.z, true);
                FireChunk newFireChunk = new FireChunk(newChunk.ChunkX, newChunk.ChunkZ, fireVFXPool.PopPool(), terrain);
                
                newFireChunk.activeCells.Add(GetKeyFromCell(cell), cell);

                activeChunks.TryAdd(GetKeyFromChunk(newFireChunk), newFireChunk);
                chunksToUpdateVFX.Add(GetKeyFromChunk(newFireChunk), newFireChunk);
            }
        }

        /// <summary>
        /// Sets the closest cell to the given vector on fire, assuming the position is inside the terrain (just simple floor rounding).
        /// </summary>
        /// <param name="position"></param>
        public void SetCellOnFire(Vector3 position)
        {
            Debug.Log($"Setting new cell on fire at {position}");
            int x = (int)position.x;
            int z = (int)position.z;
            if (x >= 0 && x < terrain.Width
                && z >= 0 && z < terrain.Height)
            {
                Debug.Log($"Setting new cell on fire at {x}, {z}");
                Debug.Log($"Cell info: {cells[x, z]}");
                SetCellOnFire(cells[x, z]);
            }
        }

        /// <summary>
        /// </summary>
        /// <param name="fireCell"></param>
        /// <returns>The key used in the hashtable to identify a given cell.</returns>
        private int GetKeyFromCell(FireCell fireCell)
        {
            return fireCell.z * terrain.Height + fireCell.x;
        }

        /// <summary>
        /// Note that the key for a terrain chunk and fire chunk should be the same.
        /// </summary>
        /// <param name="fireChunk"></param>
        /// <returns>The key used in the hashtable to identify a given chunk.</returns>
        private int GetKeyFromChunk(Chunk chunk)
        {
            return chunk.ChunkZ * terrain.Height / Chunk.Size + chunk.ChunkX;
        }

        /// <summary>
        /// Note that the key for a terrain chunk and fire chunk should be the same.
        /// </summary>
        /// <param name="fireChunk"></param>
        /// <returns>The key used in the hashtable to identify a given chunk.</returns>
        private int GetKeyFromChunk(FireChunk fireChunk)
        {
            return fireChunk.z * terrain.Height / Chunk.Size + fireChunk.x;
        }

        private void OnDrawGizmosSelected()
        {
            float maxHeight = 0f;

            //Draw active cells with red cubes
            Gizmos.color = Color.red;
            foreach (var cell in activeCells.Values)
            {
                Gizmos.DrawCube(new Vector3(cell.x, cell.height, cell.z), new Vector3(1f, 1f, 1f));
                maxHeight = maxHeight > cell.height ? maxHeight : cell.height;
            }

            //Draw active chunks with blue wire cubes
            Gizmos.color = Color.blue;
            foreach (var chunk in activeChunks.Values)
            {
                Gizmos.DrawWireCube(new Vector3((chunk.x + 0.5f) * Chunk.Size, 0, (chunk.z + 0.5f) * Chunk.Size), new Vector3(Chunk.Size, maxHeight, Chunk.Size));
            }
        }
    }
}
