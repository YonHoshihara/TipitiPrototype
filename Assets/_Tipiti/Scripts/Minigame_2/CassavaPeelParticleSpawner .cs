using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class CassavaPeelParticleSpawner : MonoBehaviour
{
    public PeelPainter painter;          // arraste no Inspector
    public Transform referenceUp;        // opcional: câmera ou world up para orientar “lascas”
    public float baseRate = 6f;          // how many particles per stamp (scaled by strength)
    public float speedNormal = 1.8f;     // impulse along surface normal
    public float speedTangent = 1.2f;    // impulse along brush tangent (drag direction)
    public float sizeMin = 0.01f;
    public float sizeMax = 0.03f;
    public float lifeMin = 0.6f;
    public float lifeMax = 1.2f;
    public float gravity = 0.4f;

    private ParticleSystem ps;
    private Vector3 lastPos;
    private bool hasLast;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        var main = ps.main;
        main.playOnAwake = false;
        main.loop = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = gravity;
    }

    private void OnEnable()
    {
        if (painter != null) painter.OnStampWorld.AddListener(HandleStamp);
    }

    private void OnDisable()
    {
        if (painter != null) painter.OnStampWorld.RemoveListener(HandleStamp);
    }

    private void HandleStamp(Vector3 pos, Vector3 normal, float strength)
    {
        // Estimate brush tangent from last world pos (simple heuristic)
        Vector3 tangent = Vector3.zero;
        if (hasLast)
        {
            tangent = (pos - lastPos);
            tangent = Vector3.ProjectOnPlane(tangent, normal);
        }
        lastPos = pos;
        hasLast = true;

        int count = Mathf.CeilToInt(baseRate * Mathf.Clamp01(strength));

        // Emit small flakes
        var emit = new ParticleSystem.EmitParams();
        for (int i = 0; i < count; i++)
        {
            // Randomize orientation using a stable up
            Vector3 up = referenceUp ? referenceUp.up : Vector3.up;
            Vector3 side = Vector3.Cross(up, normal).normalized;
            Vector3 dirTangent = tangent.sqrMagnitude > 1e-6f ? tangent.normalized : side;

            // Velocity: a bit of normal + a bit of tangent
            Vector3 vel =
                normal * (speedNormal * Random.Range(0.7f, 1.3f)) +
                dirTangent * (speedTangent * Random.Range(0.5f, 1.2f));

            emit.position = pos + normal * Random.Range(0.001f, 0.005f);
            emit.velocity = vel;
            emit.startLifetime = Random.Range(lifeMin, lifeMax);
            emit.startSize = Random.Range(sizeMin, sizeMax);
            emit.rotation3D = Random.insideUnitSphere * 360f;
            emit.startColor = Color.white;

            ps.Emit(emit, 1);
        }
    }
}
