using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Desolation Backrooms UI Kit — Runtime menu bootstrap.
/// Builds the full menu system procedurally: Canvas, background, button groups,
/// settings sliders, feedback form. Attached to a single GameObject in the MainMenu scene.
///
/// References the UI Kit mockup images as full-screen backgrounds with invisible
/// clickable buttons overlaid on the design.
/// </summary>
public class DesolationUIBootstrap : MonoBehaviour
{
    // ── Screen indices ──
    const int SCREEN_MAIN = 0;
    const int SCREEN_SAVES = 1;
    const int SCREEN_SETTINGS = 2;
    const int SCREEN_CREDITS = 3;
    const int SCREEN_FEEDBACK = 4;

    // ── UI Kit sprite asset names (in Assets/Resources/UI/) ──
    static readonly string[] ScreenSpriteNames = {
        "UI/01_main_menu_clean",
        "UI/02_save_slots_clean",
        "UI/03_settings_clean",
        "UI/04_credits_clean",
        "UI/05_feedback_clean"
    };

    // ── Runtime state ──
    Canvas canvas;
    Image background;
    GameObject[] screenGroups = new GameObject[5];
    int currentScreen = -1;
    Sprite[] screenSprites;

    void Start()
    {
        LoadSprites();
        BuildCanvas();
        BuildBackground();
        BuildMainButtons();
        BuildSaveButtons();
        BuildSettingsScreen();
        BuildCreditsScreen();
        BuildFeedbackScreen();
        BuildEventSystem();
        ShowScreen(SCREEN_MAIN);
    }

    void LoadSprites()
    {
        screenSprites = new Sprite[ScreenSpriteNames.Length];
        for (int i = 0; i < ScreenSpriteNames.Length; i++)
        {
            screenSprites[i] = Resources.Load<Sprite>(ScreenSpriteNames[i]);
            if (screenSprites[i] == null)
                Debug.LogWarning("[DesolationUIBootstrap] Missing sprite: " + ScreenSpriteNames[i]);
        }
    }

    void BuildCanvas()
    {
        GameObject go = new GameObject("MainMenuCanvas");
        canvas = go.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        go.AddComponent<GraphicRaycaster>();
    }

    void BuildBackground()
    {
        GameObject bg = new GameObject("ScreenBackground");
        bg.transform.SetParent(canvas.transform, false);
        RectTransform rt = bg.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        background = bg.AddComponent<Image>();
        background.raycastTarget = false;
    }

    GameObject CreateScreenGroup(string name, int index)
    {
        GameObject group = new GameObject(name);
        group.transform.SetParent(canvas.transform, false);
        RectTransform rt = group.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        screenGroups[index] = group;
        return group;
    }

    /// <summary>
    /// Creates an invisible clickable button at the given position.
    /// </summary>
    Button CreateInvisibleButton(string name, GameObject parent, Vector2 anchoredPos, Vector2 size, UnityEngine.Events.UnityAction onClick)
    {
        GameObject btn = new GameObject(name);
        btn.transform.SetParent(parent.transform, false);
        RectTransform rt = btn.AddComponent<RectTransform>();
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        Image img = btn.AddComponent<Image>();
        img.color = new Color(1, 1, 1, 0);
        img.raycastTarget = true;
        Button button = btn.AddComponent<Button>();
        button.transition = Selectable.Transition.None;
        button.onClick.AddListener(onClick);
        return button;
    }

    // ════════════════════════════════════════════
    //  MAIN MENU BUTTONS
    // ════════════════════════════════════════════
    void BuildMainButtons()
    {
        GameObject group = CreateScreenGroup("MainButtons", SCREEN_MAIN);

        // Positions based on the 01_main_menu_clean.png mockup layout
        // The mockup shows: PLAY, SETTINGS, CREDITS, FEEDBACK vertically centered

        CreateInvisibleButton("PlayButton", group, new Vector2(0, 120), new Vector2(420, 90), () => ShowScreen(SCREEN_SAVES));
        CreateInvisibleButton("SettingsButton", group, new Vector2(0, 10), new Vector2(420, 90), () => ShowScreen(SCREEN_SETTINGS));
        CreateInvisibleButton("CreditsButton", group, new Vector2(0, -100), new Vector2(420, 90), () => ShowScreen(SCREEN_CREDITS));
        CreateInvisibleButton("FeedbackButton", group, new Vector2(0, -210), new Vector2(420, 90), () => ShowScreen(SCREEN_FEEDBACK));
    }

    // ════════════════════════════════════════════
    //  SAVE SLOTS
    // ════════════════════════════════════════════
    void BuildSaveButtons()
    {
        GameObject group = CreateScreenGroup("SaveButtons", SCREEN_SAVES);

        CreateInvisibleButton("SaveSlot1", group, new Vector2(0, 140), new Vector2(520, 90), () => StartGame(1));
        CreateInvisibleButton("SaveSlot2", group, new Vector2(0, 20), new Vector2(520, 90), () => StartGame(2));
        CreateInvisibleButton("SaveSlot3", group, new Vector2(0, -100), new Vector2(520, 90), () => StartGame(3));
        CreateInvisibleButton("BackFromSaves", group, new Vector2(0, -260), new Vector2(300, 70), () => ShowScreen(SCREEN_MAIN));
    }

    // ════════════════════════════════════════════
    //  SETTINGS
    // ════════════════════════════════════════════
    void BuildSettingsScreen()
    {
        GameObject group = CreateScreenGroup("SettingsButtons", SCREEN_SETTINGS);

        // Create a panel background
        GameObject panel = new GameObject("SettingsPanel");
        panel.transform.SetParent(group.transform, false);
        RectTransform prt = panel.AddComponent<RectTransform>();
        prt.anchoredPosition = new Vector2(0, 0);
        prt.sizeDelta = new Vector2(700, 620);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.75f);

        // Slider settings
        string[] labels = { "Master Volume", "Music Volume", "SFX Volume", "Brightness", "Sensitivity" };
        string[] prefsKeys = { "MasterVolume", "MusicVolume", "SfxVolume", "Brightness", "Sensitivity" };
        float[] defaults = { 1.0f, 0.8f, 0.9f, 0.7f, 0.6f };
        float[] mins = { 0, 0, 0, 0.3f, 0.1f };
        float[] maxs = { 1, 1, 1, 1.5f, 2.0f };

        float yStart = 220;
        float yStep = -110;

        for (int i = 0; i < labels.Length; i++)
        {
            float y = yStart + i * yStep;

            // Label
            GameObject lbl = new GameObject(labels[i] + "Label");
            lbl.transform.SetParent(panel.transform, false);
            RectTransform lrt = lbl.AddComponent<RectTransform>();
            lrt.anchoredPosition = new Vector2(-160, y);
            lrt.sizeDelta = new Vector2(280, 40);
            TextMeshProUGUI tmp = lbl.AddComponent<TextMeshProUGUI>();
            tmp.text = labels[i];
            tmp.fontSize = 28;
            tmp.color = new Color(0.85f, 0.75f, 0.35f, 1); // gold
            tmp.alignment = TextAlignmentOptions.MiddleLeft;

            // Slider
            GameObject slider = new GameObject(labels[i] + "Slider");
            slider.transform.SetParent(panel.transform, false);
            RectTransform srt = slider.AddComponent<RectTransform>();
            srt.anchoredPosition = new Vector2(140, y);
            srt.sizeDelta = new Vector2(320, 30);
            Slider s = slider.AddComponent<Slider>();
            s.minValue = mins[i];
            s.maxValue = maxs[i];
            s.value = PlayerPrefs.GetFloat(prefsKeys[i], defaults[i]);
            s.wholeNumbers = false;

            // Slider background
            GameObject bg = new GameObject("Background");
            bg.transform.SetParent(slider.transform, false);
            RectTransform bgrt = bg.AddComponent<RectTransform>();
            bgrt.anchorMin = new Vector2(0, 0.25f);
            bgrt.anchorMax = new Vector2(1, 0.75f);
            bgrt.sizeDelta = new Vector2(0, 0);
            Image bgImg = bg.AddComponent<Image>();
            bgImg.color = new Color(0.2f, 0.15f, 0.05f, 1);

            // Fill area
            GameObject fillArea = new GameObject("Fill Area");
            fillArea.transform.SetParent(slider.transform, false);
            RectTransform firt = fillArea.AddComponent<RectTransform>();
            firt.anchorMin = new Vector2(0, 0.25f);
            firt.anchorMax = new Vector2(1, 0.75f);
            firt.sizeDelta = new Vector2(-20, 0);

            GameObject fill = new GameObject("Fill");
            fill.transform.SetParent(fillArea.transform, false);
            RectTransform fillrt = fill.AddComponent<RectTransform>();
            fillrt.sizeDelta = new Vector2(10, 0);
            Image fillImg = fill.AddComponent<Image>();
            fillImg.color = new Color(0.85f, 0.75f, 0.35f, 1);
            s.targetGraphic = fillImg;
            s.fillRect = fillrt;

            // Handle
            GameObject handle = new GameObject("Handle");
            handle.transform.SetParent(slider.transform, false);
            RectTransform hrt = handle.AddComponent<RectTransform>();
            hrt.sizeDelta = new Vector2(20, 0);
            Image hImg = handle.AddComponent<Image>();
            hImg.color = new Color(1f, 0.9f, 0.5f, 1);
            hImg.sprite = BuiltinResources.GetKnobSprite();
            s.handleRect = hrt;

            // Persist on change
            string key = prefsKeys[i];
            s.onValueChanged.AddListener(v => PlayerPrefs.SetFloat(key, v));
        }

        // Back button
        CreateInvisibleButton("BackFromSettings", group, new Vector2(0, -300), new Vector2(300, 70), () => ShowScreen(SCREEN_MAIN));
    }

    // ════════════════════════════════════════════
    //  CREDITS
    // ════════════════════════════════════════════
    void BuildCreditsScreen()
    {
        GameObject group = CreateScreenGroup("CreditsButtons", SCREEN_CREDITS);

        // Credits text
        GameObject creditsText = new GameObject("CreditsText");
        creditsText.transform.SetParent(group.transform, false);
        RectTransform crt = creditsText.AddComponent<RectTransform>();
        crt.anchoredPosition = new Vector2(0, 60);
        crt.sizeDelta = new Vector2(800, 400);
        TextMeshProUGUI ctmp = creditsText.AddComponent<TextMeshProUGUI>();
        ctmp.text = "DESOLATION: THE BACKROOMS\n\n" +
                     "A Backrooms Level 0 Horror Game\n\n" +
                     "Design, Code, Art Direction\n— TheOneWhoSpeaksJanna\n\n" +
                     "Textures\n— User-provided PBR packs\n\n" +
                     "UI Kit\n— Desolation Backrooms UI Kit\n\n" +
                     "Built with Unity 6";
        ctmp.fontSize = 32;
        ctmp.color = new Color(0.85f, 0.75f, 0.35f, 1);
        ctmp.alignment = TextAlignmentOptions.Center;

        CreateInvisibleButton("BackFromCredits", group, new Vector2(0, -280), new Vector2(300, 70), () => ShowScreen(SCREEN_MAIN));
    }

    // ════════════════════════════════════════════
    //  FEEDBACK
    // ════════════════════════════════════════════
    void BuildFeedbackScreen()
    {
        GameObject group = CreateScreenGroup("FeedbackButtons", SCREEN_FEEDBACK);

        // Panel
        GameObject panel = new GameObject("FeedbackPanel");
        panel.transform.SetParent(group.transform, false);
        RectTransform prt = panel.AddComponent<RectTransform>();
        prt.anchoredPosition = new Vector2(0, 20);
        prt.sizeDelta = new Vector2(700, 500);
        Image panelImg = panel.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.75f);

        // Issue type label
        GameObject issueLbl = new GameObject("IssueLabel");
        issueLbl.transform.SetParent(panel.transform, false);
        RectTransform irt = issueLbl.AddComponent<RectTransform>();
        irt.anchoredPosition = new Vector2(-160, 180);
        irt.sizeDelta = new Vector2(280, 40);
        TextMeshProUGUI itmp = issueLbl.AddComponent<TextMeshProUGUI>();
        itmp.text = "Issue Type";
        itmp.fontSize = 28;
        itmp.color = new Color(0.85f, 0.75f, 0.35f, 1);
        itmp.alignment = TextAlignmentOptions.MiddleLeft;

        // Issue type dropdown
        GameObject dropdown = new GameObject("IssueDropdown");
        dropdown.transform.SetParent(panel.transform, false);
        RectTransform drt = dropdown.AddComponent<RectTransform>();
        drt.anchoredPosition = new Vector2(140, 180);
        drt.sizeDelta = new Vector2(320, 40);
        TMP_Dropdown dd = dropdown.AddComponent<TMP_Dropdown>();
        dd.options.Add(new TMP_Dropdown.OptionData("General"));
        dd.options.Add(new TMP_Dropdown.OptionData("Bug Report"));
        dd.options.Add(new TMP_Dropdown.OptionData("Performance"));
        dd.options.Add(new TMP_Dropdown.OptionData("Suggestion"));
        dd.value = 0;

        // Dropdown background
        Image ddImg = dropdown.AddComponent<Image>();
        ddImg.color = new Color(0.15f, 0.12f, 0.05f, 1);
        dd.targetGraphic = ddImg;

        // Feedback input
        GameObject inputObj = new GameObject("FeedbackInput");
        inputObj.transform.SetParent(panel.transform, false);
        RectTransform frt = inputObj.AddComponent<RectTransform>();
        frt.anchoredPosition = new Vector2(0, 40);
        frt.sizeDelta = new Vector2(600, 160);
        TMP_InputField input = inputObj.AddComponent<TMP_InputField>();
        input.textComponent = inputObj.AddComponent<TextMeshProUGUI>();
        input.textComponent.fontSize = 24;
        input.textComponent.color = Color.white;
        input.placeholder = inputObj.AddComponent<TextMeshProUGUI>();
        input.placeholder.text = "Describe your feedback here...";
        input.placeholder.fontSize = 24;
        input.placeholder.color = new Color(0.5f, 0.5f, 0.5f, 1);
        input.lineType = TMP_InputField.LineType.MultiLineNewline;
        Image inputImg = inputObj.AddComponent<Image>();
        inputImg.color = new Color(0.1f, 0.08f, 0.02f, 1);
        input.targetGraphic = inputImg;

        // Email input
        GameObject emailObj = new GameObject("EmailInput");
        emailObj.transform.SetParent(panel.transform, false);
        RectTransform ert = emailObj.AddComponent<RectTransform>();
        ert.anchoredPosition = new Vector2(0, -90);
        ert.sizeDelta = new Vector2(600, 50);
        TMP_InputField email = emailObj.AddComponent<TMP_InputField>();
        email.textComponent = emailObj.AddComponent<TextMeshProUGUI>();
        email.textComponent.fontSize = 24;
        email.textComponent.color = Color.white;
        email.placeholder = emailObj.AddComponent<TextMeshProUGUI>();
        email.placeholder.text = "Email (optional)";
        email.placeholder.fontSize = 24;
        email.placeholder.color = new Color(0.5f, 0.5f, 0.5f, 1);
        Image emailImg = emailObj.AddComponent<Image>();
        emailImg.color = new Color(0.1f, 0.08f, 0.02f, 1);
        email.targetGraphic = emailImg;

        // Send button
        GameObject sendBtn = new GameObject("SendButton");
        sendBtn.transform.SetParent(panel.transform, false);
        RectTransform srt = sendBtn.AddComponent<RectTransform>();
        srt.anchoredPosition = new Vector2(0, -180);
        srt.sizeDelta = new Vector2(260, 60);
        Image simg = sendBtn.AddComponent<Image>();
        simg.color = new Color(0.85f, 0.75f, 0.35f, 0.3f);
        Button sbtn = sendBtn.AddComponent<Button>();
        sbtn.transition = Selectable.Transition.ColorTint;
        GameObject sendLbl = new GameObject("SendLabel");
        sendLbl.transform.SetParent(sendBtn.transform, false);
        RectTransform slrt = sendLbl.AddComponent<RectTransform>();
        slrt.anchorMin = Vector2.zero;
        slrt.anchorMax = Vector2.one;
        slrt.sizeDelta = Vector2.zero;
        TextMeshProUGUI stmp = sendLbl.AddComponent<TextMeshProUGUI>();
        stmp.text = "SEND";
        stmp.fontSize = 28;
        stmp.color = new Color(0.85f, 0.75f, 0.35f, 1);
        stmp.alignment = TextAlignmentOptions.Center;

        // Wire up send
        sbtn.onClick.AddListener(() =>
        {
            string feedback = input.text;
            string emailAddr = email.text;
            string issueType = dd.options[dd.value].text;

            if (string.IsNullOrWhiteSpace(feedback))
            {
                Debug.LogWarning("[DesolationFeedback] Empty feedback.");
                return;
            }

            Debug.Log($"[DesolationFeedback] Type: {issueType}, Email: {emailAddr}, Message: {feedback}");
            PlayerPrefs.SetString("LastFeedbackMessage", feedback);
            PlayerPrefs.SetString("LastFeedbackEmail", emailAddr);
            PlayerPrefs.SetString("LastFeedbackIssueType", issueType);
            input.text = "";
        });

        // Back button
        CreateInvisibleButton("BackFromFeedback", group, new Vector2(0, -300), new Vector2(300, 70), () => ShowScreen(SCREEN_MAIN));
    }

    // ════════════════════════════════════════════
    //  EVENT SYSTEM
    // ════════════════════════════════════════════
    void BuildEventSystem()
    {
        if (FindObjectOfType<EventSystem>() != null) return;
        GameObject es = new GameObject("EventSystem");
        es.AddComponent<EventSystem>();
        es.AddComponent<StandaloneInputModule>();
    }

    // ════════════════════════════════════════════
    //  SCREEN MANAGEMENT
    // ════════════════════════════════════════════
    public void ShowScreen(int index)
    {
        for (int i = 0; i < screenGroups.Length; i++)
        {
            if (screenGroups[i] != null)
                screenGroups[i].SetActive(i == index);
        }
        currentScreen = index;
        if (background != null && screenSprites != null && index >= 0 && index < screenSprites.Length && screenSprites[index] != null)
        {
            background.sprite = screenSprites[index];
        }
    }

    public void ShowMainMenu() => ShowScreen(SCREEN_MAIN);
    public void ShowSaves() => ShowScreen(SCREEN_SAVES);
    public void ShowSettings() => ShowScreen(SCREEN_SETTINGS);
    public void ShowCredits() => ShowScreen(SCREEN_CREDITS);
    public void ShowFeedback() => ShowScreen(SCREEN_FEEDBACK);

    public void StartGame(int slot)
    {
        PlayerPrefs.SetInt("SelectedSaveSlot", slot);
        SceneManager.LoadScene("DesolationBootstrap");
    }
}

/// <summary>
/// Minimal helper to get a default Unity knob sprite for sliders.
/// </summary>
internal static class BuiltinResources
{
    private static Sprite _knob;
    public static Sprite GetKnobSprite()
    {
        if (_knob == null)
        {
            // Create a simple circle texture for the slider handle
            int size = 32;
            Texture2D tex = new Texture2D(size, size);
            Color clear = new Color(0, 0, 0, 0);
            Color white = Color.white;
            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dx = x - size / 2f;
                    float dy = y - size / 2f;
                    float dist = Mathf.Sqrt(dx * dx + dy * dy);
                    tex.SetPixel(x, y, dist < size / 2f - 2 ? white : clear);
                }
            }
            tex.Apply();
            _knob = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f));
        }
        return _knob;
    }
}
