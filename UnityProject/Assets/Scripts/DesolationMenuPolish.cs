using UnityEngine;
using System;
using System.Reflection;

[DefaultExecutionOrder(10000)]
public sealed class DesolationMenuPolish : MonoBehaviour
{
    object game;
    Type type;
    FieldInfo stateField;
    MethodInfo runMethod;
    MethodInfo menuMethod;
    MethodInfo loadMethod;
    GUIStyle clearButton;
    Texture2D px;
    int page;
    float volume=.9f;
    float brightness=.9f;
    float sensitivity=1f;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot()
    {
        if (FindObjectOfType<DesolationMenuPolish>() != null) return;
        GameObject o=new GameObject("DesolationMenuPolish");
        DontDestroyOnLoad(o);
        o.AddComponent<DesolationMenuPolish>();
    }

    void Start()
    {
        volume=PlayerPrefs.GetFloat("desolation_volume",.9f);
        brightness=PlayerPrefs.GetFloat("desolation_brightness",.9f);
        sensitivity=PlayerPrefs.GetFloat("desolation_sensitivity",1f);
        px=new Texture2D(1,1);
        px.SetPixel(0,0,Color.white);
        px.Apply();
        clearButton=new GUIStyle(GUI.skin.button);
        clearButton.normal.background=null;
        clearButton.hover.background=null;
        clearButton.active.background=null;
        clearButton.normal.textColor=Color.clear;
        clearButton.hover.textColor=Color.clear;
        clearButton.active.textColor=Color.clear;
    }

    bool Link()
    {
        if(game!=null)return true;
        UnityEngine.Object obj=FindObjectOfType<FirstPlayableBatch>();
        if(obj==null)return false;
        game=obj;
        type=obj.GetType();
        stateField=type.GetField("s",BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
        runMethod=type.GetMethod("Run",BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
        menuMethod=type.GetMethod("Menu",BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
        loadMethod=type.GetMethod("Load",BindingFlags.Instance|BindingFlags.NonPublic|BindingFlags.Public);
        return true;
    }

    string State()
    {
        if(!Link()||stateField==null)return "";
        object v=stateField.GetValue(game);
        return v==null?"":v.ToString();
    }

    void SetState(string name)
    {
        if(!Link()||stateField==null)return;
        object v=Enum.Parse(stateField.FieldType,name);
        stateField.SetValue(game,v);
    }

    Rect R(float x,float y,float w,float h){return new Rect(Screen.width*x,Screen.height*y,Screen.width*w,Screen.height*h);}

    void Fill(Rect r,Color c)
    {
        GUI.color=c;
        GUI.DrawTexture(r,px);
        GUI.color=Color.white;
    }

    void Text(Rect r,string t,int size,TextAnchor a)
    {
        GUIStyle s=new GUIStyle(GUI.skin.label);
        s.fontSize=size;
        s.fontStyle=FontStyle.Bold;
        s.alignment=a;
        s.normal.textColor=new Color(0,0,0,.8f);
        Rect sh=r; sh.x+=3; sh.y+=3;
        GUI.Label(sh,t,s);
        s.normal.textColor=new Color(1f,.82f,.25f,1f);
        GUI.Label(r,t,s);
    }

    void Line(float x,float y,float w)
    {
        Fill(R(x,y,w,.004f),new Color(1f,.82f,.25f,.9f));
        Fill(R(x+w,y-.003f,.004f,.010f),new Color(1f,.82f,.25f,.9f));
    }

    void Panel(Rect r)
    {
        Fill(r,new Color(0,0,0,.52f));
        Fill(new Rect(r.x,r.y,r.width,2),new Color(1f,.82f,.25f,.7f));
        Fill(new Rect(r.x,r.yMax-2,r.width,2),new Color(1f,.82f,.25f,.7f));
        Fill(new Rect(r.x,r.y,2,r.height),new Color(1f,.82f,.25f,.7f));
        Fill(new Rect(r.xMax-2,r.y,2,r.height),new Color(1f,.82f,.25f,.7f));
    }

    bool Hot(float x,float y,float w,float h){return GUI.Button(R(x,y,w,h),"",clearButton);}

    void MenuArt()
    {
        Fill(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,.16f));
        Text(R(.29f,.055f,.50f,.15f),"DESOLATION:",66,TextAnchor.MiddleCenter);
        Text(R(.35f,.17f,.38f,.08f),"THE BACKROOMS",34,TextAnchor.MiddleCenter);
        Word(.045f,.285f,"PLAY");
        Word(.045f,.52f,"SETTINGS");
        Word(.045f,.86f,"CREDITS");
        Word(.80f,.86f,"FEEDBACK");
        if(Hot(.02f,.23f,.25f,.18f)&&runMethod!=null)runMethod.Invoke(game,null);
        if(Hot(.02f,.47f,.31f,.18f))page=1;
        if(Hot(.02f,.80f,.30f,.18f))page=2;
        if(Hot(.76f,.80f,.23f,.18f))page=3;
    }

    void Word(float x,float y,string text)
    {
        Text(R(x,y,.25f,.075f),text,34,TextAnchor.MiddleLeft);
        Line(x,y+.073f,.17f);
    }

    void SettingsArt()
    {
        Fill(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,.35f));
        Text(R(.29f,.04f,.50f,.13f),"DESOLATION:",56,TextAnchor.MiddleCenter);
        Text(R(.35f,.15f,.38f,.07f),"THE BACKROOMS",30,TextAnchor.MiddleCenter);
        Text(R(.40f,.25f,.22f,.08f),"SETTINGS",38,TextAnchor.MiddleCenter);
        Panel(R(.24f,.34f,.52f,.50f));
        Row(.38f,"MASTER VOLUME",volume,0,1);
        Row(.47f,"MUSIC VOLUME",volume*.82f,0,1);
        Row(.56f,"SFX VOLUME",volume*.86f,0,1);
        Row(.65f,"BRIGHTNESS",brightness,.35f,1.2f);
        Row(.74f,"SENSITIVITY",sensitivity,.35f,2f);
        Text(R(.27f,.81f,.15f,.05f),"GRAPHICS",24,TextAnchor.MiddleLeft);
        Box(.45f,.805f,"LOW",false);
        Box(.55f,.805f,"MEDIUM",false);
        Box(.66f,.805f,"HIGH",true);
        Box(.43f,.90f,"BACK",true);
        DragSettings();
        if(Hot(.42f,.88f,.19f,.10f))
        {
            PlayerPrefs.SetFloat("desolation_volume",volume);
            PlayerPrefs.SetFloat("desolation_brightness",brightness);
            PlayerPrefs.SetFloat("desolation_sensitivity",sensitivity);
            PlayerPrefs.Save();
            AudioListener.volume=volume;
            page=0;
            if(menuMethod!=null)menuMethod.Invoke(game,null);
        }
    }

    void Row(float y,string name,float val,float min,float max)
    {
        Text(R(.27f,y,.19f,.05f),name,20,TextAnchor.MiddleLeft);
        Fill(R(.45f,y+.028f,.28f,.006f),new Color(1f,.82f,.25f,.75f));
        float t=Mathf.InverseLerp(min,max,val);
        Fill(R(.45f+.28f*t-.006f,y+.014f,.014f,.030f),new Color(1f,.82f,.25f,1f));
    }

    void Box(float x,float y,string text,bool on)
    {
        Rect r=R(x,y,.085f,.055f);
        Panel(r);
        Text(r,text,18,TextAnchor.MiddleCenter);
        if(on)Fill(r,new Color(1f,.82f,.25f,.22f));
    }

    void DragSettings()
    {
        Event e=Event.current;
        if(e==null||(e.type!=EventType.MouseDown&&e.type!=EventType.MouseDrag))return;
        Vector2 m=e.mousePosition;
        Set(m,R(.45f,.38f,.28f,.045f),ref volume,0,1);
        Set(m,R(.45f,.65f,.28f,.045f),ref brightness,.35f,1.2f);
        Set(m,R(.45f,.74f,.28f,.045f),ref sensitivity,.35f,2f);
        if(R(.45f,.805f,.085f,.055f).Contains(m))Application.targetFrameRate=40;
        if(R(.55f,.805f,.085f,.055f).Contains(m))Application.targetFrameRate=50;
        if(R(.66f,.805f,.085f,.055f).Contains(m))Application.targetFrameRate=60;
    }

    void Set(Vector2 m,Rect r,ref float v,float min,float max)
    {
        if(r.Contains(m))v=Mathf.Lerp(min,max,Mathf.InverseLerp(r.xMin,r.xMax,m.x));
    }

    void CreditsArt()
    {
        Fill(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,.35f));
        Text(R(.29f,.05f,.50f,.13f),"DESOLATION:",56,TextAnchor.MiddleCenter);
        Text(R(.35f,.16f,.38f,.07f),"THE BACKROOMS",30,TextAnchor.MiddleCenter);
        Text(R(.42f,.27f,.18f,.07f),"CREDITS",36,TextAnchor.MiddleCenter);
        Panel(R(.24f,.39f,.52f,.43f));
        Credit(.43f,"GAME DESIGN","SOLO DEVELOPER");
        Credit(.51f,"PROGRAMMING","SOLO DEVELOPER");
        Credit(.59f,"UI DESIGN","SOLO DEVELOPER");
        Credit(.67f,"ENVIRONMENT ART","SOLO DEVELOPER");
        Credit(.75f,"SPECIAL THANKS","THE PLAYERS");
        Box(.43f,.89f,"BACK",true);
        if(Hot(.42f,.86f,.20f,.12f))page=0;
    }

    void Credit(float y,string a,string b)
    {
        Text(R(.31f,y,.20f,.05f),a,20,TextAnchor.MiddleLeft);
        Fill(R(.48f,y+.030f,.12f,.003f),new Color(1f,.82f,.25f,.65f));
        Text(R(.58f,y,.22f,.05f),b,20,TextAnchor.MiddleLeft);
    }

    void FeedbackArt()
    {
        Fill(new Rect(0,0,Screen.width,Screen.height),new Color(0,0,0,.35f));
        Text(R(.29f,.05f,.50f,.13f),"DESOLATION:",56,TextAnchor.MiddleCenter);
        Text(R(.35f,.16f,.38f,.07f),"THE BACKROOMS",30,TextAnchor.MiddleCenter);
        Text(R(.42f,.27f,.18f,.07f),"FEEDBACK",36,TextAnchor.MiddleCenter);
        Panel(R(.28f,.39f,.44f,.42f));
        Text(R(.31f,.41f,.20f,.05f),"YOUR FEEDBACK",18,TextAnchor.MiddleLeft);
        Panel(R(.31f,.47f,.38f,.16f));
        Text(R(.32f,.49f,.32f,.04f),"WRITE YOUR FEEDBACK HERE...",17,TextAnchor.MiddleLeft);
        Text(R(.31f,.65f,.20f,.05f),"EMAIL (OPTIONAL)",18,TextAnchor.MiddleLeft);
        Panel(R(.31f,.70f,.38f,.06f));
        Text(R(.32f,.705f,.32f,.04f),"ENTER YOUR EMAIL...",17,TextAnchor.MiddleLeft);
        Box(.39f,.84f,"SEND",true);
        Box(.44f,.92f,"BACK",true);
        if(Hot(.43f,.90f,.18f,.09f))page=0;
    }

    void OnGUI()
    {
        if(!Link())return;
        string st=State();
        if(st!="Menu"&&st!="Settings")return;
        if(page==0)MenuArt();
        else if(page==1)SettingsArt();
        else if(page==2)CreditsArt();
        else FeedbackArt();
    }
}
