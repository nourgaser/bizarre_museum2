using UnityEngine;

public class ItemBubbleCollector : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private ARGameManager gameManager;
    [SerializeField] private Transform cameraTransform;

    [Header("Interaction")]
    [SerializeField] private LayerMask bubbleLayerMask = ~0;
    [SerializeField] private float rayDistance = 6f;
    [SerializeField] private float groundOffset = 1.0f;
    [SerializeField] private bool highlightTargets = true;

    private ItemBubble _highlighted;

    private void Awake()
    {
        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }

        if (gameManager == null)
        {
            gameManager = FindObjectOfType<ARGameManager>();
        }
    }

    private void Update()
    {
        UpdateHighlight();

        if (WasClickOrTap())
        {
            TryCollect();
        }
    }

    private bool WasClickOrTap()
    {
#if ENABLE_INPUT_SYSTEM
        var mouse = UnityEngine.InputSystem.Mouse.current;
        if (mouse != null && mouse.leftButton.wasPressedThisFrame) return true;
        var touch = UnityEngine.InputSystem.Touchscreen.current;
        if (touch != null && touch.primaryTouch.press.wasPressedThisFrame) return true;
#else
        if (Input.GetMouseButtonDown(0)) return true;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) return true;
#endif
        return false;
    }

    private void TryCollect()
    {
        if (cameraTransform == null || gameManager == null)
        {
            return;
        }

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        if (Physics.Raycast(ray, out var hit, rayDistance, bubbleLayerMask, QueryTriggerInteraction.Collide))
        {
            var bubble = hit.collider.GetComponentInParent<ItemBubble>();
            if (bubble != null && !bubble.IsPopped)
            {
                float groundHeight = cameraTransform.position.y - groundOffset;
                bubble.SetGroundHeight(groundHeight);
                gameManager.CollectBubble(bubble);
            }
        }
    }

    private void UpdateHighlight()
    {
        if (!highlightTargets || cameraTransform == null)
        {
            SetHighlighted(null);
            return;
        }

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        ItemBubble target = null;
        if (Physics.Raycast(ray, out var hit, rayDistance, bubbleLayerMask, QueryTriggerInteraction.Collide))
        {
            target = hit.collider.GetComponentInParent<ItemBubble>();
            if (target != null && target.IsPopped)
            {
                target = null;
            }
        }

        SetHighlighted(target);
    }

    private void SetHighlighted(ItemBubble bubble)
    {
        if (_highlighted == bubble)
        {
            return;
        }

        if (_highlighted != null)
        {
            _highlighted.SetHighlighted(false);
        }

        _highlighted = bubble;

        if (_highlighted != null)
        {
            _highlighted.SetHighlighted(true);
        }
    }

    private void OnDisable()
    {
        SetHighlighted(null);
    }
}
