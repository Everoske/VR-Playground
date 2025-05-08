using UnityEngine;

[CreateAssetMenu(fileName = "Round Data", menuName = "ScriptableObjects/RoundData")]
public class RoundData : ScriptableObject
{
    public TargetType[] targets;
    public float distanceBetweenTargets;
    public float speed;
    public int loopCount;
    public int roundMultiplier;
}
