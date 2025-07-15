using NUnit.Framework;
using ShootingGallery.Game;
using ShootingGallery.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class RoundSetTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";
    private readonly int roundSet2Score = 25;

    private bool roundReleased;

    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
        roundReleased = false;
    }

    [UnityTest]
    public IEnumerator VerifyRoundSetClosesOnRoundSetTimerEnd()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Round Set 1");
        RoundSet set = gameObject.GetComponent<RoundSet>();
        set.onRoundSetReleased += RoundSetReleased;
        set.AssignTargetSets();
        yield return null;
        set.InitiateRoundSet();

        yield return new WaitForSeconds(1.0f);
        set.onRoundSetReleased -= RoundSetReleased;
        Assert.IsTrue(roundReleased, "Round Set should close on round set timer end");
    }

    [UnityTest]
    public IEnumerator VerifyGetTotalRoundSetScoreReturnsCorrectScore()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Round Set 2");
        RoundSet set = gameObject.GetComponent<RoundSet>();
        Assert.AreEqual(roundSet2Score, set.GetTotalRoundSetScore(), "Get total round set score should return the correct score");
    }

    [UnityTest]
    public IEnumerator VerifyRoundSetClosesOnAllTargetSetsComplete()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Round Set 2");
        RoundSet set = gameObject.GetComponent<RoundSet>();
        set.onRoundSetReleased += RoundSetReleased;
        set.AssignTargetSets();
        yield return null;
        set.InitiateRoundSet();
        yield return null;

        var rs2TrackObject = GameObject.Find("RS2 Moving Set Track");
        foreach (ShootingTarget target in rs2TrackObject.GetComponentsInChildren<ShootingTarget>())
        {
            if (target.TargetType == TargetType.Normal)
            {
                target.HitTarget();
            }
        }

        yield return new WaitForSeconds(1.0f);
        set.onRoundSetReleased -= RoundSetReleased;
        Assert.IsTrue(roundReleased, "Round Set should close on all target sets complete");
    }

    [UnityTest]
    public IEnumerator VerifyRoundSetClosesOnInitiateStopRoundSet()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Round Set 2");
        RoundSet set = gameObject.GetComponent<RoundSet>();
        set.onRoundSetReleased += RoundSetReleased;
        set.AssignTargetSets();
        yield return null;
        set.InitiateRoundSet();
        yield return null;
        set.InitiateStopRoundSet();

        yield return new WaitForSeconds(1.0f);
        set.onRoundSetReleased -= RoundSetReleased;
        Assert.IsTrue(roundReleased, "Round Set should close on initiate stop round set");
    }

    [UnityTest]
    public IEnumerator VerifyRoundSetClosesWhenNoTargetSets()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Round Set 3");
        RoundSet set = gameObject.GetComponent<RoundSet>();
        set.onRoundSetReleased += RoundSetReleased;
        set.AssignTargetSets();
        yield return null;
        set.InitiateRoundSet();

        yield return null;
        set.onRoundSetReleased -= RoundSetReleased;
        Assert.IsTrue(roundReleased, "Round Set should close when no target sets present");
    }

    private void RoundSetReleased()
    {
        roundReleased = true;
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
