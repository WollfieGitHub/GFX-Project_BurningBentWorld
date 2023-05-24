using UnityEngine;

namespace Code.Scripts.Gameplay
{
    public class SimpleCameraController : MonoBehaviour
    {
        [SerializeField] private float speed = 10f;

        private Vector2 _input;
        private void Update()
        {
            _input = new Vector2(
                Input.GetAxis("Horizontal"),
                Input.GetAxis("Vertical")
            );
            if (_input.x != 0)
            {
                transform.position += Vector3.right * (_input.x * speed * Time.deltaTime);
            }

            if (_input.y != 0)
            {
                transform.position += Vector3.forward * (_input.y * speed * Time.deltaTime);
            }
        }
    }
}
