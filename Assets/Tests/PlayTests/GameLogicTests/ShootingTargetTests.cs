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
    public IEnumerator VerifyShootingTargetMaterialChangeOnHit()
    {
        yield return WaitForSceneLoad();

        var gameObject = GameObject.Find("Shooting Target");
        ShootingTarget target = gameObject.GetComponent<ShootingTarget>();
        MeshRenderer meshRenderer = target.GetComponentInChildren<MeshRenderer>();
        Material activeMaterial = meshRenderer.material;
        target.HitTarget();
        Material inactiveMaterial = meshRenderer.material;

        Assert.AreNotEqual(activeMaterial, inactiveMaterial, "Shooting Target should change material on hit");
    }

    [UnityTest]
    public IEnumerator VerifyShootingTargetRotationOnHit()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Shooting Target");
        ShootingTarget target = gameObject.GetComponent<ShootingTarget>();
        target.HitTarget();

        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(270.0f, target.transform.eulerAngles.x, "Shooting Target should rotate on hit");
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
