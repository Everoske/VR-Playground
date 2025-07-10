using NUnit.Framework;
using ShootingGallery.Game;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TargetSetTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";
    private readonly int totalTargets = 2;
    private readonly int totalDecoys = 2;

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
        Assert.AreEqual(targetSet.TotalDecoys, totalDecoys, "Target Set should correctly determine how many decoys it has");
    }


    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
