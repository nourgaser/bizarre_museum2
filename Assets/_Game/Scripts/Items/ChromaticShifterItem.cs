using UnityEngine;

public class ChromaticShifterItem : MonoBehaviour, ISeededItem
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Gradient palette;
    [SerializeField] private float cycleSpeedMin = 0.2f;
    [SerializeField] private float cycleSpeedMax = 1.2f;

    private float _cycleSpeed;
    private float _t;

    public void Configure(float seed)
    {
        var rng = new System.Random(Mathf.FloorToInt(seed * 100000f));
        _cycleSpeed = Mathf.Lerp(cycleSpeedMin, cycleSpeedMax, (float)rng.NextDouble());

        if (targetRenderer == null)
        {
            targetRenderer = GetComponentInChildren<Renderer>();
        }
    }

    private void Update()
    {
        if (targetRenderer == null || palette == null)
        {
            return;
        }

        _t += Time.deltaTime * _cycleSpeed;
        var color = palette.Evaluate(Mathf.Repeat(_t, 1f));
        foreach (var mat in targetRenderer.materials)
        {
            mat.color = color;
            if (mat.HasProperty("_EmissionColor"))
            {
                mat.SetColor("_EmissionColor", color * 1.5f);
            }
        }
    }
}
