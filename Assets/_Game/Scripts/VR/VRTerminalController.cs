using UnityEngine;
using UnityEngine.UIElements;
using System;

public class VRTerminalController : MonoBehaviour
{
    [SerializeField] private bool focusCodeFieldOnStart = true;

    public event Action<ConcoctionDto> ConcoctionLoaded;

    [SerializeField] private UIDocument _doc;
    private TextField _codeField;
    private Button _submitButton;
    private Label _statusLabel;

    private void Awake()
    {
        if (_doc == null)
        {
            Debug.LogError("VRTerminalController requires a UIDocument on the same GameObject.");
            enabled = false;
            return;
        }

        var root = _doc.rootVisualElement;
        _codeField = root.Q<TextField>("codeField");
        _submitButton = root.Q<Button>("submitButton");
        _statusLabel = root.Q<Label>("statusLabel");

        SetStatus("Waiting for code...");
    }

    private void OnEnable()
    {
        if (_submitButton != null)
        {
            _submitButton.clicked += OnSubmit;
        }

        if (_codeField != null)
        {
            _codeField.RegisterCallback<KeyDownEvent>(OnKeyDown);
        }
    }

    private void OnDisable()
    {
        if (_submitButton != null)
        {
            _submitButton.clicked -= OnSubmit;
        }

        if (_codeField != null)
        {
            _codeField.UnregisterCallback<KeyDownEvent>(OnKeyDown);
        }
    }

    private void Start()
    {
        if (focusCodeFieldOnStart && _codeField != null)
        {
            _codeField.Focus();
            _codeField.SelectAll();
        }
    }

    public void SetStatus(string message)
    {
        if (_statusLabel != null)
        {
            _statusLabel.text = string.IsNullOrWhiteSpace(message) ? "" : message;
        }
    }

    private void OnKeyDown(KeyDownEvent evt)
    {
        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
        {
            evt.StopImmediatePropagation();
            OnSubmit();
        }
    }

    private async void OnSubmit()
    {
        Debug.Log("VRTerminalController: OnSubmit called.");
        var code = (_codeField?.text ?? string.Empty).Trim().ToUpperInvariant();
        if (string.IsNullOrWhiteSpace(code))
        {
            SetStatus("Enter a code.");
            return;
        }

        SetInteractable(false);
        SetStatus("Loading...");

        var api = ApiClient.Instance;
        if (api == null)
        {
            SetStatus("API client missing.");
            SetInteractable(true);
            return;
        }

        var data = await api.GetConcoctionAsync(code);
        if (data == null || data.items == null || data.items.Length == 0)
        {
            SetStatus("Code not found.");
            SetInteractable(true);
            return;
        }

        SetStatus($"Loaded {data.items.Length} items.");
        ConcoctionLoaded?.Invoke(data);
        SetInteractable(true);
    }

    private void SetInteractable(bool enabled)
    {
        _submitButton?.SetEnabled(enabled);
        _codeField?.SetEnabled(enabled);
    }
}
