using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float speed = 5f; // Speed of movement

    private void Update()
    {
        var hInput = Input.GetAxis("Horizontal");
        var vInput = Input.GetAxis("Vertical");
        transform.position += new Vector3(hInput, vInput) * speed * Time.deltaTime;
    }
}
