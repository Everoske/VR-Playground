using NUnit.Framework;
using ShootingGallery.Game;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class GameSetTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";
    private readonly int gameSet1HighScore = 150;

    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator VerifyCalculateHighestScoreCalculatesCorrectScore()
    {
        yield return WaitForSceneLoad();
        var gamesetObject = GameObject.Find("Game Set 1");
        GameSet gameSet = gamesetObject.GetComponent<GameSet>();
        yield return null;
        Assert.AreEqual(gameSet1HighScore, gameSet.HighestPossibleScore, "Game set should calculate the corect highest score");

    }

    [UnityTest]
    public IEnumerator VerifyScoreTrackerResetToZeroOnGameSetStart()
    {
        yield return WaitForSceneLoad();
        var gamesetObject = GameObject.Find("Game Set 1");
        GameSet gameSet = gamesetObject.GetComponent<GameSet>();
        yield return null;
        ScoreLocator.GetScoreTracker().AddToScore(1000);
        gameSet.StartGameSet();
        yield return null;
        Assert.AreEqual(0, ScoreLocator.GetScoreTracker().CurrentScore, "Game Set should set the current score in the score tracker to 0 on start");
    }

    [UnityTest]
    public IEnumerator VerifyAccuracyTrackerResetToZeroOnGameSetStart()
    {
        yield return WaitForSceneLoad();
        var gamesetObject = GameObject.Find("Game Set 1");
        GameSet gameSet = gamesetObject.GetComponent<GameSet>();
        yield return null;
        AccuracyLocator.GetAccuracyTracker().IncrementShotsFired();
        AccuracyLocator.GetAccuracyTracker().IncrementTargetsHit();
        gameSet.StartGameSet();
        yield return null;
        Assert.AreEqual(0, AccuracyLocator.GetAccuracyTracker().GetAccuracy(), "Game Set should set the current accuracy in the accuracy tracker to 0 on start");
    }

    [UnityTest]
    public IEnumerator VerifyAccuracyBonusNotGreaterThanZeroOnGameEndEarly()
    {
        yield return WaitForSceneLoad();
        var gamesetObject = GameObject.Find("Game Set 1");
        GameSet gameSet = gamesetObject.GetComponent<GameSet>();
        gameSet.StartGameSet();
        yield return null;
        AccuracyLocator.GetAccuracyTracker().IncrementShotsFired();
        AccuracyLocator.GetAccuracyTracker().IncrementTargetsHit();
        gameSet.InitiateStopGameSet();
        yield return new WaitForSeconds(1.0f);
        Assert.AreEqual(0, ScoreLocator.GetScoreTracker().CurrentScore, "Accuracy bonus should be 0 when game set finished early");
    }

    [UnityTest]
    public IEnumerator VerifyGameSetClosesAfterInitiateStopCalled()
    {
        yield return WaitForSceneLoad();
        var gamesetObject = GameObject.Find("Game Set 1");
        GameSet gameSet = gamesetObject.GetComponent<GameSet>();
        gameSet.StartGameSet();
        yield return null;
        gameSet.InitiateStopGameSet();
        yield return new WaitForSeconds(1.0f);
        Assert.IsFalse(gameSet.GameSetActive, "Game Set should close when initiate stop game set called");
    }

    [UnityTest]
    public IEnumerator VerifyGameSetClosesAfterAllRoundsComplete()
    {
        yield return WaitForSceneLoad();
        var gamesetObject = GameObject.Find("Game Set 2");
        GameSet gameSet = gamesetObject.GetComponent<GameSet>();
        gameSet.StartGameSet();
        yield return new WaitForSeconds(2.5f);
        Assert.IsFalse(gameSet.GameSetActive, "Game Set should close when all rounds complete");
    }

    [UnityTest]
    public IEnumerator VerifyGameSetClosesWithNoRounds()
    {
        yield return WaitForSceneLoad();

        var gamesetObject = GameObject.Find("Game Set 3");
        GameSet gameSet = gamesetObject.GetComponent<GameSet>();
        gameSet.StartGameSet();
        yield return null;
        Assert.IsFalse(gameSet.GameSetActive, "Game Set should automatically close when it has no rounds");
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
