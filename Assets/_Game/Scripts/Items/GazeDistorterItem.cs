using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GazeDistorterItem : MonoBehaviour, ISeededItem
{
    [SerializeField] private Volume volume; // assign a Volume with Lens Distortion override
    [SerializeField] private float maxStrength = 0.6f;
    [SerializeField] private float rampTime = 1.5f;
    [SerializeField] private float triggerAngleDegrees = 20f;

    private float _strength;
    private LensDistortion _lens;
    private float _t;

    public void Configure(float seed)
    {
        var rng = new System.Random(Mathf.FloorToInt(seed * 100000f));
        _strength = Mathf.Lerp(0.2f, maxStrength, (float)rng.NextDouble());

        if (volume == null)
        {
            volume = FindObjectOfType<Volume>();
        }

        if (volume != null)
        {
            volume.profile.TryGet(out _lens);
        }
    }

    private void Update()
    {
        if (_lens == null || volume == null)
        {
            return;
        }

        var cam = Camera.main;
        if (cam == null)
        {
            return;
        }

        Vector3 toItem = (transform.position - cam.transform.position);
        float distance = toItem.magnitude;
        if (distance < 0.001f)
        {
            return;
        }

        Vector3 dir = toItem / distance;
        float angle = Vector3.Angle(cam.transform.forward, dir);
        bool isLooking = angle <= triggerAngleDegrees;

        if (!isLooking)
        {
            // Fade out when not looking
            _lens.intensity.Override(Mathf.Lerp(_lens.intensity.value, 0f, Time.deltaTime * 6f));
            return;
        }

        // Animate when looking
        _t += Time.deltaTime;
        float phase = Mathf.PingPong(_t / rampTime, 1f);
        float intensity = Mathf.Lerp(0f, -_strength, phase); // negative for barrel distortion

        _lens.intensity.Override(intensity);
        _lens.xMultiplier.Override(1f);
        _lens.yMultiplier.Override(1f);

        // Localize center to item screen position
        Vector3 viewportPos = cam.WorldToViewportPoint(transform.position);
        _lens.center.Override(new Vector2(viewportPos.x, viewportPos.y));
    }
}
