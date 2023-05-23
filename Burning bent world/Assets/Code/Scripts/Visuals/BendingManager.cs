using System;
using UnityEngine;
using UnityEngine.Rendering;

//[ExecuteAlways]
public class BendingManager : MonoBehaviour
{
  private const string BENDING_FEATURE = "ENABLE_BENDING";

  private const string PLANET_FEATURE = "ENABLE_BENDING_PLANET";

  private static readonly int BENDING_AMOUNT =
    Shader.PropertyToID("_BendingAmount");

  [SerializeField] private bool enablePlanet = default;

  [SerializeField] [Range(0, 100f)] private float bendingAmount = 5;

  private float _prevAmount;

  private void Awake()
  {
    if (Application.isPlaying)
      Shader.EnableKeyword(BENDING_FEATURE);
    else
      Shader.DisableKeyword(BENDING_FEATURE);

    if (enablePlanet)
      Shader.EnableKeyword(PLANET_FEATURE);
    else
      Shader.DisableKeyword(PLANET_FEATURE);

    Debug.Log("AWAKE");
    UpdateBendingAmount();
  }

  private void OnEnable()
  {
    if (!Application.isPlaying)
      return;

    RenderPipelineManager.beginCameraRendering += OnBeginCameraRendering;
    RenderPipelineManager.endCameraRendering += OnEndCameraRendering;
  }

  private void Update()
  {
    if (Math.Abs(_prevAmount - bendingAmount) > Mathf.Epsilon)
      UpdateBendingAmount();
  }

  private void OnDisable()
  {
    RenderPipelineManager.beginCameraRendering -= OnBeginCameraRendering;
    RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
  }

  private void UpdateBendingAmount()
  {
    _prevAmount = bendingAmount / 1000f;
    Shader.SetGlobalFloat(BENDING_AMOUNT, bendingAmount / 1000f);
  }

  private static void OnBeginCameraRendering(ScriptableRenderContext ctx,
    Camera cam)
  {
    cam.cullingMatrix = Matrix4x4.Ortho(-400, 400, -99, 99, 0.001f, 1000) *
                        cam.worldToCameraMatrix;
  }

  private static void OnEndCameraRendering(ScriptableRenderContext ctx,
    Camera cam)
  {
    cam.ResetCullingMatrix();
  }
}