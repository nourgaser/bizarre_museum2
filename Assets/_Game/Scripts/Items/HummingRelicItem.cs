using UnityEngine;

public class HummingRelicItem : MonoBehaviour, ISeededItem
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float baseVolumeMin = 0.1f;
    [SerializeField] private float baseVolumeMax = 0.4f;
    [SerializeField] private float pitchMin = 0.7f;
    [SerializeField] private float pitchMax = 1.3f;
    [SerializeField] private Transform listener; // optional override (e.g., Camera)
    [SerializeField] private float audibleRadius = 6f;
    [SerializeField] private AnimationCurve distanceFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    private float _baseVolume;

    public void Configure(float seed)
    {
        var rng = new System.Random(Mathf.FloorToInt(seed * 100000f));
        _baseVolume = Mathf.Lerp(baseVolumeMin, baseVolumeMax, (float)rng.NextDouble());
        float pitch = Mathf.Lerp(pitchMin, pitchMax, (float)rng.NextDouble());

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                audioSource.loop = true;
                audioSource.playOnAwake = true;
            }
        }

        audioSource.volume = _baseVolume;
        audioSource.pitch = pitch;
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void Update()
    {
        if (audioSource == null)
        {
            return;
        }

        var target = listener != null ? listener : Camera.main?.transform;
        if (target == null)
        {
            return;
        }

        float dist = Vector3.Distance(transform.position, target.position);
        float t = Mathf.Clamp01(dist / Mathf.Max(0.01f, audibleRadius));
        float atten = distanceFalloff.Evaluate(t);
        audioSource.volume = _baseVolume * atten;
    }
}
