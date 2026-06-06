using UnityEngine;
using System.Reflection;

public sealed class HideInGameMissingUi : MonoBehaviour
{
    Texture2D cover;
    GUIStyle blank;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindObjectOfType<HideInGameMissingUi>()) return;
        GameObject g = new GameObject("HideInGameMissingUi");
        DontDestroyOnLoad(g);
        g.AddComponent<HideInGameMissingUi>();
    }

    void Start()
    {
        cover = new Texture2D(1, 1);
        cover.SetPixel(0, 0, new Color(0, 0, 0, .95f));
        cover.Apply();
        blank = new GUIStyle(GUI.skin.button);
        Texture2D clear = new Texture2D(1, 1);
        clear.SetPixel(0, 0, new Color(0, 0, 0, 0));
        clear.Apply();
        blank.normal.background = clear;
        blank.hover.background = clear;
        blank.active.background = clear;
        blank.normal.textColor = Color.clear;
        blank.hover.textColor = Color.clear;
        blank.active.textColor = Color.clear;
    }

    void OnGUI()
    {
        FirstPlayableBatch runtime = FindObjectOfType<FirstPlayableBatch>();
        if (!runtime) return;
        FieldInfo stateField = typeof(FirstPlayableBatch).GetField("s", BindingFlags.NonPublic | BindingFlags.Instance);
        if (stateField == null) stateField = typeof(FirstPlayableBatch).GetField("state", BindingFlags.NonPublic | BindingFlags.Instance);
        if (stateField == null) return;
        string state = stateField.GetValue(runtime).ToString();
        if (state != "Menu") return;
        Rect hidden = new Rect(45, 535, 330, 95);
        GUI.DrawTexture(hidden, cover);
        GUI.Button(hidden, "", blank);
    }
}
