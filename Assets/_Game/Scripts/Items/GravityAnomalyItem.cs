using UnityEngine;

public class GravityAnomalyItem : MonoBehaviour, ISeededItem
{
    [SerializeField] private float forceMin = -2f;
    [SerializeField] private float forceMax = -0.5f;
    [SerializeField] private float radius = 3f;
    [SerializeField] private Color gizmoColor = Color.cyan;
    [SerializeField] private LayerMask affectedLayers = ~0;

    private float _force;
    private Vector3 _direction;

    public void Configure(float seed)
    {
        var rng = new System.Random(Mathf.FloorToInt(seed * 100000f));
        _force = Mathf.Lerp(forceMin, forceMax, (float)rng.NextDouble());
        // Random unit direction; allow 3D pull/push
        _direction = Random.onUnitSphere;
    }

    private void FixedUpdate()
    {
        var hits = Physics.OverlapSphere(transform.position, radius, affectedLayers);
        foreach (var hit in hits)
        {
            var rb = hit.attachedRigidbody;
            if (rb != null && rb != GetComponent<Rigidbody>())
            {
                rb.AddForce(_direction * _force, ForceMode.Acceleration);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
