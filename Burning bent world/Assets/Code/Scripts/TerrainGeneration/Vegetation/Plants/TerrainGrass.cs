using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Terrain = TerrainGeneration.Components.Terrain;
using static System.Runtime.InteropServices.Marshal;

namespace Code.Scripts.TerrainGeneration.Vegetation.Plants
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class TerrainGrass : MonoBehaviour
    {
        [Header("Rendering Properties")] [SerializeField]
        private Material grassMaterial;

        [SerializeField] private Mesh grassMesh;
        [SerializeField] private Mesh grassLODMesh;

        [SerializeField] private int fieldSize = 100;
        [SerializeField] private int chunkDensity = 1;
        [SerializeField] private int numChunks = 1;

        [Range(0, 1000.0f)] public float lodCutoff = 1000.0f;
        [Range(0, 1000.0f)] public float distanceCutoff = 1000.0f;

        [Header("Wind")] [SerializeField] private float windSpeed = 1.0f;
        [SerializeField] private float frequency = 1.0f;
        [SerializeField] private float windStrength = 1.0f;

        [Header("Compute Shaders")] [SerializeField]
        private ComputeShader initializeGrassShader;

        [SerializeField] private ComputeShader generateWindShader;
        [SerializeField] private ComputeShader cullGrassShader;

        private ComputeBuffer _voteBuffer;
        private ComputeBuffer _scanBuffer;
        private ComputeBuffer _groupSumArrayBuffer;
        private ComputeBuffer _scannedGroupSumBuffer;

        private RenderTexture _wind;

        private const float DisplacementStrength = Terrain.MaxHeight - Terrain.SeaLevel;

        private int _numInstancesPerChunk,
            _chunkDimension,
            _numThreadGroups,
            _numVoteThreadGroups,
            _numGroupScanThreadGroups,
            _numWindThreadGroups,
            _numGrassInitThreadGroups;

        private struct GrassData
        {
            public Vector4 Position;
            public Vector2 Uv;
            public float Displacement;
        }

        private struct GrassChunk
        {
            public ComputeBuffer ArgsBuffer;
            public ComputeBuffer ArgsBufferLOD;
            public ComputeBuffer PositionsBuffer;
            public ComputeBuffer CulledPositionsBuffer;
            public Bounds Bounds;
            public Material Material;
        }

        private GrassChunk[] _chunks;
        private uint[] _args;
        private uint[] _argsLOD;
        private Bounds _fieldBounds;
        private Camera _camera;

        private void OnEnable()
        {
            _camera = Camera.main;
            _numInstancesPerChunk = (int)(Mathf.CeilToInt(fieldSize / (float)numChunks) * chunkDensity);
            _chunkDimension = _numInstancesPerChunk;
            _numInstancesPerChunk *= _numInstancesPerChunk;

            _numThreadGroups = Mathf.CeilToInt(_numInstancesPerChunk / 128.0f);
            if (_numThreadGroups > 128)
            {
                var powerOfTwo = 128;
                while (powerOfTwo < _numThreadGroups)
                {
                    powerOfTwo *= 2;
                }

                _numThreadGroups = powerOfTwo;
            }
            else
            {
                while (128 % _numThreadGroups != 0)
                {
                    _numThreadGroups++;
                }
            }

            _numVoteThreadGroups = Mathf.CeilToInt(_numInstancesPerChunk / 128.0f);
            _numGroupScanThreadGroups = Mathf.CeilToInt(_numInstancesPerChunk / 1024.0f);

            _voteBuffer = new ComputeBuffer(_numInstancesPerChunk, 4);
            _scanBuffer = new ComputeBuffer(_numInstancesPerChunk, 4);
            _groupSumArrayBuffer = new ComputeBuffer(_numThreadGroups, 4);
            _scannedGroupSumBuffer = new ComputeBuffer(_numThreadGroups, 4);

            _wind = new RenderTexture(1024, 1024, 0, RenderTextureFormat.ARGBFloat, RenderTextureReadWrite.Linear);
            _wind.enableRandomWrite = true;
            _wind.Create();

            _numWindThreadGroups = Mathf.CeilToInt(_wind.height / 8.0f);

            _args = new uint[]
            {
                grassMesh.GetIndexCount(0),
                0,
                grassMesh.GetIndexStart(0),
                grassMesh.GetBaseVertex(0),
                0
            };

            _argsLOD = new uint[]
            {
                grassLODMesh.GetIndexCount(0),
                0,
                grassLODMesh.GetIndexStart(0),
                grassLODMesh.GetBaseVertex(0),
            };

            InitializeChunks();

            _fieldBounds = new Bounds(Vector3.zero, new Vector3(-fieldSize, DisplacementStrength * 2, fieldSize));
        }

        void InitializeChunks()
        {
            _chunks = new GrassChunk[numChunks * numChunks];

            for (var x = 0; x < numChunks; x++)
            {
                for (var z = 0; z < numChunks; z++)
                {
                    _chunks[x + z * numChunks] = InitializeGrassChunk(x, z);
                }
            }
        }

        GrassChunk InitializeGrassChunk(int xOffset, int zOffset)
        {
            var chunk = new GrassChunk();

            chunk.ArgsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
            chunk.ArgsBufferLOD = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);

            chunk.ArgsBuffer.SetData(_args);
            chunk.ArgsBufferLOD.SetData(_argsLOD);

            chunk.PositionsBuffer = new ComputeBuffer(_numInstancesPerChunk, SizeOf(typeof(GrassData)));
            chunk.CulledPositionsBuffer = new ComputeBuffer(_numInstancesPerChunk, SizeOf(typeof(GrassData)));

            var chunkDim = Mathf.CeilToInt(fieldSize / (float)numChunks);

            var c = new Vector3(0f, 0f, 0f);

            c.y = 0f;
            c.x = -(chunkDim * 0.5f * numChunks) + chunkDim * xOffset;
            c.x = -(chunkDim * 0.5f * numChunks) + chunkDim * zOffset;
            c.x += chunkDim * 0.5f;
            c.z += chunkDim * 0.5f;

            chunk.Bounds = new Bounds(c, new Vector3(-chunkDim, 10.0f, chunkDim));

            initializeGrassShader.SetInt("_XOffset", xOffset);
            initializeGrassShader.SetInt("_ZOffset", zOffset);
            initializeGrassShader.SetBuffer(0, "_GrassDataBuffer", chunk.PositionsBuffer);
            initializeGrassShader.Dispatch(
                0,
                Mathf.CeilToInt(fieldSize / (float)numChunks) * chunkDensity,
                Mathf.CeilToInt(fieldSize / (float)numChunks) * chunkDensity,
                1
            );

            chunk.Material = new Material(grassMaterial);
            chunk.Material.SetBuffer("positionBuffer", chunk.CulledPositionsBuffer);
            chunk.Material.SetFloat("_DisplacementStrength", DisplacementStrength);
            chunk.Material.SetTexture("_WindTex", _wind);
            chunk.Material.SetInt("_ChunkNum", xOffset + zOffset * numChunks);

            return chunk;
        }


        void CullGrass(GrassChunk chunk, Matrix4x4 VP, bool noLOD)
        {
            //Reset Args
            if (noLOD)
                chunk.ArgsBuffer.SetData(_args);
            else
                chunk.ArgsBufferLOD.SetData(_argsLOD);

            // Vote
            cullGrassShader.SetMatrix("MATRIX_VP", VP);
            cullGrassShader.SetBuffer(0, "_GrassDataBuffer", chunk.PositionsBuffer);
            cullGrassShader.SetBuffer(0, "_VoteBuffer", _voteBuffer);
            cullGrassShader.SetVector("_CameraPosition", _camera.transform.position);
            cullGrassShader.SetFloat("_Distance", distanceCutoff);
            cullGrassShader.Dispatch(0, _numThreadGroups, 1, 1);

            // Scan Instances
            cullGrassShader.SetBuffer(1, "_VoteBuffer", _voteBuffer);
            cullGrassShader.SetBuffer(1, "_ScanBuffer", _scanBuffer);
            cullGrassShader.SetBuffer(1, "_GroupSumArray", _groupSumArrayBuffer);
            cullGrassShader.Dispatch(1, _numThreadGroups, 1, 1);

            // Scan Groups
            cullGrassShader.SetInt("_NumOfGroups", _numThreadGroups);
            cullGrassShader.SetBuffer(2, "_GroupSumArrayIn", _groupSumArrayBuffer);
            cullGrassShader.SetBuffer(2, "_GroupSumArrayOut", _scannedGroupSumBuffer);
            cullGrassShader.Dispatch(2, _numGroupScanThreadGroups, 1, 1);

            // Compact
            cullGrassShader.SetBuffer(3, "_GrassDataBuffer", chunk.PositionsBuffer);
            cullGrassShader.SetBuffer(3, "_VoteBuffer", _voteBuffer);
            cullGrassShader.SetBuffer(3, "_ScanBuffer", _scanBuffer);
            cullGrassShader.SetBuffer(3, "_ArgsBuffer", noLOD ? chunk.ArgsBuffer : chunk.ArgsBufferLOD);
            cullGrassShader.SetBuffer(3, "_CulledGrassOutputBuffer", chunk.CulledPositionsBuffer);
            cullGrassShader.SetBuffer(3, "_GroupSumArray", _scannedGroupSumBuffer);
            cullGrassShader.Dispatch(3, _numThreadGroups, 1, 1);
        }

        void GenerateWind()
        {
            generateWindShader.SetTexture(0, "_WindMap", _wind);
            generateWindShader.SetFloat("_Time", Time.time * windSpeed);
            generateWindShader.SetFloat("_Frequency", frequency);
            generateWindShader.SetFloat("_Amplitude", windStrength);
            generateWindShader.Dispatch(0, _numWindThreadGroups, _numWindThreadGroups, 1);
        }

        void Update()
        {
            var P = _camera.projectionMatrix;
            var V = _camera.transform.worldToLocalMatrix;
            var VP = P * V;

            GenerateWind();

            for (int i = 0; i < numChunks * numChunks; ++i)
            {
                float dist = Vector3.Distance(_camera.transform.position, _chunks[i].Bounds.center);

                bool noLOD = dist < lodCutoff;

                CullGrass(_chunks[i], VP, noLOD);
                if (noLOD)
                    Graphics.DrawMeshInstancedIndirect(grassMesh, 0, _chunks[i].Material, _fieldBounds,
                        _chunks[i].ArgsBuffer);
                else
                    Graphics.DrawMeshInstancedIndirect(grassLODMesh, 0, _chunks[i].Material, _fieldBounds,
                        _chunks[i].ArgsBufferLOD);
            }
        }

        void OnDisable()
        {
            _voteBuffer.Release();
            _scanBuffer.Release();
            _groupSumArrayBuffer.Release();
            _scannedGroupSumBuffer.Release();
            _wind.Release();
            _wind = null;
            _scannedGroupSumBuffer = null;
            _voteBuffer = null;
            _scanBuffer = null;
            _groupSumArrayBuffer = null;


            for (int i = 0; i < numChunks * numChunks; ++i)
            {
                FreeChunk(_chunks[i]);
            }

            _chunks = null;
        }

        void FreeChunk(GrassChunk chunk)
        {
            chunk.PositionsBuffer.Release();
            chunk.PositionsBuffer = null;
            chunk.CulledPositionsBuffer.Release();
            chunk.CulledPositionsBuffer = null;
            chunk.ArgsBuffer.Release();
            chunk.ArgsBuffer = null;
            chunk.ArgsBufferLOD.Release();
            chunk.ArgsBufferLOD = null;
        }

        void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            if (_chunks != null)
            {
                for (int i = 0; i < numChunks * numChunks; ++i)
                {
                    Gizmos.DrawWireCube(_chunks[i].Bounds.center, _chunks[i].Bounds.size);
                }
            }
        }
    }
}