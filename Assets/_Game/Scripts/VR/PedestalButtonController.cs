using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable))]
public class PedestalButtonController : MonoBehaviour
{
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Color offColor = Color.gray;
    [SerializeField] private Color onColor = Color.cyan;
    [SerializeField] private bool startOn = false;

    public bool IsOn { get; private set; }

    public event System.Action<bool> Toggled;

    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable _interactable;
    private Material _materialInstance;

    private void Awake()
    {
        _interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        if (targetRenderer != null)
        {
            _materialInstance = targetRenderer.material;
        }
    }

    private void OnEnable()
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.AddListener(OnSelectEntered);
        }

        startOn = false;
        SetState(startOn, false);
    }

    private void OnDisable()
    {
        if (_interactable != null)
        {
            _interactable.selectEntered.RemoveListener(OnSelectEntered);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs _)
    {
        SetState(!IsOn, true);
    }

    public void SetState(bool on, bool invoke)
    {
        IsOn = on;
        ApplyColor();
        if (invoke)
        {
            Toggled?.Invoke(IsOn);
        }
    }

    public void ApplyColor()
    {
        if (_materialInstance != null)
        {
            _materialInstance.color = IsOn ? onColor : offColor;
        }
    }
}
