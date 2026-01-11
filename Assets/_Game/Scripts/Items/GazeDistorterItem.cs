using UnityEngine;

public class GazeDistorterItem : MonoBehaviour, ISeededItem
{
    [SerializeField] private float maxStrength = 0.6f;
    [SerializeField] private float rampTime = 1.5f;

    private float _strength;

    public void Configure(float seed)
    {
        var rng = new System.Random(Mathf.FloorToInt(seed * 100000f));
        _strength = Mathf.Lerp(0.2f, maxStrength, (float)rng.NextDouble());
    }

    // Placeholder: pulsing scale to hint at distortion
    private void Update()
    {
        float t = Mathf.PingPong(Time.time / rampTime, 1f);
        float scale = Mathf.Lerp(1f, 1f + _strength * 0.25f, t);
        transform.localScale = Vector3.one * scale;
    }
}
