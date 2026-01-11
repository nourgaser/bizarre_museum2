using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PedestalController : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform buttonTarget;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;
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
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        CurrentPickup = args.interactableObject.transform.GetComponent<VRItemPickup>();
        if (logEvents)
        {
            Debug.Log($"Pedestal socket selected {CurrentPickup?.name ?? "(null)"}");
        }

        ApplyFloatingBob();
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.GetComponent<VRItemPickup>() == CurrentPickup)
        {
            CurrentPickup = null;
        }

        if (logEvents)
        {
            Debug.Log("Pedestal socket deselected item.");
        }

        RemoveFloatingBob();
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
