using System;
using UnityEngine;

namespace Code.Scripts.FireSystem
{
    public class FirePlacer : MonoBehaviour
    {        
        [SerializeField] private FireSystem fireSystem;

        private void Update()
        {
            if (Input.GetMouseButtonDown(0)) { StartCoroutine(nameof(ShootRay)); }
        }

        private void ShootRay()
        {

            Debug.Log("Shooting ray");
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out var hit, float.PositiveInfinity)) return;
            
            var pos = hit.point;
            var roundedPos = new Vector3(
                Mathf.RoundToInt(pos.x),
                Mathf.RoundToInt(pos.y),
                Mathf.RoundToInt(pos.z)
            );

            Debug.Log("Hit");
            fireSystem.SetCellOnFire(roundedPos);
        }
    }
}
