using NUnit.Framework;
using ShootingGallery.Game;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class StationarySetTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";

    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator VerifyStationarySetMovesIntoCorrectPosition()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Stationary Set 1");
        StationarySet set = gameObject.GetComponent<StationarySet>();
        set.AssignTargets();
        yield return null;
        set.InitiateTargetSet();
        yield return new WaitForSeconds(1.0f);



    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
