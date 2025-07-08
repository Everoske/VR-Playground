using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class ShootingTargetTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";

    [OneTimeSetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath);
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
