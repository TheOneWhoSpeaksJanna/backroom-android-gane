using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// FirstPlayableBatch — hardcoded first-person Backrooms experience.
/// Stripped of OnGUI menu and DesolationRuntime disabling.
/// The DesolationRuntime.cs now handles all menu screens.
/// </summary>
public sealed class FirstPlayableBatch : MonoBehaviour
{
    // ── STATE ──
    enum GameState { Menu, Game, Pause, End, Settings, Status, Missing }
    GameState s;

    Camera cam;
    CharacterController cc;
    GameObject p, e;

    GUIStyle l, b, c;
    Texture2D gd, dk, rd, gn;
    Material wall, wall2, flr, ceil, met, glw, pap;

    List<GameObject> items = new List<GameObject>();
    List<Light> lights = new List<Light>();

    string[] notes = {
        "The carpet is wet, but no pipe runs above it.",
        "Three fuses wake the grid. Two switches open maintenance.",
        "Blue opens what yellow cannot.",
        "Lockers only make you quiet.",
        "The black door wants every note."
    };

    int f, n;
    float hp = 100, sa = 100, st = 100, yaw, pit, msgT, decT, vol = .9f, sen = 1, br = .9f;
    bool yk, bk, pow, swA, swB, run, use, hide, dec;
    string msg = "", end = "";
    Vector3 target, decPos;

    // ── BOOT ──
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindObjectOfType<FirstPlayableBatch>()) return;
        var o = new GameObject("FirstPlayableBatch");
        DontDestroyOnLoad(o);
        o.AddComponent<FirstPlayableBatch>();
    }

    // ── AWAKE — DesolationRuntime is NOT disabled ──
    void Awake()
    {
        // DesolationRuntime handles the menu now — do NOT disable it
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        vol = PlayerPrefs.GetFloat("desolation_volume", .9f);
        sen = PlayerPrefs.GetFloat("desolation_sensitivity", 1);
        br = PlayerPrefs.GetFloat("desolation_brightness", .9f);
        AudioListener.volume = vol;
        Tex();
        Style();
        Mats();
        World();
        Menu();
    }

    // ── TEXTURES ──
    Texture2D Px(Color x)
    {
        var t = new Texture2D(1, 1);
        t.SetPixel(0, 0, x);
        t.Apply();
        return t;
    }

    void Tex()
    {
        gd = Px(new Color(1, .78f, .16f, .95f));
        dk = Px(new Color(0, 0, 0, .76f));
        rd = Px(new Color(.72f, .03f, .02f, .78f));
        gn = Px(new Color(.05f, .56f, .14f, .78f));
    }

    // ── STYLES ──
    void Style()
    {
        l = new GUIStyle(GUI.skin.label) { fontSize = 22, fontStyle = FontStyle.Bold, wordWrap = true };
        b = new GUIStyle(GUI.skin.button) { fontSize = 24, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        c = new GUIStyle(l) { fontSize = 30, alignment = TextAnchor.MiddleCenter };
        l.normal.textColor = b.normal.textColor = c.normal.textColor = new Color(1, .82f, .3f);
    }

    // ── MATERIALS ──
    Material Mat(Color col, string tex = "", float em = 0)
    {
        var sh = Shader.Find("Universal Render Pipeline/Lit");
        if (!sh) sh = Shader.Find("Standard");
        var m = new Material(sh);
        if (m.HasProperty("_Color")) m.color = col;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", col);
        var t = tex == "" ? null : Resources.Load<Texture2D>("Textures/" + tex);
        if (t)
        {
            if (m.HasProperty("_MainTex")) m.mainTexture = t;
            if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", t);
        }
        if (em > 0)
        {
            m.EnableKeyword("_EMISSION");
            if (m.HasProperty("_EmissionColor")) m.SetColor("_EmissionColor", col * em);
        }
        return m;
    }

    void Mats()
    {
        wall = Mat(new Color(.56f, .49f, .19f), "wall_leak_color");
        wall2 = Mat(new Color(.36f, .30f, .12f), "wall_leak_alt_c_color");
        flr = Mat(new Color(.18f, .12f, .05f), "carpet_fabric_color");
        ceil = Mat(new Color(.24f, .22f, .16f), "office_ceiling_color");
        met = Mat(new Color(.25f, .24f, .22f), "painted_metal_color");
        glw = Mat(new Color(1, .8f, .22f), "office_ceiling_emission", 1.2f);
        pap = Mat(new Color(.94f, .78f, .38f), "plastic_panel_color");
    }

    // ── WORLD BUILDING ──
    GameObject Box(string nm, Vector3 pos, Vector3 sc, Material ma, bool nc = false)
    {
        var o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        o.name = nm;
        o.transform.position = pos;
        o.transform.localScale = sc;
        o.GetComponent<Renderer>().material = ma;
        if (nc) Destroy(o.GetComponent<Collider>());
        return o;
    }

    void It(string nm, Vector3 pos, Material ma, float sz)
    {
        items.Add(Box(nm, pos, new Vector3(sz, sz, sz), ma));
    }

    void World()
    {
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(.035f, .03f, .018f);
        RenderSettings.fogDensity = .035f;
        RenderSettings.ambientLight = new Color(.22f, .19f, .1f) * br;

        Box("floor", new Vector3(0, -.1f, 0), new Vector3(86, .2f, 86), flr);
        Box("ceiling", new Vector3(0, 2.8f, 0), new Vector3(86, .1f, 86), ceil, true);

        for (int i = -10; i <= 10; i++)
        {
            Box("wall", new Vector3(i * 4, 1.2f, -42), new Vector3(4, 2.5f, .3f), wall);
            Box("wall", new Vector3(i * 4, 1.2f, 42), new Vector3(4, 2.5f, .3f), wall);
            Box("wall", new Vector3(-42, 1.2f, i * 4), new Vector3(.3f, 2.5f, 4), wall);
            Box("wall", new Vector3(42, 1.2f, i * 4), new Vector3(.3f, 2.5f, 4), wall);
        }

        for (int x = -6; x <= 6; x += 2)
            for (int z = -6; z <= 6; z += 2)
            {
                if (Mathf.Abs(x) + Mathf.Abs(z) < 4) continue;
                if ((x + z) % 4 == 0)
                    Box("pillar", new Vector3(x * 5, 1.2f, z * 5), new Vector3(1.1f, 2.5f, 1.1f),
                        UnityEngine.Random.value > .3f ? wall : wall2);
                if ((x * 3 + z) % 5 == 0)
                    Box("partition", new Vector3(x * 5 + 2, 1.2f, z * 5), new Vector3(.35f, 2.5f, 5), wall2);
            }

        // Lights
        for (int i = 0; i < 24; i++)
        {
            var li = new GameObject("lamp").AddComponent<Light>();
            li.transform.position = new Vector3(UnityEngine.Random.Range(-35, 35), 2.45f, UnityEngine.Random.Range(-35, 35));
            li.type = LightType.Point;
            li.range = 8;
            li.intensity = .55f * br;
            li.color = new Color(1, .8f, .35f);
            lights.Add(li);
            Box("lampbox", li.transform.position, new Vector3(1.2f, .05f, .4f), glw, true);
        }

        // Interactive items
        It("fuse", new Vector3(-18, .5f, -12), glw, .55f);
        It("fuse", new Vector3(14, .5f, -18), glw, .55f);
        It("fuse", new Vector3(-9, .5f, 24), glw, .55f);
        It("yellowkey", new Vector3(-28, .5f, 6), pap, .45f);
        It("bluekey", new Vector3(31, .5f, -22), pap, .45f);
        It("switchA", new Vector3(2, .8f, 22), met, .6f);
        It("switchB", new Vector3(18, .8f, 12), met, .6f);
        It("breaker", new Vector3(26, 1, -6), met, 1);
        It("exit", new Vector3(36, 1.2f, 32), met, 2);
        It("truth", new Vector3(-36, 1.2f, 34), Mat(Color.black), 2);
        It("locker", new Vector3(-20, 1, 10), met, 1.4f);
        It("locker", new Vector3(4, 1, -26), met, 1.4f);
        for (int i = 0; i < 5; i++)
            It("note", new Vector3(-26 + i * 13, .7f, -28 + (i % 2) * 48), pap, .5f);

        p = new GameObject("Player");
        p.transform.position = new Vector3(-24, 1.2f, -24);
        cc = p.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.3f;
        cc.center = new Vector3(0, 0, 0);
        cam = Camera.main;
        if (cam == null)
        {
            var camObj = new GameObject("MainCamera");
            cam = camObj.AddComponent<Camera>();
            camObj.AddComponent<AudioListener>();
            camObj.tag = "MainCamera";
        }

        e = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        e.name = "Entity";
        e.transform.localScale = new Vector3(0.6f, 1.8f, 0.6f);
        var eMat = Mat(new Color(0.05f, 0.05f, 0.05f));
        e.GetComponent<Renderer>().material = eMat;
    }

    // ── GAME FLOW ──
    void Menu()
    {
        s = GameState.Menu;
        p.SetActive(false);
        if (e) e.SetActive(false);
        if (cam)
        {
            cam.transform.SetParent(null);
            cam.transform.position = new Vector3(0, 1.6f, -9);
            cam.transform.rotation = Quaternion.Euler(5, 0, 0);
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Run()
    {
        s = GameState.Game;
        foreach (var o in items) if (o) o.SetActive(true);
        p.SetActive(true);
        e.SetActive(true);
        p.transform.position = new Vector3(-24, 1.2f, -24);
        e.transform.position = target = new Vector3(24, 1, 22);
        cam.transform.SetParent(p.transform);
        cam.transform.localPosition = new Vector3(0, 1.55f, 0);
        f = n = 0;
        yk = bk = pow = swA = swB = false;
        hp = sa = st = 100;
        yaw = pit = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Msg("LEVEL 0", 3);
    }

    void Save()
    {
        PlayerPrefs.SetFloat("x", p.transform.position.x);
        PlayerPrefs.SetFloat("z", p.transform.position.z);
        PlayerPrefs.SetInt("f", f);
        PlayerPrefs.SetInt("n", n);
        PlayerPrefs.SetInt("y", yk ? 1 : 0);
        PlayerPrefs.SetInt("bl", bk ? 1 : 0);
        PlayerPrefs.SetInt("po", pow ? 1 : 0);
        PlayerPrefs.Save();
        Msg("SAVED", 2);
    }

    void Load()
    {
        Run();
        p.transform.position = new Vector3(PlayerPrefs.GetFloat("x", -24), 1.2f, PlayerPrefs.GetFloat("z", -24));
        f = PlayerPrefs.GetInt("f", 0);
        n = PlayerPrefs.GetInt("n", 0);
        yk = PlayerPrefs.GetInt("y", 0) == 1;
        bk = PlayerPrefs.GetInt("bl", 0) == 1;
        pow = PlayerPrefs.GetInt("po", 0) == 1;
        Msg("LOADED", 2);
    }

    void End(string t)
    {
        end = t;
        Save();
        s = GameState.End;
        p.SetActive(false);
        e.SetActive(false);
        cam.transform.SetParent(null);
        cam.transform.position = new Vector3(0, 1.6f, -9);
        cam.transform.rotation = Quaternion.Euler(5, 0, 0);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // ── UPDATE ──
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Backspace)) Back();
        if (s != GameState.Game) return;

        run |= Input.GetKey(KeyCode.LeftShift);
        use |= Input.GetKeyDown(KeyCode.E);
        hide |= Input.GetKey(KeyCode.C) || Input.GetKey(KeyCode.LeftControl);
        dec |= Input.GetKeyDown(KeyCode.Q);

        Move();
        Near();
        AI();

        for (int i = 0; i < lights.Count; i++)
            lights[i].intensity = Mathf.Lerp(.2f, .85f, Mathf.PerlinNoise(i, Time.time * 1.7f)) * br;

        if (msgT > 0) msgT -= Time.deltaTime;
        if (hp <= 0 || sa <= 0) End("CONSUMED");
        run = use = hide = dec = false;
    }

    void Move()
    {
        var v = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        float sp = hide ? 1.2f : run && st > 4 ? 4.7f : 2.5f;
        var q = p.transform.right * v.x + p.transform.forward * v.y;
        q = Vector3.ClampMagnitude(q, 1) * sp;
        q.y = -9.8f;
        cc.Move(q * Time.deltaTime);
        yaw += Input.GetAxis("Mouse X") * sen * 3;
        pit = Mathf.Clamp(pit - Input.GetAxis("Mouse Y") * sen * 3, -70, 70);
        p.transform.rotation = Quaternion.Euler(0, yaw, 0);
        cam.transform.localRotation = Quaternion.Euler(pit, 0, 0);
        st = Mathf.Clamp(st + Time.deltaTime * 10 - (run ? Time.deltaTime * 28 : 0), 0, 100);
    }

    GameObject Close()
    {
        GameObject best = null;
        float bd = 2.8f;
        foreach (var o in items)
        {
            if (!o || !o.activeSelf) continue;
            float d = Vector3.Distance(p.transform.position, o.transform.position);
            if (d < bd) { bd = d; best = o; }
        }
        return best;
    }

    void Near()
    {
        var o = Close();
        if (o && o.name == "locker" && hide)
        {
            sa = Mathf.Min(100, sa + Time.deltaTime * 5);
            return;
        }

        if (dec && decT <= 0)
        {
            decT = 8;
            decPos = p.transform.position + p.transform.forward * 5;
            Box("decoy", decPos + Vector3.up * .2f, new Vector3(.3f, .3f, .3f), glw, true);
            Msg("DISTRACTION", 2);
        }
        if (decT > 0) decT -= Time.deltaTime;

        if (!o || !use) return;
        string z = o.name;

        if (z == "fuse") { f++; o.SetActive(false); Msg("FUSE " + f + "/3", 2); }
        else if (z == "yellowkey") { yk = true; o.SetActive(false); Msg("YELLOW KEY", 2); }
        else if (z == "bluekey")
        {
            if (!yk) { Msg("NEEDS YELLOW KEY", 2); return; }
            bk = true; o.SetActive(false); Msg("BLUE KEY", 2);
        }
        else if (z == "note") { Msg(notes[Mathf.Clamp(n, 0, 4)], 4); n = Mathf.Min(5, n + 1); o.SetActive(false); }
        else if (z == "switchA") { swA = true; Msg("SWITCH A", 2); }
        else if (z == "switchB") { swB = true; Msg("SWITCH B", 2); }
        else if (z == "breaker")
        {
            if (f >= 3 && swA && swB) { pow = true; Save(); Msg("POWERED", 2); }
            else Msg("NEEDS FUSES AND SWITCHES", 2);
        }
        else if (z == "exit")
        {
            if (pow) End(sa > 55 ? "ESCAPED" : "HOLLOW ESCAPE");
            else Msg("POWER THE EXIT", 2);
        }
        else if (z == "truth")
        {
            if (pow && n >= 5) End("TRUTH DOOR");
            else Msg("NEEDS POWER AND NOTES", 2);
        }
    }

    void AI()
    {
        bool h = Close() && Close().name == "locker" && hide;
        var ep = e.transform.position;
        var pp = p.transform.position;
        bool chase = Vector3.Distance(ep, pp) < (h ? 2 : 15);

        if (decT > 0) target = decPos;
        else if (chase) target = pp;
        else if (Vector3.Distance(ep, target) < 1.5f)
            target = new Vector3(UnityEngine.Random.Range(-33, 33), 1, UnityEngine.Random.Range(-33, 33));

        var d = target - ep;
        d.y = 0;
        if (d.sqrMagnitude > .2f)
        {
            e.transform.rotation = Quaternion.LookRotation(d.normalized);
            e.transform.position += d.normalized * (chase ? 3.1f : 1.4f) * Time.deltaTime;
        }

        float dist = Vector3.Distance(e.transform.position, pp);
        if (dist < 2 && !h)
        {
            hp -= Time.deltaTime * 25;
            sa -= Time.deltaTime * 18;
        }
        else if (chase && !h) sa -= Time.deltaTime * 4;
        else sa = Mathf.Min(100, sa + Time.deltaTime * .6f);
    }

    void Back()
    {
        if (s == GameState.Game) { Save(); s = GameState.Pause; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        else if (s == GameState.Pause) { s = GameState.Game; Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
        else Menu();
    }

    void Msg(string x, float t) { msg = x; msgT = t; }

    // ── ONGUI — minimal HUD only (no menu; DesolationRuntime handles that) ──
    void OnGUI()
    {
        if (s != GameState.Game && s != GameState.Pause) return;

        // Vignette / dark overlay
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), dk);

        // HUD bars
        Bar(20, 20, hp, "HEALTH");
        Bar(20, 50, sa, "SANITY");
        Bar(20, 80, st, "STAMINA");

        // Message
        if (msgT > 0)
            GUI.Label(new Rect(0, Screen.height / 2 - 40, Screen.width, 50), msg, c);

        // Crosshair
        GUI.color = new Color(1, 1, 1, .4f);
        float cx = Screen.width / 2, cy = Screen.height / 2;
        GUI.DrawTexture(new Rect(cx - 1, cy - 6, 2, 12), GUI.skin.label.normal.background);
        GUI.DrawTexture(new Rect(cx - 6, cy - 1, 12, 2), GUI.skin.label.normal.background);
        GUI.color = Color.white;

        // Pause overlay
        if (s == GameState.Pause)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), dk);
            Title("PAUSED");
            Btn(Screen.width / 2 - 130, 280, "RESUME", () => { s = GameState.Game; Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; });
            Btn(Screen.width / 2 - 130, 360, "SAVE", Save);
            Btn(Screen.width / 2 - 130, 440, "MAIN MENU", Menu);
        }

        // End screen
        if (s == GameState.End)
        {
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), dk);
            Title(end);
            Btn(Screen.width / 2 - 130, Screen.height / 2, "MAIN MENU", Menu);
        }
    }

    float Sl(float y, string t, float v, float a, float m)
    {
        GUI.Label(new Rect(140, y, 230, 35), t, l);
        return GUI.HorizontalSlider(new Rect(390, y + 10, Screen.width - 540, 30), v, a, m);
    }

    void Title(string t) { GUI.Label(new Rect(0, 55, Screen.width, 90), t, new GUIStyle(c) { fontSize = 46 }); }
    void Btn(float x, float y, string txt, Action a) { if (GUI.Button(new Rect(x, y, 260, 65), txt, b)) a(); }
    void Bar(float x, float y, float v, string t)
    {
        GUI.DrawTexture(new Rect(x, y, 220 * Mathf.Clamp01(v / 100), 16), v < 30 ? rd : v < 60 ? gd : gn);
        GUI.Label(new Rect(x + 230, y - 6, 150, 25), t, l);
    }
}
