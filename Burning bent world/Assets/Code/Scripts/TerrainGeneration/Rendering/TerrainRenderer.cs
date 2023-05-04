using System;
using System.Collections.Generic;
using System.Linq;
using TerrainGeneration.Components;
using TerrainGeneration.Generators;
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

        private void OnValidate()
        {
            foreach (var chunkRenderer in ChunkRenderers)
            {
                chunkRenderer.RenderMesh = renderMesh;
                chunkRenderer.DisplayType = displayType;
            }
        }

        private Transform _transform;
        
        public Terrain Terrain { get; private set; }

        private List<ChunkRenderer> _chunkRenderers;

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

            var generator = GetComponent<TerrainGenerator>();
            
            Terrain = generator.GenerateNew(width, height);
        }

        private void Start()
        {
            var altitude = _transform.position.y;
            
            foreach (var chunk in Terrain)
            {
                var chunkRendererObj = new GameObject($"Chunk_Renderer#{chunk.ChunkX}#{chunk.ChunkY}");
                chunkRendererObj.transform.position =
                    new Vector3(chunk.ChunkX * chunk.Width, altitude, chunk.ChunkY * chunk.Height);
                chunkRendererObj.transform.SetParent(_transform);

                chunkRendererObj.AddComponent<MeshFilter>();
                
                var renderer = chunkRendererObj.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = material;

                var chunkRen = chunkRendererObj.AddComponent<ChunkRenderer>();
                chunkRen.RenderMesh = renderMesh;
                chunkRen.DisplayType = displayType;
                ChunkRenderers.Add(chunkRen);
            }
            
            Debug.Log(Biome.RepresentedBiomes.ToCommaSeparatedString());
        }
    }
}