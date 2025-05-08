using System.Collections.Generic;
using UnityEngine;

public class TargetRack : MonoBehaviour
{

    [SerializeField]
    private ShootingTarget targetPrefab;
    [SerializeField]
    private ShootingTarget avoidTargetPrefab;

    [SerializeField]
    private Transform startPoint;
    [SerializeField]
    private Transform endPoint;
    [SerializeField]
    private Vector3 direction = Vector3.right;

    private List<ShootingTarget> shootingTargets = new List<ShootingTarget>();

    private float roundMultiplier;
    private float distanceBetweenTargets;
    private float speed;
    private int loopCount;

    private float trackLength;
    private int currentLoop;
    private bool canMove = false;

    private Vector3 currentStartPoint;
    private Vector3 currentEndPoint;
    private Vector3 currentDirection;

    private void Start()
    {
        trackLength = Vector3.Distance(startPoint.position, endPoint.position);
        TargetType[] targets = { TargetType.Normal, TargetType.Avoid, TargetType.Normal, TargetType.Normal, TargetType.Avoid };
        InitiateRound(targets, 2.5f, 1.0f, 1);
    }

    private void Update()
    {
        if (canMove && currentLoop < loopCount)
        {
            MoveTargets();
        }
    }

    public void InitiateRound(TargetType[] targets, float distanceBetweenTargets,
        float speed, int loopCount, float roundMultiplier = 1.0f)
    {
        this.roundMultiplier = roundMultiplier;
        this.distanceBetweenTargets = distanceBetweenTargets;
        this.speed = speed;
        this.loopCount = loopCount;

        currentLoop = 0;

        CreateTargets(targets);
        currentStartPoint = startPoint.position;
        currentEndPoint = endPoint.position;
        currentDirection = direction;
        canMove = true;
    }

    private bool LastTargetReachedEndPoint()
    {
        if (shootingTargets.Count <= 0) return false;
        ShootingTarget lastTarget = shootingTargets[shootingTargets.Count - 1];
        if (lastTarget == null) return false;

        return Vector3.Distance(lastTarget.transform.position, currentStartPoint) >= trackLength;
    }

    private void CreateTargets(TargetType[] targets)
    {
        shootingTargets.Clear();

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 spawnPoint = startPoint.position - direction * (i * distanceBetweenTargets);
            CreateTarget(targets[i], spawnPoint);
        }
    }    

    private void CreateTarget(TargetType type, Vector3 spawnPoint)
    {
        ShootingTarget target;

        switch(type)
        {
            case TargetType.Normal:
                target = Instantiate(targetPrefab, spawnPoint, startPoint.rotation, transform);
                shootingTargets.Add(target);
                break;
            case TargetType.Avoid:
                target = Instantiate(avoidTargetPrefab, spawnPoint, startPoint.rotation, transform);
                shootingTargets.Add(target);
                break;
            default:
                break;
        }
    }

    private void MoveTargets()
    {
        // Turn Around and Increment Loop Count
        if (LastTargetReachedEndPoint())
        {
            currentLoop++;
            currentDirection *= -1;
            Vector3 temp = currentEndPoint;
            currentEndPoint = currentStartPoint;
            currentStartPoint = temp;
        }

        for (int i = 0; i < shootingTargets.Count; i++)
        {
            shootingTargets[i].transform.Translate(currentDirection * speed * Time.deltaTime);
        }
    }
}
