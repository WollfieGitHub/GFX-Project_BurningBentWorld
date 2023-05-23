using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration.Components;
using UnityEngine;
using UnityEngine.VFX;

namespace FireSystem
{
    /// <summary>
    /// Stores all information needed to update VFX linked to each chunk.
    /// </summary>
    public class FireChunk
    {
        public int x;
        public int z;
        //Unfortunately, this is yet another copy, but I didn't find a better way of implementing this.
        public Dictionary<int, FireCell> activeCells = new Dictionary<int, FireCell>();
        public GameObject linkedVFXGameObject { get; private set; }

        public FireChunk(int x, int z, GameObject linkedVFXGameObject, TerrainGeneration.Components.Terrain terrain)
        {
            this.x = x;
            this.z = z;
            this.linkedVFXGameObject = linkedVFXGameObject;
            linkedVFXGameObject.transform.position = new Vector3(x * Chunk.Size, 0f, z * Chunk.Size);
            linkedVFXGameObject.SetActive(true);
        }

        /// <summary>
        /// Updates the texture of the VFX associated with this FireChunk if there is one. Otherwise throws an error.
        /// </summary>
        /// <param name="activeCells"></param>
        /// <param name="terrain"></param>
        public void UpdateVFXTexture(string ActiveCellsTextureVFX)
        {
            if (linkedVFXGameObject == null)
            {
                throw new System.Exception("Trying to update a VFX texture from a chunk without an associated VFX object.");
            }

            Texture t = CalculateChunkVFXTexture();

            VisualEffect ve = linkedVFXGameObject.GetComponentInChildren<VisualEffect>();
            if (!ve.HasTexture(ActiveCellsTextureVFX)) { throw new System.Exception("Wrong name!"); }

            ve.SetTexture(ActiveCellsTextureVFX, t);
        }

        /// <summary>
        /// Calculate the texture meant to be used in the VFX for fire inside a chunk.
        /// The texture has one pixel per cell where the RGB encode (in that order) the cell x, z and y position in the chunk's space.
        /// It ensures that only cells being on fire will be encoded in the texture, but that all pixels in the texture are used 
        /// anyway. Therefore if only two cells are on fire in the grid, all pixels in the texture will consist of these two fires.
        /// </summary>
        /// <param name="activeCells"></param>
        /// <param name="terrain"></param>
        /// <returns></returns>
        private Texture2D CalculateChunkVFXTexture()
        {
            Texture2D texture = new Texture2D(activeCells.Count, 1);

            int i = 0;
            foreach (FireCell cell in activeCells.Values)
            {
                var r = LinearToGamma(cell.x % Chunk.Size / (float)(Chunk.Size - 1));
                var g = LinearToGamma(cell.height / 16f);
                var b = LinearToGamma(cell.z % Chunk.Size / (float)(Chunk.Size - 1));
                texture.SetPixel(i, 0, new Color(r, g, b));
                i++;                
            }

            texture.Apply();

            return texture;
        }

        private static float LinearToGamma(float value)
        {
            return Mathf.Pow(value, 1 / 2.2f);
        }
    }
}
