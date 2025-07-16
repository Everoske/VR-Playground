using NUnit.Framework;
using ShootingGallery.Game;
using ShootingGallery.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GalleryRoundTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";
    private readonly int galleryRound1Score = 50;
    private readonly int galleryRound1TotalSets = 2;
    
    private bool roundReleased;
    private int roundSetsComplete;

    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
        roundReleased = false;
        roundSetsComplete = 0;
    }

    [UnityTest]
    public IEnumerator VerifyGetGalleryRoundScoreReturnsCorrectScore()
    {
        yield return WaitForSceneLoad();
        var roundObject = GameObject.Find("Gallery Round 1");
        GalleryRound round = roundObject.GetComponent<GalleryRound>();
        Assert.AreEqual(galleryRound1Score, round.GetTotalGalleryRoundScore(), "Get gallery round score should return correct score");
    }

    [UnityTest]
    public IEnumerator VerifyGalleryRoundClosesWhenInitiateStopCalled()
    {
        yield return WaitForSceneLoad();
        var roundObject = GameObject.Find("Gallery Round 1");
        GalleryRound round = roundObject.GetComponent<GalleryRound>();
        round.onRoundReleased += GalleryRoundReleased;
        round.AssignRoundSets();
        yield return null;
        round.InitiateGalleryRound();
        yield return null;
        round.InitiateStopRound();
        yield return new WaitForSeconds(1.0f);

        round.onRoundReleased -= GalleryRoundReleased;
        Assert.IsTrue(roundReleased, "Gallery round should close when initiate stop called");
    }

    [UnityTest]
    public IEnumerator VerifyGalleryRoundClosesWhenNoRoundSets()
    {
        yield return WaitForSceneLoad();
        var roundObject = GameObject.Find("Gallery Round 2");
        GalleryRound round = roundObject.GetComponent<GalleryRound>();
        round.onRoundReleased += GalleryRoundReleased;
        round.AssignRoundSets();
        yield return null;
        round.InitiateGalleryRound();
        yield return null;
        round.onRoundReleased -= GalleryRoundReleased;

        Assert.IsTrue(roundReleased, "Gallery round should automatically close when no round sets provided");
    }

    [UnityTest]
    public IEnumerator VerifyGalleryRoundClosesWhenAllRoundSetsComplete()
    {
        yield return WaitForSceneLoad();
        var roundObject = GameObject.Find("Gallery Round 3");
        GalleryRound round = roundObject.GetComponent<GalleryRound>();
        round.onRoundReleased += GalleryRoundReleased;
        foreach (RoundSet set in roundObject.GetComponentsInChildren<RoundSet>())
        {
            set.onRoundSetReleased += RoundSetComplete;
        }
        round.AssignRoundSets();
        yield return null;
        round.InitiateGalleryRound();

        while (roundSetsComplete < galleryRound1TotalSets)
        {
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);
        round.onRoundReleased -= GalleryRoundReleased;
        foreach (RoundSet set in roundObject.GetComponentsInChildren<RoundSet>())
        {
            set.onRoundSetReleased -= RoundSetComplete;
        }

        Assert.IsTrue(roundReleased, "Gallery round should stop after all round sets complete");
    }

    private void RoundSetComplete()
    {
        roundSetsComplete++;
    }

    private void GalleryRoundReleased()
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
