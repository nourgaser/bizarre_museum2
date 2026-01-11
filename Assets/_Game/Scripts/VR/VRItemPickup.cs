using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class VRItemPickup : MonoBehaviour
{
    [Header("Data")]
    public string slug;
    public float seed;
    public ItemDefinition definition;

    [Header("Visuals")]
    [SerializeField] private Transform visualParent;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable _grab;
    private GameObject _visualInstance;

    private void Awake()
    {
        _grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (visualParent == null)
        {
            visualParent = transform;
        }
    }

    public void Configure(ItemDefinition def, float seedValue)
    {
        definition = def;
        slug = def != null ? def.slug : slug;
        seed = seedValue;

        if (_visualInstance != null)
        {
            Destroy(_visualInstance);
        }

        if (definition == null || definition.innerPrefab == null)
        {
            Debug.LogWarning("VRItemPickup missing definition or inner prefab; cannot build visual.");
            return;
        }

        _visualInstance = Instantiate(definition.innerPrefab, visualParent);
        _visualInstance.name = $"{definition.slug}-visual";

        var seeded = _visualInstance.GetComponent<ISeededItem>();
        seeded?.Configure(seed);
    }
}
