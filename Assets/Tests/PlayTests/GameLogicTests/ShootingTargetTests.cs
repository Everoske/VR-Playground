using NUnit.Framework;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

using ShootingGallery.Game;

public class ShootingTargetTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";
    private readonly float hitRotation = 270.0f;
    private readonly float defaultRotation = 0.0f;

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

        Assert.AreNotEqual(activeMaterial.name, inactiveMaterial.name, "Shooting Target should change material on hit");
    }

    [UnityTest]
    public IEnumerator VerifyShootingTargetRotationOnHit()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Shooting Target");
        ShootingTarget target = gameObject.GetComponent<ShootingTarget>();
        target.HitTarget();

        yield return new WaitForSeconds(0.5f);
        Assert.AreEqual(hitRotation, target.transform.eulerAngles.x, "Shooting Target should rotate on hit");
    }

    [UnityTest]
    public IEnumerator VerifyShootingTargetResetMaterial()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Shooting Target");
        ShootingTarget target = gameObject.GetComponent<ShootingTarget>();
        MeshRenderer meshRenderer = target.GetComponentInChildren<MeshRenderer>();
        Material activeMaterial = meshRenderer.material;
        target.HitTarget();

        yield return null;
        target.ResetTarget();
        Material resetMaterial = meshRenderer.material;
        Assert.AreEqual(activeMaterial.name, resetMaterial.name, "Should reset material on reset target");
    }

    [UnityTest]
    public IEnumerator VerifyShootingTargetRotationReset()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Shooting Target");
        ShootingTarget target = gameObject.GetComponent<ShootingTarget>();
        target.HitTarget();

        yield return null;
        target.ResetTarget();
        Assert.AreEqual(defaultRotation, target.transform.eulerAngles.x, "Should reset rotation on reset target");
    }

    [TearDown]
    public void ResetShootingTarget()
    {
        var gameObject = GameObject.Find("Shooting Target");
        ShootingTarget target = gameObject.GetComponent<ShootingTarget>();
        target.ResetTarget();
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
