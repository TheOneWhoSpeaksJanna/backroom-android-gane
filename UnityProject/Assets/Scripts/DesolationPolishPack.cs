using UnityEngine;
using System.Collections.Generic;
using System.Reflection;

public sealed class DesolationPolishPack : MonoBehaviour
{
    FirstPlayableBatch g;
    System.Type gt;
    GameObject p;
    GameObject e;
    Camera cam;
    Material wall;
    Material floor;
    Material metal;
    Material glow;
    Material dark;
    AudioSource hum;
    AudioSource fx;
    AudioClip humClip;
    AudioClip footClip;
    AudioClip cueClip;
    AudioClip clickClip;
    Texture2D shade;
    Texture2D grain;
    bool built;
    bool slots;
    float foot;
    float cue;
    float look = 1f;
    float volume = .85f;
    float bob;
    readonly List<Renderer> renderers = new List<Renderer>();
    readonly List<Light> lights = new List<Light>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindObjectOfType<DesolationPolishPack>() != null) return;
        GameObject o = new GameObject("DesolationPolishPack");
        DontDestroyOnLoad(o);
        o.AddComponent<DesolationPolishPack>();
    }

    void Start()
    {
        look = PlayerPrefs.GetFloat("polish_touch_look", 1f);
        volume = PlayerPrefs.GetFloat("polish_volume", .85f);
        shade = new Texture2D(1, 1);
        shade.SetPixel(0, 0, Color.black);
        shade.Apply();
        grain = new Texture2D(16, 16);
        for (int y = 0; y < 16; y++)
        for (int x = 0; x < 16; x++)
        {
            float v = Random.Range(.25f, .75f);
            grain.SetPixel(x, y, new Color(v, v, v, .10f));
        }
        grain.Apply();
        wall = Mat(new Color(.55f, .48f, .20f), "wall_leak_color", 0f);
        floor = Mat(new Color(.18f, .12f, .05f), "carpet_fabric_color", 0f);
        metal = Mat(new Color(.22f, .22f, .21f), "painted_metal_color", 0f);
        glow = Mat(new Color(1f, .78f, .25f), "office_ceiling_emission", 1.4f);
        dark = Mat(new Color(.03f, .025f, .012f), "", 0f);
        humClip = Tone("hum", 4f, 70f, .10f);
        footClip = Tone("footstep", .18f, 95f, .42f);
        cueClip = Tone("cue", .55f, 210f, .45f);
        clickClip = Tone("click", .08f, 520f, .32f);
    }

    bool Link()
    {
        if (g == null)
        {
            g = FindObjectOfType<FirstPlayableBatch>();
            if (g == null) return false;
            gt = g.GetType();
        }
        p = Get<GameObject>("p");
        e = Get<GameObject>("e");
        cam = Get<Camera>("cam");
        if (cam != null && hum == null)
        {
            hum = cam.gameObject.AddComponent<AudioSource>();
            hum.clip = humClip;
            hum.loop = true;
            hum.spatialBlend = 0f;
            hum.volume = .2f * volume;
            hum.Play();
            fx = cam.gameObject.AddComponent<AudioSource>();
            fx.spatialBlend = 0f;
            fx.volume = volume;
        }
        return p != null && e != null && cam != null;
    }

    T Get<T>(string n) where T : class
    {
        FieldInfo f = gt.GetField(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return f == null ? null : f.GetValue(g) as T;
    }

    object Read(string n)
    {
        FieldInfo f = gt.GetField(n, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        return f == null ? null : f.GetValue(g);
    }

    bool Bool(string n)
    {
        object v = Read(n);
        return v is bool && (bool)v;
    }

    string State()
    {
        object v = Read("s");
        return v == null ? "" : v.ToString();
    }

    Material Mat(Color c, string texture, float emit)
    {
        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        Material m = new Material(sh);
        if (m.HasProperty("_Color")) m.color = c;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", c);
        if (texture != "")
        {
            Texture2D t = Resources.Load<Texture2D>("Textures/" + texture);
            if (t != null)
            {
                if (m.HasProperty("_MainTex")) m.mainTexture = t;
                if (m.HasProperty("_BaseMap")) m.SetTexture("_BaseMap", t);
                m.mainTextureScale = new Vector2(2.5f, 2.5f);
            }
        }
        if (emit > 0f)
        {
            m.EnableKeyword("_EMISSION");
            if (m.HasProperty("_EmissionColor")) m.SetColor("_EmissionColor", c * emit);
        }
        return m;
    }

    AudioClip Tone(string n, float sec, float hz, float amp)
    {
        int rate = 22050;
        int len = Mathf.Max(1, Mathf.RoundToInt(sec * rate));
        float[] d = new float[len];
        for (int i = 0; i < len; i++)
        {
            float t = i / (float)rate;
            d[i] = (Mathf.Sin(t * hz * 6.283f) * amp + Random.Range(-amp, amp) * .2f) * Mathf.Clamp01(1f - t / sec);
        }
        AudioClip c = AudioClip.Create(n, len, 1, rate, false);
        c.SetData(d, 0);
        return c;
    }

    void Build()
    {
        if (built) return;
        built = true;
        RenderSettings.fog = true;
        RenderSettings.fogColor = new Color(.035f, .030f, .018f);
        RenderSettings.fogDensity = .045f;
        Room(new Vector3(-34, 1, -34), 10, 7);
        Room(new Vector3(33, 1, 28), 8, 6);
        Room(new Vector3(-36, 1, 30), 7, 8);
        Room(new Vector3(30, 1, -32), 7, 7);
        for (int i = 0; i < 24; i++)
        {
            float x = -38f + (i % 8) * 10f;
            float z = -36f + (i / 8) * 24f;
            Box("trim", new Vector3(x, .22f, z), new Vector3(6f, .18f, .12f), metal, false);
            Box("outlet", new Vector3(x + 1.4f, .55f, z + .15f), new Vector3(.18f, .25f, .05f), glow, false);
            if (i % 3 == 0) Box("stain", new Vector3(x - 1.5f, .02f, z + 2f), new Vector3(1.6f, .03f, .9f), dark, false);
            if (i % 5 == 0) Box("vent", new Vector3(x - 2f, .8f, z - .15f), new Vector3(1.2f, .35f, .07f), metal, false);
        }
        for (int i = 0; i < 8; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-38f, 38f), 2.35f, Random.Range(-38f, 38f));
            Light l = new GameObject("flicker light").AddComponent<Light>();
            l.type = LightType.Point;
            l.color = new Color(1f, .75f, .25f);
            l.range = 7f;
            l.intensity = .4f;
            l.transform.position = pos;
            lights.Add(l);
            Box("broken light", pos, new Vector3(1.4f, .06f, .45f), i % 3 == 0 ? dark : glow, false);
        }
        foreach (Renderer r in FindObjectsOfType<Renderer>())
            if (r != null) renderers.Add(r);
    }

    void Room(Vector3 c, float w, float d)
    {
        Box("floor", c + new Vector3(0, -.25f, 0), new Vector3(w, .08f, d), floor, false);
        Box("wall", c + new Vector3(0, 0, d * .5f), new Vector3(w, 2.4f, .25f), wall, true);
        Box("wall", c + new Vector3(w * .5f, 0, 0), new Vector3(.25f, 2.4f, d), wall, true);
        Box("wall", c + new Vector3(-w * .5f, 0, 0), new Vector3(.25f, 2.4f, d), wall, true);
        Box("door frame", c + new Vector3(0, .15f, -d * .5f), new Vector3(2.2f, 2.1f, .22f), metal, true);
    }

    GameObject Box(string n, Vector3 p0, Vector3 s, Material m, bool col)
    {
        GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube);
        o.name = n;
        o.transform.position = p0;
        o.transform.localScale = s;
        o.GetComponent<Renderer>().material = m;
        if (!col) Destroy(o.GetComponent<Collider>());
        return o;
    }

    void Update()
    {
        if (!Link() || State() != "Game") return;
        Build();
        bool moving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0 || p.transform.hasChanged;
        if (moving && fx != null)
        {
            foot -= Time.deltaTime;
            if (foot <= 0f)
            {
                fx.PlayOneShot(footClip, .45f * volume);
                foot = Bool("run") ? .28f : .48f;
            }
        }
        float dist = Vector3.Distance(e.transform.position, p.transform.position);
        if (dist < 13f && fx != null)
        {
            cue -= Time.deltaTime;
            if (cue <= 0f)
            {
                fx.PlayOneShot(cueClip, Mathf.Lerp(.8f, .2f, dist / 13f) * volume);
                cue = 1.2f;
            }
        }
        if (moving) bob += Time.deltaTime * 6f;
        cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, new Vector3(Mathf.Sin(bob) * .025f, 1.55f + Mathf.Abs(Mathf.Cos(bob)) * .035f, 0), Time.deltaTime * 5f);
        HighCull();
        p.transform.hasChanged = false;
    }

    void HighCull()
    {
        Vector3 cp = cam.transform.position;
        Vector3 cf = cam.transform.forward;
        foreach (Renderer r in renderers)
        {
            if (r == null || r.transform.IsChildOf(p.transform) || r.gameObject == e) continue;
            Vector3 to = r.bounds.center - cp;
            float d = to.magnitude;
            float dot = Vector3.Dot(cf, to.normalized);
            bool ground = r.name.ToLower().Contains("floor") || r.name.ToLower().Contains("ceiling");
            r.enabled = d < 7f || (dot > -.05f && d < 44f) || (ground && dot > .2f && d < 28f);
        }
        foreach (Light l in lights)
            if (l != null) l.intensity = Mathf.Lerp(.05f, .75f, Mathf.PerlinNoise(l.transform.position.x, Time.time * 3f));
    }

    void OnGUI()
    {
        if (!Link()) return;
        string state = State();
        if (state == "Game")
        {
            GUI.color = new Color(0, 0, 0, .22f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, 72), shade);
            GUI.DrawTexture(new Rect(0, Screen.height - 80, Screen.width, 80), shade);
            GUI.color = new Color(1, 1, 1, .055f);
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), grain);
            GUI.color = Color.white;
            GUI.Label(new Rect(25, 170, 760, 30), "Level, audio, visual polish and high culling active");
        }
        else if (state == "Menu")
        {
            if (GUI.Button(new Rect(Screen.width - 270, 110, 240, 56), slots ? "CLOSE SLOT PREVIEWS" : "SAVE SLOT PREVIEWS"))
            {
                slots = !slots;
                if (fx != null) fx.PlayOneShot(clickClip, .6f * volume);
            }
            if (slots) DrawSlots();
        }
        else if (state == "Settings")
        {
            GUI.DrawTexture(new Rect(65, 150, 450, 280), shade);
            GUI.Label(new Rect(90, 170, 390, 45), "Polish settings");
            GUI.Label(new Rect(90, 230, 160, 30), "Touch look");
            look = GUI.HorizontalSlider(new Rect(245, 238, 220, 22), look, .35f, 2.5f);
            GUI.Label(new Rect(90, 275, 160, 30), "Audio");
            volume = GUI.HorizontalSlider(new Rect(245, 283, 220, 22), volume, 0f, 1f);
            if (GUI.Button(new Rect(130, 335, 300, 50), "SAVE POLISH SETTINGS"))
            {
                PlayerPrefs.SetFloat("polish_volume", volume);
                PlayerPrefs.SetFloat("polish_touch_look", look);
                PlayerPrefs.Save();
                if (hum != null) hum.volume = .2f * volume;
            }
        }
    }

    void DrawSlots()
    {
        GUI.DrawTexture(new Rect(Screen.width - 445, 185, 410, 315), shade);
        for (int i = 0; i < 3; i++)
        {
            float y = 210 + i * 88;
            GUI.Label(new Rect(Screen.width - 415, y, 270, 54), "Slot " + (i + 1) + " preview\n" + PlayerPrefs.GetString("polish_slot_" + i, "Ready"));
            if (GUI.Button(new Rect(Screen.width - 145, y, 90, 42), "SAVE"))
            {
                PlayerPrefs.SetString("polish_slot_" + i, System.DateTime.Now.ToString("HH:mm"));
                PlayerPrefs.Save();
            }
        }
    }
}
