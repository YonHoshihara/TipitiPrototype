using UnityEngine;

public class KnifeCursor : MonoBehaviour
{
    public Camera cam;
    public float followSpeed = 20f;
    public float tiltAmount = 15f;

    private Vector3 _targetPos;
    private Vector3 _lastPos;

    void Update()
    {
        if (cam == null) cam = Camera.main;
        Vector3 screen = (Input.touchCount > 0)
            ? (Vector3)Input.GetTouch(0).position
            : Input.mousePosition;
        screen.z = Mathf.Abs(cam.transform.position.z);
        _targetPos = cam.ScreenToWorldPoint(screen);

        // Smooth follow
        transform.position = Vector3.Lerp(transform.position, _targetPos, followSpeed * Time.deltaTime);

        // Tilt by velocity
        Vector3 vel = (_targetPos - _lastPos) / Time.deltaTime;
        float tilt = Mathf.Clamp(vel.x * 0.1f, -tiltAmount, tiltAmount);
        transform.rotation = Quaternion.Euler(0, 0, -tilt);

        _lastPos = _targetPos;
    }
}
