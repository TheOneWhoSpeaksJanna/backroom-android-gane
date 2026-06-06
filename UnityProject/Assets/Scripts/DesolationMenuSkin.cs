using UnityEngine;
using System;
using System.Reflection;

public sealed class DesolationMenuSkin : MonoBehaviour
{
    object game;
    Type gameType;
    FieldInfo stateField;
    MethodInfo runMethod;
    MethodInfo menuMethod;
    GUIStyle invisible;
    Texture2D px;
    Texture2D gold;
    Texture2D dark;
    string mode = "menu";
    float volume;
    float brightness;
    float sensitivity;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindObjectOfType<DesolationMenuSkin>() != null) return;
        GameObject o = new GameObject("DesolationMenuSkin");
        DontDestroyOnLoad(o);
        o.AddComponent<DesolationMenuSkin>();
    }

    void Start()
    {
        volume = PlayerPrefs.GetFloat("desolation_volume", .9f);
        brightness = PlayerPrefs.GetFloat("desolation_brightness", .9f);
        sensitivity = PlayerPrefs.GetFloat("desolation_sensitivity", 1f);
        px = Make(Color.white);
        gold = Make(new Color(1f, .78f, .16f, 1f));
        dark = Make(Color.black);
        invisible = new GUIStyle(GUI.skin.button);
        invisible.normal.background = px;
        invisible.hover.background = px;
        invisible.active.background = px;
        invisible.normal.textColor = Color.clear;
        invisible.hover.textColor = Color.clear;
        invisible.active.textColor = Color.clear;
    }

    Texture2D Make(Color color)
    {
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, color);
        t.Apply();
        return t;
    }

    bool Link()
    {
        if (game != null) return true;
        FirstPlayableBatch g = FindObjectOfType<FirstPlayableBatch>();
        if (g == null) return false;
        game = g;
        gameType = g.GetType();
        stateField = gameType.GetField("s", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        runMethod = gameType.GetMethod("Run", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        menuMethod = gameType.GetMethod("Menu", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        return stateField != null;
    }

    string State()
    {
        if (!Link()) return "";
        object value = stateField.GetValue(game);
        return value == null ? "" : value.ToString();
    }

    void Invoke(MethodInfo method)
    {
        if (method != null) method.Invoke(game, null);
    }

    Rect N(float x, float y, float w, float h)
    {
        return new Rect(Screen.width * x, Screen.height * y, Screen.width * w, Screen.height * h);
    }

    void Fill(Rect r, Color c)
    {
        GUI.color = c;
        GUI.DrawTexture(r, px);
        GUI.color = Color.white;
    }

    bool Hit(float x, float y, float w, float h)
    {
        GUI.color = Color.clear;
        bool v = GUI.Button(N(x, y, w, h), "", invisible);
        GUI.color = Color.white;
        return v;
    }

    void GlowText(Rect r, string text, int size, TextAnchor anchor)
    {
        GUIStyle st = new GUIStyle(GUI.skin.label);
        st.fontSize = size;
        st.fontStyle = FontStyle.Bold;
        st.alignment = anchor;
        st.normal.textColor = new Color(0f, 0f, 0f, .75f);
        Rect shadow = r;
        shadow.x += 4f;
        shadow.y += 4f;
        GUI.Label(shadow, text, st);
        st.normal.textColor = new Color(1f, .82f, .28f, .98f);
        GUI.Label(r, text, st);
    }

    void Line(Rect r)
    {
        Fill(r, new Color(1f, .78f, .16f, .85f));
    }

    void Panel(Rect r)
    {
        Fill(r, new Color(0f, 0f, 0f, .60f));
        Line(new Rect(r.x, r.y, r.width, 2f));
        Line(new Rect(r.x, r.yMax - 2f, r.width, 2f));
        Line(new Rect(r.x, r.y, 2f, r.height));
        Line(new Rect(r.xMax - 2f, r.y, 2f, r.height));
    }

    void Backdrop()
    {
        Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(.10f, .085f, .015f, .88f));
        for (int i = 0; i < 11; i++)
        {
            float x = Screen.width * (.06f + i * .095f);
            Fill(new Rect(x, Screen.height * .18f, Screen.width * .055f, Screen.height * .78f), new Color(.50f, .42f, .12f, .23f));
            Line(new Rect(x, Screen.height * .18f, Screen.width * .055f, 2f));
        }
        Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0f, 0f, 0f, .18f));
    }

    void Header(string small)
    {
        GlowText(N(.25f, .035f, .55f, .13f), "DESOLATION:", 58, TextAnchor.MiddleCenter);
        GlowText(N(.34f, .145f, .38f, .07f), "THE BACKROOMS", 28, TextAnchor.MiddleCenter);
        if (small != "") GlowText(N(.40f, .245f, .22f, .07f), small, 36, TextAnchor.MiddleCenter);
    }

    void MenuWord(float x, float y, string text)
    {
        GlowText(N(x, y, .28f, .08f), text, 36, TextAnchor.MiddleLeft);
        Line(N(x, y + .075f, .17f, .004f));
        Line(N(x + .166f, y + .071f, .006f, .012f));
    }

    void DrawMenu()
    {
        Backdrop();
        Header("");
        MenuWord(.045f, .29f, "PLAY");
        MenuWord(.045f, .52f, "SETTINGS");
        MenuWord(.045f, .86f, "CREDITS");
        MenuWord(.80f, .86f, "FEEDBACK");
        if (Hit(.02f, .22f, .25f, .20f)) Invoke(runMethod);
        if (Hit(.02f, .45f, .31f, .17f)) mode = "settings";
        if (Hit(.02f, .78f, .31f, .20f)) mode = "credits";
        if (Hit(.75f, .78f, .24f, .20f)) mode = "feedback";
    }

    void Choice(float x, float y, string text, bool active)
    {
        Rect r = N(x, y, .08f, .055f);
        Panel(r);
        if (active) Fill(r, new Color(1f, .78f, .16f, .22f));
        GlowText(r, text, 18, TextAnchor.MiddleCenter);
    }

    void SettingRow(float y, string name, float val, float min, float max)
    {
        GlowText(N(.27f, y, .18f, .05f), name, 20, TextAnchor.MiddleLeft);
        Line(N(.45f, y + .028f, .28f, .006f));
        float t = Mathf.InverseLerp(min, max, val);
        Fill(N(.45f + .28f * t - .006f, y + .014f, .014f, .030f), new Color(1f, .82f, .28f, .95f));
    }

    void DrawSettings()
    {
        Backdrop();
        Header("SETTINGS");
        Panel(N(.24f, .34f, .52f, .49f));
        SettingRow(.38f, "MASTER VOLUME", volume, 0f, 1f);
        SettingRow(.47f, "MUSIC VOLUME", volume * .82f, 0f, 1f);
        SettingRow(.56f, "SFX VOLUME", volume * .86f, 0f, 1f);
        SettingRow(.65f, "BRIGHTNESS", brightness, .35f, 1.2f);
        SettingRow(.74f, "SENSITIVITY", sensitivity, .35f, 2f);
        GlowText(N(.27f, .81f, .14f, .05f), "GRAPHICS", 24, TextAnchor.MiddleLeft);
        Choice(.45f, .805f, "LOW", false);
        Choice(.55f, .805f, "MEDIUM", false);
        Choice(.66f, .805f, "HIGH", true);
        Choice(.43f, .90f, "BACK", true);
        HandleSettings();
        if (Hit(.42f, .88f, .19f, .10f))
        {
            PlayerPrefs.SetFloat("desolation_volume", volume);
            PlayerPrefs.SetFloat("desolation_brightness", brightness);
            PlayerPrefs.SetFloat("desolation_sensitivity", sensitivity);
            PlayerPrefs.Save();
            AudioListener.volume = volume;
            mode = "menu";
        }
    }

    void HandleSettings()
    {
        Event ev = Event.current;
        if (ev == null || (ev.type != EventType.MouseDown && ev.type != EventType.MouseDrag)) return;
        Vector2 p = ev.mousePosition;
        SetFrom(p, N(.45f, .38f, .28f, .045f), ref volume, 0f, 1f);
        SetFrom(p, N(.45f, .65f, .28f, .045f), ref brightness, .35f, 1.2f);
        SetFrom(p, N(.45f, .74f, .28f, .045f), ref sensitivity, .35f, 2f);
        if (N(.45f, .805f, .08f, .055f).Contains(p)) Application.targetFrameRate = 40;
        if (N(.55f, .805f, .08f, .055f).Contains(p)) Application.targetFrameRate = 50;
        if (N(.66f, .805f, .08f, .055f).Contains(p)) Application.targetFrameRate = 60;
    }

    void SetFrom(Vector2 p, Rect r, ref float value, float min, float max)
    {
        if (r.Contains(p)) value = Mathf.Lerp(min, max, Mathf.InverseLerp(r.xMin, r.xMax, p.x));
    }

    void DrawCredits()
    {
        Backdrop();
        Header("CREDITS");
        Panel(N(.24f, .39f, .52f, .43f));
        Credit(.43f, "GAME DESIGN", "SOLO DEVELOPER");
        Credit(.51f, "PROGRAMMING", "SOLO DEVELOPER");
        Credit(.59f, "UI DESIGN", "SOLO DEVELOPER");
        Credit(.67f, "ENVIRONMENT ART", "SOLO DEVELOPER");
        Credit(.75f, "SPECIAL THANKS", "THE PLAYERS");
        Choice(.43f, .89f, "BACK", true);
        if (Hit(.42f, .86f, .20f, .12f)) mode = "menu";
    }

    void Credit(float y, string left, string right)
    {
        GlowText(N(.31f, y, .20f, .05f), left, 20, TextAnchor.MiddleLeft);
        Line(N(.48f, y + .030f, .12f, .003f));
        GlowText(N(.58f, y, .22f, .05f), right, 20, TextAnchor.MiddleLeft);
    }

    void DrawFeedback()
    {
        Backdrop();
        Header("FEEDBACK");
        Panel(N(.28f, .39f, .44f, .42f));
        GlowText(N(.31f, .41f, .20f, .05f), "YOUR FEEDBACK", 18, TextAnchor.MiddleLeft);
        Panel(N(.31f, .47f, .38f, .16f));
        GlowText(N(.32f, .49f, .32f, .04f), "WRITE YOUR FEEDBACK HERE...", 17, TextAnchor.MiddleLeft);
        GlowText(N(.31f, .65f, .20f, .05f), "EMAIL (OPTIONAL)", 18, TextAnchor.MiddleLeft);
        Panel(N(.31f, .70f, .38f, .06f));
        GlowText(N(.32f, .705f, .32f, .04f), "ENTER YOUR EMAIL...", 17, TextAnchor.MiddleLeft);
        Choice(.39f, .84f, "SEND", true);
        Choice(.44f, .92f, "BACK", true);
        if (Hit(.43f, .90f, .18f, .09f)) mode = "menu";
    }

    void OnGUI()
    {
        if (!Link()) return;
        if (State() != "Menu") return;
        GUI.depth = -10000;
        if (mode == "settings") DrawSettings();
        else if (mode == "credits") DrawCredits();
        else if (mode == "feedback") DrawFeedback();
        else DrawMenu();
    }
}
