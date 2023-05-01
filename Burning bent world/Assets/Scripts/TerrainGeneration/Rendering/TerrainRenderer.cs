using System;
using System.Collections.Generic;
using System.Linq;
using TerrainGeneration.Components;
using TerrainGeneration.Generators;
using UnityEngine;
using Terrain = TerrainGeneration.Components.Terrain;

namespace TerrainGeneration.Rendering
{
    public class TerrainRenderer : MonoBehaviour
    {
        [Header("Dimensions")]
        [SerializeField] private int width;
        [SerializeField] private int height;
        
        
        [Header("Appearance")]
        [SerializeField] private bool renderMesh = true;
        [SerializeField] private Material material;

        private void OnValidate()
        {
            foreach (var chunkRenderer in ChunkRenderers) { chunkRenderer.RenderMesh = renderMesh; }
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
            
            Terrain = TerrainGenerator.GenerateNew(
                width / Chunk.Size,
                height / Chunk.Size
            );
            
            
        }

        private void Start()
        {
            var counter = 0;

            var altitude = _transform.position.y;
            
            foreach (var chunk in Terrain)
            {
                var chunkRendererObj = new GameObject($"Chunk_Renderer#{counter++}");
                chunkRendererObj.transform.position =
                    new Vector3(chunk.ChunkX * chunk.Width, altitude, chunk.ChunkY * chunk.Height);
                chunkRendererObj.transform.SetParent(_transform);

                chunkRendererObj.AddComponent<MeshFilter>();
                
                var renderer = chunkRendererObj.AddComponent<MeshRenderer>();
                renderer.sharedMaterial = material;

                var chunkRen = chunkRendererObj.AddComponent<ChunkRenderer>();
                chunkRen.RenderMesh = renderMesh;
                ChunkRenderers.Add(chunkRen);
            }
        }
    }
}