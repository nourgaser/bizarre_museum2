using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBubbleSpawner : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private ItemBubble bubblePrefab;
    [SerializeField] private List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();
    [SerializeField] private Transform cameraTransform;

    [Header("Spawn Rules")]
    [SerializeField] private int targetBubbles = 5;
    [SerializeField] private float spawnRadius = 3.5f;
    [SerializeField] private float minDistanceFromCamera = 1.5f;
    [SerializeField] private float spawnHeightOffset = 0.5f;
    [SerializeField] private float spawnInterval = 1.5f;
    [SerializeField] private float initialDelay = 0.5f;

    [Header("Feedback")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip spawnClip;
    [SerializeField] private float spawnPulseScale = 1.2f;
    [SerializeField] private float spawnPulseTime = 0.25f;

    private readonly List<ItemBubble> _active = new List<ItemBubble>();
    private Coroutine _loop;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    private void OnEnable()
    {
        _loop = StartCoroutine(SpawnLoop());
    }

    private void OnDisable()
    {
        if (_loop != null)
        {
            StopCoroutine(_loop);
            _loop = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        if (initialDelay > 0f)
        {
            yield return new WaitForSeconds(initialDelay);
        }

        while (true)
        {
            Cleanup();
            if (_active.Count < targetBubbles)
            {
                SpawnOne();
            }

            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void Cleanup()
    {
        _active.RemoveAll(b => b == null || !b.isActiveAndEnabled || b.IsPopped);
    }

    private void SpawnOne()
    {
        if (bubblePrefab == null || itemDefinitions.Count == 0 || cameraTransform == null)
        {
            return;
        }

        var def = itemDefinitions[Random.Range(0, itemDefinitions.Count)];
        Vector3 pos = PickSpawnPosition();
        var bubble = Instantiate(bubblePrefab, pos, Quaternion.identity, transform);
        bubble.name = $"Bubble_{def.slug}";

        bubble.SetDefinition(def);

        _active.Add(bubble);
        StartCoroutine(SpawnPulse(bubble.transform));
        PlaySpawnSfx();
    }

    private Vector3 PickSpawnPosition()
    {
        Vector3 origin = cameraTransform.position;
        Vector3 forward = cameraTransform.forward;
        Vector2 circle = Random.insideUnitCircle * spawnRadius;
        Vector3 offset = (forward * minDistanceFromCamera) + new Vector3(circle.x, 0f, circle.y);
        Vector3 pos = origin + offset;
        pos.y = origin.y + spawnHeightOffset;
        return pos;
    }

    private IEnumerator SpawnPulse(Transform t)
    {
        if (t == null)
        {
            yield break;
        }

        Vector3 start = t.localScale;
        Vector3 target = start * spawnPulseScale;
        float timer = 0f;
        while (timer < spawnPulseTime)
        {
            timer += Time.deltaTime;
            float f = Mathf.Clamp01(timer / spawnPulseTime);
            float eased = Mathf.SmoothStep(0f, 1f, f);
            t.localScale = Vector3.Lerp(start, target, eased);
            yield return null;
        }

        timer = 0f;
        while (timer < spawnPulseTime)
        {
            timer += Time.deltaTime;
            float f = Mathf.Clamp01(timer / spawnPulseTime);
            float eased = Mathf.SmoothStep(0f, 1f, f);
            t.localScale = Vector3.Lerp(target, start, eased);
            yield return null;
        }

        t.localScale = start;
    }

    private void PlaySpawnSfx()
    {
        if (spawnClip == null)
        {
            return;
        }

        if (sfxSource == null)
        {
            sfxSource = FindObjectOfType<AudioSource>();
        }

        if (sfxSource != null)
        {
            sfxSource.PlayOneShot(spawnClip);
        }
    }
}
