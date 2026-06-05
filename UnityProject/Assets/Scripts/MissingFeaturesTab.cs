using UnityEngine;

public sealed class MissingFeaturesTab : MonoBehaviour
{
    bool open;
    Vector2 scroll;
    Texture2D gold, panel, dark;
    GUIStyle tab, title, body, small;

    const string Checklist =
@"WHAT IS STILL MISSING

CORE GAMEPLAY
- Full first-person playable loop, not just the menu shell.
- Real exploration objectives, notes, clue chains, and multiple endings.
- Inventory, item inspection, keys, fuses, lockers, doors, and breaker puzzles.
- Win/loss rules, fail states, tutorial prompts, and checkpoint rules.

ENEMY / HORROR AI
- Patrol, search, chase, attack, and lose-player states.
- Line-of-sight, hearing, hiding validation, and distraction tools.
- Entity animations, sound cues, jumpscare timing, and difficulty scaling.

LEVELS / CONTENT
- Modular Level 0 room prefabs and a larger maze layout.
- Damaged wallpaper, dead ends, maintenance rooms, offices, landmarks, and exits.
- More props, doors, vents, signs, stains, outlets, trim, and ceiling details.
- Additional Backrooms levels after Level 0.

MOBILE FEATURES
- Customizable touch controls, left/right hand layouts, and sensitivity calibration.
- Better Android back-button, pause/resume, haptics, and accessibility scaling.
- Real save-slot resume UX and save-slot screenshots.
- Low-data APK download/install instructions.

VISUAL POLISH
- Final authored textures and materials.
- Broken lights, dark pockets, flicker zones, fog tuning, shadows, and better exposure.
- Post-processing: film grain, vignette, color grading, bloom, and head bob.
- Menu transitions and stronger button feedback.

AUDIO POLISH
- Final fluorescent hums, carpet footsteps, ambience, buzzes, distant thuds, and UI clicks.
- Entity sounds, chase sounds, jumpscare stingers, and spatial audio.
- Audio mixer groups for master/music/SFX and compression for APK size.

SYSTEMS
- Real feedback sending or local export instead of only saving feedback text locally.
- Final credits names, progress tracking, collectibles, endings, and error screens.
- Better settings persistence and recovery if a save is corrupted.

PERFORMANCE / RELEASE
- Release export mode instead of debug export.
- Texture compression, audio compression, and unused asset removal.
- Baked/static lighting, occlusion/visibility optimization, and low-end Android quality.
- Real-device testing for crashes, heat, memory, frame rate, install, and resume.

STORE / FINAL PACKAGE
- Final app icon, splash screen, screenshots, trailer, description, versioning, and release notes.
- Privacy text if feedback, analytics, or crash logs are added.
- Clean automated APK publishing every time.

BEST NEXT ORDER
1. Finish the playable first-person gameplay loop.
2. Add real save-slot resume with screenshots.
3. Build modular rooms and landmarks.
4. Upgrade enemy AI and hiding logic.
5. Add final audio/visual polish.
6. Optimize APK size and Android performance.
7. Test on real Android devices and publish a release build.";

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindObjectOfType<MissingFeaturesTab>() != null) return;
        GameObject g = new GameObject("MissingFeaturesTab");
        DontDestroyOnLoad(g);
        g.AddComponent<MissingFeaturesTab>();
    }

    Texture2D Tex(Color c)
    {
        Texture2D t = new Texture2D(1, 1);
        t.SetPixel(0, 0, c);
        t.Apply();
        return t;
    }

    void Setup()
    {
        if (gold != null) return;
        gold = Tex(new Color(1f, .78f, .16f, .95f));
        panel = Tex(new Color(0, 0, 0, .82f));
        dark = Tex(new Color(0, 0, 0, .45f));

        tab = new GUIStyle(GUI.skin.button) { fontSize = 24, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        tab.normal.background = Tex(new Color(0, 0, 0, .35f));
        tab.hover.background = Tex(new Color(.25f, .18f, 0, .55f));
        tab.active.background = tab.hover.background;
        tab.normal.textColor = new Color(1f, .82f, .3f);
        tab.hover.textColor = Color.white;

        title = new GUIStyle(GUI.skin.label) { fontSize = 34, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        title.normal.textColor = new Color(1f, .82f, .3f);

        body = new GUIStyle(GUI.skin.label) { fontSize = 18, fontStyle = FontStyle.Bold, wordWrap = true };
        body.normal.textColor = new Color(1f, .82f, .3f);

        small = new GUIStyle(body) { fontSize = 14, alignment = TextAnchor.MiddleCenter };
    }

    void OnGUI()
    {
        Setup();
        Rect tabRect = new Rect(Screen.width - 238, 18, 218, 52);
        if (GUI.Button(tabRect, open ? "CLOSE MISSING" : "MISSING", tab)) open = !open;
        Border(tabRect);

        if (!open) return;

        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), dark);
        Rect box = new Rect(Screen.width * .11f, Screen.height * .13f, Screen.width * .78f, Screen.height * .76f);
        GUI.DrawTexture(box, panel);
        Border(box);

        GUI.Label(new Rect(box.x, box.y + 14, box.width, 46), "WHAT ARE WE MISSING", title);
        GUI.Label(new Rect(box.x + 28, box.y + 58, box.width - 56, 24), "Full live checklist for the whole game.", small);

        Rect view = new Rect(box.x + 30, box.y + 88, box.width - 60, box.height - 148);
        Rect content = new Rect(0, 0, view.width - 28, 1180);
        scroll = GUI.BeginScrollView(view, scroll, content);
        GUI.Label(new Rect(0, 0, content.width, content.height), Checklist, body);
        GUI.EndScrollView();

        Rect close = new Rect(box.x + box.width / 2 - 90, box.yMax - 52, 180, 42);
        if (GUI.Button(close, "BACK", tab)) open = false;
        Border(close);
    }

    void Border(Rect r)
    {
        GUI.DrawTexture(new Rect(r.x, r.y, r.width, 2), gold);
        GUI.DrawTexture(new Rect(r.x, r.yMax - 2, r.width, 2), gold);
        GUI.DrawTexture(new Rect(r.x, r.y, 2, r.height), gold);
        GUI.DrawTexture(new Rect(r.xMax - 2, r.y, 2, r.height), gold);
    }
}
