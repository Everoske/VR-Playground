using UnityEngine;

public class ShootingTarget : MonoBehaviour
{
    [SerializeField]
    private Material activeMaterial;

    [SerializeField]
    private Material inactiveMaterial;

    [SerializeField]
    private int points = 25;

    private MeshRenderer meshRenderer;
    private bool isTargetActive = true;

    public int Points
    {
        get => points;
        set => points = value;
    }

    private void Awake()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
    }

    public void HitTarget()
    {
        if (!isTargetActive) return;

        meshRenderer.material = inactiveMaterial;
        isTargetActive = false;

        transform.rotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
    }
}
