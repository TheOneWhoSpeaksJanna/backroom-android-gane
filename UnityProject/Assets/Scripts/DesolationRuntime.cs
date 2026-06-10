using System;
using UnityEngine;
using Desolation.Data;

namespace Desolation.UI
{
    public class DesolationRuntime : MonoBehaviour
    {
        // ── MENU STATE ──
        private enum MenuScreen
        {
            Main,
            Saves,
            Settings,
            Credits,
            Feedback
        }

        private MenuScreen currentScreen = MenuScreen.Main;
        private GameStateData gameData;

        // ── SETTINGS TEMP STATE ──
        private float tempMasterVolume = 0.85f;
        private float tempMusicVolume = 0.60f;
        private float tempSfxVolume = 0.70f;
        private float tempBrightness = 1.0f;
        private int tempGraphicsQuality = 2; // 0=Low, 1=Med, 2=High

        // ── FEEDBACK STATE ──
        private string feedbackText = "";
        private string feedbackEmail = "";
        private int feedbackTypeIndex = 0;
        private string feedbackStatus = "";
        private Color feedbackStatusColor = Color.green;
        private readonly string[] feedbackTypes = { "BUG", "SUGGESTION", "COMPLAINT", "OTHER" };
        private Vector2 feedbackScrollPos;

        // ── SCROLL ──
        private Vector2 settingsScrollPos;
        private Vector2 creditsScrollPos;

        // ── FONTS & STYLES ──
        private GUIStyle titleStyle;
        private GUIStyle subtitleStyle;
        private GUIStyle panelLabelStyle;
        private GUIStyle buttonStyle;
        private GUIStyle slotTitleStyle;
        private GUIStyle slotMetaStyle;
        private GUIStyle settingsLabelStyle;
        private GUIStyle settingsValueStyle;
        private GUIStyle creditsTextStyle;
        private GUIStyle feedbackLabelStyle;
        private GUIStyle feedbackInputStyle;
        private GUIStyle feedbackStatusStyle;
        private GUIStyle sliderStyle;
        private GUIStyle sliderThumbStyle;
        private GUIStyle capsuleStyle;
        private GUIStyle backButtonStyle;
        private GUIStyle sendButtonStyle;
        private GUIStyle smallButtonStyle;
        private GUIStyle panelStyle;
        private GUIStyle cardStyle;
        private GUIStyle lockStyle;
        private GUIStyle orbStyle;
        private GUIStyle vignetteStyle;

        // ── COLORS (from screenshot analysis) ──
        private static readonly Color GOLDEN_ORANGE = new Color(0.898f, 0.722f, 0.259f);       // #E5B842
        private static readonly Color BRIGHT_YELLOW = new Color(1f, 0.941f, 0.573f);           // #FFF092
        private static readonly Color SUBTLE_WHITE = new Color(1f, 0.969f, 0.761f);            // #FFF7C2
        private static readonly Color DARK_PANEL = new Color(0.039f, 0.035f, 0.02f, 0.85f);    // #0A0905D9
        private static readonly Color VIGNETTE = new Color(0.008f, 0.008f, 0.004f, 0.67f);     // #020201AA
        private static readonly Color DARK_BG = new Color(0.02f, 0.02f, 0.01f, 0.95f);
        private static readonly Color LOCK_OVERLAY = new Color(0f, 0f, 0f, 0.75f);
        private static readonly Color CARD_BG = new Color(0.06f, 0.05f, 0.03f, 0.9f);
        private static readonly Color CARD_BORDER = new Color(0.898f, 0.722f, 0.259f, 0.4f);
        private static readonly Color SLIDER_BG = new Color(0.12f, 0.10f, 0.04f, 0.8f);
        private static readonly Color CAPSULE_ACTIVE = new Color(0.898f, 0.722f, 0.259f, 0.24f);
        private static readonly Color CAPSULE_INACTIVE = new Color(0f, 0f, 0f, 0.6f);
        private static readonly Color CORRUPTED_RED = new Color(0.7f, 0.15f, 0.15f, 0.85f);
        private static readonly Color STATUS_GREEN = new Color(0.5f, 0.78f, 0.52f, 1f);
        private static readonly Color STATUS_RED = new Color(0.9f, 0.3f, 0.3f, 1f);

        // ── TEXTURES ──
        private Texture2D bgTexture;
        private Texture2D menuBackgroundTexture;
        private Texture2D scanlineTexture;
        private Texture2D panelTexture;
        private Texture2D cardTexture;
        private Texture2D sliderBgTexture;
        private Texture2D sliderFillTexture;
        private Texture2D vignetteTexture;
        private Texture2D orbTexture;

        // ── TIMING ──
        private float glitchTimer;
        private float glitchOffset;
        private bool isTransitioning;
        private float transitionAlpha;
        private MenuScreen transitionTarget;

        // ── LAYOUT CONSTANTS ──
        private const float TITLE_Y = 60f;
        private const float SUBTITLE_Y = 100f;
        private const float PANEL_LABEL_Y = 130f;
        private const float BUTTON_WIDTH = 260f;
        private const float BUTTON_HEIGHT = 42f;
        private const float BUTTON_SPACING = 12f;
        private const float CARD_WIDTH = 200f;
        private const float CARD_HEIGHT = 280f;
        private const float CARD_SPACING = 24f;
        private const float SLIDER_WIDTH = 220f;
        private const float SLIDER_HEIGHT = 24f;
        private const float CAPSULE_WIDTH = 80f;
        private const float CAPSULE_HEIGHT = 32f;

        private void Awake()
        {
            Application.targetFrameRate = 60;
            LoadGameData();
            BuildTextures();
            LoadMenuBackground();
        }

        private void LoadMenuBackground()
        {
            menuBackgroundTexture = Resources.Load<Texture2D>("Textures/backrooms_corridor_bg");
        }

        private void LoadGameData()
        {
            gameData = UnitySaveManager.LoadProgress();
            tempMasterVolume = PlayerPrefs.GetFloat("Settings_MasterVolume", 0.85f);
            tempMusicVolume = PlayerPrefs.GetFloat("Settings_MusicVolume", 0.60f);
            tempSfxVolume = PlayerPrefs.GetFloat("Settings_SFXVolume", 0.70f);
            tempBrightness = PlayerPrefs.GetFloat("Settings_Brightness", 1.0f);
            tempGraphicsQuality = PlayerPrefs.GetInt("Settings_GraphicsQuality", 2);
        }

        private void BuildTextures()
        {
            // Background: dark yellow-brown noise
            bgTexture = new Texture2D(2, 2);
            Color[] bgColors = new Color[4];
            for (int i = 0; i < 4; i++)
            {
                float n = UnityEngine.Random.Range(0.015f, 0.035f);
                bgColors[i] = new Color(n, n * 0.85f, n * 0.4f, 1f);
            }
            bgTexture.SetPixels(bgColors);
            bgTexture.Apply();

            // Scanlines: thin horizontal dark lines
            scanlineTexture = new Texture2D(1, 4);
            scanlineTexture.SetPixel(0, 0, new Color(0, 0, 0, 0));
            scanlineTexture.SetPixel(0, 1, new Color(0, 0, 0, 0.08f));
            scanlineTexture.SetPixel(0, 2, new Color(0, 0, 0, 0));
            scanlineTexture.SetPixel(0, 3, new Color(0, 0, 0, 0.06f));
            scanlineTexture.wrapMode = TextureWrapMode.Repeat;
            scanlineTexture.Apply();

            // Panel: dark semi-transparent
            panelTexture = new Texture2D(1, 1);
            panelTexture.SetPixel(0, 0, DARK_PANEL);
            panelTexture.Apply();

            // Card: dark with slight warm tint
            cardTexture = new Texture2D(1, 1);
            cardTexture.SetPixel(0, 0, CARD_BG);
            cardTexture.Apply();

            // Slider background
            sliderBgTexture = new Texture2D(1, 1);
            sliderBgTexture.SetPixel(0, 0, SLIDER_BG);
            sliderBgTexture.Apply();

            // Slider fill
            sliderFillTexture = new Texture2D(1, 1);
            sliderFillTexture.SetPixel(0, 0, GOLDEN_ORANGE);
            sliderFillTexture.Apply();

            // Vignette: radial gradient approximation
            vignetteTexture = new Texture2D(2, 2);
            vignetteTexture.SetPixel(0, 0, new Color(0, 0, 0, 0));
            vignetteTexture.SetPixel(1, 0, VIGNETTE);
            vignetteTexture.SetPixel(0, 1, VIGNETTE);
            vignetteTexture.SetPixel(1, 1, VIGNETTE);
            vignetteTexture.Apply();

            // Orb: small dot
            orbTexture = new Texture2D(1, 1);
            orbTexture.SetPixel(0, 0, GOLDEN_ORANGE);
            orbTexture.Apply();
        }

        private void InitStyles()
        {
            // ── TITLE: Large, bold, golden orange, glitchy ──
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.07f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter,
                wordWrap = true
            };

            // ── SUBTITLE: Smaller, muted straw yellow ──
            subtitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.032f),
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.UpperCenter,
                wordWrap = true
            };

            // ── PANEL LABEL: Faint golden text ──
            panelLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.028f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.UpperCenter
            };

            // ── MAIN MENU BUTTONS: Underlined golden text ──
            buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.035f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = false,
                padding = new RectOffset(0, 0, 8, 8),
                margin = new RectOffset(0, 0, 4, 4)
            };

            // ── SLOT TITLE: Bold golden ──
            slotTitleStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.03f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            // ── SLOT META: Small, muted ──
            slotMetaStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.022f),
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            // ── SETTINGS LABEL ──
            settingsLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.025f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            // ── SETTINGS VALUE ──
            settingsValueStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.022f),
                alignment = TextAnchor.MiddleRight
            };

            // ── CREDITS TEXT ──
            creditsTextStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.024f),
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            // ── FEEDBACK LABEL ──
            feedbackLabelStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.024f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft
            };

            // ── FEEDBACK INPUT ──
            feedbackInputStyle = new GUIStyle(GUI.skin.textField)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.022f),
                alignment = TextAnchor.UpperLeft,
                wordWrap = true,
                padding = new RectOffset(8, 8, 8, 8)
            };

            // ── FEEDBACK STATUS ──
            feedbackStatusStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.02f),
                fontStyle = FontStyle.Italic,
                alignment = TextAnchor.MiddleCenter,
                wordWrap = true
            };

            // ── CAPSULE BUTTON (graphics quality) ──
            capsuleStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.02f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(4, 4, 4, 4)
            };

            // ── BACK BUTTON ──
            backButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.028f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 6, 6)
            };

            // ── SEND BUTTON ──
            sendButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.026f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 6, 6)
            };

            // ── SMALL BUTTON ──
            smallButtonStyle = new GUIStyle(GUI.skin.button)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.022f),
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(4, 4, 4, 4)
            };

            // ── PANEL BOX ──
            panelStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperCenter,
                padding = new RectOffset(20, 20, 20, 20)
            };

            // ── CARD STYLE ──
            cardStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(10, 10, 10, 10)
            };

            // ── LOCK STYLE ──
            lockStyle = new GUIStyle(GUI.skin.label)
            {
                fontSize = Mathf.RoundToInt(Screen.width * 0.028f),
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            // ── ORB STYLE ──
            orbStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter
            };

            // ── VIGNETTE STYLE ──
            vignetteStyle = new GUIStyle();
        }

        private void OnGUI()
        {
            if (titleStyle == null) InitStyles();

            glitchTimer += Time.deltaTime;
            glitchOffset = Mathf.Sin(glitchTimer * 6f) * 1.2f;

            // ── DRAW BACKGROUND ──
            DrawBackground();

            // ── DRAW VIGNETTE OVERLAY ──
            DrawVignette();

            // ── DRAW SCANLINES ──
            DrawScanlines();

            // ── DRAW TITLE (always visible except on main menu where it's the hero) ──
            DrawHeader();

            // ── DRAW CURRENT SCREEN ──
            switch (currentScreen)
            {
                case MenuScreen.Main:
                    DrawMainMenu();
                    break;
                case MenuScreen.Saves:
                    DrawSavesScreen();
                    break;
                case MenuScreen.Settings:
                    DrawSettingsScreen();
                    break;
                case MenuScreen.Credits:
                    DrawCreditsScreen();
                    break;
                case MenuScreen.Feedback:
                    DrawFeedbackScreen();
                    break;
            }
        }

        // ═══════════════════════════════════════════
        //  BACKGROUND
        // ═══════════════════════════════════════════
        private void DrawBackground()
        {
            if (currentScreen == MenuScreen.Main && menuBackgroundTexture != null)
            {
                // Cover-mode: fill entire screen, crop to preserve aspect ratio
                float screenAspect = (float)Screen.width / Screen.height;
                float imageAspect = (float)menuBackgroundTexture.width / menuBackgroundTexture.height;
                Rect drawRect;
                if (screenAspect > imageAspect)
                {
                    // Screen is wider — fit to height, crop sides
                    float h = Screen.height;
                    float w = h * imageAspect;
                    float x = (Screen.width - w) * 0.5f;
                    drawRect = new Rect(x, 0, w, h);
                }
                else
                {
                    // Screen is taller — fit to width, crop top/bottom
                    float w = Screen.width;
                    float h = w / imageAspect;
                    float y = (Screen.height - h) * 0.5f;
                    drawRect = new Rect(0, y, w, h);
                }
                GUI.color = Color.white;
                GUI.DrawTexture(drawRect, menuBackgroundTexture, ScaleMode.StretchToFill);
                // Dark overlay to keep text/buttons readable
                GUI.color = new Color(0, 0, 0, 0.45f);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bgTexture, ScaleMode.StretchToFill);
                GUI.color = Color.white;
            }
            else
            {
                // Sub-screens: dark procedural background
                GUI.color = new Color(1, 1, 1, 0.12f);
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), bgTexture, ScaleMode.StretchToFill);
                GUI.color = Color.white;
            }
        }

        private void DrawVignette()
        {
            float vw = Screen.width * 1.2f;
            float vh = Screen.height * 1.2f;
            GUI.color = new Color(0, 0, 0, 0.5f);
            GUI.DrawTexture(new Rect(-vw * 0.1f, -vh * 0.1f, vw, vh), vignetteTexture, ScaleMode.StretchToFill);
            GUI.color = Color.white;
        }

        private void DrawScanlines()
        {
            GUI.color = new Color(1, 1, 1, 0.05f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height * 3f), scanlineTexture, ScaleMode.StretchToFill);
            GUI.color = Color.white;
        }

        // ═══════════════════════════════════════════
        //  HEADER
        // ═══════════════════════════════════════════
        private void DrawHeader()
        {
            float titlePulse = 0.65f + Mathf.Cos(glitchTimer * 2.5f) * 0.35f;
            Color titleCol = GOLDEN_ORANGE;
            titleCol.a = titlePulse;

            // Glitch offset
            float gx = (glitchTimer % 0.7f < 0.05f) ? UnityEngine.Random.Range(-3f, 3f) : 0f;

            // Title: "DESOLATION:"
            titleStyle.normal.textColor = titleCol;
            GUI.Label(new Rect(gx, TITLE_Y, Screen.width, 60), "DESOLATION:", titleStyle);

            // Subtitle: "THE BACKROOMS"
            subtitleStyle.normal.textColor = new Color(0.85f, 0.78f, 0.45f, 0.7f);
            GUI.Label(new Rect(0, SUBTITLE_Y, Screen.width, 30), "THE BACKROOMS", subtitleStyle);

            // Panel indicator (for sub-screens)
            if (currentScreen != MenuScreen.Main)
            {
                string label = "";
                switch (currentScreen)
                {
                    case MenuScreen.Saves: label = "SAVES"; break;
                    case MenuScreen.Settings: label = "SETTINGS"; break;
                    case MenuScreen.Credits: label = "CREDITS"; break;
                    case MenuScreen.Feedback: label = "FEEDBACK"; break;
                }
                panelLabelStyle.normal.textColor = new Color(GOLDEN_ORANGE.r, GOLDEN_ORANGE.g, GOLDEN_ORANGE.b, 0.35f);
                GUI.Label(new Rect(0, PANEL_LABEL_Y, Screen.width, 24), label, panelLabelStyle);
            }
        }

        // ═══════════════════════════════════════════
        //  MAIN MENU
        // ═══════════════════════════════════════════
        private void DrawMainMenu()
        {
            float centerX = Screen.width / 2f;
            float startY = Screen.height * 0.38f;
            float bw = BUTTON_WIDTH;
            float bh = BUTTON_HEIGHT;
            float spacing = BUTTON_SPACING;

            // Menu buttons: Play, Settings, Credits
            string[] labels = { "PLAY", "SETTINGS", "CREDITS" };
            for (int i = 0; i < labels.Length; i++)
            {
                float y = startY + i * (bh + spacing);
                Rect btnRect = new Rect(centerX - bw / 2f, y, bw, bh);

                // Draw button background
                DrawButtonBackground(btnRect, GOLDEN_ORANGE, 0.08f);

                // Draw underline
                GUI.color = new Color(GOLDEN_ORANGE.r, GOLDEN_ORANGE.g, GOLDEN_ORANGE.b, 0.6f);
                GUI.DrawTexture(new Rect(btnRect.x + 20, btnRect.yMax - 4, bw - 40, 2), panelTexture, ScaleMode.StretchToFill);
                GUI.color = Color.white;

                // Button text
                buttonStyle.normal.textColor = GOLDEN_ORANGE;
                buttonStyle.hover.textColor = BRIGHT_YELLOW;
                buttonStyle.active.textColor = BRIGHT_YELLOW;

                if (GUI.Button(btnRect, labels[i], buttonStyle))
                {
                    switch (i)
                    {
                        case 0: SwitchScreen(MenuScreen.Saves); break;
                        case 1: SwitchScreen(MenuScreen.Settings); break;
                        case 2: SwitchScreen(MenuScreen.Credits); break;
                    }
                }
            }

            // Feedback button: bottom-right corner
            float fbW = Screen.width * 0.22f;
            float fbH = Screen.height * 0.045f;
            Rect fbRect = new Rect(Screen.width - fbW - 16, Screen.height - fbH - 16, fbW, fbH);

            DrawButtonBackground(fbRect, GOLDEN_ORANGE, 0.05f);
            smallButtonStyle.normal.textColor = new Color(GOLDEN_ORANGE.r, GOLDEN_ORANGE.g, GOLDEN_ORANGE.b, 0.6f);
            smallButtonStyle.hover.textColor = GOLDEN_ORANGE;

            if (GUI.Button(fbRect, "FEEDBACK", smallButtonStyle))
            {
                SwitchScreen(MenuScreen.Feedback);
            }
        }

        // ═══════════════════════════════════════════
        //  SAVE SLOTS
        // ═══════════════════════════════════════════
        private void DrawSavesScreen()
        {
            float centerX = Screen.width / 2f;
            float cardY = Screen.height * 0.28f;
            float totalW = CARD_WIDTH * 3 + CARD_SPACING * 2;
            float startX = centerX - totalW / 2f;

            for (int i = 0; i < 3; i++)
            {
                float x = startX + i * (CARD_WIDTH + CARD_SPACING);
                Rect cardRect = new Rect(x, cardY, CARD_WIDTH, CARD_HEIGHT);
                bool unlocked = i <= gameData.maxUnlockedLevel;

                DrawSaveCard(cardRect, i, unlocked);
            }

            // Back button
            float backY = cardY + CARD_HEIGHT + 30;
            DrawBackButton(new Rect(centerX - BUTTON_WIDTH / 2f, backY, BUTTON_WIDTH, BUTTON_HEIGHT));
        }

        private void DrawSaveCard(Rect rect, int slotIndex, bool unlocked)
        {
            // Card background
            GUI.color = new Color(1, 1, 1, 0.85f);
            GUI.Box(rect, "", cardStyle);

            // Card border glow
            DrawRectBorder(rect, CARD_BORDER, 2f);

            // Slot title
            slotTitleStyle.normal.textColor = GOLDEN_ORANGE;
            GUI.Label(new Rect(rect.x, rect.y + 15, rect.width, 30), "SAVE " + (slotIndex + 1), slotTitleStyle);

            // Level info
            string levelName = slotIndex == 0 ? "Level 1" : slotIndex == 1 ? "Level 2" : "Level 3";
            string metaText = unlocked ? "Backrooms\n" + levelName : "CORRUPTED\n[ENCRYPTED]";
            slotMetaStyle.normal.textColor = unlocked
                ? new Color(0.7f, 0.65f, 0.4f, 0.8f)
                : CORRUPTED_RED;
            GUI.Label(new Rect(rect.x, rect.y + 60, rect.width, 50), metaText, slotMetaStyle);

            // Lock overlay for locked slots
            if (!unlocked)
            {
                GUI.color = LOCK_OVERLAY;
                GUI.DrawTexture(rect, panelTexture, ScaleMode.StretchToFill);
                GUI.color = Color.white;

                lockStyle.normal.textColor = new Color(0.6f, 0.6f, 0.6f, 0.7f);
                GUI.Label(new Rect(rect.x, rect.y + rect.height * 0.4f, rect.width, 30), "🔒", lockStyle);
            }

            // Clickable area
            if (GUI.Button(rect, "", GUIStyle.none))
            {
                if (unlocked)
                {
                    SelectSaveSlot(slotIndex);
                }
            }
        }

        // ═══════════════════════════════════════════
        //  SETTINGS
        // ═══════════════════════════════════════════
        private void DrawSettingsScreen()
        {
            float centerX = Screen.width / 2f;
            float panelW = Screen.width * 0.8f;
            float panelH = Screen.height * 0.55f;
            float panelX = centerX - panelW / 2f;
            float panelY = Screen.height * 0.22f;

            // Panel background
            GUI.color = DARK_PANEL;
            GUI.Box(new Rect(panelX, panelY, panelW, panelH), "", panelStyle);
            GUI.color = Color.white;

            // Scrollable settings area
            settingsScrollPos = GUI.BeginScrollView(
                new Rect(panelX + 10, panelY + 10, panelW - 20, panelH - 20),
                settingsScrollPos,
                new Rect(0, 0, panelW - 60, 500)
            );

            float y = 0;
            float rowH = 50;
            float labelX = 10;
            float sliderX = panelW * 0.45f;
            float sliderW = panelW * 0.35f;
            float valueX = panelW * 0.82f;

            // Master Volume
            DrawSliderRow(ref y, rowH, labelX, sliderX, sliderW, valueX,
                "MASTER VOLUME", ref tempMasterVolume, "{(int)(tempMasterVolume * 100)}%");

            // Music Volume
            DrawSliderRow(ref y, rowH, labelX, sliderX, sliderW, valueX,
                "MUSIC VOLUME", ref tempMusicVolume, "{(int)(tempMusicVolume * 100)}%");

            // SFX Volume
            DrawSliderRow(ref y, rowH, labelX, sliderX, sliderW, valueX,
                "SFX VOLUME", ref tempSfxVolume, "{(int)(tempSfxVolume * 100)}%");

            // Brightness
            DrawSliderRow(ref y, rowH, labelX, sliderX, sliderW, valueX,
                "BRIGHTNESS", ref tempBrightness, "{(int)(tempBrightness * 100)}%");

            // Sensitivity
            float sensVal = gameData.lookSensitivity;
            DrawSliderRow(ref y, rowH, labelX, sliderX, sliderW, valueX,
                "SENSITIVITY", ref sensVal, "{sensVal:F1}x");
            gameData.lookSensitivity = sensVal;
            UnitySaveManager.SaveProgress(gameData);

            y += 15;

            // Graphics Quality Capsules
            settingsLabelStyle.normal.textColor = GOLDEN_ORANGE;
            GUI.Label(new Rect(labelX, y, panelW - 20, 24), "GRAPHICS", settingsLabelStyle);
            y += 30;

            string[] gfxLabels = { "LOW", "MEDIUM", "HIGH" };
            float capsuleX = labelX;
            for (int i = 0; i < 3; i++)
            {
                Rect capRect = new Rect(capsuleX, y, CAPSULE_WIDTH, CAPSULE_HEIGHT);
                bool active = i == tempGraphicsQuality;

                // Capsule background
                GUI.color = active ? CAPSULE_ACTIVE : CAPSULE_INACTIVE;
                GUI.Box(capRect, "", GUI.skin.box);
                GUI.color = Color.white;

                // Border
                DrawRectBorder(capRect, active ? GOLDEN_ORANGE : new Color(0.3f, 0.3f, 0.3f, 0.5f), 1f);

                capsuleStyle.normal.textColor = active ? BRIGHT_YELLOW : new Color(0.5f, 0.5f, 0.5f, 0.7f);
                if (GUI.Button(capRect, gfxLabels[i], capsuleStyle))
                {
                    tempGraphicsQuality = i;
                }

                capsuleX += CAPSULE_WIDTH + 10;
            }

            GUI.EndScrollView();

            // Back button
            float backY = panelY + panelH + 15;
            DrawBackButton(new Rect(centerX - BUTTON_WIDTH / 2f, backY, BUTTON_WIDTH, BUTTON_HEIGHT));
        }

        private void DrawSliderRow(ref float y, float rowH, float labelX, float sliderX, float sliderW, float valueX,
            string label, ref float value, string valueFormat)
        {
            // Label
            settingsLabelStyle.normal.textColor = GOLDEN_ORANGE;
            GUI.Label(new Rect(labelX, y + 4, sliderX - labelX - 10, rowH), label, settingsLabelStyle);

            // Slider background
            GUI.color = new Color(SLIDER_BG.r, SLIDER_BG.g, SLIDER_BG.b, SLIDER_BG.a);
            GUI.DrawTexture(new Rect(sliderX, y + rowH * 0.35f, sliderW, rowH * 0.3f), sliderBgTexture, ScaleMode.StretchToFill);
            GUI.color = Color.white;

            // Slider fill
            GUI.color = new Color(GOLDEN_ORANGE.r, GOLDEN_ORANGE.g, GOLDEN_ORANGE.b, 0.7f);
            GUI.DrawTexture(new Rect(sliderX, y + rowH * 0.35f, sliderW * value, rowH * 0.3f), sliderFillTexture, ScaleMode.StretchToFill);
            GUI.color = Color.white;

            // Slider thumb
            float thumbX = sliderX + sliderW * value - 6;
            float thumbY = y + rowH * 0.2f;
            GUI.color = BRIGHT_YELLOW;
            GUI.DrawTexture(new Rect(thumbX, thumbY, 12, rowH * 0.6f), sliderFillTexture, ScaleMode.StretchToFill);
            GUI.color = Color.white;

            // Value text
            string valStr = valueFormat.Replace("{", "").Replace("}", "");
            if (valueFormat.Contains("(int)"))
                valStr = ((int)(value * 100)) + "%";
            else if (valueFormat.Contains(":F1"))
                valStr = value.ToString("F1") + "x";

            settingsValueStyle.normal.textColor = new Color(0.6f, 0.55f, 0.35f, 0.8f);
            GUI.Label(new Rect(valueX, y + 4, 60, rowH), valStr, settingsValueStyle);

            // Invisible button for interaction
            Rect sliderRect = new Rect(sliderX - 10, y, sliderW + 20, rowH);
            if (GUI.Button(sliderRect, "", GUIStyle.none))
            {
                float mouseX = Event.current.mousePosition.x - sliderX;
                value = Mathf.Clamp01(mouseX / sliderW);
            }

            y += rowH;
        }

        // ═══════════════════════════════════════════
        //  CREDITS
        // ═══════════════════════════════════════════
        private void DrawCreditsScreen()
        {
            float centerX = Screen.width / 2f;
            float panelW = Screen.width * 0.75f;
            float panelH = Screen.height * 0.5f;
            float panelX = centerX - panelW / 2f;
            float panelY = Screen.height * 0.24f;

            // Credits box
            GUI.color = DARK_PANEL;
            GUI.Box(new Rect(panelX, panelY, panelW, panelH), "", panelStyle);
            GUI.color = Color.white;

            // Border
            DrawRectBorder(new Rect(panelX, panelY, panelW, panelH), new Color(GOLDEN_ORANGE.r, GOLDEN_ORANGE.g, GOLDEN_ORANGE.b, 0.3f), 2f);

            creditsScrollPos = GUI.BeginScrollView(
                new Rect(panelX + 15, panelY + 15, panelW - 30, panelH - 30),
                creditsScrollPos,
                new Rect(0, 0, panelW - 50, 400)
            );

            float y = 0;
            float rowH = 40;

            // Credit entries: Role <---orb---> Name
            string[,] credits = {
                { "GAME DESIGN", "JANNA" },
                { "PROGRAMMING", "JANNA" },
                { "ART DIRECTION", "JANNA" },
                { "LEVEL DESIGN", "JANNA" },
                { "SOUND DESIGN", "JANNA" },
                { "TESTING", "JANNA" },
                { "SPECIAL THANKS", "THE BACKROOMS COMMUNITY" },
                { "INSPIRED BY", "THE BACKROOMS / LIMINAL SPACES" },
                { "BUILT WITH", "UNITY ENGINE" },
                { "VERSION", "1.0.0" }
            };

            for (int i = 0; i < credits.GetLength(0); i++)
            {
                string role = credits[i, 0];
                string name = credits[i, 1];

                float colW = (panelW - 50) / 2f;

                // Role (right-aligned)
                creditsTextStyle.normal.textColor = new Color(0.6f, 0.55f, 0.35f, 0.7f);
                GUI.Label(new Rect(0, y, colW - 20, rowH), role, creditsTextStyle);

                // Orb dot in center
                float orbX = colW - 6;
                GUI.color = GOLDEN_ORANGE;
                GUI.DrawTexture(new Rect(orbX, y + rowH / 2f - 3, 6, 6), orbTexture, ScaleMode.StretchToFill);
                GUI.color = Color.white;

                // Wire lines from orb
                GUI.color = new Color(GOLDEN_ORANGE.r, GOLDEN_ORANGE.g, GOLDEN_ORANGE.b, 0.3f);
                GUI.DrawTexture(new Rect(colW - 20, y + rowH / 2f - 0.5f, 14, 1), panelTexture);
                GUI.DrawTexture(new Rect(colW + 6, y + rowH / 2f - 0.5f, 14, 1), panelTexture);
                GUI.color = Color.white;

                // Name (left-aligned)
                creditsTextStyle.normal.textColor = GOLDEN_ORANGE;
                GUI.Label(new Rect(colW + 20, y, colW - 20, rowH), name, creditsTextStyle);

                y += rowH;
            }

            GUI.EndScrollView();

            // Back button
            float backY = panelY + panelH + 15;
            DrawBackButton(new Rect(centerX - BUTTON_WIDTH / 2f, backY, BUTTON_WIDTH, BUTTON_HEIGHT));
        }

        // ═══════════════════════════════════════════
        //  FEEDBACK
        // ═══════════════════════════════════════════
        private void DrawFeedbackScreen()
        {
            float centerX = Screen.width / 2f;
            float panelW = Screen.width * 0.8f;
            float panelH = Screen.height * 0.55f;
            float panelX = centerX - panelW / 2f;
            float panelY = Screen.height * 0.22f;

            // Form container
            GUI.color = DARK_PANEL;
            GUI.Box(new Rect(panelX, panelY, panelW, panelH), "", panelStyle);
            GUI.color = Color.white;

            float x = panelX + 20;
            float y = panelY + 15;
            float w = panelW - 40;

            // Feedback text label
            feedbackLabelStyle.normal.textColor = GOLDEN_ORANGE;
            GUI.Label(new Rect(x, y, w, 24), "YOUR FEEDBACK", feedbackLabelStyle);
            y += 28;

            // Feedback text area
            feedbackInputStyle.normal.textColor = SUBTLE_WHITE;
            feedbackInputStyle.normal.background = panelTexture;
            feedbackText = GUI.TextArea(new Rect(x, y, w, panelH * 0.25f), feedbackText, feedbackInputStyle);
            y += panelH * 0.25f + 12;

            // Email label
            GUI.Label(new Rect(x, y, w, 24), "EMAIL (OPTIONAL)", feedbackLabelStyle);
            y += 28;

            // Email input
            feedbackEmail = GUI.TextField(new Rect(x, y, w, 28), feedbackEmail, feedbackInputStyle);
            y += 36;

            // Issue type label
            GUI.Label(new Rect(x, y, 100, 24), "ISSUE TYPE", feedbackLabelStyle);

            // Issue type buttons (horizontal)
            float typeX = x + 110;
            for (int i = 0; i < feedbackTypes.Length; i++)
            {
                Rect typeRect = new Rect(typeX, y, 80, 26);
                bool active = i == feedbackTypeIndex;

                GUI.color = active ? CAPSULE_ACTIVE : CAPSULE_INACTIVE;
                GUI.Box(typeRect, "", GUI.skin.box);
                GUI.color = Color.white;

                DrawRectBorder(typeRect, active ? GOLDEN_ORANGE : new Color(0.3f, 0.3f, 0.3f, 0.4f), 1f);

                smallButtonStyle.normal.textColor = active ? BRIGHT_YELLOW : new Color(0.5f, 0.5f, 0.5f, 0.6f);
                if (GUI.Button(typeRect, feedbackTypes[i], smallButtonStyle))
                {
                    feedbackTypeIndex = i;
                }

                typeX += 86;
            }
            y += 36;

            // Telemetry status
            if (!string.IsNullOrEmpty(feedbackStatus))
            {
                feedbackStatusStyle.normal.textColor = feedbackStatusColor;
                GUI.Label(new Rect(x, y, w, 24), feedbackStatus, feedbackStatusStyle);
                y += 28;
            }

            // Buttons row
            float btnW = panelW * 0.35f;
            float btnH = 36;
            float btnY = panelY + panelH - btnH - 15;

            // Send button
            Rect sendRect = new Rect(x, btnY, btnW, btnH);
            DrawButtonBackground(sendRect, GOLDEN_ORANGE, 0.1f);
            sendButtonStyle.normal.textColor = GOLDEN_ORANGE;
            if (GUI.Button(sendRect, "SEND", sendButtonStyle))
            {
                SendFeedback();
            }

            // Back button
            Rect backRect = new Rect(x + w - btnW, btnY, btnW, btnH);
            DrawButtonBackground(backRect, GOLDEN_ORANGE, 0.05f);
            backButtonStyle.normal.textColor = new Color(GOLDEN_ORANGE.r, GOLDEN_ORANGE.g, GOLDEN_ORANGE.b, 0.6f);
            if (GUI.Button(backRect, "BACK", backButtonStyle))
            {
                SwitchScreen(MenuScreen.Main);
            }
        }

        // ═══════════════════════════════════════════
        //  HELPER METHODS
        // ═══════════════════════════════════════════
        private void DrawButtonBackground(Rect rect, Color color, float alpha)
        {
            Color bg = color;
            bg.a = alpha;
            GUI.color = bg;
            GUI.DrawTexture(rect, panelTexture, ScaleMode.StretchToFill);
            GUI.color = Color.white;
        }

        private void DrawRectBorder(Rect rect, Color color, float thickness)
        {
            GUI.color = color;
            // Top
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, thickness), panelTexture);
            // Bottom
            GUI.DrawTexture(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), panelTexture);
            // Left
            GUI.DrawTexture(new Rect(rect.x, rect.y, thickness, rect.height), panelTexture);
            // Right
            GUI.DrawTexture(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), panelTexture);
            GUI.color = Color.white;
        }

        private void DrawBackButton(Rect rect)
        {
            DrawButtonBackground(rect, GOLDEN_ORANGE, 0.05f);
            backButtonStyle.normal.textColor = new Color(GOLDEN_ORANGE.r, GOLDEN_ORANGE.g, GOLDEN_ORANGE.b, 0.6f);
            backButtonStyle.hover.textColor = GOLDEN_ORANGE;

            if (GUI.Button(rect, "BACK", backButtonStyle))
            {
                SwitchScreen(MenuScreen.Main);
            }
        }

        private void SwitchScreen(MenuScreen next)
        {
            currentScreen = next;
            settingsScrollPos = Vector2.zero;
            creditsScrollPos = Vector2.zero;
        }

        private void SelectSaveSlot(int slotIndex)
        {
            PlayerPrefs.SetInt("Active_LevelIndex", slotIndex);
            PlayerPrefs.SetInt("Settings_Difficulty", gameData.currentDifficulty);
            PlayerPrefs.SetFloat("Settings_Sensitivity", gameData.lookSensitivity);
            PlayerPrefs.SetFloat("Settings_MasterVolume", tempMasterVolume);
            PlayerPrefs.SetFloat("Settings_MusicVolume", tempMusicVolume);
            PlayerPrefs.SetFloat("Settings_SFXVolume", tempSfxVolume);
            PlayerPrefs.SetFloat("Settings_Brightness", tempBrightness);
            PlayerPrefs.SetInt("Settings_GraphicsQuality", tempGraphicsQuality);
            PlayerPrefs.Save();

            UnityEngine.SceneManagement.SceneManager.LoadScene("GameplayScene");
        }

        private void SendFeedback()
        {
            if (string.IsNullOrWhiteSpace(feedbackText))
            {
                feedbackStatusColor = STATUS_RED;
                feedbackStatus = "⚠ RECORD CORRUPTED: TEXT BODY CANNOT BE EMPTY";
                return;
            }

            string type = feedbackTypes[feedbackTypeIndex];
            Debug.Log($"[FEEDBACK TRANSMISSION] Issue: {type} | Email: {feedbackEmail} | Text: {feedbackText}");

            feedbackStatusColor = STATUS_GREEN;
            feedbackStatus = "📡 CONNECTING TO ANALOG SUITE... SUBMITTED!";

            feedbackText = "";
            feedbackEmail = "";
        }

        private void OnDestroy()
        {
            // Save settings on destroy
            PlayerPrefs.SetFloat("Settings_MasterVolume", tempMasterVolume);
            PlayerPrefs.SetFloat("Settings_MusicVolume", tempMusicVolume);
            PlayerPrefs.SetFloat("Settings_SFXVolume", tempSfxVolume);
            PlayerPrefs.SetFloat("Settings_Brightness", tempBrightness);
            PlayerPrefs.SetInt("Settings_GraphicsQuality", tempGraphicsQuality);
            PlayerPrefs.Save();
        }
    }
}
