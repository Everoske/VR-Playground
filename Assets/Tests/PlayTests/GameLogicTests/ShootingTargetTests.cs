using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

using ShootingGallery.Game;

public class ShootingTargetTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";

    [OneTimeSetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath);
    }

    [UnityTest]
    public IEnumerator VerifyShootingTargetMaterialChange()
    {
        yield return WaitForSceneLoad();

        var gameObject = GameObject.Find("Shooting Target");
        ShootingTarget target = gameObject.GetComponent<ShootingTarget>();
        MeshRenderer targetMesh = target.GetComponentInChildren<MeshRenderer>();
        Material activeMaterial = targetMesh.material;
        target.HitTarget();
        Material inactiveMaterial = targetMesh.material;

        Assert.AreNotEqual(activeMaterial, inactiveMaterial);
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
