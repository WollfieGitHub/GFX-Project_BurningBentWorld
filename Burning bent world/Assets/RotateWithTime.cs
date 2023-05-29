using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateWithTime : MonoBehaviour
{
    [SerializeField] private float speed;
    
    private void Update()
    {
        transform.Rotate(Vector3.up, speed * Time.deltaTime);
    }
}
