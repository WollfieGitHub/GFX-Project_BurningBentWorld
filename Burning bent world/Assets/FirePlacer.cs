using System;
using UnityEngine;

public class FirePlacer : MonoBehaviour
{
    [SerializeField] private GameObject firePrefab;

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
            var pos = hit.point;
            var roundedPos = new Vector3(
                Mathf.RoundToInt(pos.x),
                Mathf.RoundToInt(pos.y),
                Mathf.RoundToInt(pos.z)
            );
            Instantiate(firePrefab, roundedPos, Quaternion.identity);
        }
    }
}
