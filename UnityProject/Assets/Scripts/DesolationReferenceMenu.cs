using UnityEngine;
using System;
using System.Reflection;

[DefaultExecutionOrder(10000)]
public sealed class DesolationReferenceMenu : MonoBehaviour
{
    MonoBehaviour game;
    Type type;
    FieldInfo stateField;
    MethodInfo runMethod;
    MethodInfo menuMethod;
    Texture2D gold;
    Texture2D black;
    Texture2D clear;
    GUIStyle hit;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindObjectOfType<DesolationReferenceMenu>() != null) return;
        GameObject o = new GameObject("DesolationReferenceMenu");
        DontDestroyOnLoad(o);
        o.AddComponent<DesolationReferenceMenu>();
    }

    void Start()
    {
        gold = Pixel(new Color(1f, .78f, .18f, .95f));
        black = Pixel(new Color(0f, 0f, 0f, .75f));
        clear = Pixel(new Color(0f, 0f, 0f, 0f));
        hit = new GUIStyle(GUI.skin.button);
        hit.normal.background = clear;
        hit.hover.background = clear;
        hit.active.background = clear;
        hit.focused.background = clear;
        hit.normal.textColor = Color.clear;
        hit.hover.textColor = Color.clear;
        hit.active.textColor = Color.clear;
        hit.focused.textColor = Color.clear;
    }

    Texture2D Pixel(Color c)
    {
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, c);
        t.Apply();
        return t;
    }

    bool Link()
    {
        if (game != null) return true;
        game = FindObjectOfType<FirstPlayableBatch>();
        if (game == null) return false;
        type = game.GetType();
        stateField = type.GetField("s", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        runMethod = type.GetMethod("Run", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        menuMethod = type.GetMethod("Menu", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        return stateField != null;
    }

    void SetState(string value)
    {
        if (!Link()) return;
        stateField.SetValue(game, Enum.Parse(stateField.FieldType, value));
    }

    void Call(MethodInfo m)
    {
        if (!Link() || m == null) return;
        m.Invoke(game, null);
    }

    Rect R(float x, float y, float w, float h)
    {
        return new Rect(Screen.width * x, Screen.height * y, Screen.width * w, Screen.height * h);
    }

    bool Hot(float x, float y, float w, float h)
    {
        return GUI.Button(R(x, y, w, h), "", hit);
    }

    void Line(Rect r)
    {
        GUI.color = new Color(1f, .78f, .18f, .9f);
        GUI.DrawTexture(r, gold);
        GUI.color = Color.white;
    }

    void Label(Rect r, string text, int size, TextAnchor anchor)
    {
        GUIStyle s = new GUIStyle(GUI.skin.label);
        s.fontSize = size;
        s.fontStyle = FontStyle.Bold;
        s.alignment = anchor;
        s.normal.textColor = new Color(0f, 0f, 0f, .82f);
        Rect shadow = r;
        shadow.x += 3f;
        shadow.y += 3f;
        GUI.Label(shadow, text, s);
        s.normal.textColor = new Color(1f, .82f, .28f, .98f);
        GUI.Label(r, text, s);
    }

    void Panel(Rect r)
    {
        GUI.color = new Color(0f, 0f, 0f, .42f);
        GUI.DrawTexture(r, black);
        GUI.color = new Color(1f, .78f, .18f, .65f);
        GUI.DrawTexture(new Rect(r.x, r.y, r.width, 2), gold);
        GUI.DrawTexture(new Rect(r.x, r.yMax - 2, r.width, 2), gold);
        GUI.DrawTexture(new Rect(r.x, r.y, 2, r.height), gold);
        GUI.DrawTexture(new Rect(r.xMax - 2, r.y, 2, r.height), gold);
        GUI.color = Color.white;
    }

    void MenuWord(float x, float y, string text)
    {
        Label(R(x, y, .28f, .08f), text, 34, TextAnchor.MiddleLeft);
        Line(R(x, y + .075f, .17f, .004f));
        Line(R(x + .167f, y + .072f, .005f, .010f));
    }

    void DrawHeader(string page)
    {
        Label(R(.29f, .04f, .49f, .13f), "DESOLATION:", 56, TextAnchor.MiddleCenter);
        Label(R(.35f, .15f, .37f, .07f), "THE BACKROOMS", 28, TextAnchor.MiddleCenter);
        if (page != "") Label(R(.39f, .25f, .24f, .08f), page, 36, TextAnchor.MiddleCenter);
    }

    void OnGUI()
    {
        if (!Link()) return;
        string state = stateField.GetValue(game).ToString();
        if (state == "Game" || state == "Pause" || state == "End") return;
        GUI.color = new Color(0f, 0f, 0f, .08f);
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), black);
        GUI.color = Color.white;

        if (state == "Menu")
        {
            DrawHeader("");
            MenuWord(.045f, .29f, "PLAY");
            MenuWord(.045f, .52f, "SETTINGS");
            MenuWord(.045f, .86f, "CREDITS");
            MenuWord(.80f, .86f, "FEEDBACK");
            if (Hot(.015f, .18f, .28f, .20f)) Call(runMethod);
            if (Hot(.015f, .43f, .32f, .18f)) SetState("Settings");
            if (Hot(.015f, .78f, .32f, .20f)) SetState("Status");
            if (Hot(.76f, .78f, .23f, .20f)) SetState("Missing");
            return;
        }

        if (state == "Settings")
        {
            DrawHeader("SETTINGS");
            Panel(R(.24f, .34f, .52f, .49f));
            Row(.38f, "MASTER VOLUME", .88f);
            Row(.47f, "MUSIC VOLUME", .60f);
            Row(.56f, "SFX VOLUME", .68f);
            Row(.65f, "BRIGHTNESS", .66f);
            Row(.74f, "SENSITIVITY", .72f);
            Label(R(.27f, .81f, .14f, .05f), "GRAPHICS", 24, TextAnchor.MiddleLeft);
            ButtonBox(.45f, .805f, "LOW", false);
            ButtonBox(.55f, .805f, "MEDIUM", false);
            ButtonBox(.66f, .805f, "HIGH", true);
            ButtonBox(.43f, .90f, "BACK", true);
            if (Hot(.42f, .88f, .20f, .11f)) Call(menuMethod);
            return;
        }

        if (state == "Status")
        {
            DrawHeader("CREDITS");
            Panel(R(.24f, .39f, .52f, .43f));
            Credit(.43f, "GAME DESIGN", "SOLO DEVELOPER");
            Credit(.51f, "PROGRAMMING", "SOLO DEVELOPER");
            Credit(.59f, "UI DESIGN", "SOLO DEVELOPER");
            Credit(.67f, "ENVIRONMENT ART", "SOLO DEVELOPER");
            Credit(.75f, "SPECIAL THANKS", "THE PLAYERS");
            ButtonBox(.43f, .89f, "BACK", true);
            if (Hot(.42f, .86f, .20f, .12f)) Call(menuMethod);
            return;
        }

        if (state == "Missing")
        {
            DrawHeader("FEEDBACK");
            Panel(R(.28f, .39f, .44f, .42f));
            Label(R(.31f, .41f, .20f, .05f), "YOUR FEEDBACK", 18, TextAnchor.MiddleLeft);
            Panel(R(.31f, .47f, .38f, .16f));
            Label(R(.32f, .49f, .32f, .04f), "WRITE YOUR FEEDBACK HERE...", 17, TextAnchor.MiddleLeft);
            Label(R(.31f, .65f, .20f, .05f), "EMAIL (OPTIONAL)", 18, TextAnchor.MiddleLeft);
            Panel(R(.31f, .70f, .38f, .06f));
            Label(R(.32f, .705f, .32f, .04f), "ENTER YOUR EMAIL...", 17, TextAnchor.MiddleLeft);
            ButtonBox(.39f, .84f, "SEND", true);
            ButtonBox(.44f, .92f, "BACK", true);
            if (Hot(.43f, .90f, .18f, .09f)) Call(menuMethod);
        }
    }

    void Row(float y, string name, float value)
    {
        Label(R(.27f, y, .18f, .05f), name, 20, TextAnchor.MiddleLeft);
        Line(R(.45f, y + .028f, .28f, .006f));
        GUI.color = new Color(1f, .82f, .28f, .96f);
        GUI.DrawTexture(R(.45f + .28f * value - .006f, y + .014f, .014f, .030f), gold);
        GUI.color = Color.white;
    }

    void ButtonBox(float x, float y, string text, bool active)
    {
        Rect r = R(x, y, .08f, .055f);
        Panel(r);
        Label(r, text, 18, TextAnchor.MiddleCenter);
        if (!active) return;
        GUI.color = new Color(1f, .82f, .28f, .22f);
        GUI.DrawTexture(r, gold);
        GUI.color = Color.white;
    }

    void Credit(float y, string left, string right)
    {
        Label(R(.31f, y, .20f, .05f), left, 20, TextAnchor.MiddleLeft);
        Line(R(.48f, y + .030f, .12f, .003f));
        Label(R(.58f, y, .22f, .05f), right, 20, TextAnchor.MiddleLeft);
    }
}
