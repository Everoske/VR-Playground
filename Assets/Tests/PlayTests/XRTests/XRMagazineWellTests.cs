using NUnit.Framework;
using ShootingGallery.XR.Weapon;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class XRMagazineWellTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";

    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator VerifyConsumeRoundReducesAmmoByOne()
    {
        yield return WaitForSceneLoad();
        var magWellObject = GameObject.Find("Mag Well 1");
        XRMagazineWell magWell = magWellObject.GetComponent<XRMagazineWell>();
        yield return null;
        magWell.ConsumeRound();
        Assert.AreEqual(0, magWell.GetAmmoInMag(), "Consume round should reduce the ammo count in magazine by 1");
    }

    [UnityTest]
    public IEnumerator VerifyConsumeRoundDoesNotReduceAmmoBelowZero()
    {
        yield return WaitForSceneLoad();
        var magWellObject = GameObject.Find("Mag Well 1");
        XRMagazineWell magWell = magWellObject.GetComponent<XRMagazineWell>();
        yield return null;
        magWell.ConsumeRound();
        magWell.ConsumeRound();
        Assert.AreEqual(0, magWell.GetAmmoInMag(), "Consume round should not reduce ammo count in magazine below 0");
    }

    [UnityTest]
    public IEnumerator VerifyGetAmmoInMagReturnsAmmoInMagazine()
    {
        yield return WaitForSceneLoad();
        var magWellObject = GameObject.Find("Mag Well 1");
        XRMagazineWell magWell = magWellObject.GetComponent<XRMagazineWell>();
        var magObject = GameObject.Find("XR_Magazine 1");
        XRMagazine mag = magObject.GetComponent<XRMagazine>();
        yield return null;
        Assert.AreEqual(mag.CurrentAmmo, magWell.GetAmmoInMag(), "Get ammo in mag should return the ammo count in the inserted magazine");
    }

    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
