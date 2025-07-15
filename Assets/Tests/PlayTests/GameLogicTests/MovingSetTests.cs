using NUnit.Framework;
using ShootingGallery.Enums;
using ShootingGallery.Game;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class MovingSetTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";
    private readonly int movingSet2Passes = 2;
    private readonly int movingSet3Passes = 1;

    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator VerifyMovingSetTerminatesOnAllNormalTargetsHit()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Moving Set 1");
        MovingSet set = gameObject.GetComponent<MovingSet>();
        set.AssignTargets();
        yield return null;
        set.InitiateTargetSet();
        yield return new WaitForSeconds(1.0f);

        var track = GameObject.Find("Moving Set 1 Track");
        foreach (ShootingTarget target in track.GetComponentsInChildren<ShootingTarget>())
        {
            if (target.TargetType == TargetType.Normal)
            {
                target.HitTarget();
            }
        }

        yield return new WaitForSeconds(1.0f);

        int numberOfTargets = track.GetComponentsInChildren<ShootingTarget>(true).Length;
        Assert.AreEqual(0, numberOfTargets, "Moving Set should terminate on all normal targets hit");
    }

    [UnityTest]
    public IEnumerator VerifyMovingSetTerminatesAfterAllPassesCompleted()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Moving Set 2");
        MovingSet set = gameObject.GetComponent<MovingSet>();
        set.AssignTargets();
        yield return null;
        set.InitiateTargetSet();
        yield return new WaitForSeconds(1.0f);

        var track = GameObject.Find("Moving Set 2 Track");
        yield return new WaitForSeconds(1.0f);

        int numberOfTargets = track.GetComponentsInChildren<ShootingTarget>(true).Length;
        bool terminatedAfterAllPasses = numberOfTargets == 0 && set.CurrentPass == movingSet2Passes;
        Assert.IsTrue(terminatedAfterAllPasses, "Moving Set should terminate after all successful passes");
    }

    [UnityTest]
    public IEnumerator VerifyMovingSetContinuouslyMakesPassesWhenSetToLoop()
    {
        yield return WaitForSceneLoad();
        var gameObject = GameObject.Find("Moving Set 3");
        MovingSet set = gameObject.GetComponent<MovingSet>();
        set.AssignTargets();
        yield return null;
        set.InitiateTargetSet();
        yield return new WaitForSeconds(1.0f);

        var track = GameObject.Find("Moving Set 3 Track");
        yield return new WaitForSeconds(1.0f);

        int numberOfTargets = track.GetComponentsInChildren<ShootingTarget>(true).Length;
        bool continuesAfterAllPasses = numberOfTargets > 0 && set.CurrentPass > movingSet3Passes;
        Assert.IsTrue(continuesAfterAllPasses, "Moving Set should continuously make passes when set to loop");
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
