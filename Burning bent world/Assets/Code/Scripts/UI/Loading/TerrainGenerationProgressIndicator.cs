using System;
using System.Collections;
using System.Collections.Generic;
using TerrainGeneration.Generators;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class TerrainGenerationProgressIndicator : MonoBehaviour
{
    [SerializeField] private TerrainGenerator terrainGenerator;
    
    private void OnEnable() => terrainGenerator.OnProgressReported += OnProgress;
    private void OnDisable() => terrainGenerator.OnProgressReported -= OnProgress;

    private void OnProgress(TerrainGenerator.ProgressStatus status)
    {
        Debug.Log(status.Progress);
        _slider.value = status.Progress;
        _text.text = "Loading...\n" +
                     $"[{status.StackName}] : {status.Progress * 100:00}%";
    }

    private TextMeshProUGUI _text;
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponentInChildren<Slider>();
        _text = GetComponentInChildren<TextMeshProUGUI>();
    }
}
