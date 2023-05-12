using UnityEngine;

public class SimpleCameraController : MonoBehaviour
{
    [SerializeField] private float speed = 10f;

    private Vector2 input;
    private void Update()
    {
        input = new Vector2(
            Input.GetAxis("Horizontal"),
            Input.GetAxis("Vertical")
        );
        if (input.x != 0)
        {
            transform.position += Vector3.right * (input.x * speed * Time.deltaTime);
        }

        if (input.y != 0)
        {
            transform.position += Vector3.forward * (input.y * speed * Time.deltaTime);
        }
    }
}
