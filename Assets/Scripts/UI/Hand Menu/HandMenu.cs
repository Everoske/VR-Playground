using UnityEngine;

namespace ShootingGallery.UI.HandMenu
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HandMenu : MonoBehaviour
    {
        [Header("Menu Containers")]
        [SerializeField]
        private GameObject mainMenu;
        [SerializeField]
        private GameObject settingsMenu;
        [SerializeField]
        private GameObject quitMenu;

        [SerializeField]
        private float fadeTime = 0.5f;

        private bool isOpen;
        private bool shouldFadeIn = false;
        private bool shouldFadeOut = false;
        private float fadeCounter = 0.0f;
        private CanvasGroup alphaController;

        private void Awake()
        {
            alphaController = GetComponent<CanvasGroup>();  
        }

        private void Start()
        {
            isOpen = false;
            gameObject.SetActive(false);
        }

        private void Update()
        {
            HandleFade();
        }

        public void OpenHandMenu()
        {
            if (isOpen) return;
            gameObject.SetActive(true);
            shouldFadeOut = false;
            isOpen = true;

            if (alphaController.alpha > 0.0f)
            {
                fadeCounter = (1 - alphaController.alpha / 1) * fadeTime;
            }
            else
            {
                fadeCounter = 0.0f;
            }

            shouldFadeIn = true;
        }

        public void CloseHandMenu()
        {
            if (!isOpen) return;
            isOpen = false;
            shouldFadeIn = false;

            if (alphaController.alpha < 1.0f)
            {
                fadeCounter = (alphaController.alpha / 1) * fadeTime;
            }
            else
            {
                fadeCounter = 0;
            }

            shouldFadeOut = true;
        }

        private void HandleFade()
        {
            if (!shouldFadeIn && !shouldFadeOut) return;

            fadeCounter += Time.deltaTime;

            if (shouldFadeIn)
            {
                FadeIn();
            }
            else
            {
                FadeOut();
            }
        }

        private void FadeIn()
        {
            if (fadeCounter < fadeTime)
            {
                alphaController.alpha = Mathf.Lerp(0.0f, 1.0f, fadeCounter / fadeTime);
                return;
            }

            alphaController.alpha = 1.0f;
            shouldFadeIn = false;
        }

        private void FadeOut()
        {
            if (fadeCounter < fadeTime)
            {
                alphaController.alpha = Mathf.Lerp(1.0f, 0.0f, fadeCounter / fadeTime);
                return;
            }

            alphaController.alpha = 0.0f;
            shouldFadeOut = false;
            gameObject.SetActive(false);
        }
    }
}
