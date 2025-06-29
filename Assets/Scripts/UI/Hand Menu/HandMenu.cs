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

        public void OpenHandMenu()
        {
            if (isOpen) return;
            gameObject.SetActive(true);
            isOpen = true;
        }

        public void CloseHandMenu()
        {
            if (!isOpen) return;
            gameObject.SetActive(false);
            isOpen = false;
        }
    }
}
