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
        [SerializeField] private TerrainRenderer terrainRenderer;
        [SerializeField] private TerrainGenerator terrainGenerator;
        [SerializeField] private GameObject firePrefab;
        [SerializeField] private int initialCell_x = 0;
        [SerializeField] private int initialCell_z = 0;

        [Header("Options")]
        [SerializeField] private int maxNumberOfFireVFX = 10;
        [SerializeField] private float baseCellHP = 50f;
        [SerializeField] private float damageMultiplier = 1f;
        [SerializeField] private float averageTemperature = 20f;
        [Tooltip("Coefficient that will be applied to increase HP from the cell average temperature.")]
        [SerializeField] private float temperatureHPMultplier = 0.5f;
        [SerializeField] private float averageHumidity = 100f;
        [Tooltip("Coefficient that will be applied to increase HP from the cell average humidity.")]
        [SerializeField] private float humidityHPMultiplier = 0.5f;
        [SerializeField] private float burningLifetime = 10f;
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
        //private HashSet<Chunk> activeChunks = new HashSet<Chunk>();
        private List<FireCell> consumedActiveCells = new List<FireCell>();
        private HashSet<Chunk> chunksToUpdateTextures = new HashSet<Chunk>();

        //VFX management
        private List<GameObject> fireObjectsPool = new List<GameObject>();       

        // Start is called before the first frame update
        void Start()
        {
            terrainRenderer.OnGenerationFinished += StartFireSystem;
        }

        // Update is called once per frame
        void Update()
        {
            if (active)
            {
                UpdateActiveCells();
            }
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
                    cells[x, y] = ComputeCellStats(c, x, c.Height, y);
                }
            }

            activeCells.Add(GetKeyFromCell(cells[initialCell_x, initialCell_z]),
                cells[initialCell_x, initialCell_z]);

            active = true;

            for (int i = 0; i < maxNumberOfFireVFX; i++)
            {
                GameObject newFireVFX = Instantiate(firePrefab,
                    new Vector3(0f, 0f, 0f),
                    Quaternion.identity, 
                    transform);
                newFireVFX.SetActive(false);
                fireObjectsPool.Add(newFireVFX);
                //newFireVFX.GetComponentInChildren<VisualEffect>().SetTexture()
            }            
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
                //activeChunks.Add(terrain.GetChunkAt(cell.x, cell.z, true));
            }

            // This foreachs are here because the dict above cannot be modified while iterating through it.
            foreach (var cell in newActiveCells) { activeCells.Add(GetKeyFromCell(cell), cell); }
            newActiveCells.Clear();

            foreach (var cell in consumedActiveCells)
            {
                terrain.GetCellAt(cell.x, cell.z).burnt = true;
                chunksToUpdateTextures.Add(terrain.GetChunkAt(cell.x, cell.z, true));
                Destroy(cell.linkedGameobject);
                cell.linkedGameobject = null;
                activeCells.Remove(GetKeyFromCell(cell));
            }
            consumedActiveCells.Clear();

            foreach (var chunk in chunksToUpdateTextures)
            {
                ChunkRenderer cr = terrainRenderer.GetChunkRenderer(chunk.ChunkX, chunk.ChunkZ);
                cr.GenerateTexture();
            }
            chunksToUpdateTextures.Clear();
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
        /// Set cell 'burning' boolean on and adds the cell to the list of new active cells. Updates the flames VFX cluster.
        /// </summary>
        /// <param name="cell"></param>
        private void SetCellOnFire(FireCell cell)
        {
            cell.burning = true;
            newActiveCells.Add(cell);

            if (firePrefab != null)
            {
                cell.linkedGameobject = Instantiate(firePrefab,
                new Vector3(cell.x + 0.5f, cell.height, cell.z + 0.5f),
                Quaternion.identity, this.transform);               
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
        /// This method is responsible for determining the HP a cell will have.
        /// </summary>
        /// <param name="c">The cell from the terrain.</param>
        /// <param name="x">The cell x position.</param>
        /// <param name="y">The cell y position.</param>
        /// <returns>The newly created FireCell.</returns>
        private FireCell ComputeCellStats(Cell c, int x, float height, int z)
        {
            float cellHP;
            float burningLifetimeCell;
            if (c.Info.Ocean || c.Info.Biome.IsRiver)
            {
                cellHP = -1;
                burningLifetimeCell = burningLifetime;
            }
            else
            {
                //TODO: Revise this calculation. For now it is very arbitrary, but the idea is that the higher the temperature, the lower 
                //the HP; and the higher the precipitation, the higher the HP.
                cellHP = averageTemperature / c.Info.Temperature * temperatureHPMultplier + c.Info.Precipitation / averageHumidity * humidityHPMultiplier + baseCellHP;
                burningLifetimeCell = burningLifetime * Random.Range(1 - burningLifetimeRandomizer, 1 + burningLifetimeRandomizer);
            }

            return new FireCell(x, height, z, cellHP, burningLifetimeCell);
        }

        /// <summary>
        /// </summary>
        /// <param name="fireCell"></param>
        /// <returns>The key used in the hashtable to identify a given cell.</returns>
        private int GetKeyFromCell(FireCell fireCell)
        {
            return fireCell.z * terrain.Height + fireCell.x;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            foreach (var cell in activeCells.Values)
            {
                Gizmos.DrawCube(new Vector3(cell.x, cell.height, cell.z), new Vector3(1f, 1f, 1f));
            }
        }
    }
}
