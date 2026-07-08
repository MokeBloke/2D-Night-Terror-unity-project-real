using UnityEngine;

public class DestroyWhenHidden : MonoBehaviour
{
    public enum HideMode
    {
        Any,                // any of the checks below counts as hidden
        Inactive,           // GameObject.SetActive(false) / not active in hierarchy
        SpriteRendererOff,  // SpriteRenderer.enabled == false
        RendererOff,        // any Renderer.enabled == false
        CanvasGroupHidden,  // CanvasGroup.alpha <= threshold
        Destroyed           // target was destroyed (null)
    }

    [Tooltip("The GameObject to watch. If null the script will do nothing.")]
    public GameObject target;

    [Tooltip("Which condition counts as 'hidden'.")]
    public HideMode mode = HideMode.Any;

    [Tooltip("When using CanvasGroupHidden this threshold determines 'hidden'.")]
    public float canvasAlphaThreshold = 0.01f;

    [Tooltip("How often (seconds) to check. Use >0 to reduce cost; 0 checks every frame.")]
    public float checkInterval = 0.1f;

    float nextCheckTime;

    void Start()
    {
        if (target == null)
            Debug.LogWarning("DestroyWhenHidden: target not assigned on " + name);
        nextCheckTime = Time.time;
    }

    void Update()
    {
        if (target == null && mode != HideMode.Destroyed)
        {
            // If target was destroyed but mode isn't Destroyed, treat destroyed as hidden only for 'Any' or Destroyed modes.
            if (mode != HideMode.Any)
                return;
        }

        if (checkInterval > 0f)
        {
            if (Time.time < nextCheckTime) return;
            nextCheckTime = Time.time + checkInterval;
        }

        if (IsHidden())
        {
            Destroy(gameObject);
        }
    }

    bool IsHidden()
    {
        // Consider destroyed/null target as hidden
        if (target == null)
            return mode == HideMode.Destroyed || mode == HideMode.Any;

        switch (mode)
        {
            case HideMode.Any:
                if (!target.activeInHierarchy) return true;

                var sr = target.GetComponent<SpriteRenderer>();
                if (sr != null && !sr.enabled) return true;

                var rend = target.GetComponent<Renderer>();
                if (rend != null && !rend.enabled) return true;

                var cg = target.GetComponent<CanvasGroup>();
                if (cg != null && cg.alpha <= canvasAlphaThreshold) return true;

                return false;

            case HideMode.Inactive:
                return !target.activeInHierarchy;

            case HideMode.SpriteRendererOff:
                var spr = target.GetComponent<SpriteRenderer>();
                return spr != null && !spr.enabled;

            case HideMode.RendererOff:
                var r = target.GetComponent<Renderer>();
                return r != null && !r.enabled;

            case HideMode.CanvasGroupHidden:
                var canvas = target.GetComponent<CanvasGroup>();
                return canvas != null && canvas.alpha <= canvasAlphaThreshold;

            case HideMode.Destroyed:
                return target == null;

            default:
                return false;
        }
    }

    void OnDrawGizmosSelected()
    {
        if (target == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(target.transform.position, 0.2f);
    }
}