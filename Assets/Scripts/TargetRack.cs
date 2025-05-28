using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TargetRack : MonoBehaviour, ITargetHitNotify
{

    [SerializeField]
    private ShootingTarget targetPrefab;
    [SerializeField]
    private ShootingTarget decoyPrefab;

    [Tooltip("Initial Number of Targets to Initiate for Object Pool")]
    [SerializeField]
    private int initialTargetPool = 10;
    [Tooltip("Initial Number of Decoys to Initiate for Object Pool")]
    [SerializeField]
    private int initialDecoyPool = 10;

    [SerializeField]
    private Transform targetParent;

    [SerializeField]
    private Transform startPoint; // Under question
    [SerializeField]
    private Transform endPoint; // Under question
    [SerializeField]
    private Vector3 direction = Vector3.right; // Move to Moving Rack

    // Protected?
    private List<ShootingTarget> shootingTargets = new List<ShootingTarget>();

    private int roundMultiplier;
    private float distanceBetweenTargets; // Under question
    private float speed; // Move to Moving Rack
    private int totalLoops; // Move to Moving Rack

    private float trackLength; // Move to Moving Rack
    private int currentLoop; // Move to Moving Rack
    private int leadIndex; // Under Question
    private bool canMove = false; // Move to Moving Rack
    private float totalTrackLength; // Move to Moving Rack
    private int totalTargetsThisRound; // Protected?
    private int totalDecoysThisRound; // Protected?
    private int targetHits; // Protected?

    private ShootingTarget[] targetPool; // Protected?
    private ShootingTarget[] decoyPool; // Protected?

    private Vector3 currentStartPoint; // Move to Moving Rack
    private Vector3 currentEndPoint; // Move to Moving Rack
    private Vector3 currentDirection; // Move to Moving Rack

    public UnityAction<int> onTargetHit; 
    public UnityAction onRoundComplete; // Needs examination

    private void Start()
    {
        trackLength = Vector3.Distance(startPoint.position, endPoint.position); // Move to Moving Rack
        CreatePools();
    }

    private void Update()
    {

        // Moving Rack Logic - start
        if (canMove && currentLoop <= totalLoops)
        {
            MoveTargets();
        }

        if (targetHits >= totalTargetsThisRound)
        {
            currentLoop = totalLoops;
        }
        // Moving Rack Logic - end
    }

    // Make virtual and rename?
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

    // This is specific to Moving Rack
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

    // Logic specific to Moving Rack, make protected?
    public void TerminateRound()
    {
        currentLoop = totalLoops;
    }

    // Can Keep
    public bool IsTargetRackFree()
    {
        return shootingTargets.Count == 0;
    }

    // Specific to Moving Rack
    private bool LeadTargetReachedEndpoint()
    {
        if (shootingTargets.Count <= 0) return false;
        ShootingTarget leadTarget = shootingTargets[leadIndex];
        if (leadTarget == null) return false;

        return Vector3.Distance(leadTarget.transform.position, currentStartPoint) >= totalTrackLength;
    }

    // Can Keep
    private void CreatePools()
    {
        if (targetParent == null)
        {
            targetParent = transform;
        }
        CreatePool(out targetPool, initialTargetPool, TargetType.Normal);
        CreatePool(out decoyPool, initialDecoyPool, TargetType.Decoy);
    }

    // Can Keep
    private void CreatePool(out ShootingTarget[] pool, int poolSize, TargetType type)
    {
        pool = new ShootingTarget[poolSize];

        for (int i = 0; i < poolSize; i++)
        {
            pool[i] = Instantiate(
                type == TargetType.Normal ? targetPrefab : decoyPrefab,
                new Vector3(0.0f, -1000.0f, 0.0f),
                startPoint.rotation, targetParent
                );
            pool[i].TargetHitNotify = this;
            pool[i].TargetType = type;
            pool[i].gameObject.SetActive(false);
        }
    }

    // Can Keep
    private void ExpandPool(ref ShootingTarget[] pool, TargetType type)
    {
        ShootingTarget[] temp = new ShootingTarget[pool.Length + initialDecoyPool];

        for (int i = 0; i < pool.Length; i++)
        {
            temp[i] = pool[i];
            pool[i] = null;
        }

        for (int i = pool.Length - 1; i < temp.Length; i++)
        {
            temp[i] = Instantiate(
                type == TargetType.Normal ? targetPrefab : decoyPrefab,
                new Vector3(0.0f, -1000.0f, 0.0f),
                startPoint.rotation, targetParent
                );
            temp[i].TargetHitNotify = this;
            temp[i].TargetType = type;
            temp[i].gameObject.SetActive(false);
        }

        pool = new ShootingTarget[pool.Length + initialDecoyPool];

        for (int i = 0; i < pool.Length; i++)
        {
            pool[i] = temp[i];
            temp[i] = null;
        }
    }

    // Can Keep
    private void CreateTargets(TargetType[] targets)
    {
        totalTargetsThisRound = 0;
        totalDecoysThisRound = 0;
        targetHits = 0;

        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 spawnPoint = startPoint.position - direction * (i * distanceBetweenTargets);
            AssignTarget(targets[i], spawnPoint);
        }
    }

    // Can Keep
    private void AssignTarget(TargetType type, Vector3 spawnPoint)
    {
        switch (type)
        {
            case TargetType.Normal:
                AllocateTarget(spawnPoint);
                break;
            case TargetType.Decoy:
                AllocateDecoy(spawnPoint);
                break;
        }
    }

    // Can Keep
    private void AllocateTarget(Vector3 spawnPoint)
    {
        totalTargetsThisRound++;

        if (totalTargetsThisRound > targetPool.Length)
        {
            ExpandPool(ref targetPool, TargetType.Normal);
        }

        shootingTargets.Add(targetPool[totalTargetsThisRound - 1]);
        targetPool[totalTargetsThisRound - 1].transform.position = spawnPoint;
        targetPool[totalTargetsThisRound - 1].gameObject.SetActive(true);
    }

    // Can Keep
    private void AllocateDecoy(Vector3 spawnPoint)
    {
        totalDecoysThisRound++;

        if (totalDecoysThisRound > decoyPool.Length)
        {
            ExpandPool(ref decoyPool, TargetType.Decoy);
        }

        shootingTargets.Add(decoyPool[totalDecoysThisRound - 1]);
        decoyPool[totalDecoysThisRound - 1].transform.position = spawnPoint;
        decoyPool[totalDecoysThisRound - 1].gameObject.SetActive(true);
    }

    // Specific to Moving Rack
    private void SetTotalTrackLength()
    {
        Vector3 lastSpawn = startPoint.position -
            direction * (distanceBetweenTargets * (shootingTargets.Count - 1));
        float spawnOffset = Vector3.Distance(startPoint.position, lastSpawn);
        totalTrackLength = trackLength + spawnOffset;
    }

    // Move to Moving Rack

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
                RemoveTargets();
                return;
            }
        }

        for (int i = 0; i < shootingTargets.Count; i++)
        {
            shootingTargets[i].transform.Translate(currentDirection * speed * Time.deltaTime);
        }
    }

    // Can Keep But Rename to DespawnTargets
    private void RemoveTargets()
    {
        canMove = false;

        foreach (ShootingTarget target in shootingTargets)
        {
            target.ResetTarget();
            target.gameObject.SetActive(false);
            target.transform.position = new Vector3(0.0f, -1000.0f, 0.0f);
        }

        shootingTargets.Clear();
        onRoundComplete?.Invoke();
    }

    // Can Keep
    public void OnTargetHit(int points, TargetType type)
    {
        // Increase normal count
        if (type == TargetType.Normal)
        {
            targetHits++;
            points = points * roundMultiplier;
        }

        onTargetHit?.Invoke(points);
    }
}
