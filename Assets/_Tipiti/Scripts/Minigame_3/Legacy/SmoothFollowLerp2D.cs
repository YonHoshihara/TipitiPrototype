// SmoothFollowLerp2D.cs
using UnityEngine;

public class SmoothFollowLerp2D : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] Camera cam;                 // sua câmera 2D (Orthographic)
    [SerializeField] bool onlyWhenHeld = true;   // true = segue só quando botão/touch pressionado
    [SerializeField] LayerMask pickMask = ~0;    // opcional: só começa se clicar na mandioca

    [Header("Motion")]
    [SerializeField] float lerpSpeed = 12f;      // quanto maior, mais colado
    [SerializeField] float maxStep = 100f;       // clamp opcional por frame (em unidades/seg)

    Vector3 _target; 
    bool _dragging;
    bool _pickedThis; // se exigimos clicar no próprio objeto
    Vector3 _lastPos;
    public  Vector3 Velocity { get; private set; } // útil p/ outros scripts (lavagem etc.)

    void Awake()
    {
        if (!cam) cam = Camera.main;
        _target = transform.position;
        _lastPos = transform.position;
    }

    void Update()
    {
        // 1) alvo do ponteiro em world
        Vector3 sp = (Input.touchCount > 0) ? (Vector3)Input.GetTouch(0).position : Input.mousePosition;
        Vector3 wp = cam.ScreenToWorldPoint(sp); 
        wp.z = transform.position.z;

        bool down     = (Input.touchCount > 0) ? Input.GetTouch(0).phase != TouchPhase.Ended && Input.GetTouch(0).phase != TouchPhase.Canceled
                                               : Input.GetMouseButton(0);
        bool pressed  = Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);
        bool released = Input.GetMouseButtonUp(0)   || (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Ended || Input.GetTouch(0).phase == TouchPhase.Canceled));

        // 2) lógica de “pegar” o objeto (opcional via pickMask)
        if (pressed)
        {
            _pickedThis = true;
            if (pickMask.value != ~0) // se filtrando layer
            {
                var hit = Physics2D.OverlapPoint(wp, pickMask);
                _pickedThis = hit && hit.transform == transform || (hit && hit.transform.IsChildOf(transform));
            }
            _dragging = _pickedThis;
        }
        if (released) _dragging = false;

        // 3) define o alvo
        if (onlyWhenHeld)
        {
            if (_dragging) _target = wp;
        }
        else
        {
            _target = wp; // sempre segue o mouse
        }

        // 4) Lerp suave até o alvo (frame-rate independente)
        float t = 1f - Mathf.Exp(-lerpSpeed * Time.deltaTime); // lerp exponencial
        Vector3 next = Vector3.Lerp(transform.position, _target, t);

        // clamp opcional do passo (evita teleporte se mouse saltar)
        float maxMove = maxStep * Time.deltaTime;
        if ((next - transform.position).sqrMagnitude > maxMove * maxMove)
            next = transform.position + (next - transform.position).normalized * maxMove;

        transform.position = next;

        // 5) velocidade “fake” baseada no delta (útil pra lavagem)
        Velocity = (transform.position - _lastPos) / Mathf.Max(Time.deltaTime, 1e-6f);
        _lastPos = transform.position;
    }
}
