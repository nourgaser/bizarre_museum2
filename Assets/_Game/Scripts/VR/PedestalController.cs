using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PedestalController : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private Transform buttonTarget;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor socketInteractor;

    public Transform SpawnPoint => spawnPoint;
    public Transform ButtonTarget => buttonTarget;
    public UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor Socket => socketInteractor;
    public VRItemPickup CurrentPickup { get; private set; }

    private void OnEnable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.AddListener(OnSelectEntered);
            socketInteractor.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnDisable()
    {
        if (socketInteractor != null)
        {
            socketInteractor.selectEntered.RemoveListener(OnSelectEntered);
            socketInteractor.selectExited.RemoveListener(OnSelectExited);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        CurrentPickup = args.interactableObject.transform.GetComponent<VRItemPickup>();
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (args.interactableObject.transform.GetComponent<VRItemPickup>() == CurrentPickup)
        {
            CurrentPickup = null;
        }
    }

    public void ClearSlot()
    {
        CurrentPickup = null;
        // Socket handles the release; no direct destroy here.
    }
}
