using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DesolationMenuController : MonoBehaviour
{
    [Header("Background Image")]
    public Image screenBackground;

    [Header("Screen Sprites")]
    public Sprite mainMenuSprite;
    public Sprite savesSprite;
    public Sprite settingsSprite;
    public Sprite creditsSprite;
    public Sprite feedbackSprite;

    [Header("Button Groups")]
    public GameObject mainButtonsGroup;
    public GameObject saveButtonsGroup;
    public GameObject settingsButtonsGroup;
    public GameObject creditsButtonsGroup;
    public GameObject feedbackButtonsGroup;

    [Header("Gameplay")]
    public string level0SceneName = "Level0";

    private void Start()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        SetScreen(mainMenuSprite, mainButtonsGroup);
    }

    public void ShowSaves()
    {
        SetScreen(savesSprite, saveButtonsGroup);
    }

    public void ShowSettings()
    {
        SetScreen(settingsSprite, settingsButtonsGroup);
    }

    public void ShowCredits()
    {
        SetScreen(creditsSprite, creditsButtonsGroup);
    }

    public void ShowFeedback()
    {
        SetScreen(feedbackSprite, feedbackButtonsGroup);
    }

    public void StartGameFromSave1()
    {
        PlayerPrefs.SetInt("SelectedSaveSlot", 1);
        LoadLevel0();
    }

    public void StartGameFromSave2()
    {
        PlayerPrefs.SetInt("SelectedSaveSlot", 2);
        LoadLevel0();
    }

    public void StartGameFromSave3()
    {
        PlayerPrefs.SetInt("SelectedSaveSlot", 3);
        LoadLevel0();
    }

    private void LoadLevel0()
    {
        SceneManager.LoadScene(level0SceneName);
    }

    private void SetScreen(Sprite sprite, GameObject activeGroup)
    {
        if (screenBackground != null && sprite != null)
        {
            screenBackground.sprite = sprite;
        }

        SetGroup(mainButtonsGroup, false);
        SetGroup(saveButtonsGroup, false);
        SetGroup(settingsButtonsGroup, false);
        SetGroup(creditsButtonsGroup, false);
        SetGroup(feedbackButtonsGroup, false);

        SetGroup(activeGroup, true);
    }

    private void SetGroup(GameObject group, bool active)
    {
        if (group != null)
        {
            group.SetActive(active);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
