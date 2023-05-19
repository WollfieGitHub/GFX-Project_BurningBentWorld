using System.Collections.Generic;
using TerrainGeneration.Rendering;
using TerrainGeneration.Components;
using UnityEngine;
using TerrainGeneration.Generators;
using UnityEngine.VFX;

public class testErase: MonoBehaviour {


    [SerializeField] private float red = 0;
    [SerializeField] private float green = 0;
    [SerializeField] private float blue = 0;

    //private void Start()
    //{
    //    Texture2D texture = new Texture2D(32, 32);
    //    for (int i = 0; i < 32; i++)
    //    {
    //        for (int j = 0; j < 16; j++)
    //        {
    //            texture.SetPixel(i, j, new Color(255, 255, 255));
    //        }
    //        for (int h = 17; h < 32; h++)
    //        {
    //            texture.SetPixel(i, h, new Color(20, 20, 20));
    //        }
    //    }

    //    texture.Apply();

    //    Debug.Log("I am here");
    //    GetComponentInChildren<VisualEffect>().SetTexture("ActiveCells", texture);
    //    GetComponentInChildren<VisualEffect>().SetFloat("FlamesRate", 118f);
    //    Debug.Log(((VisualEffect)GetComponentInChildren<VisualEffect>()).HasTexture("ActiveCells"));
    //    Debug.Log(GetComponentInChildren<VisualEffect>().GetTexture("ActiveCells"));
    //}

    private void Update()
    {
        Texture2D texture = new Texture2D(32, 1);
        for (int i = 0; i < 32; i++)
        {
            texture.SetPixel(i, 0, new Color(red * i, green, blue));
            //for (int j = 0; j < 16; j++)
            //{
            //    texture.SetPixel(i, j, new Color(red, green, blue));
            //}
            //for (int h = 17; h < 32; h++)
            //{
            //    texture.SetPixel(i, h, new Color(255, 255, 255));
            //}
        }

        texture.Apply();

        Debug.Log("I am here");
        GetComponentInChildren<VisualEffect>().SetTexture("ActiveCells", texture);
    }
}