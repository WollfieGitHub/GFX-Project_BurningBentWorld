using System;
using System.Linq;
using TerrainGeneration.Components;
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

        private static bool _noWindZoneWarningDisplayed;

        private void Awake()
        {
            _windZone = FindObjectOfType<WindZone>();
            
            if (!_noWindZoneWarningDisplayed && _windZone == null)
            {
                Debug.LogWarning("No wind zone found");
                _noWindZoneWarningDisplayed = true;
            }
        }

        private void Update()
        {
            var velocity = _windZone.transform.forward * _windZone.windMain;
            _renderer.materials.ToList().ForEach(_ => _.SetVector("_WindVelocity", velocity));
            _renderer.materials.ToList().ForEach(_ => _.SetFloat("_WindFrequency", _windZone.windPulseFrequency));
        }

        public void Init(Chunk chunk)
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

            // Send color mapping
            _renderer.materials.ToList().ForEach(_ => _.SetTexture("_GrassBaseColor", baseTexture));
            _renderer.materials.ToList().ForEach(_ => _.SetTexture("_GrassTipColor", tipTexture));
            _renderer.materials.ToList().ForEach(_ => _.SetTexture("_GrassMap", grassVisibility));
            
            // Give displacement due to chunk coordinates
            _renderer.materials.ToList().ForEach(_ => _.SetInt("_ChunkX", chunk.ChunkX * Chunk.Size));
            _renderer.materials.ToList().ForEach(_ => _.SetInt("_ChunkZ", chunk.ChunkZ * Chunk.Size));
        }
    }
}