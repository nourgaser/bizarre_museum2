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

    private bool _popped;
    private bool _initialized;
    private Rigidbody _innerRigidbody;
    private GameObject _innerInstance;

    private void Start()
    {
        InitializeIfNeeded();
    }

    private void InitializeIfNeeded()
    {
        if (_initialized || itemDefinition == null)
        {
            return;
        }

        if (bubbleRenderer != null)
        {
            var mat = bubbleRenderer.material;
            if (mat != null)
            {
                mat.color = itemDefinition.hintColor;
            }
        }

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
            var seed = Random.value;
            seeded.Configure(seed);
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
    }

    public string Slug => itemDefinition != null ? itemDefinition.slug : "unknown";
    public bool IsPopped => _popped;

    public void SetDefinition(ItemDefinition def)
    {
        itemDefinition = def;
        _initialized = false;
        InitializeIfNeeded();
    }
}
