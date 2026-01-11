using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PedestalController : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform buttonTarget;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;
    [SerializeField] private PedestalButtonController button;
    [SerializeField] private bool logEvents = false;

    public Transform SpawnPoint => spawnPoint;
    public Transform ButtonTarget => buttonTarget;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor Socket => socketInteractor;
    public VRItemPickup CurrentPickup { get; private set; }

    private FloatingBob _floatingBob;

    private void OnEnable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnSelectEntered);
            socketInteractor.selectExited.AddListener(OnSelectExited);
            socketInteractor.hoverEntered.AddListener(OnHoverEntered);
            socketInteractor.hoverExited.AddListener(OnHoverExited);
        }

        if (button != null)
        {
            button.Toggled += OnButtonToggled;
            button.SetState(false, false);
        }
    }

    private void OnDisable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnSelectEntered);
            socketInteractor.selectExited.RemoveListener(OnSelectExited);
            socketInteractor.hoverEntered.RemoveListener(OnHoverEntered);
            socketInteractor.hoverExited.RemoveListener(OnHoverExited);
        }

        if (button != null)
        {
            button.Toggled -= OnButtonToggled;
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        CurrentPickup = args.interactableObject.transform.GetComponent<VRItemPickup>();
        if (logEvents)
        {
            Debug.Log($"Pedestal socket selected {CurrentPickup?.name ?? "(null)"}");
        }

        ApplyFloatingBob();

        if (button != null)
        {
            button.SetState(false, false);
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        var pickup = args.interactableObject.transform.GetComponent<VRItemPickup>();
        if (pickup == CurrentPickup)
        {
            CurrentPickup.SetSeededEnabled(false);
            CurrentPickup = null;
        }

        if (logEvents)
        {
            Debug.Log("Pedestal socket deselected item.");
        }

        RemoveFloatingBob();

        if (button != null)
        {
            button.SetState(false, true);
        }
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        if (logEvents)
        {
            Debug.Log($"Pedestal socket hover enter: {args.interactableObject.transform.name}");
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        if (logEvents)
        {
            Debug.Log("Pedestal socket hover exit");
        }
    }

    private void OnButtonToggled(bool isOn)
    {
        if (CurrentPickup != null)
        {
            CurrentPickup.SetSeededEnabled(isOn);
        }
    }

    private void ApplyFloatingBob()
    {
        RemoveFloatingBob();

        if (CurrentPickup == null) return;

        _floatingBob = CurrentPickup.gameObject.AddComponent<FloatingBob>();
        _floatingBob.SetAmplitude(0.1f);
        _floatingBob.SetFrequency(0.8f);
        _floatingBob.SetAxis(new Vector3(0.1f, 1f, 0.1f));
        _floatingBob.SetAxisRandomness(new Vector3(0.1f, 0.1f, 0.1f));
    }

    private void RemoveFloatingBob()
    {
        if (_floatingBob != null)
        {
            Destroy(_floatingBob);
            _floatingBob = null;
        }
    }

    public void ClearSlot()
    {
        CurrentPickup = null;
        // Socket handles the release; no direct destroy here.
    }
}
