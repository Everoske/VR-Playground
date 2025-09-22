using ShootingGallery.UI;
using System.Collections;
using UnityEngine;

namespace ShootingGallery.Game
{
    // Will allow for selecting, starting, and stopping different game sets
    public class GameSetManager : MonoBehaviour
    {
        [SerializeField]
        private GameSet[] gameSets;
        [SerializeField]
        private GunDrawer gunDrawer;
        [SerializeField]
        private AmmoDrawer[] ammoDrawers;

        [SerializeField]
        private GameSelectUI gameSelectUI;
        [SerializeField]
        private RoundUI roundUI;

        [SerializeField]
        private float changeSelectionTimer = 1.0f;

        private ScoreTracker scoreTracker;
        private AccuracyTracker accuracyTracker;

        private int setViewIndex = -1;
        private int selectedSet = -1;

        private bool canChangeSelection = true;

        private void Awake()
        {
            scoreTracker = new ScoreTracker();
            accuracyTracker = new AccuracyTracker();
            ScoreLocator.Provide(scoreTracker);
            AccuracyLocator.Provide(accuracyTracker);
        }

        private void Start()
        {
            foreach (GameSet gameSet in gameSets)
            {
                gameSet.RoundUI = roundUI;
            }

            if (gameSets.Length > 0)
            {
                setViewIndex = 0;
                DisplayGameSetInfo(setViewIndex);
                ToggleNavigationUI();
            }
            else
            {
                throw new System.Exception("Error: GameSetManager: No GameSets Provided.");
            }
        }

        private void OnEnable()
        {
            foreach (GameSet gameSet in gameSets)
            {
                gameSet.onGameSetEnd += OnGameSetEnd;
                gameSet.onHighScoreChanged += OnHighScoreChanged;
            }

            gunDrawer.onDrawerClose += OnDrawerClose;
            
            foreach (AmmoDrawer aDrawer in ammoDrawers)
            {
                aDrawer.onDrawerClosed += OnDrawerClose;
            }
            
            scoreTracker.onUpdateScore += OnScoreUpdated;
            accuracyTracker.onShotFired += OnShotFired;
        }

        private void OnDisable()
        {
            foreach (GameSet gameSet in gameSets)
            {
                gameSet.onGameSetEnd -= OnGameSetEnd;
                gameSet.onHighScoreChanged -= OnHighScoreChanged;
            }

            gunDrawer.onDrawerClose -= OnDrawerClose;

            foreach (AmmoDrawer aDrawer in ammoDrawers)
            {
                aDrawer.onDrawerClosed -= OnDrawerClose;
            }

            scoreTracker.onUpdateScore -= OnScoreUpdated;
            accuracyTracker.onShotFired -= OnShotFired;
        }

        /// <summary>
        /// Select the currently viewed game set 
        /// or deselect if already selected.
        /// </summary>
        public void SelectGameSet()
        {
            if (IsGameInProgress()) return;
            if (!canChangeSelection) return;

            if (selectedSet == setViewIndex)
            {
                DeselectGameSet();
                return;
            }

            int previousSet = selectedSet;
            selectedSet = setViewIndex;
            gameSelectUI.SetSelectedSetNameText(gameSets[selectedSet].GetGameSetName());
            gameSelectUI.SetSelectButtonText("Remove");
            StartCoroutine(DisableSelectionTemporarily());

            if (gunDrawer.IsDrawerClosing()) return; 

            if (previousSet >= 0)
            {
                if (!DoSetsHaveSameWeapons(previousSet, selectedSet))
                {
                    CloseDrawers();
                }
            }
            else
            {
                SpawnCurrentSetWeapons();
                SpawnAmmoVolumes();
            }
        }

        /// <summary>
        /// View the next game set.
        /// </summary>
        public void ViewNextGameSet()
        {
            if (IsGameInProgress()) return;
            if (setViewIndex + 1 >= gameSets.Length) return;
            setViewIndex++;
            DisplayGameSetInfo(setViewIndex);
            ToggleNavigationUI();
        }

        /// <summary>
        /// View the previous game set.
        /// </summary>
        public void ViewPreviousGameSet()
        {
            if (IsGameInProgress()) return;
            if (setViewIndex == 0) return;
            setViewIndex--;
            DisplayGameSetInfo(setViewIndex);
            ToggleNavigationUI();
        }

        /// <summary>
        /// Deselect currently selected game set.
        /// </summary>
        private void DeselectGameSet()
        {
            if (AreAnyDrawersClosing()) return;

            selectedSet = -1;
            gameSelectUI.SetSelectedSetNameText("No Game Selected");
            gameSelectUI.SetSelectButtonText("Select");
            scoreTracker.ResetScore();
            CloseDrawers();
            StartCoroutine(DisableSelectionTemporarily());
        }

        /// <summary>
        /// Start selected game set.
        /// </summary>
        public void StartSelectedSet()
        {
            if (IsGameInProgress()) return;
            if (selectedSet < 0) return;

            scoreTracker.ResetScore();
            accuracyTracker.ResetAccuracyTracker();
            accuracyTracker.UnpauseTracking();
            roundUI.SetAccuracyText(0.0f);
            gameSelectUI.HideGameSelectionUI();
            roundUI.HideFinalScoreUI();
            gameSets[selectedSet].StartGameSet();
        }

        /// <summary>
        /// Stop the active game set early. 
        /// </summary>
        public void StopSelectedSet()
        {
            if (!IsGameInProgress()) return;
            gameSets[selectedSet].InitiateStopGameSet();
        }

        /// <summary>
        /// Spawn weapons of currently selected set.
        /// </summary>
        private void SpawnCurrentSetWeapons()
        {
            gunDrawer.SpawnGuns(gameSets[selectedSet].GetWeaponSmallPrefab(), gameSets[selectedSet].GetWeaponLargePrefab());
        }

        /// <summary>
        /// Spawn ammo volumes in respective ammo drawers.
        /// </summary>
        private void SpawnAmmoVolumes()
        {
            GameObject[] currentAmmoVolumes = gameSets[selectedSet].GetAmmoVolumePrefabs();
            if (currentAmmoVolumes == null || currentAmmoVolumes.Length == 0) return;

            for (int i = 0; i < ammoDrawers.Length; i++)
            {
                if (i < currentAmmoVolumes.Length)
                {
                    ammoDrawers[i].SpawnAmmoVolume(currentAmmoVolumes[i]);
                }
            }
        }

        /// <summary>
        /// Check if selected game set is being played.
        /// </summary>
        /// <returns></returns>
        private bool IsGameInProgress()
        {
            if (selectedSet < 0) return false;
            return gameSets[selectedSet].GameSetActive;
        }

        /// <summary>
        /// Show final score section when game set ends.
        /// </summary>
        /// <param name="finalScore"></param>
        private void OnGameSetEnd(int finalScore)
        {
            accuracyTracker.PauseTracking();
            roundUI.SetScoreText(finalScore);
            roundUI.SetFinalScoreText(finalScore);
            roundUI.ShowFinalScoreUI();
            gameSelectUI.ShowGameSelectionUI();
        }

        /// <summary>
        /// Update Game Selection UI when player gets a new high score.
        /// </summary>
        /// <param name="highScore"></param>
        private void OnHighScoreChanged(int highScore)
        {
            gameSelectUI.SetBestScoreText(highScore);
        }

        /// <summary>
        /// Spawn currently selected game set's guns after
        /// removing the guns of the previous set. 
        /// </summary>
        private void OnDrawerClose()
        {
            if (selectedSet < 0) return;
            if (!AreAllDrawersClosed()) return;

            SpawnCurrentSetWeapons();
            SpawnAmmoVolumes();
        }

        /// <summary>
        /// Update score UI when score updated.
        /// </summary>
        /// <param name="score"></param>
        private void OnScoreUpdated(int score)
        {
            roundUI.SetScoreText(score);
        }

        /// <summary>
        /// Update accuracy UI when shot fired.
        /// </summary>
        private void OnShotFired()
        {
            StopCoroutine(UpdateAccuracyUI());
            StartCoroutine(UpdateAccuracyUI());
        }

        /// <summary>
        /// Display the game set info for the currently
        /// viewed game set in the set selection UI.
        /// </summary>
        /// <param name="index"></param>
        private void DisplayGameSetInfo(int index)
        {
            if (index >= gameSets.Length) return;

            gameSelectUI.SetInfoNameText(gameSets[index].GetGameSetName());
            gameSelectUI.SetNumberRoundsText(gameSets[index].GetNumberRounds());
            gameSelectUI.SetDifficultyText(gameSets[index].GetDifficulty());
            gameSelectUI.SetWeapon1Text(gameSets[index].GetWeaponSmallName());
            gameSelectUI.SetWeapon2Text(gameSets[index].GetWeaponLargeName());
            gameSelectUI.SetHighestPossibleText(gameSets[index].GetHighestPossibleScore());
            gameSelectUI.SetBestScoreText(gameSets[index].GetHighScore());
        }

        /// <summary>
        /// Set navigation UI to reflect view index.
        /// </summary>
        private void ToggleNavigationUI()
        {
            gameSelectUI.ToggleLeftButtonInteraction(setViewIndex > 0);
            gameSelectUI.ToggleRightButtonInteraction(setViewIndex < gameSets.Length - 1);
            gameSelectUI.SetSelectButtonText(setViewIndex == selectedSet ? "Deselect" : "Select");
        }

        /// <summary>
        /// Check if currently viewed game set and the
        /// one already selected have the same weapons.
        /// </summary>
        /// <returns></returns>
        private bool DoSetsHaveSameWeapons(int previousSet, int currentSet)
        {
            if (previousSet == currentSet) return true;
            if (gameSets[previousSet].GetWeaponSmallPrefab() !=
                gameSets[currentSet].GetWeaponSmallPrefab()) return false;
            if (gameSets[previousSet].GetWeaponLargePrefab() !=
                gameSets[currentSet].GetWeaponLargePrefab()) return false;

            return true;
        }

        /// <summary>
        /// Temporarily disable selection for a short
        /// period of time and then re-enable it.
        /// </summary>
        /// <returns></returns>
        private IEnumerator DisableSelectionTemporarily()
        {
            gameSelectUI.ToggleSelectButtonInteraction(false);
            canChangeSelection = false;
            yield return new WaitForSeconds(changeSelectionTimer);
            gameSelectUI.ToggleSelectButtonInteraction(true);
            canChangeSelection = true;
        }

        /// <summary>
        /// Update accuracy UI after a short period of time.
        /// </summary>
        /// <returns></returns>
        private IEnumerator UpdateAccuracyUI()
        {
            yield return new WaitForSeconds(0.1f);
            roundUI.SetAccuracyText(accuracyTracker.GetAccuracy() * 100.0f);
        }

        /// <summary>
        /// Check if the gun drawer or any of the
        /// ammo drawers are currently closing.
        /// </summary>
        /// <returns></returns>
        private bool AreAnyDrawersClosing()
        {
            if (gunDrawer.IsDrawerClosing()) return true;

            foreach (AmmoDrawer aDrawer in ammoDrawers)
            {
                if (aDrawer.IsDrawerClosing()) return true;
            }

            return false;
        }

        /// <summary>
        /// Check if all drawers are closed.
        /// </summary>
        /// <returns></returns>
        private bool AreAllDrawersClosed()
        {
            if (!gunDrawer.IsDrawerClosed()) return false;
            foreach (AmmoDrawer aDrawer in ammoDrawers)
            {
                if (!aDrawer.IsDrawerClosed()) return false;
            }
            return true;
        }

        /// <summary>
        /// Close ammo drawers and despawn ammo volumes.
        /// </summary>
        private void InitiateRemoveAmmoVolumes()
        {
            foreach (AmmoDrawer aDrawer in ammoDrawers)
            {
                aDrawer.InitiateRemoveAmmoVolume();
            }
        }

        /// <summary>
        /// Close all drawers.
        /// </summary>
        private void CloseDrawers()
        {
            gunDrawer.InitiateRemoveActiveWeapons();
            InitiateRemoveAmmoVolumes();
        }
    }
}
