using UnityEngine;
using System;

public sealed class DesolationRuntime : MonoBehaviour
{
    enum Screen { Menu, Saves, Settings, Credits, Feedback, Game }
    Screen current = Screen.Menu;

    // Textures
    Texture2D px, bg, gold, dark, clear, none, panelBg, highlight;

    // Styles
    GUIStyle titleStyle, subStyle, headerStyle, labelStyle, buttonStyle,
             ghostStyle, fieldStyle, centerStyle, slotStyle, smallLabel;

    // Settings
    float masterVol = 0.9f, musicVol = 0.75f, sfxVol = 0.85f,
          bright = 0.9f, sens = 1f;
    int gfx = 2, issueIndex;

    // Feedback
    string feedbackText = "", emailText = "", toastText = "", gameMsg = "";
    float toastUntil;
    readonly string[] issueTypes = { "BUG", "BALANCE", "PERFORMANCE", "IDEA", "OTHER" };

    // Layout metrics (all relative to screen)
    const float BTN_W = 0.28f;
    const float BTN_H = 0.072f;
    const float BTN_GAP = 0.085f;
    const float BTN_START_Y = 0.355f;

    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.orientation = ScreenOrientation.LandscapeLeft;
        LoadPrefs();
        BuildTextures();
        BuildStyles();
        BuildBackground();
        ApplySettings();

        int slot = PlayerPrefs.GetInt("SelectedSaveSlot", 0);
        if (slot > 0 && PlayerPrefs.GetInt("SaveSlot" + slot + "_Exists", 0) == 1)
            current = Screen.Game;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace))
                Back();
    }

    void OnGUI()
    {
        if (px == null) { BuildTextures(); BuildStyles(); BuildBackground(); }

        // Background
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bg,
                         ScaleMode.StretchToFill);

        // Decor (vertical bars, graffiti hint, horizontal gold lines)
        DrawDecor();

        // Screen tint for certain screens
        if (current == Screen.Saves || current == Screen.Feedback)
        {
            GUI.color = new Color(0.15f, 0.08f, 0f, 0.12f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), px);
            GUI.color = Color.white;
        }

        if (bright < 0.98f)
        {
            GUI.color = new Color(0, 0, 0, (1f - bright) * 0.45f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), px);
            GUI.color = Color.white;
        }

        switch (current)
        {
            case Screen.Menu:     DrawMenu();     break;
            case Screen.Saves:    DrawSaves();    break;
            case Screen.Settings: DrawSettings(); break;
            case Screen.Credits:  DrawCredits();  break;
            case Screen.Feedback: DrawFeedback(); break;
            default:              DrawGame();     break;
        }

        DrawToast();
    }

    // ─── TITLE BANNER (shared by all screens) ───
    void DrawBanner(string pageTitle)
    {
        GlowLabel(R(0, 0.042f, 1f, 0.095f), "DESOLATION:", titleStyle);
        GlowLabel(R(0, 0.13f, 1f, 0.045f), "THE BACKROOMS", subStyle);

        if (!string.IsNullOrEmpty(pageTitle))
        {
            GlowLabel(R(0, 0.215f, 1f, 0.058f), pageTitle, headerStyle);

            // Decorative diamonds on each side of page title
            float tw = pageTitle.Length * 0.018f;
            float cx = 0.5f;
            float dy = 0.238f;
            // Left line + diamond
            GoldLine(R(cx - tw / 2f - 0.06f, dy, 0.045f, 0.003f));
            GUI.DrawTexture(R(cx - tw / 2f - 0.012f, dy - 0.006f, 0.01f, 0.01f), gold);
            // Right line + diamond
            GoldLine(R(cx + tw / 2f + 0.012f, dy, 0.045f, 0.003f));
            GUI.DrawTexture(R(cx + tw / 2f + 0.004f, dy - 0.006f, 0.01f, 0.01f), gold);
        }
    }

    // ─── MAIN MENU ───
    void DrawMenu()
    {
        DrawBanner("");

        // Gold glow behind buttons
        GUI.color = new Color(1f, 0.78f, 0.18f, 0.14f);
        GUI.DrawTexture(R(0.33f, 0.32f, 0.34f, 0.44f), px);
        GUI.color = Color.white;

        MetalButton(0.36f, BTN_START_Y, BTN_W, BTN_H, "PLAY",     () => current = Screen.Saves);
        MetalButton(0.36f, BTN_START_Y + BTN_GAP, BTN_W, BTN_H, "SETTINGS", () => current = Screen.Settings);
        MetalButton(0.36f, BTN_START_Y + BTN_GAP * 2f, BTN_W, BTN_H, "CREDITS",  () => current = Screen.Credits);
        MetalButton(0.36f, BTN_START_Y + BTN_GAP * 3f, BTN_W, BTN_H, "FEEDBACK", () => current = Screen.Feedback);
    }

    // ─── SAVES ───
    void DrawSaves()
    {
        DrawBanner("SAVES");

        float[] xs = { 0.275f, 0.445f, 0.615f };
        for (int i = 0; i < 3; i++)
            DrawSaveSlot(xs[i], 0.37f, 0.14f, 0.34f, "SAVE " + (i + 1),
                         "Backrooms\nLevel 1", () => Play(i + 1));

        MetalButton(0.42f, 0.78f, 0.16f, 0.06f, "BACK", () => current = Screen.Menu);
    }

    void DrawSaveSlot(float x, float y, float w, float h, string header,
                      string desc, Action onClick)
    {
        Rect r = R(x, y, w, h);

        // Panel background
        DrawPanel(r);

        // Header
        GUI.Label(new Rect(r.x, r.y + 10, r.width, 36), header, headerStyle);

        // Gold divider line
        GoldLine(new Rect(r.x + 20, r.y + 48, r.width - 40, 3));

        // Description
        GUI.Label(new Rect(r.x + 10, r.y + 62, r.width - 20, 55), desc, centerStyle);

        // Thumbnail area (small preview image placeholder)
        Rect thumb = new Rect(r.x + 18, r.y + 125, r.width - 36, 72);
        GUI.DrawTexture(thumb, dark);
        // Fake room thumbnail: wallpaper + floor + light
        GUI.color = new Color(0.55f, 0.48f, 0.2f, 0.6f);
        GUI.DrawTexture(new Rect(thumb.x, thumb.y, thumb.width, thumb.height * 0.65f), px);
        GUI.color = new Color(0.18f, 0.12f, 0.05f, 0.8f);
        GUI.DrawTexture(new Rect(thumb.x, thumb.y + thumb.height * 0.65f,
                                  thumb.width, thumb.height * 0.35f), px);
        GUI.color = new Color(1f, 0.85f, 0.35f, 0.5f);
        GUI.DrawTexture(new Rect(thumb.x + thumb.width * 0.35f, thumb.y + 4,
                                  thumb.width * 0.3f, 12), px);
        GUI.color = Color.white;
        GoldLine(new Rect(thumb.x, thumb.y + thumb.height * 0.65f,
                           thumb.width, 2));

        // Invisible clickable overlay
        if (GUI.Button(r, "", onClick != null ? ghostStyle : null) && onClick != null)
            onClick();
    }

    // ─── SETTINGS ───
    void DrawSettings()
    {
        DrawBanner("SETTINGS");

        // Panel background
        Rect panel = R(0.28f, 0.33f, 0.44f, 0.44f);
        DrawPanel(panel);

        bool changed = false;

        MetalSlider(0.39f, "MASTER VOLUME", masterVol, 0, 1, ref changed);
        MetalSlider(0.46f, "MUSIC VOLUME", musicVol, 0, 1, ref changed);
        MetalSlider(0.53f, "SFX VOLUME", sfxVol, 0, 1, ref changed);
        MetalSlider(0.60f, "BRIGHTNESS", bright, 0.35f, 1.2f, ref changed);
        MetalSlider(0.67f, "SENSITIVITY", sens, 0.35f, 2f, ref changed);

        // Graphics row
        GUI.Label(R(0.31f, 0.72f, 0.1f, 0.045f), "GRAPHICS", labelStyle);
        string[] gfxNames = { "LOW", "MEDIUM", "HIGH" };
        for (int i = 0; i < 3; i++)
        {
            Rect gr = R(0.43f + i * 0.06f, 0.718f, 0.052f, 0.045f);
            if (gfx == i)
            {
                GUI.color = new Color(1f, 0.72f, 0.1f, 0.3f);
                GUI.DrawTexture(gr, px);
                GUI.color = Color.white;
            }
            if (GUI.Button(gr, gfxNames[i], buttonStyle))
            {
                gfx = i;
                changed = true;
                ApplySettings();
            }
        }

        MetalButton(0.40f, 0.805f, 0.2f, 0.065f, "BACK",
                    () => { SavePrefs(); current = Screen.Menu; });
    }

    // ─── CREDITS ───
    void DrawCredits()
    {
        DrawBanner("CREDITS");

        Rect panel = R(0.29f, 0.35f, 0.42f, 0.42f);
        DrawPanel(panel);

        string[] roles = { "GAME DESIGN", "PROGRAMMING", "UI DESIGN",
                           "ENVIRONMENT ART", "SOUND DESIGN", "SPECIAL THANKS" };
        string[] names = { "SOLO DEVELOPER", "SOLO DEVELOPER", "SOLO DEVELOPER",
                           "SOLO DEVELOPER", "SOLO DEVELOPER", "THE PLAYERS" };

        for (int i = 0; i < roles.Length; i++)
        {
            float ry = 0.39f + i * 0.052f;
            GUI.Label(R(0.33f, ry, 0.16f, 0.04f), roles[i], labelStyle);
            GoldLine(R(0.49f, ry + 0.022f, 0.06f, 0.003f));
            GUI.Label(R(0.56f, ry, 0.14f, 0.04f), names[i], labelStyle);
        }

        MetalButton(0.43f, 0.79f, 0.14f, 0.065f, "BACK", () => current = Screen.Menu);
    }

    // ─── FEEDBACK ───
    void DrawFeedback()
    {
        DrawBanner("FEEDBACK");

        Rect panel = R(0.27f, 0.35f, 0.46f, 0.46f);
        DrawPanel(panel);

        GUI.Label(R(0.3f, 0.38f, 0.18f, 0.04f), "YOUR FEEDBACK", labelStyle);
        feedbackText = GUI.TextArea(R(0.3f, 0.42f, 0.4f, 0.14f), feedbackText, 700, fieldStyle);

        GUI.Label(R(0.3f, 0.58f, 0.2f, 0.04f), "EMAIL (OPTIONAL)", labelStyle);
        emailText = GUI.TextField(R(0.3f, 0.62f, 0.4f, 0.045f), emailText, 120, fieldStyle);

        GUI.Label(R(0.3f, 0.68f, 0.16f, 0.04f), "ISSUE TYPE", labelStyle);
        Rect ir = R(0.3f, 0.72f, 0.4f, 0.045f);
        if (GUI.Button(ir, issueTypes[issueIndex] + "    ▾", fieldStyle))
            issueIndex = (issueIndex + 1) % issueTypes.Length;

        MetalButton(0.41f, 0.805f, 0.18f, 0.062f, "SEND", SendFeedback);
        MetalButton(0.43f, 0.88f, 0.14f, 0.055f, "BACK", () => current = Screen.Menu);
    }

    // ─── GAME SCREEN ───
    void DrawGame()
    {
        DrawBanner("LEVEL 0");
        Rect panel = R(0.28f, 0.37f, 0.44f, 0.26f);
        DrawPanel(panel);
        GUI.Label(new Rect(panel.x, panel.y + 30, panel.width, 50), gameMsg, centerStyle);
        MetalButton(0.42f, 0.65f, 0.16f, 0.06f, "MAIN MENU", () => current = Screen.Menu);
    }

    // ─── UI PRIMITIVES ───

    void MetalButton(float x, float y, float w, float h, string text, Action action)
    {
        Rect r = R(x, y, w, h);

        // Shadow
        GUI.color = new Color(0, 0, 0, 0.4f);
        GUI.DrawTexture(new Rect(r.x + 2, r.y + 2, r.width, r.height), dark);
        GUI.color = Color.white;

        // Plate background
        GUI.DrawTexture(r, dark);

        // Gold border
        GoldBorder(r);

        // Rivets
        float rv = 5f;
        GUI.color = new Color(0.7f, 0.7f, 0.68f, 0.9f);
        GUI.DrawTexture(new Rect(r.x + 4, r.y + 4, rv, rv), px);
        GUI.DrawTexture(new Rect(r.xMax - rv - 4, r.y + 4, rv, rv), px);
        GUI.DrawTexture(new Rect(r.x + 4, r.yMax - rv - 4, rv, rv), px);
        GUI.DrawTexture(new Rect(r.xMax - rv - 4, r.yMax - rv - 4, rv, rv), px);
        GUI.color = Color.white;

        // Text
        GUI.Label(r, text, buttonStyle);

        // Click
        if (GUI.Button(r, "", ghostStyle) && action != null)
            action();
    }

    void MetalSlider(float y, string label, float value, float min, float max,
                     ref bool changed)
    {
        Rect labelRect = R(0.31f, y - 0.012f, 0.18f, 0.04f);
        GUI.Label(labelRect, label, labelStyle);

        // Slider track background
        Rect track = R(0.45f, y, 0.2f, 0.035f);
        GUI.DrawTexture(new Rect(track.x, track.center.y - 2, track.width, 4), dark);

        // Gold fill
        float t = Mathf.InverseLerp(min, max, value);
        GUI.DrawTexture(new Rect(track.x, track.center.y - 2, track.width * t, 4), gold);

        // Knob
        float kx = track.x + track.width * t - 6;
        GUI.DrawTexture(new Rect(kx, track.center.y - 9, 12, 18), gold);

        // Invisible slider for interaction
        Rect hit = new Rect(track.x - 16, track.y - 12, track.width + 32, 32);
        float newValue = GUI.HorizontalSlider(hit, value, min, max, ghostStyle, ghostStyle);
        if (Mathf.Abs(newValue - value) > 0.001f)
        {
            value = newValue;
            changed = true;
            ApplySettings();
        }
    }

    void DrawPanel(Rect r)
    {
        // Background
        GUI.DrawTexture(r, panelBg);
        // Gold border
        GoldBorder(r);
        // Corner rivets
        float rv = 5f;
        GUI.color = new Color(0.7f, 0.7f, 0.68f, 0.85f);
        GUI.DrawTexture(new Rect(r.x + 4, r.y + 4, rv, rv), px);
        GUI.DrawTexture(new Rect(r.xMax - rv - 4, r.y + 4, rv, rv), px);
        GUI.DrawTexture(new Rect(r.x + 4, r.yMax - rv - 4, rv, rv), px);
        GUI.DrawTexture(new Rect(r.xMax - rv - 4, r.yMax - rv - 4, rv, rv), px);
        GUI.color = Color.white;
    }

    void GoldBorder(Rect r)
    {
        GoldLine(new Rect(r.x, r.y, r.width, 3));
        GoldLine(new Rect(r.x, r.yMax - 3, r.width, 3));
        GoldLine(new Rect(r.x, r.y, 3, r.height));
        GoldLine(new Rect(r.xMax - 3, r.y, 3, r.height));
    }

    void GoldLine(Rect r) { GUI.DrawTexture(r, gold); }

    void GlowLabel(Rect r, string text, GUIStyle style)
    {
        Color c = style.normal.textColor;
        // Glow passes
        style.normal.textColor = new Color(1f, 0.62f, 0.08f, 0.28f);
        GUI.Label(new Rect(r.x - 2, r.y, r.width, r.height), text, style);
        GUI.Label(new Rect(r.x + 2, r.y, r.width, r.height), text, style);
        GUI.Label(new Rect(r.x, r.y - 2, r.width, r.height), text, style);
        GUI.Label(new Rect(r.x, r.y + 2, r.width, r.height), text, style);
        // Main text
        style.normal.textColor = c;
        GUI.Label(r, text, style);
    }

    void DrawDecor()
    {
        // Dark bottom band
        GUI.color = new Color(0, 0, 0, 0.35f);
        GUI.DrawTexture(R(0, 0.73f, 1f, 0.27f), px);
        GUI.color = Color.white;

        // Vertical gold bars on left
        GUI.color = new Color(1f, 0.78f, 0.18f, 0.16f);
        for (int i = 0; i < 8; i++)
        {
            float x = 0.07f + i * 0.13f;
            GUI.DrawTexture(R(x, 0.12f, 0.045f, 0.72f), px);
            GUI.DrawTexture(R(x - 0.006f, 0.12f, 0.057f, 0.01f), gold);
        }

        // Horizontal gold accent lines
        GUI.color = new Color(1f, 0.84f, 0.28f, 0.5f);
        for (int i = 0; i < 3; i++)
        {
            float x = 0.23f + i * 0.25f;
            GUI.DrawTexture(R(x, 0.10f, 0.12f, 0.012f), gold);
            GUI.color = new Color(1f, 0.75f, 0.18f, 0.08f);
            GUI.DrawTexture(R(x - 0.03f, 0.09f, 0.18f, 0.05f), px);
            GUI.color = new Color(1f, 0.84f, 0.28f, 0.5f);
        }

        GUI.color = Color.white;

        // Graffiti text
        GUI.Label(R(0.04f, 0.29f, 0.12f, 0.06f), "DON'T\nLOOK BACK", labelStyle);
    }

    void DrawToast()
    {
        if (Time.realtimeSinceStartup > toastUntil || toastText == "") return;
        Rect r = R(0.28f, 0.915f, 0.44f, 0.055f);
        DrawPanel(r);
        GUI.Label(r, toastText, centerStyle);
    }

    // ─── ACTIONS ───
    void Play(int slot)
    {
        PlayerPrefs.SetInt("SelectedSaveSlot", slot);
        PlayerPrefs.SetInt("SaveSlot" + slot + "_Exists", 1);
        PlayerPrefs.Save();
        gameMsg = "SAVE " + slot + " SELECTED - LEVEL 0 READY";
        current = Screen.Game;
    }

    void SendFeedback()
    {
        if (string.IsNullOrWhiteSpace(feedbackText))
        {
            toastText = "WRITE FEEDBACK FIRST";
            toastUntil = Time.realtimeSinceStartup + 2f;
            return;
        }
        PlayerPrefs.SetString("LastFeedbackMessage", feedbackText.Trim());
        PlayerPrefs.SetString("LastFeedbackEmail", emailText.Trim());
        PlayerPrefs.SetString("LastFeedbackIssueType", issueTypes[issueIndex]);
        PlayerPrefs.Save();
        feedbackText = "";
        toastText = "FEEDBACK SAVED LOCALLY";
        toastUntil = Time.realtimeSinceStartup + 2.4f;
    }

    void Back()
    {
        if (current == Screen.Settings) SavePrefs();
        if (current != Screen.Menu)
        {
            current = Screen.Menu;
            toastText = "";
        }
    }

    // ─── PREFS ───
    void LoadPrefs()
    {
        masterVol = PlayerPrefs.GetFloat("MasterVolume", 0.9f);
        musicVol  = PlayerPrefs.GetFloat("MusicVolume", 0.75f);
        sfxVol    = PlayerPrefs.GetFloat("SfxVolume", 0.85f);
        bright    = PlayerPrefs.GetFloat("Brightness", 0.9f);
        sens      = PlayerPrefs.GetFloat("Sensitivity", 1f);
        gfx       = Mathf.Clamp(PlayerPrefs.GetInt("GraphicsQuality", 2), 0, 2);
    }

    void SavePrefs()
    {
        PlayerPrefs.SetFloat("MasterVolume", masterVol);
        PlayerPrefs.SetFloat("MusicVolume", musicVol);
        PlayerPrefs.SetFloat("SfxVolume", sfxVol);
        PlayerPrefs.SetFloat("Brightness", bright);
        PlayerPrefs.SetFloat("Sensitivity", sens);
        PlayerPrefs.SetInt("GraphicsQuality", gfx);
        PlayerPrefs.Save();
    }

    void ApplySettings()
    {
        AudioListener.volume = Mathf.Clamp01(masterVol);
        Application.targetFrameRate = gfx == 0 ? 30 : 60;
        if (QualitySettings.names.Length > 0)
            QualitySettings.SetQualityLevel(
                Mathf.Clamp(gfx, 0, QualitySettings.names.Length - 1), true);
    }

    // ─── TEXTURES ───
    void BuildTextures()
    {
        px       = Tex(Color.white);
        gold     = Tex(new Color(1f, 0.74f, 0.12f, 0.95f));
        dark     = Tex(new Color(0, 0, 0, 0.62f));
        clear    = Tex(new Color(0, 0, 0, 0.22f));
        none     = Tex(new Color(0, 0, 0, 0));
        panelBg  = Tex(new Color(0, 0, 0, 0.72f));
        highlight = Tex(new Color(1f, 0.78f, 0.18f, 0.2f));
    }

    Texture2D Tex(Color c)
    {
        var t = new Texture2D(1, 1, TextureFormat.RGBA32, false);
        t.SetPixel(0, 0, c);
        t.Apply();
        return t;
    }

    void BuildBackground()
    {
        bg = new Texture2D(512, 288, TextureFormat.RGB24, false);
        for (int y = 0; y < 288; y++)
            for (int x = 0; x < 512; x++)
            {
                float v = Mathf.PerlinNoise(x * 0.045f, y * 0.045f);
                float d = 1f - y / 370f;
                bg.SetPixel(x, y, new Color(
                    0.34f * d + 0.08f * v,
                    0.29f * d + 0.06f * v,
                    0.11f * d + 0.03f * v));
            }
        bg.Apply();
    }

    // ─── STYLES ───
    void BuildStyles()
    {
        float s = Mathf.Clamp(Screen.height / 720f, 0.75f, 1.4f);

        titleStyle = St(46, TextAnchor.MiddleCenter, true);
        subStyle   = St(18, TextAnchor.MiddleCenter, true);
        headerStyle = St(28, TextAnchor.MiddleCenter, true);
        labelStyle = St(15, TextAnchor.MiddleLeft, true);
        centerStyle = St(16, TextAnchor.MiddleCenter, true);
        buttonStyle = St(20, TextAnchor.MiddleCenter, true);
        slotStyle  = St(14, TextAnchor.MiddleCenter, true);
        smallLabel = St(13, TextAnchor.MiddleLeft, false);

        buttonStyle.normal.background = clear;
        buttonStyle.hover.background  = dark;
        buttonStyle.active.background = gold;
        buttonStyle.normal.textColor  = Color.white;
        buttonStyle.hover.textColor   = Color.white;
        buttonStyle.active.textColor  = Color.white;

        fieldStyle = St(14, TextAnchor.UpperLeft, false);
        fieldStyle.normal.background = dark;
        fieldStyle.focused.background = dark;
        fieldStyle.wordWrap = true;
        fieldStyle.padding = new RectOffset(8, 8, 6, 6);

        ghostStyle = new GUIStyle(GUI.skin.button);
        ghostStyle.normal.background = none;
        ghostStyle.hover.background  = none;
        ghostStyle.active.background = none;
        ghostStyle.focused.background = none;
        ghostStyle.normal.textColor  = Color.clear;
        ghostStyle.hover.textColor   = Color.clear;
        ghostStyle.active.textColor  = Color.clear;
    }

    GUIStyle St(int size, TextAnchor anchor, bool bold)
    {
        var s = new GUIStyle(GUI.skin.label);
        s.fontSize = Mathf.Max(12, Mathf.RoundToInt(size *
                             Mathf.Clamp(Screen.height / 720f, 0.75f, 1.4f)));
        s.alignment = anchor;
        s.fontStyle = bold ? FontStyle.Bold : FontStyle.Normal;
        s.normal.textColor = new Color(1f, 0.82f, 0.34f);
        return s;
    }

    Rect R(float x, float y, float w, float h)
    {
        return new Rect(Screen.width * x, Screen.height * y,
                        Screen.width * w, Screen.height * h);
    }
}
