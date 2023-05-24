using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using Code.Scripts.TerrainGeneration.Generators;
using Code.Scripts.TerrainGeneration.Loaders.Serialization;
using Code.Scripts.Utils;
using Unity.VisualScripting;
using UnityEngine;
using static Utils.Utils;

namespace Code.Scripts.TerrainGeneration.Components
{
    /// <summary>
    /// Contains the loaded map of cells on a
    /// <see cref="Size"/> x <see cref="Size"/> grid
    /// </summary>
    public class SuperChunk
    {
        private static readonly BinaryFormatter Formatter;

        private const string SerializationFolderName = "/SuperChunks";

        private static string GetSerializedName(int xOffset, int zOffset) =>
            $"/SuperChunk#{xOffset}#{zOffset}.schunk";

        static SuperChunk()
        {
            var selector = new SurrogateSelector();
            selector.AddSurrogate(typeof(SuperChunk), new StreamingContext(StreamingContextStates.All), new SuperChunkSerializer());
            selector.AddSurrogate(typeof(Cell), new StreamingContext(StreamingContextStates.All), new CellSerializer());
            selector.AddSurrogate(typeof(CellInfo), new StreamingContext(StreamingContextStates.All), new CellInfoSerializer());
            selector.AddSurrogate(typeof(BiomeAttribute), new StreamingContext(StreamingContextStates.All), new BiomeAttributeSerializer());
            selector.AddSurrogate(typeof(Biome), new StreamingContext(StreamingContextStates.All), new BiomeSerializer());
            
            Formatter = new BinaryFormatter(
                selector, new StreamingContext(StreamingContextStates.All)
            );
        }

        private bool _loaded;
        private Task _cellLoader;

        public int XOffset;
        public int ZOffset;

        public Cell[,] Cells => _cells;

        /// <summary>
        /// Computes the coordinates of the <see cref="SuperChunk"/> containing
        /// the chunk with the specified coordinates
        /// </summary>
        /// <param name="xChunk">The X coordinate of the chunk</param>
        /// <param name="zChunk">The Z coordinate of the chunk</param>
        /// <returns>The coordinate of the SuperChunk</returns>
        public static (int, int) GetCoordinatesFrom(int xChunk, int zChunk)
        {
            var ratio = Size / Chunk.Size;
            
            return (
                xChunk / ratio, zChunk / ratio    
            );
        }

        /// <summary>
        /// The dimension of the width and height
        /// of the <see cref="SuperChunk"/>
        /// </summary>
        private const int Size = 512;
        
        /// <summary>
        /// The cells of the <see cref="SuperChunk"/>
        /// </summary>
        private Cell[,] _cells;
        
        /// <summary>
        /// Loads the map in a <see cref="Size"/> x <see cref="Size"/> grid
        /// from the given generator
        /// </summary>
        /// <param name="cells">The cells of the <see cref="SuperChunk"/></param>
        /// <param name="xOffset">The X offset for the terrain</param>
        /// <param name="zOffset">The Z offset for the terrain</param>
        public void Load(Cell[,] cells, int xOffset, int zOffset)
        {
            // Save coordinates
            XOffset = xOffset;
            ZOffset = zOffset;
            
            _cells = cells;
            _loaded = true;
        }

        public SuperChunk() { }

        public SuperChunk(int xOffset, int zOffset, TerrainGenerator generator)
        {
            _loaded = false;
            _cellLoader = LoadAt(xOffset, zOffset, generator);
        }

        /// <summary>
        /// Finds the group of cells of the desired chunk
        /// </summary>
        /// <param name="xChunk">
        /// The X offset of the chunk in this <see cref="SuperChunk"/>'s coordinate system
        /// </param>
        /// <param name="zChunk">
        /// The Z offset of the chunk in this <see cref="SuperChunk"/>'s coordinate system
        /// </param>
        /// <returns>
        /// An array of <see cref="CellInfo"/> of dimensions <see cref="Chunk.Size"/>
        /// by <see cref="Chunk.Size"/>
        /// </returns>
        public async Task<Efficient2DArray<Cell>> GetChunkCells(int xChunk, int zChunk)
        {
            if (!_loaded) { await _cellLoader; }
            
            var result = new Efficient2DArray<Cell>(Chunk.Size, Chunk.Size);

            xChunk = Mathf.RoundToInt(MathModulus(xChunk, Size / Chunk.Size));
            zChunk = Mathf.RoundToInt(MathModulus(zChunk, Size / Chunk.Size));
            
            for (var x = 0; x < Chunk.Size; x++)
            {
                for (var z = 0; z < Chunk.Size; z++)
                {
                    result[x, z] = _cells[
                        xChunk * Chunk.Size + x,
                        zChunk * Chunk.Size + z
                    ];
                }
            }

            return result;
        }
        
// //======================================================================================\\
// ||                                                                                      ||
// ||                                         LOADING                                      ||
// ||                                                                                      ||
// \\======================================================================================//

        private static string GetLoadedDirectoryPath()
        {
            var basePath = Application.dataPath + SerializationFolderName;

            Directory.CreateDirectory(basePath);

            return basePath;
        }
        
        
        /// <summary>
        /// Loads the super chunk of the specified coordinates from disk
        /// </summary>
        /// <param name="xOffset">The X coordinate of the super chunk</param>
        /// <param name="zOffset">The Z coordinate of the super chunk</param>
        /// <param name="generator">The <see cref="TerrainGenerator"/> to use to
        ///  generate the <see cref="SuperChunk"/>'s cells in case no <see cref="SuperChunk"/>
        /// already exists with the specified coordinates</param>
        /// <returns>The loaded super chunk at the specified coordinates</returns>
        private async Task LoadAt(int xOffset, int zOffset, TerrainGenerator generator)
        {
            try 
            {
                var basePath = GetLoadedDirectoryPath();

                await using var stream = new FileStream(
                    basePath + GetSerializedName(xOffset, zOffset), FileMode.Open
                );
                var other = (SuperChunk)Formatter.Deserialize(stream);
                Load(other.Cells, other.XOffset, other.ZOffset);
            }
            catch (Exception)
            {
                // The super chunk does not exist, create a new super chunk
                await CreateNewAt(xOffset, zOffset, generator);
            }
        }

        /// <summary>
        /// Purge all super chunks from the directory saved until now
        /// </summary>
        public static void PurgeSavedSuperChunks()
        {
            var basePath = GetLoadedDirectoryPath();
            Directory.Delete(basePath, true);
        }

        /// <summary>
        /// Creates a new <see cref="SuperChunk"/> with the specified offsets position
        /// and generator
        /// </summary>
        /// <param name="xOffset">The X Offset of the <see cref="SuperChunk"/></param>
        /// <param name="zOffset">The Z Offset of the <see cref="SuperChunk"/></param>
        /// <param name="generator">The <see cref="TerrainGenerator"/> to use
        /// to generate the <see cref="SuperChunk"/>'s cells</param>
        /// <returns>The newly created <see cref="SuperChunk"/></returns>
        private async Task CreateNewAt(
            int xOffset, int zOffset, TerrainGenerator generator
        ) {
            var cells = await Task.Run(
                () =>
                {
                    var cells = new Cell[Size,Size];
                    var info = generator.GenerateNew(
                        xOffset * Size, zOffset * Size, Size, Size
                    );
                    for (var x = 0; x < Size; x++)
                    {
                        for (var z = 0; z < Size; z++)
                        {
                            cells[x, z] = new Cell(info[x, z]);
                        }
                    }
                    return cells;
                }
            // Return result on thread pool and not on player thread
            ).ConfigureAwait(false);
            Load(cells, xOffset, zOffset);
        }

        /// <summary>
        /// Unloads the super chunk, saving it to disk
        /// </summary>
        public async Task Unload()
        {
            var basePath = GetLoadedDirectoryPath();

            await using var stream = new FileStream(
                basePath + GetSerializedName(XOffset, ZOffset), FileMode.OpenOrCreate
            );
            Formatter.Serialize(stream, this);
        }

    }
}