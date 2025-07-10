using NUnit.Framework;
using ShootingGallery.Game;
using ShootingGallery.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class StationarySetTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";
    private readonly int totalNormalSet1 = 1;
    private readonly int totalDecoySet1 = 1;
    private readonly int totalNormalSet2 = 1;
    private readonly int totalDecoySet2 = 2;
    private readonly float displacement = 5.0f;

    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator VerifyStationarySetMovesIntoCorrectPositionWhenEven()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Stationary Set 1");
        StationarySet set = gameObject.GetComponent<StationarySet>();
        set.AssignTargets();
        yield return null;
        set.InitiateTargetSet();
        yield return new WaitForSeconds(1.0f);

        var track = GameObject.Find("Stationary Set 1 Track");

        var rackObject = GameObject.Find("Target Set Rack");
        TargetRack rack = rackObject.GetComponent<TargetRack>();
        float leadOffset = ((totalNormalSet1 + totalDecoySet1) / 2 - 1) * displacement + displacement / 2;

        Vector3 expectedPosition = rack.GetCenterPoint() + leadOffset * rack.GetRackDirection();

        Assert.AreEqual(expectedPosition, track.transform.position, "Stationary Set with even number of targets should move into correct position when active");
    }

    [UnityTest]
    public IEnumerator VerifyStationarySetMovesIntoCorrectPositionWhenOdd()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Stationary Set 2");
        StationarySet set = gameObject.GetComponent<StationarySet>();
        set.AssignTargets();
        yield return null;
        set.InitiateTargetSet();
        yield return new WaitForSeconds(1.0f);

        var track = GameObject.Find("Stationary Set 2 Track");

        var rackObject = GameObject.Find("Target Set Rack");
        TargetRack rack = rackObject.GetComponent<TargetRack>();
        float leadOffset = ((totalNormalSet2 + totalDecoySet2) - 1) / 2 * displacement;

        Vector3 expectedPosition = rack.GetCenterPoint() + leadOffset * rack.GetRackDirection();

        Assert.AreEqual(expectedPosition, track.transform.position, "Stationary Set with odd number of targets should move into correct position when active");
    }

    [UnityTest]
    public IEnumerator VerifyStationarySetTerminatesOnTimerEnd()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Stationary Set 3");
        StationarySet set = gameObject.GetComponent<StationarySet>();
        set.AssignTargets();
        yield return null;
        set.InitiateTargetSet();
        yield return new WaitForSeconds(2.0f);

        var track = GameObject.Find("Stationary Set 3 Track");
        int numberOfTargets = track.GetComponentsInChildren<ShootingTarget>(true).Length;
        Assert.AreEqual(0, numberOfTargets, "Stationary Set should terminate on timer completion");
    }

    [UnityTest]
    public IEnumerator VerifyStationarySetTerminatesOnAllNormalTargetsHit()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Stationary Set 1");
        StationarySet set = gameObject.GetComponent<StationarySet>();
        set.AssignTargets();
        yield return null;
        set.InitiateTargetSet();
        yield return new WaitForSeconds(1.0f);

        var track = GameObject.Find("Stationary Set 1 Track");
        foreach (ShootingTarget target in track.GetComponentsInChildren<ShootingTarget>())
        {
            if (target.TargetType == TargetType.Normal)
            {
                target.HitTarget();
            }
        }

        yield return new WaitForSeconds(1.0f);

        int numberOfTargets = track.GetComponentsInChildren<ShootingTarget>(true).Length;
        Assert.AreEqual(0, numberOfTargets, "Stationary Set should terminate on all normal targets hit");
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
