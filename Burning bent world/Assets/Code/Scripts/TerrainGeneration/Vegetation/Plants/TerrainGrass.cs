using System.Runtime.CompilerServices;
using UnityEngine;

namespace Code.Scripts.TerrainGeneration.Vegetation.Plants
{
    public class TerrainGrass : MonoBehaviour
    {
        [Header("Rendering Properties")]
        [Tooltip("Compute shader for generating transformation matrices.")] public ComputeShader ComputeShader;
        [Tooltip("Material for rendering each grass blade.")] public Material Material;
        
        
        
    }
}