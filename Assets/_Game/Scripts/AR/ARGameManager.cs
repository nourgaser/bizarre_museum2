using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ARGameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ARHUDController hud;
    [SerializeField] private string vrSceneName = "VRScene";

    [Header("Debug Collection (Editor)")]
    [SerializeField] private string[] debugItems =
    {
        "gravity-anomaly",
        "humming-relic",
        "chromatic-shifter"
    };

    private readonly List<Slot> _slots = new List<Slot>(3);

    private void Awake()
    {
        EnsureHud();
    }

    private async void Start()
    {
        if (hud == null)
        {
            return;
        }

        hud.UploadRequested += OnUploadRequested;
        hud.GoToVRRequested += OnGoToVRRequested;
        hud.SetUploadEnabled(false, "Collect 3 items to enable.");
        hud.SetStatus("Initializing");
        hud.SetNetwork(false);
        hud.SetVRNavigationEnabled(false, "Generate a code first.");
        SyncSlots();

        await ProbeBackend();
    }

    private void OnDestroy()
    {
        if (hud != null)
        {
            hud.UploadRequested -= OnUploadRequested;
            hud.GoToVRRequested -= OnGoToVRRequested;
        }
    }

    private void Update()
    {
        // Editor-only debug collection
        if (WasKeyPressed(1)) CollectDebug(0);
        if (WasKeyPressed(2)) CollectDebug(1);
        if (WasKeyPressed(3)) CollectDebug(2);
    }

    public void CollectBubblePickup(ItemBubble bubble)
    {
        if (bubble == null || bubble.IsCollected)
        {
            return;
        }

        if (_slots.Count >= 3)
        {
            hud?.SetStatus("Inventory full (3)");
            return;
        }

        if (!bubble.IsPopped)
        {
            bubble.TryPop();
        }

        var slug = bubble.Slug;
        var seed = bubble.Seed;

        bubble.MarkCollected();

        _slots.Add(new Slot(slug, seed));
        hud?.SetStatus($"Collected {slug}");
        SyncSlots();
    }

    private static bool WasKeyPressed(int digit)
    {
        switch (digit)
        {
            case 1:
#if ENABLE_INPUT_SYSTEM
                return UnityEngine.InputSystem.Keyboard.current != null &&
                       UnityEngine.InputSystem.Keyboard.current.digit1Key.wasPressedThisFrame;
#else
                return Input.GetKeyDown(KeyCode.Alpha1);
#endif
            case 2:
#if ENABLE_INPUT_SYSTEM
                return UnityEngine.InputSystem.Keyboard.current != null &&
                       UnityEngine.InputSystem.Keyboard.current.digit2Key.wasPressedThisFrame;
#else
                return Input.GetKeyDown(KeyCode.Alpha2);
#endif
            case 3:
#if ENABLE_INPUT_SYSTEM
                return UnityEngine.InputSystem.Keyboard.current != null &&
                       UnityEngine.InputSystem.Keyboard.current.digit3Key.wasPressedThisFrame;
#else
                return Input.GetKeyDown(KeyCode.Alpha3);
#endif
            default:
                return false;
        }
    }

    private void CollectDebug(int index)
    {
        if (index < 0 || index >= debugItems.Length) return;
        if (_slots.Count >= 3)
        {
            hud?.SetStatus("Inventory full (3)");
            return;
        }

        var slug = debugItems[index];
        var seed = Random.value;
        _slots.Add(new Slot(slug, seed));
        hud?.SetStatus($"Collected {slug}");
        SyncSlots();
    }

    private async void OnUploadRequested()
    {
        if (_slots.Count < 3)
        {
            hud?.SetStatus("Need 3 items to upload");
            return;
        }

        hud?.SetStatus("Uploading concoction...");
        var api = ApiClient.Instance;
        if (api == null)
        {
            hud?.SetStatus("API client missing");
            return;
        }

        var slugs = new string[_slots.Count];
        for (int i = 0; i < _slots.Count; i++)
        {
            slugs[i] = _slots[i].slug;
        }

        var resp = await api.CreateConcoctionAsync(slugs);
        if (resp == null || string.IsNullOrWhiteSpace(resp.code))
        {
            hud?.SetStatus("Upload failed");
            return;
        }

        hud?.SetStatus($"Code: {resp.code}");
        hud?.SetCode(resp.code);
        hud?.SetVRNavigationEnabled(true, "Jump to VR to place it.");
    }

    private void OnGoToVRRequested()
    {
        if (string.IsNullOrWhiteSpace(vrSceneName))
        {
            hud?.SetStatus("VR scene not configured");
            return;
        }

        SceneManager.LoadScene(vrSceneName);
    }

    private async Task ProbeBackend()
    {
        var api = ApiClient.Instance;
        if (api == null)
        {
            hud?.SetStatus("API client missing");
            return;
        }

        var ok = await api.PingAsync();
        hud?.SetNetwork(ok);
        hud?.SetStatus(ok ? "Online" : "Offline");
    }

    private void SyncSlots()
    {
        if (hud == null) return;

        var data = new ARHUDController.SlotData[3];
        for (int i = 0; i < data.Length; i++)
        {
            if (i < _slots.Count)
            {
                var slot = _slots[i];
                data[i] = new ARHUDController.SlotData($"Slot {i + 1}", slot.slug, slot.seed);
            }
            else
            {
                data[i] = new ARHUDController.SlotData($"Slot {i + 1}", "Empty", null);
            }
        }

        hud.SetSlots(data);
        hud.SetUploadEnabled(_slots.Count >= 3, _slots.Count >= 3 ? "" : "Collect 3 items to enable.");
    }

    private void EnsureHud()
    {
        if (hud == null)
        {
            hud = FindObjectOfType<ARHUDController>();
        }
    }

    private readonly struct Slot
    {
        public readonly string slug;
        public readonly float seed;

        public Slot(string slug, float seed)
        {
            this.slug = slug;
            this.seed = seed;
        }
    }
}
