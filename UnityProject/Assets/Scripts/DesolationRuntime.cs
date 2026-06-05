using UnityEngine;

public sealed class DesolationRuntime : MonoBehaviour
{
    CharacterController controller;
    Camera cam;
    Vector3 velocity;
    float yaw;
    float pitch;
    int fuses;
    string message = "Find three blue fuses and reach the service exit.";

    void Start()
    {
        Application.targetFrameRate = 60;
        MakeBox("Carpet", new Vector3(0f, -0.05f, 0f), new Vector3(30f, 0.1f, 30f), new Color(0.28f, 0.22f, 0.12f));
        MakeBox("Ceiling", new Vector3(0f, 2.6f, 0f), new Vector3(30f, 0.1f, 30f), new Color(0.55f, 0.52f, 0.36f));
        MakeBox("NorthWall", new Vector3(0f, 1.25f, 15f), new Vector3(30f, 2.5f, 0.2f), new Color(0.7f, 0.62f, 0.26f));
        MakeBox("SouthWall", new Vector3(0f, 1.25f, -15f), new Vector3(30f, 2.5f, 0.2f), new Color(0.7f, 0.62f, 0.26f));
        MakeBox("EastWall", new Vector3(15f, 1.25f, 0f), new Vector3(0.2f, 2.5f, 30f), new Color(0.7f, 0.62f, 0.26f));
        MakeBox("WestWall", new Vector3(-15f, 1.25f, 0f), new Vector3(0.2f, 2.5f, 30f), new Color(0.7f, 0.62f, 0.26f));
        MakeBox("BlueFuse", new Vector3(-9f, 0.5f, -9f), Vector3.one * 0.45f, Color.cyan);
        MakeBox("BlueFuse", new Vector3(9f, 0.5f, -4f), Vector3.one * 0.45f, Color.cyan);
        MakeBox("BlueFuse", new Vector3(-4f, 0.5f, 10f), Vector3.one * 0.45f, Color.cyan);
        MakeBox("ServiceExit", new Vector3(14f, 1f, 12f), new Vector3(0.25f, 2f, 2.1f), new Color(0.22f, 0.21f, 0.19f));
        Light light = new GameObject("Light").AddComponent<Light>();
        light.type = LightType.Point;
        light.range = 25f;
        light.intensity = 1.2f;
        light.transform.position = new Vector3(0f, 2.4f, 0f);
        GameObject player = new GameObject("Player");
        controller = player.AddComponent<CharacterController>();
        controller.height = 1.75f;
        controller.radius = 0.32f;
        player.transform.position = new Vector3(0f, 0.9f, 0f);
        cam = new GameObject("PlayerCamera").AddComponent<Camera>();
        cam.transform.SetParent(player.transform);
        cam.transform.localPosition = new Vector3(0f, 0.65f, 0f);
        cam.fieldOfView = 72f;
        cam.gameObject.AddComponent<AudioListener>();
    }

    void Update()
    {
        if (controller == null) return;
        Vector2 move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        if (Input.touchCount > 0 && Input.GetTouch(0).position.x < Screen.width * 0.45f)
        {
            Vector2 d = (Input.GetTouch(0).position - new Vector2(140f, 540f)) / 100f;
            move += Vector2.ClampMagnitude(d, 1f);
        }
        yaw += Input.GetAxis("Mouse X") * 4f;
        pitch = Mathf.Clamp(pitch - Input.GetAxis("Mouse Y") * 3f, -80f, 80f);
        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            if (t.position.x > Screen.width * 0.45f && t.phase == TouchPhase.Moved)
            {
                yaw += t.deltaPosition.x * 0.18f;
                pitch = Mathf.Clamp(pitch - t.deltaPosition.y * 0.16f, -80f, 80f);
            }
        }
        controller.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        cam.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        Vector3 motion = controller.transform.right * move.x + controller.transform.forward * move.y;
        if (motion.magnitude > 1f) motion.Normalize();
        velocity = new Vector3(motion.x * 3.6f, controller.isGrounded ? -0.1f : velocity.y - 18f * Time.deltaTime, motion.z * 3.6f);
        controller.Move(velocity * Time.deltaTime);
        CheckObjects();
    }

    void CheckObjects()
    {
        foreach (GameObject obj in FindObjectsByType<GameObject>(FindObjectsSortMode.None))
        {
            float d = Vector3.Distance(controller.transform.position, obj.transform.position);
            if (obj.name == "BlueFuse" && d < 1.4f)
            {
                fuses++;
                Destroy(obj);
                message = "Fuse collected: " + fuses + "/3";
            }
            if (obj.name == "ServiceExit" && d < 2f)
            {
                message = fuses >= 3 ? "Escape ending. Unity cloud build works." : "The exit needs three fuses.";
            }
        }
    }

    void MakeBox(string n, Vector3 p, Vector3 s, Color c)
    {
        GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
        g.name = n;
        g.transform.position = p;
        g.transform.localScale = s;
        Material m = new Material(Shader.Find("Standard"));
        m.color = c;
        g.GetComponent<Renderer>().material = m;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(20, 20, 1000, 40), "Desolation: The Backrooms - Unity Migration Scaffold");
        GUI.Label(new Rect(20, 55, 1000, 40), "Fuses " + fuses + "/3");
        GUI.Label(new Rect(20, 90, 1100, 60), message);
        GUI.Box(new Rect(35, 430, 230, 230), "MOVE");
        GUI.Box(new Rect(930, 575, 190, 70), "SPRINT");
    }
}
