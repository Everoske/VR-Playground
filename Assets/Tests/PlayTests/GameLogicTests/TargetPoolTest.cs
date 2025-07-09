using NUnit.Framework;
using ShootingGallery.Game;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TargetPoolTest
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";
    private readonly int initialPoolSize = 3;
    private readonly int numberOfChildPools = 2;

    [OneTimeSetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath);
    }

    [UnityTest]
    public IEnumerator VerifyCreateTargetPool()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        yield return new WaitForSeconds(0.25f);

        int targetCount = gameObject.GetComponentsInChildren<ShootingTarget>(true).Length;
        Assert.AreEqual(initialPoolSize * numberOfChildPools, targetCount, "Target Pool should create two pools of the initial pool size");
    }


    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
