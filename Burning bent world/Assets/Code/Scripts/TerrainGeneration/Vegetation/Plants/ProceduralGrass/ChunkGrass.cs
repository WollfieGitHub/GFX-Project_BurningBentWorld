﻿using Code.Scripts.TerrainGeneration.Components;
using TerrainGeneration.Rendering;
using UnityEngine;
using Utils;
using static Utils.Utils;

namespace Code.Scripts.TerrainGeneration.Vegetation.Plants.ProceduralGrass
{
    public class ChunkGrass : MonoBehaviour
    {
        private static readonly Color BaseColorAnchor = Color255(112, 171, 87);
        private static readonly Color TipColorAnchor = Color255(212, 240, 146);
        
        private WindZone _windZone;
        private Renderer _renderer;

        private Material _grassMaterialInstance;

        private Transform _cameraTransform;
        private Transform _transform;
        private Chunk _chunk;

        private const int VisibilityThreshold = 50;

        private void Awake()
        {
            _windZone = FindObjectOfType<WindZone>();
            
            if (_windZone == null)
            {
                var windZone = new GameObject { name = "WindZone" };
                _windZone = windZone.AddComponent<WindZone>();
            }

            _transform = transform;
            _cameraTransform = Camera.main.transform;

            _chunk = GetComponent<Chunk>();
        }

        private void Update()
        {
            var velocity = _windZone.transform.forward * _windZone.windMain;
            _grassMaterialInstance.SetVector("_WindVelocity", velocity);
            _grassMaterialInstance.SetFloat("_WindFrequency", _windZone.windPulseFrequency);
            
            var visible = Vector3.Distance(_cameraTransform.position, _transform.position) < VisibilityThreshold;
            
            // Set if it should be displayed
            _renderer.materials[TerrainRenderer.GrassMaterialIdx] = visible ? _grassMaterialInstance : null;

        }

        private void Start() { Init(_chunk); }

        private void Init(Chunk chunk)
        {
            _renderer = GetComponent<Renderer>();

            var baseTexture = CreateTexture(Chunk.Size, Chunk.Size, 
                (x, z) => chunk.GetCellAt(x, z).Info.Biome.Color.Mix(BaseColorAnchor, 0.5f)
            );

            var tipTexture = CreateTexture(Chunk.Size, Chunk.Size,
                (x, z) => chunk.GetCellAt(x, z).Info.Biome.Color.Mix(TipColorAnchor, 0.5f)
            );
            
            var grassVisibility = CreateTexture(Chunk.Size, Chunk.Size,
                (x, z) =>
                {
                    var info = chunk.GetCellAt(x, z).Info;
                    var removeGrass = info.Biome.IsRiver
                                      || Biome.Shore.Equals(info.Biome)
                                      || info.Ocean;
                    return removeGrass ? Color.black : Color.white;
                }
            );

            // _grassMaterialInstance = _renderer.materials[TerrainRenderer.GrassMaterialIdx];
            
            // Send color mapping
            _grassMaterialInstance.SetTexture("_GrassBaseColor", baseTexture);
            _grassMaterialInstance.SetTexture("_GrassTipColor", tipTexture);
            _grassMaterialInstance.SetTexture("_GrassMap", grassVisibility);
            
            // Give displacement due to chunk coordinates
            _grassMaterialInstance.SetInt("_ChunkX", chunk.ChunkX * Chunk.Size);
            _grassMaterialInstance.SetInt("_ChunkZ", chunk.ChunkZ * Chunk.Size);
            
            _grassMaterialInstance.SetInt("_Visible", 0);
        }

        public void SetMaterial(Material material)
        {
            _grassMaterialInstance = material;
        }
    }
}