using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetRack : MonoBehaviour, ITargetHitNotify
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

    private int roundMultiplier;
    private float distanceBetweenTargets;
    private float speed;
    private int totalLoops;

    private float trackLength;
    private int currentLoop;
    private int leadIndex;
    private bool canMove = false;
    private float totalTrackLength;
    private int totalNormal;
    private int normalHit;

    private Vector3 currentStartPoint;
    private Vector3 currentEndPoint;
    private Vector3 currentDirection;

    public UnityAction<int> onTargetHit;
    public UnityAction onRoundComplete;

    private void Start()
    {
        trackLength = Vector3.Distance(startPoint.position, endPoint.position);
    }

    private void Update()
    {
        if (canMove && currentLoop <= totalLoops)
        {
            MoveTargets();
        }

        if (normalHit >= totalNormal)
        {
            currentLoop = totalLoops;
        }
    }

    public void InitiateRound(TargetType[] targets, float distanceBetweenTargets,
        float speed, int loopCount, int roundMultiplier = 1)
    {
        this.roundMultiplier = roundMultiplier;
        this.distanceBetweenTargets = distanceBetweenTargets;
        this.speed = speed;
        this.totalLoops = loopCount;

        currentLoop = 0;
        
        CreateTargets(targets);
        SetTotalTrackLength();
        currentStartPoint = startPoint.position;
        currentEndPoint = endPoint.position;
        currentDirection = direction;
        canMove = true;
        leadIndex = 0;
    }

    public void InitiateRound(RoundData round)
    {
        InitiateRound(
            round.targets,
            round.distanceBetweenTargets,
            round.speed,
            round.loopCount,
            round.roundMultiplier
            );
    }

    public bool RoundFinished()
    {
        return shootingTargets.Count == 0;
    }

    private bool LeadTargetReachedEndpoint()
    {
        if (shootingTargets.Count <= 0) return false;
        ShootingTarget leadTarget = shootingTargets[leadIndex];
        if (leadTarget == null) return false;

        return Vector3.Distance(leadTarget.transform.position, currentStartPoint) >= totalTrackLength;
    }

    private void CreateTargets(TargetType[] targets)
    {
        shootingTargets = new List<ShootingTarget>();
        totalNormal = 0;
        normalHit = 0;

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
                target.Points *= roundMultiplier;
                target.TargetHitNotify = this;
                target.TargetType = type;
                totalNormal++;
                shootingTargets.Add(target);
                break;
            case TargetType.Avoid:
                target = Instantiate(avoidTargetPrefab, spawnPoint, startPoint.rotation, transform);
                target.TargetHitNotify = this;
                target.TargetType = type;
                shootingTargets.Add(target);
                break;
            default:
                break;
        }
    }

    private void SetTotalTrackLength()
    {
        Vector3 lastSpawn = startPoint.position -
            direction * (distanceBetweenTargets * (shootingTargets.Count - 1));
        float spawnOffset = Vector3.Distance(startPoint.position, lastSpawn);
        totalTrackLength = trackLength + spawnOffset;
        Debug.Log($"Spawn Offset: {spawnOffset}");
        Debug.Log($"Track Length: {trackLength}");
        Debug.Log($"Total Track Length: {totalTrackLength}");
    }

    private void MoveTargets()
    {
        if (LeadTargetReachedEndpoint())
        {
            currentLoop++;
            currentDirection *= -1;
            Vector3 temp = currentEndPoint;
            currentEndPoint = currentStartPoint;
            currentStartPoint = temp;
            leadIndex = leadIndex > 0 ? 0 : shootingTargets.Count - 1;

            if (currentLoop >= totalLoops)
            {
                DestroyTargets();
                return;
            }
        }

        for (int i = 0; i < shootingTargets.Count; i++)
        {
            shootingTargets[i].transform.Translate(currentDirection * speed * Time.deltaTime);
        }
    }

    private void DestroyTargets()
    {
        canMove = false;

        for (int i = 0; i < shootingTargets.Count; i++)
        {
            Destroy(shootingTargets[i].gameObject, 2.0f);
        }

        shootingTargets.Clear();
        onRoundComplete?.Invoke();
    }

    public void OnTargetHit(int points, TargetType type)
    {
        // Increase normal count
        if (type == TargetType.Normal)
        {
            normalHit++;
        }

        onTargetHit?.Invoke(points);
    }
}
