using UnityEngine;

public class ShootingTarget : MonoBehaviour
{
    [SerializeField]
    private Material activeMaterial;

    [SerializeField]
    private Material inactiveMaterial;

    [SerializeField]
    private int points = 25;

    [SerializeField]
    private float speed = 1f;

    private MeshRenderer meshRenderer;
    private bool isTargetActive = true;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void HitTarget()
    {
        if (!isTargetActive) return;

        meshRenderer.material = inactiveMaterial;
        isTargetActive = false;
    }
}
