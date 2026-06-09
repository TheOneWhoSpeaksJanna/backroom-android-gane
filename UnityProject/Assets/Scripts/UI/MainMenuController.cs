using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Desolation.Data;

namespace Desolation.UI
{
    public class MainMenuController : MonoBehaviour
    {
        [Header("Screens Panels")]
        [SerializeField] private GameObject menuRootPanel;
        [SerializeField] private GameObject levelSelectPanel;
        [SerializeField] private GameObject optionsPanel;

        [Header("Settings Controls")]
        [SerializeField] private Slider sensitivitySlider;
        [SerializeField] private Toggle soundToggle;
        [SerializeField] private Dropdown difficultyDropdown;

        [Header("Level Buttons Grid UI")]
        [SerializeField] private Button[] levelButtons; 
        [SerializeField] private GameObject[] lockOverlays;

        private GameStateData currentData;

        private void Start()
        {
            // Set consistent frame rate for smooth movement loops
            Application.targetFrameRate = 60;

            LoadAndInitUI();
        }

        private void LoadAndInitUI()
        {
            currentData = UnitySaveManager.LoadProgress();

            // Populate sliders/toggles
            if (sensitivitySlider != null)
            {
                sensitivitySlider.value = currentData.lookSensitivity;
                sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            }

            if (soundToggle != null)
            {
                soundToggle.isOn = currentData.soundHumEnabled;
                soundToggle.onValueChanged.AddListener(OnSoundToggled);
            }

            if (difficultyDropdown != null)
            {
                difficultyDropdown.value = currentData.currentDifficulty;
                difficultyDropdown.onValueChanged.AddListener(OnDifficultyChanged);
            }

            RefreshLevelButtonsGrid();
        }

        private void RefreshLevelButtonsGrid()
        {
            for (int i = 0; i < levelButtons.Length; i++)
            {
                if (levelButtons[i] == null) continue;

                bool isUnlocked = i <= currentData.maxUnlockedLevel;
                levelButtons[i].interactable = isUnlocked;

                if (i < lockOverlays.Length && lockOverlays[i] != null)
                {
                    lockOverlays[i].SetActive(!isUnlocked);
                }
            }
        }

        // --- SCREEN ROUTINES ---
        public void LoadGameplayScene(int levelIndex)
        {
            // Save level context before loading scene, so the gameplay scene reads the parameters
            PlayerPrefs.SetInt("Active_LevelIndex", levelIndex);
            PlayerPrefs.SetInt("Settings_Difficulty", currentData.currentDifficulty);
            PlayerPrefs.SetFloat("Settings_Sensitivity", currentData.lookSensitivity);
            PlayerPrefs.SetInt("Settings_SoundEnabled", currentData.soundHumEnabled ? 1 : 0);
            PlayerPrefs.Save();

            // Load primary layout scene ("Gameplay" or index matching)
            SceneManager.LoadScene("GameplayScene");
        }

        public void EnterLobbySelect()
        {
            menuRootPanel.SetActive(false);
            levelSelectPanel.SetActive(true);
            optionsPanel.SetActive(false);
        }

        public void OpenSettings()
        {
            menuRootPanel.SetActive(false);
            levelSelectPanel.SetActive(false);
            optionsPanel.SetActive(true);
        }

        public void BackToMenu()
        {
            menuRootPanel.SetActive(true);
            levelSelectPanel.SetActive(false);
            optionsPanel.SetActive(false);
        }

        // --- SETTINGS CHANGE CAPTURES ---
        private void OnSensitivityChanged(float newVal)
        {
            currentData.lookSensitivity = newVal;
            UnitySaveManager.SaveProgress(currentData);
        }

        private void OnSoundToggled(bool value)
        {
            currentData.soundHumEnabled = value;
            UnitySaveManager.SaveProgress(currentData);
        }

        private void OnDifficultyChanged(int index)
        {
            currentData.currentDifficulty = index;
            UnitySaveManager.SaveProgress(currentData);
        }

        public void ResetGameProgress()
        {
            UnitySaveManager.ResetProgress();
            LoadAndInitUI();
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
