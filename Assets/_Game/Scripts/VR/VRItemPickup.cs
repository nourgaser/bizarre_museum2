using UnityEngine;

using System.Collections.Generic;
using System.Linq;

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
    private readonly List<MonoBehaviour> _seededBehaviours = new List<MonoBehaviour>();
    private bool _seededEnabled;

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

        CacheSeededBehaviours();
        ConfigureSeeded(seed);
        SetSeededEnabled(false);
    }

    private void CacheSeededBehaviours()
    {
        _seededBehaviours.Clear();
        var behaviours = _visualInstance.GetComponentsInChildren<MonoBehaviour>(true);
        foreach (var mb in behaviours)
        {
            if (mb is ISeededItem)
            {
                _seededBehaviours.Add(mb);
            }
        }
    }

    private void ConfigureSeeded(float seedValue)
    {
        foreach (var mb in _seededBehaviours)
        {
            if (mb is ISeededItem seeded)
            {
                seeded.Configure(seedValue);
            }
        }
    }

    public void SetSeededEnabled(bool enable)
    {
        _seededEnabled = enable;
        foreach (var mb in _seededBehaviours.Where(b => b != null))
        {
            mb.enabled = enable;
        }
    }
}
