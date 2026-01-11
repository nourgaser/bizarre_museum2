using UnityEngine;

public class ItemBubble : MonoBehaviour
{
    [Header("Definition")]
    [SerializeField] private ItemDefinition itemDefinition;

    [Header("References")]
    [SerializeField] private Renderer bubbleRenderer;
    [SerializeField] private Collider bubbleCollider;
    [SerializeField] private Transform innerAnchor;

    [Header("Pop Settings")]
    [SerializeField] private float popForce = 0.8f;
    [SerializeField] private float popTorque = 0.2f;

    [Header("Highlight")]
    [SerializeField] private Color highlightColor = Color.cyan;
    [SerializeField, Range(0f, 1f)] private float highlightTintStrength = 0.35f;
    [SerializeField] private float highlightScale = 1.08f;
    [SerializeField] private float highlightLerp = 12f;

    [Header("Grounding")]
    [SerializeField] private bool enableGrounding = true;
    [SerializeField] private float groundSnapOffset = 0.02f;
    [SerializeField] private float groundedDrag = 5f;

    private bool _popped;
    private bool _initialized;
    private bool _collected;
    private float _seed = -1f;
    private bool _highlighted;
    private bool _hasGroundHeight;
    private float _groundHeight;
    private Rigidbody _innerRigidbody;
    private GameObject _innerInstance;
    private MaterialPropertyBlock _mpb;
    private Vector3 _baseScale;

    private void Start()
    {
        InitializeIfNeeded();
    }

    private void LateUpdate()
    {
        UpdateHighlightVisual();
        HandleGrounding();
    }

    private void InitializeIfNeeded()
    {
        if (_initialized || itemDefinition == null)
        {
            return;
        }

        if (_seed < 0f)
        {
            _seed = Random.value;
        }

        _baseScale = transform.localScale;

        ApplyTint(itemDefinition.hintColor, false);

        SpawnInner();
        if (_innerRigidbody != null)
        {
            _innerRigidbody.isKinematic = true;
            _innerRigidbody.useGravity = false;
        }

        _initialized = true;
    }

    private void SpawnInner()
    {
        if (itemDefinition == null || itemDefinition.innerPrefab == null)
        {
            return;
        }

        Transform parent = innerAnchor != null ? innerAnchor : transform;
        _innerInstance = Instantiate(itemDefinition.innerPrefab, parent);
        _innerInstance.transform.localPosition = Vector3.zero;
        _innerInstance.transform.localRotation = Quaternion.identity;

        _innerRigidbody = _innerInstance.GetComponent<Rigidbody>();
        if (_innerRigidbody == null)
        {
            _innerRigidbody = _innerInstance.AddComponent<Rigidbody>();
        }

        var seeded = _innerInstance.GetComponent<ISeededItem>();
        if (seeded != null)
        {
            seeded.Configure(_seed);
        }

        _innerInstance.SetActive(true);
    }

    public void TryPop()
    {
        if (_popped)
        {
            return;
        }

        _popped = true;

        if (bubbleRenderer != null)
        {
            bubbleRenderer.enabled = false;
        }

        if (bubbleCollider != null)
        {
            bubbleCollider.enabled = false;
        }

        if (_innerInstance != null)
        {
            _innerInstance.SetActive(true);
        }

        if (_innerRigidbody != null)
        {
            _innerRigidbody.isKinematic = false;
            _innerRigidbody.useGravity = true;
            _innerRigidbody.AddForce(Vector3.down * popForce, ForceMode.Impulse);
            _innerRigidbody.AddTorque(Random.insideUnitSphere * popTorque, ForceMode.Impulse);
        }

        SetHighlighted(false);
    }

    public string Slug => itemDefinition != null ? itemDefinition.slug : "unknown";
    public bool IsPopped => _popped;
    public bool IsCollected => _collected;
    public bool IsReadyForPickup => _popped && !_collected;
    public Vector3 InnerPosition => _innerInstance != null ? _innerInstance.transform.position : transform.position;

    public float Seed => _seed;

    public void SetDefinition(ItemDefinition def)
    {
        SetDefinition(def, Random.value);
    }

    public void SetDefinition(ItemDefinition def, float seed)
    {
        itemDefinition = def;
        _seed = seed;
        _initialized = false;
        InitializeIfNeeded();
    }

    public void SetHighlighted(bool value)
    {
        _highlighted = value && !_popped;
    }

    public void SetGroundHeight(float height)
    {
        _groundHeight = height;
        _hasGroundHeight = true;
    }

    private void UpdateHighlightVisual()
    {
        if (!_initialized)
        {
            return;
        }

        float targetScale = _highlighted ? highlightScale : 1f;
        transform.localScale = Vector3.Lerp(transform.localScale, _baseScale * targetScale, Time.deltaTime * highlightLerp);

        if (bubbleRenderer == null)
        {
            return;
        }

        Color baseColor = itemDefinition != null ? itemDefinition.hintColor : Color.white;
        Color mixed = _highlighted ? Color.Lerp(baseColor, highlightColor, highlightTintStrength) : baseColor;
        ApplyTint(mixed, _highlighted);
    }

    private void ApplyTint(Color color, bool emissive)
    {
        if (bubbleRenderer == null)
        {
            return;
        }

        _mpb ??= new MaterialPropertyBlock();
        bubbleRenderer.GetPropertyBlock(_mpb);
        _mpb.SetColor("_BaseColor", color);
        _mpb.SetColor("_Color", color);

        if (emissive)
        {
            _mpb.SetColor("_EmissionColor", color * 1.5f);
        }

        bubbleRenderer.SetPropertyBlock(_mpb);
    }

    private void HandleGrounding()
    {
        if (!_popped || !_hasGroundHeight || !enableGrounding || _innerInstance == null || _innerRigidbody == null)
        {
            return;
        }

        float floor = _groundHeight + groundSnapOffset;
        if (_innerInstance.transform.position.y <= floor)
        {
            var pos = _innerInstance.transform.position;
            pos.y = floor;
            _innerInstance.transform.position = pos;

            _innerRigidbody.linearVelocity = Vector3.zero;
            _innerRigidbody.angularVelocity = Vector3.zero;
            _innerRigidbody.linearDamping = groundedDrag;
            _innerRigidbody.angularDamping = groundedDrag;
            _innerRigidbody.useGravity = false;
            _innerRigidbody.Sleep();
        }
    }

    public void MarkCollected()
    {
        if (_collected)
        {
            return;
        }

        _collected = true;
        _highlighted = false;

        if (_innerInstance != null)
        {
            _innerInstance.SetActive(false);
        }
    }
}
