using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Code.Scripts.TerrainGeneration.Vegetation.Plants;
using TerrainGeneration.Components;
using TerrainGeneration.Generators;
using TerrainGeneration.Vegetation;
using Unity.VisualScripting;
using UnityEngine;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration.Rendering
{
    [RequireComponent(typeof(TerrainGenerator))]
    public class TerrainRenderer : MonoBehaviour
    {
        [Header("Dimensions")]
        [SerializeField] private int width;
        [SerializeField] private int height;

        [Header("Appearance")]
        [SerializeField] private bool renderMesh = true;
        [SerializeField] private ChunkTexture.DisplayType displayType;
        [SerializeField] private Material material;

        private Progress<TerrainGenerator.ProgressStatus> _progress;
        public event Action OnGenerationFinished;

        public Terrain Terrain { get; private set; }

        private Transform _transform;

        private List<ChunkRenderer> _chunkRenderers;


        private bool _needMeshRefresh = false;
        private void OnValidate() => _needMeshRefresh = true;

        /// <summary>
        /// Return the chunk renderer corresponding to these cell's coordinates.
        /// </summary>
        /// <param name="chunkX">X coordinate of the chunk.</param>
        /// <param name="chunkY">Y coordinate of the chunk.</param>
        public ChunkRenderer GetChunkRenderer(int chunkX, int chunkY)
        {
            //TODO: Not implemented, waiting for Léo's modifications.
            return null;
        }

        /// <summary>
        /// Force recalculation of mesh and texture of all chunks in the terrain
        /// </summary>
        private void RecalculateMesh()
        {
            foreach (var chunkRenderer in ChunkRenderers)
            {
                chunkRenderer.RenderMesh = renderMesh;
                chunkRenderer.DisplayType = displayType;
            }
        }        

        private List<ChunkRenderer> ChunkRenderers
        {
            get
            {
                if (_chunkRenderers == null || _chunkRenderers.Count == 0)
                {
                    _chunkRenderers = new List<ChunkRenderer>();
                    _chunkRenderers.AddRange(GetComponentsInChildren<ChunkRenderer>().ToList());
                }
                return _chunkRenderers;
            }
        }
        
        private void Awake()
        {
            _transform = transform;
            
            Debug.Log("Starting terrain generation...");

            InitRender();
        }

        private async void InitRender()
        {
            var generator = GetComponent<TerrainGenerator>();
            
            Terrain = await Task.Run(() => generator.GenerateNew(width, height));
            Debug.Log($"Initiating terrain rendering with dimensions {Terrain.Width}x{Terrain.Height}...");
            
            var altitude = _transform.position.y;
            
            foreach (var chunk in Terrain)
            {
                var chunkRendererObj = new GameObject($"Chunk_Renderer#{chunk.ChunkX}#{chunk.ChunkZ}");
                chunkRendererObj.transform.position =
                    new Vector3(chunk.ChunkX * chunk.Width, altitude, chunk.ChunkZ * chunk.Height);
                chunkRendererObj.transform.SetParent(_transform);

                chunkRendererObj.AddComponent<MeshFilter>();
                
                var renderer = chunkRendererObj.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = material;

                var chunkCollider = chunkRendererObj.AddComponent<ChunkCollider>();
                chunkCollider.Chunk = chunk;

                var rb = chunkRendererObj.AddComponent<Rigidbody>();
                rb.isKinematic = true;
                rb.useGravity = false;

                var chunkGrass = chunkRendererObj.AddComponent<ChunkGrass>();
                
                var chunkRen = chunkRendererObj.AddComponent<ChunkRenderer>();
                chunkRen.RenderMesh = renderMesh;
                chunkRen.DisplayType = displayType;
                chunkRen.Init(chunk);
                ChunkRenderers.Add(chunkRen);
            }
            
            Debug.Log(Biome.RepresentedBiomes.ToCommaSeparatedString());

            //TODO: I was looking for a better way to do this, open to suggestions.
            OnGenerationFinished?.Invoke();
        }

        private void Update()
        {
            if (_needMeshRefresh)
            {
                _needMeshRefresh = false;
                StartCoroutine(nameof(RecalculateMesh));
            }
        }
    }
}