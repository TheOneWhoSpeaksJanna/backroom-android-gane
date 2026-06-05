extends Node3D

enum State { LOADING, TITLE, PLAY, PAUSE, WIN, CAUGHT }
const W:=11
const H:=11
const CELL:=4.0
var state:=State.LOADING
var open:={}
var rng:=RandomNumberGenerator.new()
var player:CharacterBody3D
var cam:Camera3D
var monster:CharacterBody3D
var breakers:=[]
var done:=[]
var exit_door:Node3D
var count:=0
var move:=Vector2.ZERO
var look:=Vector2.ZERO
var sprint:=false
var stamina:=1.0
var pitch:=0.0
var joy_id:=-1
var look_id:=-1
var sprint_id:=-1
var joy_base:=Vector2.ZERO
var msg:="Tap START to enter Level 0"
var msg_t:=4.0
var loading_t:=1.25
var debug_log_path:="user://BackroomsLevelZeroLogs/latest.log"
var debug_tick:=0.0
var monster_target:=Vector3.ZERO
var alert:=0.0
var hud:Label
var msg_label:Label
var bar:ProgressBar
var pause_b:Button
var sprint_b:Button
var start_b:Button
var exit_b:Button
var resume_b:Button
var restart_b:Button
var menu_b:Button
var quit_b:Button
var end_restart_b:Button
var loading_panel:Panel
var menu_panel:Panel
var pause_panel:Panel
var end_panel:Panel
var shade:ColorRect
var log_file:FileAccess
var log_path:=""
var log_tick:=0.0
var pause_rect:=Rect2()
var sprint_rect:=Rect2()
var wall:StandardMaterial3D
var floor_m:StandardMaterial3D
var ceil_m:StandardMaterial3D
var light_m:StandardMaterial3D
var black:StandardMaterial3D
var yellow:StandardMaterial3D
var red:StandardMaterial3D
var brown:StandardMaterial3D

func _ready():
	DirAccess.open("user://").make_dir_recursive("BackroomsLevelZeroLogs")
	FileAccess.open(debug_log_path, FileAccess.WRITE).store_line("Backrooms Level Zero debug log started\nsession_log_started")
	rng.seed=19740416
	setup_logging()
	log_event("ready","startup")
	make_materials()
	make_world_env()
	make_map()
	build_level()
	make_player()
	make_monster()
	make_ui()

func make_materials():
	wall=mat(Color(.72,.63,.28),.95)
	floor_m=mat(Color(.33,.27,.13),.98)
	ceil_m=mat(Color(.62,.60,.46),.86)
	light_m=mat(Color(1,.92,.55),.2); light_m.emission_enabled=true; light_m.emission=Color(1,.88,.5); light_m.emission_energy_multiplier=1.4
	black=mat(Color(.004,.004,.003),.8)
	yellow=mat(Color(.95,.78,.05),.6)
	red=mat(Color(.86,.03,.02),.6)
	brown=mat(Color(.32,.16,.07),.75)

func mat(c,r):
	var m:=StandardMaterial3D.new(); m.albedo_color=c; m.roughness=r; return m

func make_world_env():
	var we:=WorldEnvironment.new(); var e:=Environment.new()
	e.background_mode=Environment.BG_COLOR; e.background_color=Color(.055,.052,.036)
	e.ambient_light_source=Environment.AMBIENT_SOURCE_COLOR; e.ambient_light_color=Color(.55,.50,.32); e.ambient_light_energy=.56
	e.fog_enabled=true; e.fog_density=.03; e.fog_light_color=Color(.70,.63,.38)
	we.environment=e; add_child(we)

func make_map():
	var stack=[Vector2i(1,1)]; var visited={Vector2i(1,1):true}; open[Vector2i(1,1)]=true
	var dirs=[Vector2i(2,0),Vector2i(-2,0),Vector2i(0,2),Vector2i(0,-2)]
	while not stack.is_empty():
		var c=stack.back(); var choices=[]
		for d in dirs:
			var n=c+d
			if n.x>0 and n.y>0 and n.x<W-1 and n.y<H-1 and not visited.has(n): choices.append(d)
		if choices.is_empty(): stack.pop_back()
		else:
			var d=choices[rng.randi_range(0,choices.size()-1)]
			open[c+d/2]=true; open[c+d]=true; visited[c+d]=true; stack.append(c+d)
	for i in range(25):
		var c=Vector2i(rng.randi_range(1,W-2),rng.randi_range(1,H-2)); open[c]=true
	for c in [Vector2i(1,1),Vector2i(9,9),Vector2i(3,3),Vector2i(6,7),Vector2i(8,4)]: open[c]=true

func build_level():
	for x in range(W):
		for y in range(H):
			var c=Vector2i(x,y)
			if not is_open(c): continue
			var p=cell_to_world(c)
			box(self,p+Vector3(0,-.06,0),Vector3(CELL,.12,CELL),floor_m)
			box(self,p+Vector3(0,2.55,0),Vector3(CELL,.12,CELL),ceil_m)
			if not is_open(c+Vector2i(1,0)): box(self,p+Vector3(CELL*.5,1.2,0),Vector3(.14,2.5,CELL),wall)
			if not is_open(c+Vector2i(-1,0)): box(self,p+Vector3(-CELL*.5,1.2,0),Vector3(.14,2.5,CELL),wall)
			if not is_open(c+Vector2i(0,1)): box(self,p+Vector3(0,1.2,CELL*.5),Vector3(CELL,2.5,.14),wall)
			if not is_open(c+Vector2i(0,-1)): box(self,p+Vector3(0,1.2,-CELL*.5),Vector3(CELL,2.5,.14),wall)
			if (x*7+y*11)%4==0: add_light(p)
			if rng.randf()<.08: add_chair(p+Vector3(rng.randf_range(-1,1),0,rng.randf_range(-1,1)))
	for c in [Vector2i(3,3),Vector2i(6,7),Vector2i(8,4)]:
		var n=Node3D.new(); add_child(n); n.global_position=cell_to_world(c)+Vector3(0,.9,-1.85); box(n,Vector3.ZERO,Vector3(.6,.8,.12),red); breakers.append(n); done.append(false)
	exit_door=Node3D.new(); add_child(exit_door); exit_door.global_position=cell_to_world(Vector2i(9,9))+Vector3(0,1.05,-1.85); box(exit_door,Vector3.ZERO,Vector3(1.1,2.1,.16),brown)

func add_light(p):
	box(self,p+Vector3(0,2.43,0),Vector3(1.7,.05,.35),light_m)
	var l:=OmniLight3D.new(); l.light_color=Color(1,.88,.48); l.light_energy=.8; l.omni_range=7; add_child(l); l.global_position=p+Vector3(0,2.15,0)

func add_chair(p):
	var n=Node3D.new(); add_child(n); n.global_position=p; n.rotate_y(rng.randf_range(0,TAU))
	box(n,Vector3(0,.45,0),Vector3(.8,.12,.7),brown); box(n,Vector3(0,1,-.32),Vector3(.8,.9,.12),brown)
	box(n,Vector3(.3,.2,.25),Vector3(.07,.4,.07),black); box(n,Vector3(-.3,.2,.25),Vector3(.07,.4,.07),black)

func make_player():
	player=CharacterBody3D.new(); add_child(player); player.global_position=cell_to_world(Vector2i(1,1))+Vector3(0,.05,0)
	var cs:=CollisionShape3D.new(); var cap:=CapsuleShape3D.new(); cap.height=1.55; cap.radius=.32; cs.shape=cap; cs.position=Vector3(0,.85,0); player.add_child(cs)
	cam=Camera3D.new(); cam.position=Vector3(0,1.62,0); cam.fov=72; player.add_child(cam)
	box(cam,Vector3(-.35,-.35,-.85),Vector3(.18,.18,.75),yellow); box(cam,Vector3(.35,-.35,-.85),Vector3(.18,.18,.75),yellow)

func make_monster():
	monster=CharacterBody3D.new(); add_child(monster); monster.global_position=cell_to_world(Vector2i(8,8))
	var cs:=CollisionShape3D.new(); var cap:=CapsuleShape3D.new(); cap.height=2.1; cap.radius=.32; cs.shape=cap; cs.position=Vector3(0,1.05,0); monster.add_child(cs)
	box(monster,Vector3(0,1.05,0),Vector3(.45,1.7,.35),black); box(monster,Vector3(0,2.05,0),Vector3(.34,.38,.34),black)
	box(monster,Vector3(-.35,1,0),Vector3(.1,1.3,.1),black); box(monster,Vector3(.35,1,0),Vector3(.1,1.3,.1),black)

func make_ui():
var layer=CanvasLayer.new(); add_child(layer)

shade=ColorRect.new(); shade.color=Color(.010,.009,.004,.58); shade.set_anchors_preset(Control.PRESET_FULL_RECT); layer.add_child(shade)

var bg=Control.new(); bg.set_anchors_preset(Control.PRESET_FULL_RECT); layer.add_child(bg)
var base=ColorRect.new(); base.color=Color(.070,.061,.026,.84); base.set_anchors_preset(Control.PRESET_FULL_RECT); bg.add_child(base)
for i in range(18):
var stripe=ColorRect.new(); stripe.color=Color(.70,.62,.23,.10 if i%2==0 else .18); stripe.position=Vector2(i*82,0); stripe.size=Vector2(34,720); bg.add_child(stripe)
for i in range(12):
var grime=ColorRect.new(); grime.color=Color(.02,.018,.010,.10); grime.position=Vector2((i*137)%1280, 500+(i*23)%180); grime.size=Vector2(170+(i%4)*35, 10+(i%3)*14); bg.add_child(grime)

loading_panel=make_panel(layer,Vector2(260,190),Vector2(760,315),Color(.040,.034,.016,.96))
var lt=make_label(loading_panel,"BACKROOMS
LEVEL ZERO",Vector2(48,36),52,Color(.98,.86,.38)); lt.size=Vector2(660,130); lt.horizontal_alignment=HORIZONTAL_ALIGNMENT_CENTER
var ll=make_label(loading_panel,"loading fluorescent maze...",Vector2(72,205),28,Color(.82,.76,.48)); ll.size=Vector2(620,60); ll.horizontal_alignment=HORIZONTAL_ALIGNMENT_CENTER

menu_panel=make_panel(layer,Vector2(380,105),Vector2(520,485),Color(.050,.043,.020,.95))
var title=make_label(menu_panel,"BACKROOMS
LEVEL ZERO",Vector2(34,34),46,Color(.98,.86,.38)); title.size=Vector2(452,125); title.horizontal_alignment=HORIZONTAL_ALIGNMENT_CENTER
var sub=make_label(menu_panel,"Find the breakers. Reach the strange exit.
Avoid what listens in the yellow rooms.",Vector2(44,170),21,Color(.80,.75,.52)); sub.size=Vector2(432,90); sub.horizontal_alignment=HORIZONTAL_ALIGNMENT_CENTER
start_b=button(menu_panel,"ENTER LEVEL 0",Vector2(112,286),Vector2(296,66)); start_b.pressed.connect(start_game)
exit_b=button(menu_panel,"EXIT",Vector2(160,372),Vector2(200,54)); exit_b.pressed.connect(func(): log_event("quit","main menu"); get_tree().quit())

hud=Label.new(); hud.position=Vector2(28,22); hud.add_theme_font_size_override("font_size",26); hud.add_theme_color_override("font_color",Color(.95,.86,.45)); layer.add_child(hud)
msg_label=Label.new(); msg_label.position=Vector2(28,62); msg_label.add_theme_font_size_override("font_size",22); msg_label.add_theme_color_override("font_color",Color(.90,.82,.55)); layer.add_child(msg_label)
bar=ProgressBar.new(); bar.position=Vector2(28,670); bar.size=Vector2(280,18); bar.max_value=100; bar.show_percentage=false; layer.add_child(bar)

pause_b=button(layer,"II",Vector2(1180,24),Vector2(70,62)); pause_b.pressed.connect(open_pause)
sprint_b=button(layer,"SPRINT",Vector2(1050,575),Vector2(170,78)); sprint_b.button_down.connect(func(): sprint=true); sprint_b.button_up.connect(func(): sprint=false)

pause_panel=make_panel(layer,Vector2(405,122),Vector2(470,470),Color(.040,.035,.018,.97))
var pt=make_label(pause_panel,"PAUSED",Vector2(115,32),48,Color(.96,.84,.40)); pt.size=Vector2(240,70); pt.horizontal_alignment=HORIZONTAL_ALIGNMENT_CENTER
resume_b=button(pause_panel,"RESUME",Vector2(95,132),Vector2(280,62)); resume_b.pressed.connect(resume_game)
restart_b=button(pause_panel,"RESTART",Vector2(95,212),Vector2(280,62)); restart_b.pressed.connect(func(): log_event("restart","pause menu"); get_tree().reload_current_scene())
menu_b=button(pause_panel,"MAIN MENU",Vector2(95,292),Vector2(280,62)); menu_b.pressed.connect(back_to_menu)
quit_b=button(pause_panel,"QUIT",Vector2(95,370),Vector2(280,54)); quit_b.pressed.connect(func(): log_event("quit","pause menu"); get_tree().quit())

end_panel=make_panel(layer,Vector2(335,185),Vector2(610,310),Color(.040,.035,.018,.97))
var et=make_label(end_panel,"LEVEL 0",Vector2(60,42),46,Color(.96,.84,.40)); et.size=Vector2(490,65); et.horizontal_alignment=HORIZONTAL_ALIGNMENT_CENTER
var ed=make_label(end_panel,"Tap restart to try again.",Vector2(70,120),25,Color(.84,.78,.55)); ed.size=Vector2(470,60); ed.horizontal_alignment=HORIZONTAL_ALIGNMENT_CENTER
end_restart_b=button(end_panel,"RESTART",Vector2(185,215),Vector2(240,58)); end_restart_b.pressed.connect(func(): get_tree().reload_current_scene())

update_hud()

func make_panel(parent,p,s,c):
var n=Panel.new(); n.position=p; n.size=s
var st=StyleBoxFlat.new(); st.bg_color=c; st.border_color=Color(.68,.57,.23,.92); st.set_border_width_all(2)
st.corner_radius_top_left=10; st.corner_radius_top_right=10; st.corner_radius_bottom_left=10; st.corner_radius_bottom_right=10
n.add_theme_stylebox_override("panel",st); parent.add_child(n); return n

func make_label(parent,t,p,fs,col):
var l=Label.new(); l.text=t; l.position=p; l.add_theme_font_size_override("font_size",fs); l.add_theme_color_override("font_color",col); parent.add_child(l); return l

func button(parent,t,p,s):
var b:=Button.new(); b.text=t; b.position=p; b.size=s; b.focus_mode=Control.FOCUS_NONE; b.add_theme_font_size_override("font_size",22); b.add_theme_color_override("font_color",Color(.96,.86,.48))
var st:=StyleBoxFlat.new(); st.bg_color=Color(.18,.15,.055,.90); st.border_color=Color(.78,.65,.25,.95); st.set_border_width_all(2)
st.corner_radius_top_left=8; st.corner_radius_top_right=8; st.corner_radius_bottom_left=8; st.corner_radius_bottom_right=8
var pr:StyleBoxFlat=st.duplicate(); pr.bg_color=Color(.34,.28,.085,.98)
b.add_theme_stylebox_override("normal",st); b.add_theme_stylebox_override("hover",st); b.add_theme_stylebox_override("pressed",pr)
parent.add_child(b); return b

func _physics_process(delta):
if state==State.LOADING:
loading_t-=delta
if loading_t<=0:
state=State.TITLE
show("Tap ENTER LEVEL 0 to begin.")
log_event("loading_complete","title menu shown")
update_hud(); return
if state==State.PLAY:
var key=Vector2.ZERO
if Input.is_key_pressed(KEY_A): key.x-=1
if Input.is_key_pressed(KEY_D): key.x+=1
if Input.is_key_pressed(KEY_W): key.y+=1
if Input.is_key_pressed(KEY_S): key.y-=1
var mv=move if key.length()<.1 else key.normalized()
move_player(mv,delta); move_monster(delta); check_objectives()
if player.global_position.y < -4.0:
player.global_position=cell_to_world(Vector2i(1,1))+Vector3(0,.35,0); player.velocity=Vector3.ZERO
show("The carpet drops away. Level 0 puts you back."); log_event("void_recovery",str(player.global_position))
if player.global_position.distance_to(monster.global_position)<1.1:
state=State.CAUGHT; show("It found you."); log_event("caught","player=%s monster=%s" % [str(player.global_position),str(monster.global_position)])
if msg_t>0: msg_t-=delta
log_tick+=delta
if state==State.PLAY and log_tick>=3.0:
log_tick=0.0; log_event("runtime","pos=%s stamina=%.2f objective=%d/3" % [str(player.global_position),stamina,count])
update_hud()

func move_player(mv,delta):
player.rotate_y(-look.x*.0038); pitch=clamp(pitch+look.y*-.0033,deg_to_rad(-80),deg_to_rad(80)); cam.rotation.x=pitch; look=Vector2.ZERO
var wish=player.global_transform.basis*Vector3(mv.x,0,-mv.y); wish.y=0
if wish.length()>1: wish=wish.normalized()
var run=sprint and wish.length()>.05 and stamina>0.0
if run:
stamina=max(0.0,stamina-delta*.11)
if stamina<=0.0: sprint=false
elif wish.length()<.05 and not sprint:
stamina=min(1.0,stamina+delta*.20)
var sp=5.15 if run else 3.2
player.velocity.x=wish.x*sp; player.velocity.z=wish.z*sp; player.velocity.y=-.1 if player.is_on_floor() else player.velocity.y-18*delta
player.move_and_slide()
if run: alert=min(1,alert+delta*.25)

func move_monster(delta):
	var d=monster.global_position.distance_to(player.global_position)
	var target=player.global_position if d<13 or alert>.35 else monster_target
	if target==Vector3.ZERO or monster.global_position.distance_to(target)<.8: monster_target=random_open(); target=monster_target
	var n=next_path(monster.global_position,target); var dir=n-monster.global_position; dir.y=0
	if dir.length()>.05: dir=dir.normalized()
	var sp=2.05 if d<13 or alert>.35 else .85
	monster.velocity.x=dir.x*sp; monster.velocity.z=dir.z*sp; monster.velocity.y=-.1 if monster.is_on_floor() else monster.velocity.y-18*delta
	monster.move_and_slide(); alert=max(0,alert-delta*.05)

func check_objectives():
	for i in range(breakers.size()):
		if not done[i] and player.global_position.distance_to(breakers[i].global_position)<1.55:
			done[i]=true; count+=1; alert=1; show("Emergency breaker %d/3 activated."%count)
			for ch in breakers[i].get_children():
				if ch is MeshInstance3D: ch.material_override=mat(Color(.04,.55,.05),.55)
	if count>=3 and player.global_position.distance_to(exit_door.global_position)<1.6: state=State.WIN; show("You found the strange exit door.")

func _input(e):
if e is InputEventMouseMotion and OS.get_name()!="Android" and Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT) and state==State.PLAY:
look+=e.relative
if e is InputEventScreenTouch:
var p=e.position
if e.pressed:
if is_ui_point(p): return
if state==State.TITLE or state==State.LOADING: return
if state==State.WIN or state==State.CAUGHT: return
if state!=State.PLAY: return
var screen=get_viewport().get_visible_rect().size
if p.x<screen.x*.45 and joy_id==-1:
joy_id=e.index; joy_base=p
elif p.x>=screen.x*.50 and look_id==-1:
look_id=e.index
else:
if e.index==joy_id:
joy_id=-1; move=Vector2.ZERO
if e.index==look_id:
look_id=-1
if e.index==sprint_id:
sprint=false; sprint_id=-1
elif e is InputEventScreenDrag:
if state!=State.PLAY: return
if e.index==joy_id:
var r:float=min(get_viewport().get_visible_rect().size.y*.16,150.0); var d=e.position-joy_base
if d.length()>r: d=d.normalized()*r
move=Vector2(d.x/r,-d.y/r)
elif e.index==look_id:
look+=e.relative

func update_hud():
hud.text=objective(); msg_label.text=msg if msg_t>0 else ""; bar.value=stamina*100.0
var loading=state==State.LOADING; var title=state==State.TITLE; var play=state==State.PLAY; var pause=state==State.PAUSE; var ended=state==State.WIN or state==State.CAUGHT
if shade: shade.visible=loading or title or pause or ended
if loading_panel: loading_panel.visible=loading
if menu_panel: menu_panel.visible=title
if pause_panel: pause_panel.visible=pause
if end_panel: end_panel.visible=ended
pause_b.visible=play or pause; sprint_b.visible=play; bar.visible=play
hud.visible=not loading and not title; msg_label.visible=not loading and not title
pause_rect=Rect2(pause_b.position,pause_b.size); sprint_rect=Rect2(sprint_b.position,sprint_b.size)

func objective():
if state==State.LOADING: return "Backrooms Level Zero
Loading..."
if state==State.TITLE: return "Backrooms Level Zero"
if state==State.PAUSE: return "Paused"
if state==State.WIN: return "Escaped Level 0."
if state==State.CAUGHT: return "Caught. Try again."
if count<3: return "Objective: activate breakers %d/3"%count
return "Objective: reach the strange exit door"

func start_game():
state=State.PLAY; show("Objective: activate 3 red breaker boxes."); log_event("start_game",str(player.global_position))

func open_pause():
if state==State.PLAY:
state=State.PAUSE; sprint=false; log_event("pause","opened"); update_hud()

func resume_game():
if state==State.PAUSE:
state=State.PLAY; log_event("pause","resume"); update_hud()

func back_to_menu():
if state==State.PAUSE:
state=State.TITLE; sprint=false; move=Vector2.ZERO; look=Vector2.ZERO; log_event("main_menu","returned from pause"); update_hud()

func toggle_pause():
if state==State.PLAY: open_pause()
elif state==State.PAUSE: resume_game()

func show(t):
msg=t; msg_t=4

func show_msg(t):
show(t)

func is_ui_point(p):
for b in [start_b,exit_b,pause_b,sprint_b,resume_b,restart_b,menu_b,quit_b,end_restart_b]:
if b and b.visible and b.get_global_rect().has_point(p): return true
if pause_panel and pause_panel.visible and pause_panel.get_global_rect().has_point(p): return true
if menu_panel and menu_panel.visible and menu_panel.get_global_rect().has_point(p): return true
if end_panel and end_panel.visible and end_panel.get_global_rect().has_point(p): return true
return false

func setup_logging():
var d=DirAccess.open("user://")
if d: d.make_dir_recursive("BackroomsLevelZeroLogs")
var stamp=Time.get_datetime_string_from_system(false,true).replace(":","-").replace(" ","_")
log_path="user://BackroomsLevelZeroLogs/session_%s.log"%stamp
log_file=FileAccess.open(log_path,FileAccess.WRITE)
log_event("log_open","path=%s"%log_path)

func log_event(tag,text=""):
var line="[%s] %s %s"%[Time.get_datetime_string_from_system(false,true),tag,text]
print(line)
if log_file:
log_file.store_line(line); log_file.flush()

func is_open(c): return c.x>=0 and c.y>=0 and c.x<W and c.y<H and open.has(c)
func cell_to_world(c): return Vector3((c.x-W/2.0)*CELL,0,(c.y-H/2.0)*CELL)
func world_to_cell(p): return Vector2i(int(floor(p.x/CELL+W/2.0)),int(floor(p.z/CELL+H/2.0)))
func random_open():
	for i in range(30):
		var c=Vector2i(randi()%W,randi()%H)
		if is_open(c): return cell_to_world(c)
	return cell_to_world(Vector2i(8,8))

func next_path(a,b):
	var s=world_to_cell(a); var g=world_to_cell(b); var q=[s]; var came={s:s}; var dirs=[Vector2i(1,0),Vector2i(-1,0),Vector2i(0,1),Vector2i(0,-1)]
	while not q.is_empty():
		var c=q.pop_front()
		for d in dirs:
			var n=c+d
			if is_open(n) and not came.has(n): came[n]=c; q.append(n)
	if not came.has(g): return b
	var step=g
	while came[step]!=s and came[step]!=step: step=came[step]
	return cell_to_world(step)

func box(parent,pos,size,material,collidable:=true):
	var m=MeshInstance3D.new(); var b=BoxMesh.new(); b.size=size; m.mesh=b; m.material_override=material; parent.add_child(m); m.position=pos
	if collidable and not (parent is CharacterBody3D) and not (parent is Camera3D):
		var body:=StaticBody3D.new(); var shape:=CollisionShape3D.new(); var bs:=BoxShape3D.new()
		bs.size=size; shape.shape=bs; body.add_child(shape); parent.add_child(body); body.position=pos
	return m
