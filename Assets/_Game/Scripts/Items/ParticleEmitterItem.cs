using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class ParticleEmitterItem : MonoBehaviour, ISeededItem
{
    private ParticleSystem _ps;

    public void Configure(float seed)
    {
        if (_ps == null)
        {
            _ps = GetComponent<ParticleSystem>();
        }

        var rng = new System.Random(Mathf.FloorToInt(seed * 100000f));
        float rate = Mathf.Lerp(5f, 25f, (float)rng.NextDouble());
        float size = Mathf.Lerp(0.05f, 0.2f, (float)rng.NextDouble());
        var main = _ps.main;
        main.startSize = size;
        var emission = _ps.emission;
        emission.rateOverTime = rate;

        if (!_ps.isPlaying)
        {
            _ps.Play();
        }
    }
}
