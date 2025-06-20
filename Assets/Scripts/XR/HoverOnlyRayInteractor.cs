using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactors.Visuals;
using UnityEngine.XR.Interaction.Toolkit.UI;

public class HoverOnlyRayInteractor : XRRayInteractor
{
    private LineRenderer lineRenderer;
    private XRInteractorLineVisual lineVisual;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
        lineVisual = GetComponent<XRInteractorLineVisual>();
    }

    protected override void Start()
    {
        base.Start();
        lineRenderer.enabled = false;
        lineVisual.enabled = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        uiHoverEntered.AddListener(ShowLineVisuals);
        uiHoverExited.AddListener(HideLineVisuals);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        uiHoverEntered.RemoveListener(ShowLineVisuals);
        uiHoverExited.RemoveListener(HideLineVisuals);
    }

    private void HideLineVisuals(UIHoverEventArgs args)
    {
        lineRenderer.enabled = false;
        lineVisual.enabled = false;
    }

    private void ShowLineVisuals(UIHoverEventArgs args)
    {
        lineRenderer.enabled = true;
        lineVisual.enabled = true;
    }
}
