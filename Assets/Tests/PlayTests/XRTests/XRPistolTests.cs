using NUnit.Framework;
using ShootingGallery.XR.Weapon;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class XRPistolTests
{
    private readonly string scenePath = "Assets/Scenes/TestingScene.unity";

    [SetUp]
    public void LoadScene()
    {
        SceneManager.LoadScene(scenePath, LoadSceneMode.Single);
    }

    [UnityTest]
    public IEnumerator VerifyShootingPistolReducesAmmoInMagazineWell()
    {
        yield return WaitForSceneLoad();
        var pistolObject = GameObject.Find("XR_Pistol 1");
        XRPistol pistol = pistolObject.GetComponent<XRPistol>();
        yield return null;
        pistol.PullTrigger();
        yield return new WaitForSeconds(2.0f);
        var magWellObject = GameObject.Find("Mag Well 1");
        XRMagazineWell magWell = magWellObject.GetComponent<XRMagazineWell>();
        Assert.AreEqual(0, magWell.GetAmmoInMag(), "XR Pistol should reduce ammo in magazine when fired");
    }

    [UnityTest]
    public IEnumerator VerifyPistolSlideMovesToSlideStopPositionWhenFiredUntilEmpty()
    {
        yield return WaitForSceneLoad();
        var pistolObject = GameObject.Find("XR_Pistol 2");
        XRPistol pistol = pistolObject.GetComponent<XRPistol>();
        yield return null;
        pistol.PullTrigger();
        yield return new WaitForSeconds(2.0f);

        var slideStopTransform = GameObject.Find("SlideStopPoint 2");
        var slider = GameObject.Find("XR Slider 2");
        bool sliderAtStopPoint = slideStopTransform.transform.position == slider.transform.position;
        Assert.IsTrue(sliderAtStopPoint, "XR Slider should move to slide stop position when XR Pistol fired until empty");
    }


    private IEnumerator WaitForSceneLoad()
    {
        while (!SceneManager.GetActiveScene().isLoaded)
        {
            yield return null;
        }
    }
}
