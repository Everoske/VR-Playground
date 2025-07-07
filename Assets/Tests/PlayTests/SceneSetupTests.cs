using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class SceneSetupTests
{
    private readonly string scenePath = "Assets/Scenes/SampleScene.unity";

    [OneTimeSetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath);
    }

    [UnityTest]
    public IEnumerator VerifyXROriginAndControllers()
    {
        yield return WaitForSceneLoad();

        var xrOrigin = GameObject.Find("XR Origin (VR)");
        var leftController = GameObject.Find("Left Hand");
        var rightController = GameObject.Find("Right Hand");

        Assert.NotNull(xrOrigin, "XR Origin should exist");
        Assert.NotNull(leftController, "Left controller should exist");
        Assert.NotNull(rightController, "Right controller should exist");
    }

    [UnityTest]
    public IEnumerator VerifySettingsAndData()
    {
        yield return WaitForSceneLoad();
        var settingsManager = GameObject.Find("SettingsManager");
        var dataManager = GameObject.Find("DataManager");
        Assert.NotNull(settingsManager, "Settings manager should exist");
        Assert.NotNull(dataManager, "Data manager should exist");
    }

    [UnityTest]
    public IEnumerator VerifyHandMenu()
    {
        yield return WaitForSceneLoad();
        var handMenuOrigin = GameObject.Find("HandMenuOrigin");
        Assert.NotNull(handMenuOrigin, "Hand menu origin should exist");
    }

    [UnityTest]
    public IEnumerator VerifyGameLogicUI()
    {
        yield return WaitForSceneLoad();
        var scoreCanvas = GameObject.Find("Score Canvas");
        Assert.NotNull(scoreCanvas, "Score canvas should exist");
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
