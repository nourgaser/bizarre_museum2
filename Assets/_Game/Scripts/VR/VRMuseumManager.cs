using System.Collections.Generic;
using UnityEngine;

public class VRMuseumManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private VRTerminalController terminal;
    [SerializeField] private PedestalController[] pedestals;
    [SerializeField] private Transform[] dropSpawns;
    [SerializeField] private VRItemPickup pickupPrefab;

    [Header("Data")]
    [SerializeField] private List<ItemDefinition> itemDefinitions = new List<ItemDefinition>();

    private readonly Dictionary<string, ItemDefinition> _definitions = new Dictionary<string, ItemDefinition>();
    private readonly List<VRItemPickup> _spawnedPickups = new List<VRItemPickup>();

    private void Awake()
    {
        if (terminal == null)
        {
            terminal = FindObjectOfType<VRTerminalController>();
        }

        BuildLookup();
    }

    private void OnEnable()
    {
        if (terminal != null)
        {
            terminal.ConcoctionLoaded += OnConcoctionLoaded;
        }
    }

    private void OnDisable()
    {
        if (terminal != null)
        {
            terminal.ConcoctionLoaded -= OnConcoctionLoaded;
        }
    }

    private void BuildLookup()
    {
        _definitions.Clear();
        foreach (var def in itemDefinitions)
        {
            if (def == null || string.IsNullOrWhiteSpace(def.slug))
            {
                continue;
            }

            var key = def.slug.Trim().ToLowerInvariant();
            if (_definitions.ContainsKey(key))
            {
                continue;
            }

            _definitions.Add(key, def);
        }
    }

    private void OnConcoctionLoaded(ConcoctionDto data)
    {
        if (data == null || data.items == null)
        {
            return;
        }

        ClearPedestals();
        ClearSpawnedPickups();

        for (int i = 0; i < data.items.Length; i++)
        {
            var item = data.items[i];
            var def = ResolveDefinition(item.slug);
            if (def == null)
            {
                Debug.LogWarning($"No ItemDefinition found for slug '{item.slug}'.");
                continue;
            }

            var spawn = GetDropSpawn(i);
            var pickup = SpawnPickup(def, item.seed, spawn);
            if (pickup != null)
            {
                _spawnedPickups.Add(pickup);
            }
        }
    }

    private ItemDefinition ResolveDefinition(string slug)
    {
        if (string.IsNullOrWhiteSpace(slug)) return null;
        _definitions.TryGetValue(slug.Trim().ToLowerInvariant(), out var def);
        return def;
    }

    private void ClearPedestals()
    {
        if (pedestals == null) return;
        foreach (var pedestal in pedestals)
        {
            pedestal?.ClearSlot();
        }
    }

    private void ClearSpawnedPickups()
    {
        foreach (var pickup in _spawnedPickups)
        {
            if (pickup != null)
            {
                Destroy(pickup.gameObject);
            }
        }

        _spawnedPickups.Clear();
    }

    private Transform GetDropSpawn(int index)
    {
        if (dropSpawns != null && dropSpawns.Length > 0)
        {
            var safeIndex = Mathf.Clamp(index, 0, dropSpawns.Length - 1);
            if (dropSpawns[safeIndex] != null)
            {
                return dropSpawns[safeIndex];
            }
        }

        return transform; // fallback to manager origin
    }

    private VRItemPickup SpawnPickup(ItemDefinition def, float seed, Transform spawn)
    {
        if (pickupPrefab == null)
        {
            Debug.LogWarning("VRMuseumManager missing pickupPrefab; cannot spawn items.");
            return null;
        }

        var pos = spawn != null ? spawn.position : transform.position;
        var rot = spawn != null ? spawn.rotation : Quaternion.identity;
        var pickup = Instantiate(pickupPrefab, pos, rot);
        pickup.Configure(def, seed);
        return pickup;
    }
}
