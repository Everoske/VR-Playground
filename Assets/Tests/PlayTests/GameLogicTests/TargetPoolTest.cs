using NUnit.Framework;
using ShootingGallery.Game;
using ShootingGallery.Enums;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TargetPoolTest
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";
    private readonly int initialPoolSize = 3;
    private readonly int numberOfChildPools = 2;

    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator VerifyCreateTargetPool()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        yield return null;

        int targetCount = gameObject.GetComponentsInChildren<ShootingTarget>(true).Length;
        Assert.AreEqual(initialPoolSize * numberOfChildPools, targetCount, "Target Pool should create two pools of the initial pool size");
    }

    [UnityTest]
    public IEnumerator VerifyAllocateTargetIncrementsTargetsAllocated()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        yield return null;

        targetPool.AllocateTarget(null);
        Assert.AreEqual(1, targetPool.AllocatedTargets, "Target Pool should increment targets allocated on allocate target");
    }

    [UnityTest]
    public IEnumerator VerifyAllocateDecoyIncrementsDecoysAllocated()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        yield return null;

        targetPool.AllocateDecoy(null);
        Assert.AreEqual(1, targetPool.AllocatedDecoys, "Target Pool should increment decoys allocated on allocate decoy");
    }

    [UnityTest]
    public IEnumerator VerifyAllocateTargetNotNull()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        yield return null;

        ShootingTarget target = targetPool.AllocateTarget(null);
        Assert.NotNull(target, "Target Pool should return non-null target on allocate target");
    }

    [UnityTest]
    public IEnumerator VerifyAllocateDecoyNotNull()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        yield return null;

        ShootingTarget decoy = targetPool.AllocateDecoy(null);
        Assert.NotNull(decoy, "Target Pool should return non-null decoy on allocate decoy");
    }

    [UnityTest]
    public IEnumerator VerifyPoolSizeAdjustsWhenTooManyTargetsAllocated()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        yield return null;

        for (int i = 0; i < 4; i++)
        {
            targetPool.AllocateTarget(null);
        }

        int targetCount = gameObject.GetComponentsInChildren<ShootingTarget>(true).Length;
        int intendedLength = initialPoolSize * numberOfChildPools + initialPoolSize;
        Assert.AreEqual(targetCount, intendedLength, "Target Pool should increase the target pool's length by the initial pool size when surpassed");
    }

    [UnityTest]
    public IEnumerator VerifyPoolSizeAdjustsWhenTooManyDecoysAllocated()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        yield return null;

        for (int i = 0; i < 4; i++)
        {
            targetPool.AllocateDecoy(null);
        }

        int targetCount = gameObject.GetComponentsInChildren<ShootingTarget>(true).Length;
        int intendedLength = initialPoolSize * numberOfChildPools + initialPoolSize;
        Assert.AreEqual(targetCount, intendedLength, "Target Pool should increase the decoy pool's length by the initial pool size when surpassed");
    }

    [UnityTest]
    public IEnumerator VerifyAllocateTargetNotNullOnSizeAdjustment()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        yield return null;

        for (int i = 0; i < 3; i++)
        {
            targetPool.AllocateTarget(null);
        }

        ShootingTarget target = targetPool.AllocateTarget(null);
        Assert.IsNotNull(target, "Target Pool should return non-null target after pool size adjustment");
    }

    [UnityTest]
    public IEnumerator VerifyAllocateDecoyNotNullOnSizeAdjustment()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        yield return null;

        for (int i = 0; i < 3; i++)
        {
            targetPool.AllocateDecoy(null);
        }

        ShootingTarget decoy = targetPool.AllocateDecoy(null);
        Assert.IsNotNull(decoy, "Target Pool should return non-null decoy after pool size adjustment");
    }

    [UnityTest]
    public IEnumerator VerifyDeallocateTargetDecrementsTargetsAllocated()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        yield return null;

        ShootingTarget target = targetPool.AllocateTarget(null);
        yield return null;
        targetPool.DeallocateShootingTarget(target);
        Assert.AreEqual(0, targetPool.AllocatedTargets, "Target Pool should decrement targets allocated on deallocate shooting target");
    }

    [UnityTest]
    public IEnumerator VerifyDeallocateTargetDecrementsDecoysAllocated()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        yield return null;

        ShootingTarget target = targetPool.AllocateDecoy(null);
        yield return null;
        targetPool.DeallocateShootingTarget(target);
        Assert.AreEqual(0, targetPool.AllocatedDecoys, "Target Pool should decrement decoys allocated on deallocate shooting target");
    }

    [UnityTest]
    public IEnumerator VerifyTargetPoolDeallocatesItsOwnTargets()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Target Pool");
        TargetPool targetPool = gameObject.GetComponent<TargetPool>();
        ShootingTarget newTarget = new ShootingTarget();
        newTarget.TargetType = TargetType.Normal;
        yield return null;

        targetPool.AllocateTarget(null);
        targetPool.DeallocateShootingTarget(newTarget);
        Assert.AreEqual(1, targetPool.AllocatedTargets, "Target Pool should not deallocate targets not belonging to it");
    }
    

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
