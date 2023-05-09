using System;
using UnityEngine;

public class FirePlacer : MonoBehaviour
{
    /*[SerializeField]
    private GameObject*/
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            ShootRay();
    }

    private void ShootRay()
    {
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, Single.PositiveInfinity))
        {
            
            
        }
    }
}
