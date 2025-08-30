using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterArea : MonoBehaviour
{
    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"Debug water: {other.name}");
        var washer = other.GetComponentInParent<CassavaWasher2D>();
        // if (washer) washer.SetInWater(true);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"Exited water: {other.name}");
        var washer = other.GetComponentInParent<CassavaWasher2D>();
        // if (washer) washer.SetInWater(false);
    }
}
