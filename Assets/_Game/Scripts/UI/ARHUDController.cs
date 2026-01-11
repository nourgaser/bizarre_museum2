using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ARHUDController : MonoBehaviour
{
    [SerializeField] private bool enableUploadOnStart = false;
    [SerializeField] private bool enableVRButtonOnStart = false;

    public event Action UploadRequested;
    public event Action GoToVRRequested;

    private UIDocument _doc;
    private Label _statusLabel;
    private Button _uploadButton;
    private Label _hintLabel;
    private VisualElement _netIndicator;
    private Label _netLabel;
    private VisualElement _codeCard;
    private Label _codeValue;
    private Label _codeSub;
    private Button _goVrButton;
    private SlotUI[] _slots;

    private void Awake()
    {
        _doc = GetComponent<UIDocument>();
        if (_doc == null)
        {
            Debug.LogError("ARHUDController requires a UIDocument.");
            return;
        }

        var root = _doc.rootVisualElement;
        _statusLabel = root.Q<Label>("status-label");
        _uploadButton = root.Q<Button>("btn-upload");
        _hintLabel = root.Q<Label>("hint-upload");
        _netIndicator = root.Q<VisualElement>("net-indicator");
        _netLabel = root.Q<Label>("net-label");
        _codeCard = root.Q<VisualElement>("code-card");
        _codeValue = root.Q<Label>("code-value");
        _codeSub = root.Q<Label>("code-sub");
        _goVrButton = root.Q<Button>("btn-to-vr");

        _slots = new[]
        {
            new SlotUI(root, 1),
            new SlotUI(root, 2),
            new SlotUI(root, 3)
        };

        _uploadButton?.SetEnabled(enableUploadOnStart);
        if (_uploadButton != null)
        {
            _uploadButton.clicked += OnUploadClicked;
        }

        if (_goVrButton != null)
        {
            _goVrButton.clicked += OnGoToVRClicked;
            _goVrButton.SetEnabled(enableVRButtonOnStart);
        }

        SetStatus("HUD ready (Editor)");
        SetNetwork(false);
        SetCode(string.Empty, "Generate a code first.");
        SetSlots(new[]
        {
            new SlotData("Slot 1", "Empty", null),
            new SlotData("Slot 2", "Empty", null),
            new SlotData("Slot 3", "Empty", null)
        });
    }

    private void OnDestroy()
    {
        if (_uploadButton != null)
        {
            _uploadButton.clicked -= OnUploadClicked;
        }

        if (_goVrButton != null)
        {
            _goVrButton.clicked -= OnGoToVRClicked;
        }
    }

    public void SetStatus(string text)
    {
        if (_statusLabel != null)
        {
            _statusLabel.text = $"Status: {text}";
        }
    }

    public void SetNetwork(bool online)
    {
        if (_netIndicator != null)
        {
            _netIndicator.EnableInClassList("online", online);
            _netIndicator.EnableInClassList("offline", !online);
        }

        if (_netLabel != null)
        {
            _netLabel.text = online ? "Online" : "Offline";
            _netLabel.style.color = new StyleColor(online ? new Color(0, 1f, 1f) : new Color(1f, 0.5f, 0.31f));
        }
    }

    public void SetUploadEnabled(bool enabled, string hint = "")
    {
        if (_uploadButton != null)
        {
            _uploadButton.SetEnabled(enabled);
        }

        if (_hintLabel != null)
        {
            _hintLabel.text = string.IsNullOrWhiteSpace(hint) ? "" : hint;
        }
    }

    public void SetVRNavigationEnabled(bool enabled, string hint = "")
    {
        if (_goVrButton != null)
        {
            _goVrButton.SetEnabled(enabled);
        }

        if (_codeSub != null && !string.IsNullOrWhiteSpace(hint))
        {
            _codeSub.text = hint;
        }
    }

    public void SetCode(string code, string subtitle = "")
    {
        bool hasCode = !string.IsNullOrWhiteSpace(code);

        if (_codeCard != null)
        {
            _codeCard.style.display = hasCode ? DisplayStyle.Flex : DisplayStyle.None;
        }

        if (_codeValue != null)
        {
            _codeValue.text = hasCode ? code : "------";
        }

        if (_codeSub != null)
        {
            _codeSub.text = hasCode
                ? (string.IsNullOrWhiteSpace(subtitle) ? "Use this code in the VR room." : subtitle)
                : "Generate a code first.";
        }
    }

    public void SetSlots(SlotData[] slots)
    {
        if (slots == null || _slots == null)
        {
            return;
        }

        for (int i = 0; i < _slots.Length && i < slots.Length; i++)
        {
            _slots[i].Apply(slots[i]);
        }
    }

    private void OnUploadClicked()
    {
        UploadRequested?.Invoke();
    }

    private void OnGoToVRClicked()
    {
        GoToVRRequested?.Invoke();
    }

    [Serializable]
    public struct SlotData
    {
        public string title;
        public string state;
        public float? seed;

        public SlotData(string title, string state, float? seed)
        {
            this.title = title;
            this.state = state;
            this.seed = seed;
        }
    }

    private class SlotUI
    {
        private readonly Label _label;
        private readonly Label _sub;

        public SlotUI(VisualElement root, int index)
        {
            _label = root.Q<Label>($"slot-label-{index}");
            _sub = root.Q<Label>($"slot-sub-{index}");
        }

        public void Apply(SlotData data)
        {
            if (_label != null)
            {
                _label.text = string.IsNullOrWhiteSpace(data.state)
                    ? data.title
                    : $"{data.title}: {data.state}";
            }

            if (_sub != null)
            {
                _sub.text = data.seed.HasValue ? $"Seed: {data.seed:F4}" : "Seed: -";
            }
        }
    }
}
