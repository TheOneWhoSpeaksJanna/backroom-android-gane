using UnityEngine;
using System;

public sealed class DesolationRuntime:MonoBehaviour{
enum S{Menu,Saves,Settings,Credits,Feedback,Game}
S s=S.Menu;Texture2D px,bg,gold,dark,clear,none;GUIStyle title,sub,lab,btn,ghost,field,center;float master=.9f,music=.75f,sfx=.85f,bright=.9f,sens=1f;int gfx=2,issue;string fb="",mail="",toast="",gameMsg="";float toastUntil;readonly string[] issues={"BUG","BALANCE","PERFORMANCE","IDEA","OTHER"};
<<<<<<< HEAD
void Start(){
    Application.targetFrameRate=60;
    Screen.orientation=ScreenOrientation.LandscapeLeft;
    Load();Tex();Styles();MakeBg();Apply();
    // If a save slot was selected via the UI Kit menu, go straight to game
    int slot = PlayerPrefs.GetInt("SelectedSaveSlot", 0);
    if (slot > 0 && PlayerPrefs.GetInt("SaveSlot"+slot+"_Exists", 0) == 1) {
        s = S.Game;
    }
}
=======
void Start(){Application.targetFrameRate=60;Screen.orientation=ScreenOrientation.LandscapeLeft;Load();Tex();Styles();MakeBg();Apply();}
>>>>>>> f8040f4204ffc9036c4dc7d9ff4b4a812cf323e0
void Update(){if(Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Backspace))Back();}
void OnGUI(){if(px==null){Tex();Styles();MakeBg();}GUI.DrawTexture(R(0,0,1,1),bg,ScaleMode.StretchToFill);Decor();TintByScreen();if(bright<.98f){GUI.color=new Color(0,0,0,(1f-bright)*.45f);GUI.DrawTexture(R(0,0,1,1),px);GUI.color=Color.white;}switch(s){case S.Menu:Menu();break;case S.Saves:Saves();break;case S.Settings:Settings();break;case S.Credits:Credits();break;case S.Feedback:Feedback();break;default:Game();break;}Toast();}
void Menu(){Brand("");B(.365f,.355f,.27f,.074f,"PLAY",()=>s=S.Saves);B(.365f,.465f,.27f,.074f,"SETTINGS",()=>s=S.Settings);B(.365f,.575f,.27f,.074f,"CREDITS",()=>s=S.Credits);B(.365f,.685f,.27f,.074f,"FEEDBACK",()=>s=S.Feedback);}
void Saves(){Brand("SAVES");Card(.285f,"SAVE 1","Backrooms\nLevel 1",()=>Play(1));Card(.44f,"SAVE 2","Backrooms\nLevel 1",()=>Play(2));Card(.595f,"SAVE 3","Backrooms\nLevel 1",()=>Play(3));B(.455f,.82f,.09f,.06f,"BACK",()=>s=S.Menu);}
void Settings(){Brand("SETTINGS");P(R(.305f,.35f,.39f,.40f));bool c=false;master=Sl("MASTER VOLUME",.39f,master,0,1,ref c);music=Sl("MUSIC VOLUME",.46f,music,0,1,ref c);sfx=Sl("SFX VOLUME",.53f,sfx,0,1,ref c);bright=Sl("BRIGHTNESS",.60f,bright,.35f,1.2f,ref c);sens=Sl("SENSITIVITY",.67f,sens,.35f,2,ref c);GUI.Label(R(.325f,.72f,.08f,.045f),"GRAPHICS",lab);string[] n={"LOW","MEDIUM","HIGH"};for(int i=0;i<3;i++){Rect r=R(.435f+i*.06f,.718f,.052f,.045f);if(gfx==i){GUI.color=new Color(1,.72f,.1f,.25f);GUI.DrawTexture(r,px);GUI.color=Color.white;}if(GUI.Button(r,n[i],btn)){gfx=i;c=true;Apply();}}if(B(.405f,.805f,.19f,.065f,"BACK",()=>{Save();s=S.Menu;})){}if(c)Save();}
void Credits(){Brand("CREDITS");Rect p=R(.31f,.36f,.38f,.36f);P(p);string[] a={"GAME DESIGN","PROGRAMMING","UI DESIGN","ENVIRONMENT ART","SOUND DESIGN","SPECIAL THANKS"};for(int i=0;i<a.Length;i++){float y=.395f+i*.046f;GUI.Label(R(.355f,y,.18f,.04f),a[i],lab);Line(R(.49f,y+.022f,.08f,.004f));GUI.Label(R(.58f,y,.13f,.04f),i==5?"THE PLAYERS":"SOLO DEVELOPER",lab);}B(.43f,.79f,.14f,.065f,"BACK",()=>s=S.Menu);}
void Feedback(){Brand("FEEDBACK");P(R(.29f,.36f,.42f,.42f));GUI.Label(R(.315f,.39f,.16f,.04f),"YOUR FEEDBACK",lab);fb=GUI.TextArea(R(.315f,.43f,.37f,.14f),fb,700,field);GUI.Label(R(.315f,.59f,.18f,.04f),"EMAIL (OPTIONAL)",lab);mail=GUI.TextField(R(.315f,.63f,.37f,.045f),mail,120,field);GUI.Label(R(.315f,.69f,.14f,.04f),"ISSUE TYPE",lab);Rect ir=R(.315f,.73f,.37f,.045f);if(GUI.Button(ir,issues[issue]+"    ▾",field))issue=(issue+1)%issues.Length;B(.425f,.805f,.15f,.062f,"SEND",Send);B(.44f,.88f,.12f,.055f,"BACK",()=>s=S.Menu);}
void Game(){Brand("LEVEL 0");Rect p=R(.3f,.38f,.4f,.25f);P(p);GUI.Label(new Rect(p.x,p.y+35,p.width,45),gameMsg,center);B(.42f,.64f,.16f,.06f,"MAIN MENU",()=>s=S.Menu);}
void Brand(string h){Glow(R(0,.045f,1,.09f),"DESOLATION:",title);Glow(R(0,.13f,1,.045f),"THE BACKROOMS",sub);if(h!="")Glow(R(0,.215f,1,.06f),h,sub);}
void Card(float x,string h,string d,Action a){Rect r=R(x,.38f,.12f,.33f);P(r);GUI.Label(new Rect(r.x,r.y+15,r.width,36),h,center);Line(new Rect(r.x+22,r.y+58,r.width-44,3));GUI.Label(new Rect(r.x+14,r.y+78,r.width-28,60),d,center);Mini(new Rect(r.x+22,r.y+145,r.width-44,75));if(GUI.Button(r,"",ghost))a();}
float Sl(string t,float y,float v,float mn,float mx,ref bool c){GUI.Label(R(.33f,y-.012f,.16f,.04f),t,lab);Rect tr=R(.455f,y,.17f,.035f);GUI.DrawTexture(new Rect(tr.x,tr.center.y-2,tr.width,4),dark);float old=v,n=Mathf.InverseLerp(mn,mx,v);GUI.DrawTexture(new Rect(tr.x,tr.center.y-2,tr.width*n,4),gold);GUI.DrawTexture(new Rect(tr.x+tr.width*n-6,tr.center.y-9,12,18),gold);v=GUI.HorizontalSlider(new Rect(tr.x-16,tr.y-12,tr.width+32,32),v,mn,mx,ghost,ghost);if(Mathf.Abs(v-old)>.001f){c=true;Apply();}return v;}
bool B(float x,float y,float w,float h,string t,Action a){Rect r=R(x,y,w,h);bool hit=GUI.Button(r,t,btn);if(hit&&a!=null)a();return hit;}
void P(Rect r){GUI.DrawTexture(r,dark);Line(new Rect(r.x,r.y,r.width,3));Line(new Rect(r.x,r.yMax-3,r.width,3));Line(new Rect(r.x,r.y,3,r.height));Line(new Rect(r.xMax-3,r.y,3,r.height));}
void Line(Rect r){GUI.DrawTexture(r,gold);}
void Mini(Rect r){GUI.DrawTexture(r,clear);P(r);GUI.color=new Color(1,.75f,.12f,.16f);GUI.DrawTexture(new Rect(r.x+r.width*.18f,r.y+8,24,r.height-16),px);GUI.DrawTexture(new Rect(r.x+r.width*.55f,r.y+8,30,r.height-16),px);GUI.color=Color.white;Line(new Rect(r.x+15,r.y+r.height*.55f,r.width-30,3));}
void Glow(Rect r,string t,GUIStyle st){Color o=st.normal.textColor;st.normal.textColor=new Color(1,.62f,.08f,.28f);GUI.Label(new Rect(r.x-2,r.y,r.width,r.height),t,st);GUI.Label(new Rect(r.x+2,r.y,r.width,r.height),t,st);GUI.Label(new Rect(r.x,r.y-2,r.width,r.height),t,st);GUI.Label(new Rect(r.x,r.y+2,r.width,r.height),t,st);st.normal.textColor=o;GUI.Label(r,t,st);}
void Toast(){if(Time.realtimeSinceStartup>toastUntil||toast=="")return;Rect r=R(.28f,.915f,.44f,.055f);P(r);GUI.Label(r,toast,center);}

void Decor(){GUI.color=new Color(0,0,0,.35f);GUI.DrawTexture(R(0,.73f,1,.27f),px);GUI.color=new Color(1,.78f,.18f,.16f);for(int i=0;i<8;i++){float x=.07f+i*.13f;GUI.DrawTexture(R(x,.12f,.045f,.72f),px);GUI.DrawTexture(R(x-.006f,.12f,.057f,.01f),gold);}GUI.color=new Color(1,.84f,.28f,.5f);for(int i=0;i<3;i++){float x=.23f+i*.25f;GUI.DrawTexture(R(x,.10f,.12f,.012f),gold);GUI.color=new Color(1,.75f,.18f,.08f);GUI.DrawTexture(R(x-.03f,.09f,.18f,.05f),px);GUI.color=new Color(1,.84f,.28f,.5f);}GUI.color=Color.white;GUI.Label(R(.04f,.29f,.12f,.06f),"DON'T\nLOOK BACK",lab);}
void TintByScreen(){if(s==S.Saves||s==S.Feedback){GUI.color=new Color(.15f,.08f,0,.12f);GUI.DrawTexture(R(0,0,1,1),px);GUI.color=Color.white;}}
<<<<<<< HEAD
void Play(int slot){
    PlayerPrefs.SetInt("SelectedSaveSlot",slot);
    PlayerPrefs.SetInt("SaveSlot"+slot+"_Exists",1);
    PlayerPrefs.Save();
    gameMsg="SAVE "+slot+" SELECTED - LEVEL 0 READY";
    s=S.Game;
}
=======
void Play(int slot){PlayerPrefs.SetInt("SelectedSaveSlot",slot);PlayerPrefs.SetInt("SaveSlot"+slot+"_Exists",1);PlayerPrefs.Save();gameMsg="SAVE "+slot+" SELECTED - LEVEL 0 READY";s=S.Game;}
>>>>>>> f8040f4204ffc9036c4dc7d9ff4b4a812cf323e0
void Send(){if(string.IsNullOrWhiteSpace(fb)){toast="WRITE FEEDBACK FIRST";toastUntil=Time.realtimeSinceStartup+2;return;}PlayerPrefs.SetString("LastFeedbackMessage",fb.Trim());PlayerPrefs.SetString("LastFeedbackEmail",mail.Trim());PlayerPrefs.SetString("LastFeedbackIssueType",issues[issue]);PlayerPrefs.Save();fb="";toast="FEEDBACK SAVED LOCALLY";toastUntil=Time.realtimeSinceStartup+2.4f;}
void Back(){if(s==S.Settings)Save();if(s!=S.Menu){s=S.Menu;toast="";}}
void Load(){master=PlayerPrefs.GetFloat("MasterVolume",.9f);music=PlayerPrefs.GetFloat("MusicVolume",.75f);sfx=PlayerPrefs.GetFloat("SfxVolume",.85f);bright=PlayerPrefs.GetFloat("Brightness",.9f);sens=PlayerPrefs.GetFloat("Sensitivity",1);gfx=Mathf.Clamp(PlayerPrefs.GetInt("GraphicsQuality",2),0,2);}
void Save(){PlayerPrefs.SetFloat("MasterVolume",master);PlayerPrefs.SetFloat("MusicVolume",music);PlayerPrefs.SetFloat("SfxVolume",sfx);PlayerPrefs.SetFloat("Brightness",bright);PlayerPrefs.SetFloat("Sensitivity",sens);PlayerPrefs.SetInt("GraphicsQuality",gfx);PlayerPrefs.Save();}
void Apply(){AudioListener.volume=Mathf.Clamp01(master);Application.targetFrameRate=gfx==0?30:60;if(QualitySettings.names.Length>0)QualitySettings.SetQualityLevel(Mathf.Clamp(gfx,0,QualitySettings.names.Length-1),true);}
void Tex(){px=T(Color.white);gold=T(new Color(1,.74f,.12f,.95f));dark=T(new Color(0,0,0,.62f));clear=T(new Color(0,0,0,.22f));none=T(new Color(0,0,0,0));}
Texture2D T(Color c){var t=new Texture2D(1,1,TextureFormat.RGBA32,false);t.SetPixel(0,0,c);t.Apply();return t;}
void MakeBg(){bg=new Texture2D(512,288,TextureFormat.RGB24,false);for(int y=0;y<288;y++)for(int x=0;x<512;x++){float v=Mathf.PerlinNoise(x*.045f,y*.045f);float d=1f-y/370f;bg.SetPixel(x,y,new Color(.34f*d+.08f*v,.29f*d+.06f*v,.11f*d+.03f*v));}bg.Apply();}
void Styles(){title=St(44,TextAnchor.MiddleCenter,true);sub=St(20,TextAnchor.MiddleCenter,true);lab=St(16,TextAnchor.MiddleLeft,true);center=St(18,TextAnchor.MiddleCenter,true);btn=St(21,TextAnchor.MiddleCenter,true);btn.normal.background=clear;btn.hover.background=dark;btn.active.background=gold;field=St(15,TextAnchor.UpperLeft,false);field.normal.background=dark;field.focused.background=dark;field.wordWrap=true;field.padding=new RectOffset(8,8,6,6);ghost=new GUIStyle(GUI.skin.button);ghost.normal.background=ghost.hover.background=ghost.active.background=ghost.focused.background=none;ghost.normal.textColor=ghost.hover.textColor=ghost.active.textColor=Color.clear;}
GUIStyle St(int z,TextAnchor a,bool b){var g=new GUIStyle(GUI.skin.label);g.fontSize=Mathf.Max(12,Mathf.RoundToInt(z*Mathf.Clamp(Screen.height/720f,.75f,1.4f)));g.alignment=a;g.fontStyle=b?FontStyle.Bold:FontStyle.Normal;g.normal.textColor=new Color(1,.82f,.34f);return g;}
Rect R(float x,float y,float w,float h){return new Rect(Screen.width*x,Screen.height*y,Screen.width*w,Screen.height*h);}
}
