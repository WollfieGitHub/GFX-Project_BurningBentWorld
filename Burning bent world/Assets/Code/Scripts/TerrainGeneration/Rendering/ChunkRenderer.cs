﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Code.Scripts.TerrainGeneration.Components;
using Unity.VisualScripting;
using UnityEngine;
using static Code.Scripts.TerrainGeneration.Rendering.ChunkTexture;
using static Utils.Utils;
using static Utils.Utils;

namespace Code.Scripts.TerrainGeneration.Rendering
{
    [RequireComponent(typeof(Renderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class ChunkRenderer : MonoBehaviour
    {

        private event Action OnTextureLoaded;
        
//======== ====== ==== ==
//      PROPERTIES
//======== ====== ==== ==
        
        private Texture2D _texture;
        private Renderer _rend;
        private MeshFilter _meshFilter;

        private static readonly Dictionary<(int, int), bool> NorthDirs = new ();
        private static readonly Dictionary<(int, int), bool> NorthCalled = new ();
        private static readonly Dictionary<(int, int), bool> NorthInvoked = new ();
        
        private static readonly Dictionary<(int, int), bool> EastDirs = new ();
        private static readonly Dictionary<(int, int), bool> EastCalled = new ();
        private static readonly Dictionary<(int, int), bool> EastInvoked = new ();

        private Mesh _mesh;

        private Chunk _chunk;

        /// <summary>
        /// Whether the mesh should be rendered using the cell's heights in the chunk
        /// or as a planar mesh if set to false
        /// </summary>
        private bool _renderMesh;
        ///<summary><inheritdoc cref="_displayType"/></summary>
        public bool RenderMesh
        {
            set
            {
                _renderMesh = value;
                if (_started)
                {
                    StartCoroutine(nameof(CalcNewMesh));
                }
            }
        }

        /// <summary>
        /// The <see cref="ChunkTexture.DisplayType"/> that
        /// the renderer should use to display the chunk
        /// </summary>
        private DisplayType _displayType;
        ///<summary><inheritdoc cref="_displayType"/></summary>
        public DisplayType DisplayType
        {
            set
            {
                _displayType = value;
                if (_started)
                {
                    StartCoroutine(nameof(CalcNewTexture));
                }
            }
        }

        private bool _started;
        
//======== ====== ==== ==
//      LIFECYCLE
//======== ====== ==== ==

        private void Awake()
        {
            _chunk = GetComponent<Chunk>();
            
            // Collect rendering components
            _rend = GetComponent<Renderer>();
            _meshFilter = GetComponent<MeshFilter>();
        }

        private void OnEnable() => _chunk.NeighbourStateChanged += OnNeighbourChanged;

        private void OnDisable()
        {
            _chunk.NeighbourStateChanged -= OnNeighbourChanged;
            
            var c = NorthDirs.Keys
                .Union(EastDirs.Keys)
                .Union(NorthCalled.Keys)
                .Union(EastCalled.Keys);

            var sb = new StringBuilder();
            foreach (var (chunkX, chunkZ) in c)
            {
                var north = NorthDirs.GetValueOrDefault((chunkX, chunkZ), false);
                var northCalled = NorthCalled.GetValueOrDefault((chunkX, chunkZ), false);
                var northInvoked = NorthInvoked.GetValueOrDefault((chunkX, chunkZ), false);
                
                var east = EastDirs.GetValueOrDefault((chunkX, chunkZ), false);
                var eastCalled = EastCalled.GetValueOrDefault((chunkX, chunkZ), false);
                var eastInvoked = EastInvoked.GetValueOrDefault((chunkX, chunkZ), false);

                if (!north || !east || !northCalled || !eastCalled)
                {
                    sb.Append($"[{chunkX}, {chunkZ}]> North :")
                        .Append(north).Append(" - C:[").Append(northCalled).Append("]:I:[").Append(northInvoked)
                        .Append("], East : ")
                        .Append(east).Append(" - C:[").Append(eastCalled).Append("]:I:[").Append(eastInvoked)
                        .Append("]\n");
                }
            }
            LogInfoOnce(sb.ToString());
        }

        /// <summary>
        /// A neighbour's state has changed, we must recompute the
        /// mesh so that the boundaries adapt to the neighbouring chunk's bound appearance
        /// </summary>
        private void OnNeighbourChanged(Direction direction)
        {
            CalcNewMesh();
            TryUpdateTexture(direction);
        }

        private void TryUpdateTexture(Direction direction)
        {
            switch (direction)
            {
                case Direction.North: NorthInvoked[(_chunk.ChunkX, _chunk.ChunkZ)] = true; break;
                case Direction.East: EastInvoked[(_chunk.ChunkX, _chunk.ChunkZ)] = true; break;
            }
            
            if (!_chunk.NeighbouringChunks[direction].GetIfLoaded(out var neighbour)) { return; }
            
            var srcText = neighbour.ChunkRenderer._texture;
            var dstText = _texture;
            
            switch (direction)
            {
                case Direction.North: NorthCalled[(_chunk.ChunkX, _chunk.ChunkZ)] = true; break;
                case Direction.East: EastCalled[(_chunk.ChunkX, _chunk.ChunkZ)] = true; break;
            }
            
            if (dstText == null || dstText.IsDestroyed())
            {
                // Come back later when dest texture is loaded
                OnTextureLoaded += () => TryUpdateTexture(direction);
                return;
            }
            
            if (srcText == null || srcText.IsDestroyed())
            {
                // Come back later when src texture is loaded
                neighbour.ChunkRenderer.OnTextureLoaded += () => TryUpdateTexture(direction);
                return;
            }
            
            UpdateTexture(direction, srcText, dstText);
        }

        private void UpdateTexture(Direction direction, Texture src, Texture dst)
        {
            var dstX = (direction == Direction.East ? Chunk.Size+1 : 1) * ChunkTexture.Resolution;
            var dstY = (direction == Direction.North ? Chunk.Size+1 : 1) * ChunkTexture.Resolution;

            var width = (direction == Direction.East ? 1 : Chunk.Size) * ChunkTexture.Resolution;
            var height = (direction == Direction.North ? 1 : Chunk.Size) * ChunkTexture.Resolution;

            const int srcX = 1 * ChunkTexture.Resolution;
            const int srcY = 1 * ChunkTexture.Resolution;

            switch (direction)
            {
                case Direction.North: NorthDirs[(_chunk.ChunkX, _chunk.ChunkZ)] = true; break;
                case Direction.East: EastDirs[(_chunk.ChunkX, _chunk.ChunkZ)] = true; break;
            }

            Graphics.CopyTexture(
                src: src, srcElement: 0, srcMip: 0, srcX: srcX, srcY: srcY, 
                srcWidth: width, srcHeight: height,
                dst: dst, dstElement: 0, dstMip: 0, dstX: dstX, dstY: dstY
            );
        }

        public void UpdateBurntColor()
        {
            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var z = 0; z < Chunk.Size; z++)
                {
                    var cell = _chunk.GetCellAt(x, z);
                    if (cell.Burnt)
                    {
                        Debug.Log("Burnt");
                        SetCellColor(x, z, Color.Lerp(
                            Color.black, 
                            cell.Info.Biome.Color, 0.5f)
                        );
                    }
                }
            }
            _texture.Apply();
        }

        private void SetCellColor(int cellX, int cellZ, Color color)
        {
            var colors = new Color[ChunkTexture.Resolution * ChunkTexture.Resolution];
            Array.Fill(colors, color);
            
            _texture.SetPixels(
                (1 + cellX) * ChunkTexture.Resolution, (1 + cellZ) * ChunkTexture.Resolution, 
                ChunkTexture.Resolution, ChunkTexture.Resolution,
                colors
            );
        }

        private void Start()
        {
            // Set up the texture.
            CalcNewTexture();
            CalcNewMesh();
            _started = true;
        }
        
//======== ====== ==== ==
//      CONTROLS
//======== ====== ==== ==

        private void CalcNewMesh()
        {
            if (_mesh != null && !_mesh.IsDestroyed()) { Destroy(_mesh); }

            // Debug.Log($"#Neighbours = {_chunk.NeighbouringChunks.ToList().Count(_ => _.Loaded)}");

            _mesh = _renderMesh ? GenerateChunkMesh(_chunk) : GeneratePlanarMesh();
            
            _meshFilter.sharedMesh = _mesh;
        }
        /// <summary>
        /// Recomputes the new texture for the mesh given the <see cref="_displayType"/>
        /// </summary>
        private void CalcNewTexture()
        {
            if (_texture != null && !_texture.IsDestroyed()) { Destroy(_texture); }
            
            _texture = From(_chunk, _displayType);
            _rend.material.mainTexture = _texture;

            OnTextureLoaded?.Invoke();
        }

        /// <summary>
        /// Set the materials of the renderer
        /// </summary>
        /// <param name="materials">The materials that the renderer should use</param>
        public void SetMaterials(IEnumerable<Material> materials) => 
            _rend.SetMaterials(new List<Material>(materials));

//======== ====== ==== ==
//      HELPERS
//======== ====== ==== ==
        
        /// <summary>
        /// Generate a planar mesh which size is the same
        /// as the chunk's
        /// </summary>
        /// <returns>The newly created plane mesh</returns>
        private static Mesh GeneratePlanarMesh()
        {
            var mesh = new Mesh();
            var vertices = new [] {
                new Vector3(0.0f, 0.0f, 0.0f),
                new Vector3(Chunk.Size, 0.0f, 0.0f),
                new Vector3(0.0f, 0.0f, Chunk.Size),
                new Vector3(Chunk.Size, 0.0f, Chunk.Size),
            };
            var triangles = new[]
            {
                0, 3, 1,
                0, 2, 3
            };
            var uvs = new[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
            };

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.uv = uvs;

            mesh.Optimize();
            mesh.RecalculateNormals();

            return mesh;
        }

        /// <summary>
        /// Generates a mesh for the chunk using se <see cref="ChunkMesh"/> class
        /// </summary>
        /// <param name="chunk">The chunk for which we generate the mesh</param>
        /// <returns>The newly created chunk mesh</returns>
        private static Mesh GenerateChunkMesh(Chunk chunk) => ChunkMesh.From(chunk);
    }
}