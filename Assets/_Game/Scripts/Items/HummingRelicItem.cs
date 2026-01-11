using UnityEngine;

public class HummingRelicItem : MonoBehaviour, ISeededItem
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private float baseVolumeMin = 0.1f;
    [SerializeField] private float baseVolumeMax = 0.4f;
    [SerializeField] private float pitchMin = 0.7f;
    [SerializeField] private float pitchMax = 1.3f;

    public void Configure(float seed)
    {
        var rng = new System.Random(Mathf.FloorToInt(seed * 100000f));
        float baseVol = Mathf.Lerp(baseVolumeMin, baseVolumeMax, (float)rng.NextDouble());
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

        audioSource.volume = baseVol;
        audioSource.pitch = pitch;
        if (!audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }
}
