using UnityEngine;

public class FloatingBob : MonoBehaviour
{
    [SerializeField] private float amplitude = 0.25f;
    [SerializeField] private float frequency = 0.8f;
    [SerializeField] private Vector3 axis = new Vector3(0.25f, 1f, 0.25f);

    [SerializeField] private Vector3 axisRandomness = new Vector3(0.1f, 0.1f, 0.1f);

    private Vector3 _startLocalPos;
    private float _seed;

    private void Awake()
    {
        _startLocalPos = transform.localPosition;
        _seed = Random.Range(0f, 1000f);
    }

    private void Update()
    {
        float t = (Time.time + _seed) * frequency;
        float offset = Mathf.Sin(t) * amplitude;
        transform.localPosition = _startLocalPos + axis.normalized * offset;

        if (axisRandomness != Vector3.zero)
        {
            Vector3 randomOffset = new Vector3(
                Mathf.PerlinNoise(t, 0f) - 0.5f,
                Mathf.PerlinNoise(0f, t) - 0.5f,
                Mathf.PerlinNoise(t, t) - 0.5f
            );
            randomOffset.Scale(axisRandomness);
            transform.localPosition += randomOffset;
        }
    }

    public void SetAmplitude(float value)
    {
        amplitude = value;
    }

    public void SetFrequency(float value)
    {
        frequency = value;
    }

    public void SetAxis(Vector3 value)
    {
        axis = value;
    }

    public void SetAxisRandomness(Vector3 value)
    {
        axisRandomness = value;
    }
}
