// PointerDragger2D.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
[RequireComponent(typeof(TargetJoint2D))]
public class PointerDragger2D : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] Camera cam;                 // orthographic camera
    [SerializeField] LayerMask draggableMask;    // layer da mandioca (opcional)

    [Header("Joint Tuning")]
    [SerializeField] float maxForce = 2000f;     // how hard it tries to reach the target
    [SerializeField] float dampingRatio = 0.8f;  // 0..1
    [SerializeField] float frequency = 5f;       // spring freq (Hz)
    [SerializeField] float breakForce = Mathf.Infinity; // if you want limits

    Rigidbody2D rb2d;
    TargetJoint2D joint;
    bool dragging;

    void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        joint = GetComponent<TargetJoint2D>();
        joint.enabled = false;
        joint.maxForce = maxForce;
        joint.dampingRatio = dampingRatio;
        joint.frequency = frequency;
        joint.breakForce = breakForce;

        if (!cam) cam = Camera.main;
    }

    void Update()
    {
        // Pointer position to world
        Vector3 sp = Input.touchCount > 0 ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
        Vector3 wp = cam.ScreenToWorldPoint(sp);
        wp.z = 0f;

        bool down = Input.touchCount > 0 ? Input.GetTouch(0).phase != TouchPhase.Ended && Input.GetTouch(0).phase != TouchPhase.Canceled
                                         : Input.GetMouseButton(0);
        bool pressed = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        bool released = Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled));

        if (pressed)
        {
            // Optional: only start dragging if we actually clicked this object
            var hit = Physics2D.OverlapPoint(wp, draggableMask.value == 0 ? Physics2D.AllLayers : draggableMask);
            if (hit && hit.attachedRigidbody == rb2d)
            {
                dragging = true;
                joint.anchor = rb2d.transform.InverseTransformPoint(wp); // start at local hit
                joint.target = wp;
                joint.enabled = true;
            }
        }

        if (dragging && down)
        {
            joint.target = wp;
        }

        if (released)
        {
            dragging = false;
            joint.enabled = false;
        }
    }
}
