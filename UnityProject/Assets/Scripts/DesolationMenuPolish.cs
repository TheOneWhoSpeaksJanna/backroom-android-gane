using UnityEngine;
using System;
using System.Reflection;

[DefaultExecutionOrder(10000)]
public sealed class DesolationMenuPolish : MonoBehaviour
{
    object game;
    FieldInfo stateField;
    MethodInfo runMethod;
    MethodInfo menuMethod;
    GUIStyle hit;
    Texture2D px;
    int page;
    float volume = .9f, brightness = .9f, sensitivity = 1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindObjectOfType<DesolationMenuPolish>() != null) return;
        GameObject o = new GameObject("DesolationMenuPolish");
        DontDestroyOnLoad(o);
        o.AddComponent<DesolationMenuPolish>();
    }

    void Start()
    {
        volume = PlayerPrefs.GetFloat("desolation_volume", .9f);
        brightness = PlayerPrefs.GetFloat("desolation_brightness", .9f);
        sensitivity = PlayerPrefs.GetFloat("desolation_sensitivity", 1f);
        px = new Texture2D(1, 1);
        px.SetPixel(0, 0, Color.white);
        px.Apply();
        hit = new GUIStyle(GUI.skin.button);
        hit.normal.background = hit.hover.background = hit.active.background = null;
        hit.normal.textColor = hit.hover.textColor = hit.active.textColor = Color.clear;
    }

    bool Link()
    {
        if (game != null) return true;
        UnityEngine.Object obj = FindObjectOfType<FirstPlayableBatch>();
        if (obj == null) return false;
        game = obj;
        Type t = obj.GetType();
        BindingFlags f = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        stateField = t.GetField("s", f);
        runMethod = t.GetMethod("Run", f);
        menuMethod = t.GetMethod("Menu", f);
        return true;
    }

    string State()
    {
        if (!Link() || stateField == null) return "";
        object value = stateField.GetValue(game);
        return value == null ? "" : value.ToString();
    }

    Rect R(float x, float y, float w, float h) { return new Rect(Screen.width * x, Screen.height * y, Screen.width * w, Screen.height * h); }

    void Fill(Rect r, Color c)
    {
        GUI.color = c;
        GUI.DrawTexture(r, px);
        GUI.color = Color.white;
    }

    string Spread(string s)
    {
        if (s.Length > 18) return s;
        return string.Join(" ", s.ToCharArray());
    }

    void Label(Rect r, string text, int size, TextAnchor align)
    {
        GUIStyle st = new GUIStyle(GUI.skin.label);
        st.fontSize = Mathf.RoundToInt(size * Mathf.Clamp(Screen.height / 720f, .8f, 1.6f));
        st.fontStyle = FontStyle.Bold;
        st.alignment = align;
        st.normal.textColor = new Color(0, 0, 0, .85f);
        Rect sh = r;
        sh.x += 3;
        sh.y += 3;
        GUI.Label(sh, Spread(text), st);
        st.normal.textColor = new Color(1f, .82f, .25f, 1f);
        GUI.Label(r, Spread(text), st);
    }

    void Panel(Rect r)
    {
        Fill(r, new Color(0, 0, 0, .56f));
        Color gold = new Color(1f, .82f, .25f, .72f);
        Fill(new Rect(r.x, r.y, r.width, 2), gold);
        Fill(new Rect(r.x, r.yMax - 2, r.width, 2), gold);
        Fill(new Rect(r.x, r.y, 2, r.height), gold);
        Fill(new Rect(r.xMax - 2, r.y, 2, r.height), gold);
    }

    void Line(float x, float y, float w)
    {
        Fill(R(x, y, w, .004f), new Color(1f, .82f, .25f, .9f));
        Fill(R(x + w, y - .003f, .004f, .010f), new Color(1f, .82f, .25f, .9f));
    }

    bool Hot(float x, float y, float w, float h) { return GUI.Button(R(x, y, w, h), "", hit); }

    void Header(string title)
    {
        Label(R(.29f, .04f, .52f, .13f), "DESOLATION:", 56, TextAnchor.MiddleCenter);
        Label(R(.35f, .15f, .40f, .07f), "THE BACKROOMS", 30, TextAnchor.MiddleCenter);
        Label(R(.40f, .25f, .24f, .08f), title, 38, TextAnchor.MiddleCenter);
        Line(.43f, .335f, .13f);
    }

    void Word(float x, float y, string text)
    {
        Label(R(x, y, .30f, .075f), text, 34, TextAnchor.MiddleLeft);
        Line(x, y + .073f, .17f);
    }

    void MenuArt()
    {
        Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0, 0, 0, .18f));
        Label(R(.29f, .055f, .52f, .15f), "DESOLATION:", 66, TextAnchor.MiddleCenter);
        Label(R(.35f, .17f, .40f, .08f), "THE BACKROOMS", 34, TextAnchor.MiddleCenter);
        Word(.045f, .285f, "PLAY");
        Word(.045f, .520f, "SETTINGS");
        Word(.045f, .860f, "CREDITS");
        Word(.800f, .860f, "FEEDBACK");
        if (Hot(.02f, .23f, .25f, .18f) && runMethod != null) runMethod.Invoke(game, null);
        if (Hot(.02f, .47f, .31f, .18f)) page = 1;
        if (Hot(.02f, .80f, .30f, .18f)) page = 2;
        if (Hot(.76f, .80f, .23f, .18f)) page = 3;
    }

    void SliderRow(float y, string name, float value, float min, float max)
    {
        Label(R(.27f, y, .19f, .05f), name, 20, TextAnchor.MiddleLeft);
        Fill(R(.45f, y + .028f, .28f, .006f), new Color(1f, .82f, .25f, .75f));
        float t = Mathf.InverseLerp(min, max, value);
        Fill(R(.45f + .28f * t - .006f, y + .014f, .014f, .030f), new Color(1f, .82f, .25f, 1f));
    }

    void SmallBox(float x, float y, string text, bool on)
    {
        Rect r = R(x, y, .085f, .055f);
        Panel(r);
        if (on) Fill(r, new Color(1f, .82f, .25f, .22f));
        Label(r, text, 18, TextAnchor.MiddleCenter);
    }

    void SettingsArt()
    {
        Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0, 0, 0, .38f));
        Header("SETTINGS");
        Panel(R(.24f, .34f, .52f, .50f));
        SliderRow(.38f, "MASTER VOLUME", volume, 0, 1);
        SliderRow(.47f, "MUSIC VOLUME", volume * .82f, 0, 1);
        SliderRow(.56f, "SFX VOLUME", volume * .86f, 0, 1);
        SliderRow(.65f, "BRIGHTNESS", brightness, .35f, 1.2f);
        SliderRow(.74f, "SENSITIVITY", sensitivity, .35f, 2f);
        Label(R(.27f, .81f, .15f, .05f), "GRAPHICS", 24, TextAnchor.MiddleLeft);
        SmallBox(.45f, .805f, "LOW", false);
        SmallBox(.55f, .805f, "MEDIUM", false);
        SmallBox(.66f, .805f, "HIGH", true);
        SmallBox(.43f, .90f, "BACK", true);

        Event e = Event.current;
        if (e != null && (e.type == EventType.MouseDown || e.type == EventType.MouseDrag))
        {
            Vector2 m = e.mousePosition;
            Drag(m, R(.45f, .38f, .28f, .045f), ref volume, 0, 1);
            Drag(m, R(.45f, .65f, .28f, .045f), ref brightness, .35f, 1.2f);
            Drag(m, R(.45f, .74f, .28f, .045f), ref sensitivity, .35f, 2f);
        }

        if (Hot(.42f, .88f, .19f, .10f))
        {
            PlayerPrefs.SetFloat("desolation_volume", volume);
            PlayerPrefs.SetFloat("desolation_brightness", brightness);
            PlayerPrefs.SetFloat("desolation_sensitivity", sensitivity);
            PlayerPrefs.Save();
            AudioListener.volume = volume;
            page = 0;
            if (menuMethod != null) menuMethod.Invoke(game, null);
        }
    }

    void Drag(Vector2 m, Rect r, ref float v, float min, float max)
    {
        if (r.Contains(m)) v = Mathf.Lerp(min, max, Mathf.InverseLerp(r.xMin, r.xMax, m.x));
    }

    void CreditsArt()
    {
        Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0, 0, 0, .38f));
        Header("CREDITS");
        Panel(R(.24f, .39f, .52f, .43f));
        Credit(.43f, "GAME DESIGN", "SOLO DEVELOPER");
        Credit(.51f, "PROGRAMMING", "SOLO DEVELOPER");
        Credit(.59f, "UI DESIGN", "SOLO DEVELOPER");
        Credit(.67f, "ENVIRONMENT ART", "SOLO DEVELOPER");
        Credit(.75f, "SPECIAL THANKS", "THE PLAYERS");
        SmallBox(.43f, .89f, "BACK", true);
        if (Hot(.42f, .86f, .20f, .12f)) page = 0;
    }

    void Credit(float y, string a, string b)
    {
        Label(R(.31f, y, .20f, .05f), a, 20, TextAnchor.MiddleLeft);
        Fill(R(.48f, y + .030f, .12f, .003f), new Color(1f, .82f, .25f, .65f));
        Label(R(.58f, y, .22f, .05f), b, 20, TextAnchor.MiddleLeft);
    }

    void FeedbackArt()
    {
        Fill(new Rect(0, 0, Screen.width, Screen.height), new Color(0, 0, 0, .38f));
        Header("FEEDBACK");
        Panel(R(.28f, .39f, .44f, .42f));
        Label(R(.31f, .41f, .20f, .05f), "YOUR FEEDBACK", 18, TextAnchor.MiddleLeft);
        Panel(R(.31f, .47f, .38f, .16f));
        Label(R(.32f, .49f, .32f, .04f), "WRITE YOUR FEEDBACK HERE...", 17, TextAnchor.MiddleLeft);
        Label(R(.31f, .65f, .20f, .05f), "EMAIL (OPTIONAL)", 18, TextAnchor.MiddleLeft);
        Panel(R(.31f, .70f, .38f, .06f));
        Label(R(.32f, .705f, .32f, .04f), "ENTER YOUR EMAIL...", 17, TextAnchor.MiddleLeft);
        SmallBox(.39f, .84f, "SEND", true);
        SmallBox(.44f, .92f, "BACK", true);
        if (Hot(.43f, .90f, .18f, .09f)) page = 0;
    }

    void OnGUI()
    {
        if (!Link()) return;
        string state = State();
        if (state != "Menu" && state != "Settings") return;
        if (page == 0) MenuArt();
        else if (page == 1) SettingsArt();
        else if (page == 2) CreditsArt();
        else FeedbackArt();
    }
}
