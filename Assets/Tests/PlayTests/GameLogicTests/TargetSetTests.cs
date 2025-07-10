using NUnit.Framework;
using ShootingGallery.Game;
using ShootingGallery.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TargetSetTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";
    private readonly int totalTargets = 2;
    private readonly int totalDecoys = 2;
    private readonly int setScore = 25;

    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator VerifyDetermineTargetTypeCount()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Set");
        TargetSet targetSet = gameObject.GetComponent<TargetSet>();
        Assert.AreEqual(targetSet.TotalTargets, totalTargets, "Target Set should correctly determine how many targets it has");
    }

    [UnityTest]
    public IEnumerator VerifyDetermineDecoyTypeCount()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Set");
        TargetSet targetSet = gameObject.GetComponent<TargetSet>();
        Assert.AreEqual(totalDecoys, targetSet.TotalDecoys, "Target Set should correctly determine how many decoys it has");
    }

    [UnityTest]
    public IEnumerator VerifyTotalTargetSetScore()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Set");
        TargetSet targetSet = gameObject.GetComponent<TargetSet>();
        int totalScore = totalTargets * setScore;
        Assert.AreEqual(totalScore, targetSet.GetTotalTargetSetScore(), "Target Set should calculate correct total target set score");
    }

    [UnityTest]
    public IEnumerator VerifyTargetSetAssignsTargets()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Set");
        TargetSet targetSet = gameObject.GetComponent<TargetSet>();
        targetSet.AssignTargets();
        yield return null;

        var track = GameObject.Find("Target Set Track");
        int trackCount = track.GetComponentsInChildren<ShootingTarget>(true).Length;
        int expectedCount = totalDecoys + totalTargets;
        Assert.AreEqual(expectedCount, trackCount, "Target Set should allocate the correct number of shooting targets to the target track");
    }

    [UnityTest]
    public IEnumerator VerifyTargetSetDeallocatesShootingTargetsOnStopInReadyState()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Set");
        TargetSet targetSet = gameObject.GetComponent<TargetSet>();
        targetSet.AssignTargets();
        yield return null;
        targetSet.StopTargetSet();
        yield return null;

        var track = GameObject.Find("Target Set Track");
        int trackCount = track.GetComponentsInChildren<ShootingTarget>(true).Length;
        Assert.AreEqual(0, trackCount, "Target Set should deallocate all shooting targets when told to stop in the ready state");
    }

    [UnityTest]
    public IEnumerator VerifyTargetSetAllocatesCorrectNumberNormalTargets()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Set");
        TargetSet targetSet = gameObject.GetComponent<TargetSet>();
        targetSet.AssignTargets();
        yield return null;

        var track = GameObject.Find("Target Set Track");
        int trackCount = 0;
        foreach (ShootingTarget target in track.GetComponentsInChildren<ShootingTarget>(true))
        {
            if (target.TargetType == TargetType.Normal)
            {
                trackCount++;
            }
        }
        Assert.AreEqual(totalTargets, trackCount, "Target Set should allocate correct number of normal targets");
    }

    [UnityTest]
    public IEnumerator VerifyTargetSetAllocatesCorrectNumberDecoyTargets()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Set");
        TargetSet targetSet = gameObject.GetComponent<TargetSet>();
        targetSet.AssignTargets();
        yield return null;

        var track = GameObject.Find("Target Set Track");
        int trackCount = 0;
        foreach (ShootingTarget target in track.GetComponentsInChildren<ShootingTarget>(true))
        {
            if (target.TargetType == TargetType.Decoy)
            {
                trackCount++;
            }
        }
        Assert.AreEqual(totalDecoys, trackCount, "Target Set should allocate correct number of decoy targets");
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
