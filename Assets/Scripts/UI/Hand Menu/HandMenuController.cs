using UnityEngine;

namespace ShootingGallery.UI.HandMenu
{
    /// <summary>
    /// Represents a Hand Menu the player can use to change game settings or quit the game.
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class HandMenuController : MonoBehaviour
    {
        [Header("Menu Containers")]
        [SerializeField]
        private GameObject mainMenu;
        [SerializeField]
        private GameObject settingsMenu;
        [SerializeField]
        private GameObject soundMenu;
        [SerializeField]
        private GameObject gameplayMenu;
        [SerializeField]
        private GameObject videoMenu;
        [SerializeField]
        private GameObject quitMenu;

        [SerializeField]
        private float fadeTime = 0.5f;

        private bool isOpen;
        private bool shouldFadeIn = false;
        private bool shouldFadeOut = false;
        private float fadeCounter = 0.0f;
        private CanvasGroup alphaController;

        private GameObject activeMenu;

        private void Awake()
        {
            alphaController = GetComponent<CanvasGroup>();  
        }

        private void Start()
        {
            isOpen = false;
            SetInitialState();
        }

        private void Update()
        {
            HandleFade();
        }

        /// <summary>
        /// Activate the hand menu and fade in smoothly.
        /// </summary>
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

        /// <summary>
        /// Initiate a smooth fade out process to close hand menu.
        /// </summary>
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
        public void OpenMainMenu() => ChangeMenu(mainMenu);
        public void OpenSettingsMenu() => ChangeMenu(settingsMenu);
        public void OpenSoundMenu() => ChangeMenu(soundMenu);
        public void OpenGamePlayMenu() => ChangeMenu(gameplayMenu);
        public void OpenVideoMenu() => ChangeMenu(videoMenu);
        public void OpenQuitMenu() => ChangeMenu(quitMenu);

        public void QuitGame()
        {
            Application.Quit();
        }

        /// <summary>
        /// Handle fade in and fade out effects.
        /// </summary>
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

        /// <summary>
        /// Smoothly fade in the hand menu's canvas.
        /// </summary>
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

        /// <summary>
        /// Smoothly fade out the hand menu's canvas and deactivate hand menu on completion.
        /// </summary>
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
            OpenMainMenu();
        }

        /// <summary>
        /// Open the target menu and close the current menu.
        /// </summary>
        /// <param name="target">Menu to open.</param>
        private void ChangeMenu(GameObject target)
        {
            activeMenu.SetActive(false);
            activeMenu = target;
            activeMenu.SetActive(true);
        }

        /// <summary>
        /// Set initial state of hand menu on game start.
        /// </summary>
        private void SetInitialState()
        {
            gameObject.SetActive(false);

            settingsMenu.SetActive(false);
            soundMenu.SetActive(false);
            gameplayMenu.SetActive(false);
            videoMenu.SetActive(false);
            quitMenu.SetActive(false);

            mainMenu.SetActive(true);
            activeMenu = mainMenu;
        }
    }
}
