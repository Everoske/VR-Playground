using System.Collections;
using UnityEngine;

namespace ShootingGallery.XR.Weapon
{
    public class XRRifleChamber : MonoBehaviour
    {
        [SerializeField]
        private Transform chamberTransform;

        [SerializeField]
        private GameObject[] bullets;

        [SerializeField]
        private GameObject loadBulletMesh;

        [SerializeField]
        private float bulletSpacing = 0.006f;

        [SerializeField]
        private float initialDisplacement = 0.0015f;

        [SerializeField]
        private float displacementDuration = 0.25f;

        private Animator chamberAnimator;

        private int roundsInChamber = 0;

        private float defaultYPosition;
        private float lastYPosition;
        private float targetYPosition;
        private float displacementTimer = 0.0f;

        private void Awake()
        {
            chamberAnimator = GetComponent<Animator>();
        }

        private void Start()
        {
            defaultYPosition = chamberTransform.localPosition.y;
            lastYPosition = defaultYPosition;
            targetYPosition = defaultYPosition;

            StartCoroutine(LoadBulletAfterTime());
        }

        private void Update()
        {
            if (chamberTransform.localPosition.y != targetYPosition)
            {
                displacementTimer += Time.deltaTime;
                float newYPosition = Mathf.Lerp(lastYPosition, targetYPosition, displacementTimer / displacementDuration);
                chamberTransform.localPosition = new Vector3(
                    chamberTransform.localPosition.x,
                    newYPosition,
                    chamberTransform.localPosition.z
                    );

                // TODO: FOR TESTING: REMOVE
                if (newYPosition == targetYPosition && roundsInChamber < bullets.Length)
                {
                    StartCoroutine(LoadBulletAfterTime());
                }
            }
        }

        public void OnLoadAnimationComplete()
        {
            loadBulletMesh.SetActive(false);
            bullets[roundsInChamber].SetActive(true);
            
            if (roundsInChamber == 0)
            {
                lastYPosition = defaultYPosition;
                targetYPosition = chamberTransform.localPosition.y - (initialDisplacement + bulletSpacing);
            }
            else
            {
                lastYPosition = targetYPosition;
                targetYPosition = chamberTransform.localPosition.y - bulletSpacing;
            }

            displacementTimer = 0.0f;
            roundsInChamber++;
        }

        // private void InsertRifleRound(XRRifleRound round)
        private void InsertRifleRound()
        {
            // Detach XRRifleRound
            // Destroy XRRifleRound game object

            // Unhide bullet mesh on load transform
            loadBulletMesh.SetActive(true);

            if (roundsInChamber == 0)
            {
                chamberAnimator.SetTrigger("LoadInitial");
            }
            else if (roundsInChamber % 2 == 0)
            {
                chamberAnimator.SetTrigger("LoadRight");
            }
            else
            {
                chamberAnimator.SetTrigger("LoadLeft");
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            // If (chamber full or bolt not fully open) return;

            // Check if other has a XRRifleRound component attached
            // If it does, call
            // InsertRifleRound(rifleRound);
        }

        // TODO: FOR TESTING: REMOVE
        private IEnumerator LoadBulletAfterTime()
        {
            yield return new WaitForSeconds(5.0f);

            InsertRifleRound();
        }
    }
}
