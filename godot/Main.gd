extends Node3D
const S:="user://checkpoint_slot_"
const C:="user://desolation_settings.cfg"
var p:CharacterBody3D;var cam:Camera3D;var e:CharacterBody3D;var ui:CanvasLayer;var h:Label;var m:Label;var q:Label;var ep:Panel;var el:Label;var pp:Panel;var sh:ColorRect
var mat={};var it=[];var fs=[];var rn=[];var got=[];var sw={};var mv:=Vector2.ZERO;var lk:=Vector2.ZERO;var pitch:=0.0;var st:=1.0;var sa:=1.0;var run:=false;var pa:=false;var over:=false;var sl:=0;var scraps:=2;var hide:=0.0;var seen:=Vector3(10,.3,10);var lure:=Vector3.ZERO;var lure_t:=0.0;var mt:=0.0;var sens:=1.0;var bs:=1.0;var hand:=0.0;var lz:=.48

func _ready():
 sl=int(get_tree().get_meta("desolation_slot",0));loadset();mats();level();ply();makeui();loadg();saveg();say("Find 3 fuses. Read 5 notes for the truth ending.",4)

func mats():
 mat.w=sm(Color(.72,.63,.25),"res://assets/textures/wallpaper_yellow_color.png");mat.f=sm(Color(.35,.27,.13),"res://assets/textures/uploaded/carpet_fabric_color.jpg");mat.c=sm(Color(.62,.6,.46),"res://assets/textures/uploaded/office_ceiling_color.jpg");mat.m=sm(Color(.22,.21,.2),"res://assets/textures/uploaded/painted_metal_color.jpg");mat.r=sm(Color(.15,.15,.14),"res://assets/textures/uploaded/raw_metal_color.jpg");mat.wo=sm(Color(.32,.22,.12),"res://assets/textures/uploaded/wood_trim_color.jpg");mat.l=sm(Color(.2,.15,.08,.8),"res://assets/textures/uploaded/wall_leak_color.jpg");mat.d=sm(Color(.01,.01,.008),"");mat.b=sm(Color(.1,.75,1),"");mat.b.emission_enabled=true;mat.b.emission=Color(.1,.75,1);mat.b.emission_energy_multiplier=1.4

func sm(c,path):
 var a:=StandardMaterial3D.new();a.albedo_color=c;a.roughness=.9
 if path!="" and ResourceLoader.exists(path):a.albedo_texture=load(path)
 return a

func level():
 var w:=WorldEnvironment.new();var en:=Environment.new();en.background_mode=Environment.BG_COLOR;en.background_color=Color(.04,.035,.02);en.ambient_light_source=Environment.AMBIENT_SOURCE_COLOR;en.ambient_light_color=Color(.55,.5,.34);en.ambient_light_energy=.65;en.set("fog_enabled",true);en.set("fog_density",.055);w.environment=en;add_child(w)
 bx(Vector3(0,-.05,0),Vector3(34,.1,34),mat.f,true);bx(Vector3(0,2.55,0),Vector3(34,.1,34),mat.c,false)
 for x in [-17,17]:bx(Vector3(x,1.2,0),Vector3(.2,2.5,34),mat.w,true)
 for z in [-17,17]:bx(Vector3(0,1.2,z),Vector3(34,2.5,.2),mat.w,true)
 for z in [-10,0,10]:bx(Vector3(-5,1.2,z),Vector3(.18,2.5,6),mat.w,true);bx(Vector3(5,1.2,z),Vector3(.18,2.5,6),mat.w,true)
 for a in [Vector3(-10,2.45,-10),Vector3(0,2.45,0),Vector3(10,2.45,10),Vector3(9,2.45,-8)]:
  bx(a,Vector3(2,.06,.38),mat.c,false);var l:=OmniLight3D.new();l.light_energy=.6;l.omni_range=8;add_child(l);l.global_position=a
 for a in [Vector3(-16,1.2,-3),Vector3(16,1.2,8),Vector3(2,1.2,16)]:leak(a)
 f("f1",Vector3(-12,.55,-11));f("f2",Vector3(13,.55,-5));f("f3",Vector3(-5,.55,13))
 n("n1",Vector3(0,.7,-16),"BLUE power wakes the service exit.");n("n2",Vector3(-16,.7,0),"Leaks mark safe routes.");n("n3",Vector3(16,.7,2),"It follows sound. Throw scraps.");n("n4",Vector3(-10,.7,15),"Read before leaving.");n("n5",Vector3(12,.7,15),"Truth needs memory and sanity.")
 for a in [Vector3(-14,.8,6),Vector3(8,.8,-14)]:itadd(bx(a,Vector3(1.2,1.3,.7),mat.w,true),"hide","Hiding stain")
 itadd(bx(Vector3(-15,1,-15),Vector3(.7,.9,.2),mat.r,false),"switch","Breaker")
 itadd(bx(Vector3(0,1,-5),Vector3(2,2,.2),mat.m,true),"lock","Locked door")
 itadd(bx(Vector3(2,.4,14),Vector3(.8,.4,.4),mat.r,false),"radio","Broken radio")
 itadd(bx(Vector3(16.8,1,14),Vector3(.25,2,2),mat.m,true),"exit","Exit")
 enemy()

func leak(a):
 var mi:=MeshInstance3D.new();var qm:=QuadMesh.new();qm.size=Vector2(2,2);mi.mesh=qm;mi.material_override=mat.l;add_child(mi);mi.global_position=a;if abs(a.x)>abs(a.z):mi.rotation_degrees.y=90

func f(id,a):
 var o:=bx(a,Vector3(.45,.45,.45),mat.b,false);o.name=id;fs.append(o);itadd(o,"fuse","Blue fuse")
func n(id,a,t):
 var o:=bx(a,Vector3(.8,.05,.5),mat.wo,false);o.name=id;itadd(o,"note",t)

func enemy():
 e=CharacterBody3D.new();add_child(e);e.global_position=Vector3(12,.35,12);var co:=CollisionShape3D.new();var ca:=CapsuleShape3D.new();ca.height=1.7;ca.radius=.3;co.shape=ca;e.add_child(co);var mi:=MeshInstance3D.new();var cm:=CapsuleMesh.new();cm.height=1.7;cm.radius=.3;mi.mesh=cm;mi.material_override=mat.d;e.add_child(mi)

func ply():
 p=CharacterBody3D.new();add_child(p);p.global_position=Vector3.ZERO;var co:=CollisionShape3D.new();var ca:=CapsuleShape3D.new();ca.height=1.55;ca.radius=.32;co.shape=ca;co.position=Vector3(0,.85,0);p.add_child(co);cam=Camera3D.new();cam.current=true;cam.position=Vector3(0,1.58,0);p.add_child(cam)

func makeui():
 ui=CanvasLayer.new();add_child(ui);sh=ColorRect.new();sh.set_anchors_preset(Control.PRESET_FULL_RECT);sh.mouse_filter=Control.MOUSE_FILTER_IGNORE;ui.add_child(sh);h=lbl(Vector2(20,16),22);m=lbl(Vector2(20,52),20);q=lbl(Vector2(430,565),22);buttons();pp=Panel.new();pp.position=Vector2(405,150);pp.size=Vector2(470,300);pp.visible=false;ui.add_child(pp);pbt("RESUME",70,resume);pbt("SAVE",140,saveg);pbt("MENU",210,menu);ep=Panel.new();ep.position=Vector2(300,130);ep.size=Vector2(680,360);ep.visible=false;ui.add_child(ep);el=lbl(Vector2(340,170),26);ep.add_child(el)

func lbl(pos,fs):
 var l:=Label.new();l.position=pos;l.size=Vector2(900,120);l.add_theme_font_size_override("font_size",fs);l.add_theme_color_override("font_color",Color(.95,.85,.45));ui.add_child(l);return l
func buttons():
 var lx:=40 if hand<.5 else 930;var rx:=990 if hand<.5 else 40
 bt("↑",Vector2(lx+85,520),func():mv.y=1,func():mv.y=0);bt("↓",Vector2(lx+85,610),func():mv.y=-1,func():mv.y=0);bt("←",Vector2(lx,610),func():mv.x=-1,func():mv.x=0);bt("→",Vector2(lx+170,610),func():mv.x=1,func():mv.x=0);bt("SPRINT",Vector2(rx,585),func():run=true,func():run=false);bt("USE",Vector2(555,628),use);bt("THROW",Vector2(rx,665),throw);bt("MENU",Vector2(1140,22),pause)
func bt(t,pos,d,u:=Callable()):
 var b:=Button.new();b.text=t;b.position=pos;b.size=(Vector2(76,66) if t.length()<2 else Vector2(175,62))*bs;if u.is_valid():b.button_down.connect(d);b.button_up.connect(u)
 else:b.pressed.connect(d)
 ui.add_child(b)
func pbt(t,y,cb):
 var b:=Button.new();b.text=t;b.position=Vector2(95,y);b.size=Vector2(280,56);b.pressed.connect(cb);pp.add_child(b)

func _physics_process(d):
 if pa or over:return
 var k:=Vector2.ZERO;if Input.is_key_pressed(KEY_A):k.x-=1;if Input.is_key_pressed(KEY_D):k.x+=1;if Input.is_key_pressed(KEY_W):k.y+=1;if Input.is_key_pressed(KEY_S):k.y-=1
 var mo:=k+mv;if mo.length()>1:mo=mo.normalized()
 p.rotate_y(-lk.x*.004*sens);pitch=clamp(pitch+lk.y*-.003*sens,-1.4,1.4);cam.rotation.x=pitch;lk=Vector2.ZERO
 var dir:=p.global_transform.basis*Vector3(mo.x,0,-mo.y);dir.y=0;if dir.length()>1:dir=dir.normalized()
 var speed:=5.2 if run and st>.05 else 3.4;p.velocity=Vector3(dir.x*speed,p.velocity.y,dir.z*speed);p.velocity.y=p.velocity.y-18*d if not p.is_on_floor() else -.1;p.move_and_slide()
 st=max(0,st-d*.15) if run and dir.length()>.1 else min(1,st+d*.18);sa=min(1,sa+d*.04) if Time.get_ticks_msec()/1000.0<hide else max(0,sa-d*.004)
 ai(d);chk();h.text="Slot %d | Fuses %d/3 | Notes %d/5 | Scraps %d | Stamina %d%% | Sanity %d%%"%[sl+1,got.size(),rn.size(),scraps,int(st*100),int(sa*100)];mt-=d;if mt<=0:m.text=""

func ai(d):
 var now:=Time.get_ticks_msec()/1000.0;var hid:=now<hide;var tar:=lure if now<lure_t else seen
 if see() and not hid:seen=p.global_position;tar=seen
 var v:=tar-e.global_position;v.y=0;if v.length()>.2:e.velocity=Vector3(v.normalized().x*2.2,-.1,v.normalized().z*2.2)
 else:e.velocity=Vector3.ZERO
 e.move_and_slide();var dist:=e.global_position.distance_to(p.global_position);if dist<4.8 and not hid:sa=max(0,sa-d*.09)
 sh.color=Color(0,0,0,clamp((1-sa)*.4,0,.65));if dist<1.2 and not hid:end("lost");if sa<=0:end("desolation")

func see():
 var r:=PhysicsRayQueryParameters3D.create(e.global_position+Vector3(0,1,0),p.global_position+Vector3(0,1,0));r.exclude=[e];var hit:=get_world_3d().direct_space_state.intersect_ray(r);return hit.is_empty() or hit.get("collider")==p
func chk():
 var c:=near();q.text="" if c.is_empty() else "USE: "+c.prompt;if got.size()>=3:sw.service=true
func use():
 var c:=near();if c.is_empty():say("Nothing close.",1);return
 if c.kind=="fuse":got.append(c.node.name);fs.erase(c.node);it.erase(c);c.node.queue_free();say("Fuse "+str(got.size())+"/3",2);saveg()
 elif c.kind=="note":if not rn.has(c.node.name):rn.append(c.node);say(c.prompt,4);saveg()
 elif c.kind=="hide":hide=Time.get_ticks_msec()/1000.0+3;say("You hide. The pressure fades.",2)
 elif c.kind=="switch":sw.breaker=true;say("Breaker on. Locked route opens.",2);saveg()
 elif c.kind=="radio":scraps+=1;it.erase(c);say("Scrap gained.",2)
 elif c.kind=="lock":say("Locked. Find breaker.",2)
 elif c.kind=="exit":if got.size()<3:say("No power.",2);elif rn.size()>=5 and sa>.42:end("truth");elif sa<.28:end("desolation");else:end("escape")
func throw():
 if scraps<=0:say("No scraps.",1);return
 scraps-=1;lure=p.global_position+-p.global_transform.basis.z*6;lure.y=.4;lure_t=Time.get_ticks_msec()/1000.0+5;say("Metal clatters away.",2)
func near():
 var b={};var bd=R;for a in it:if is_instance_valid(a.node):var d=p.global_position.distance_to(a.node.global_position);if d<bd:bd=d;b=a
 return b
func itadd(n,k,t):it.append({"node":n,"kind":k,"prompt":t})
func saveg():
 var c:=ConfigFile.new();c.set_value("p","pos",p.global_position);c.set_value("s","f",",".join(got));c.set_value("s","n",",".join(rn));c.set_value("s","scr",scraps);c.set_value("s","san",sa);c.save(S+str(sl)+".cfg")
func loadg():
 var c:=ConfigFile.new();if c.load(S+str(sl)+".cfg")!=OK:return
 p.global_position=c.get_value("p","pos",Vector3.ZERO);got=arr(c.get_value("s","f",""));rn=arr(c.get_value("s","n",""));scraps=int(c.get_value("s","scr",2));sa=float(c.get_value("s","san",1));for f in fs.duplicate():if got.has(f.name):fs.erase(f);f.queue_free()
func arr(t):var a=[];for x in str(t).split(",",false):if x!="":a.append(x);return a
func loadset():
 var c:=ConfigFile.new();c.load(C);sens=float(c.get_value("settings","sensitivity",1));bs=float(c.get_value("controls","button_scale",1));hand=float(c.get_value("controls","handedness",0));lz=float(c.get_value("controls","look_zone",.48));Engine.max_fps=int(c.get_value("settings","frame_cap",60))
func end(k):
 if over:return
 over=true;ep.visible=true;saveg();el.text={"truth":"TRUTH ENDING\nYou understood the route.","desolation":"DESOLATION ENDING\nThe room leaves with you.","lost":"LOST ENDING\nIt learned your steps."}.get(k,"ESCAPE ENDING\nYou escaped, but unread.")
func pause():if over:return;pa=not pa;pp.visible=pa
func resume():pa=false;pp.visible=false
func menu():get_tree().change_scene_to_file("res://backrooms_ui/scenes/main_menu.tscn")
func say(t,d):m.text=t;mt=d
func _input(ev):
 if ev is InputEventScreenDrag:var w:=get_viewport().get_visible_rect().size.x;if (ev.position.x>w*lz if hand<.5 else ev.position.x<w*(1-lz)):lk+=ev.relative
 if ev is InputEventKey and ev.pressed:if ev.keycode==KEY_E:use();elif ev.keycode==KEY_F:throw();elif ev.keycode==KEY_ESCAPE or ev.keycode==KEY_BACK:pause()
func bx(pos,sz,ma,col:=true):
 var mi:=MeshInstance3D.new();var bm:=BoxMesh.new();bm.size=sz;mi.mesh=bm;mi.material_override=ma;add_child(mi);mi.global_position=pos;if col:var sb:=StaticBody3D.new();var co:=CollisionShape3D.new();var sh:=BoxShape3D.new();sh.size=sz;co.shape=sh;sb.add_child(co);mi.add_child(sb)
 return mi
