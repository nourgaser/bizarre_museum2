using UnityEngine;

public class ItemBubbleCollector : MonoBehaviour
{
    [Header("Setup")]
    [SerializeField] private ARGameManager gameManager;
    [SerializeField] private Transform cameraTransform;

    [Header("Interaction")]
    [SerializeField] private LayerMask bubbleLayerMask = ~0;
    [SerializeField] private LayerMask groundLayerMask = ~0;
    [SerializeField] private float rayDistance = 6f;
    [SerializeField] private float groundOffset = 1.0f;
    [SerializeField] private bool highlightTargets = true;
    [SerializeField] private float pickupRadius = 0.6f;
    [SerializeField] private float groundRayLength = 10f;

    private ItemBubble _highlighted;
    private readonly System.Collections.Generic.List<ItemBubble> _pending = new();

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
        UpdatePickupProximity();

        if (WasClickOrTap())
        {
            TryPop();
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

    private void TryPop()
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
                float groundHeight = ResolveGroundHeight(bubble.transform.position);
                bubble.SetGroundHeight(groundHeight);
                bubble.TryPop();
                EnqueuePending(bubble);
            }
        }
    }

    private void EnqueuePending(ItemBubble bubble)
    {
        if (bubble == null)
        {
            return;
        }

        if (!_pending.Contains(bubble))
        {
            _pending.Add(bubble);
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

    private float ResolveGroundHeight(Vector3 origin)
    {
        if (Physics.Raycast(origin + Vector3.up * 0.25f, Vector3.down, out var hit, groundRayLength, groundLayerMask, QueryTriggerInteraction.Ignore))
        {
            return hit.point.y;
        }

        return cameraTransform != null ? cameraTransform.position.y - groundOffset : origin.y - groundOffset;
    }

    private void UpdatePickupProximity()
    {
        if (gameManager == null || cameraTransform == null)
        {
            return;
        }

        for (int i = _pending.Count - 1; i >= 0; i--)
        {
            var bubble = _pending[i];
            if (bubble == null || bubble.IsCollected)
            {
                _pending.RemoveAt(i);
                continue;
            }

            if (!bubble.IsReadyForPickup)
            {
                continue;
            }

            float dist = Vector3.Distance(cameraTransform.position, bubble.InnerPosition);
            if (dist <= pickupRadius)
            {
                gameManager.CollectBubblePickup(bubble);
                _pending.RemoveAt(i);
            }
        }
    }
}
