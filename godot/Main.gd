extends Node3D

enum State { LOADING, TITLE, PLAY, PAUSE }

var state := State.LOADING
var player: CharacterBody3D
var cam: Camera3D
var move := Vector2.ZERO
var look := Vector2.ZERO
var joy_id := -1
var look_id := -1
var joy_base := Vector2.ZERO
var sprint := false
var stamina := 1.0
var pitch := 0.0
var loading_time := 1.2
var log_file: FileAccess
var msg := "BACKROOMS LEVEL ZERO\nloading fluorescent maze..."
var msg_time := 3.0
var ui: CanvasLayer
var shade: ColorRect
var loading_panel: Panel
var menu_panel: Panel
var pause_panel: Panel
var hud: Label
var msg_label: Label
var bar: ProgressBar
var start_b: Button
var exit_b: Button
var pause_b: Button
var sprint_b: Button
var resume_b: Button
var restart_b: Button
var quit_b: Button
var joy_ring: Panel
var joy_knob: Panel
var wall_m: StandardMaterial3D
var floor_m: StandardMaterial3D
var ceil_m: StandardMaterial3D
var light_m: StandardMaterial3D

func _ready():
	setup_logs()
	log_event("ready", "app started")
	make_materials()
	make_level()
	make_player()
	make_ui()
	update_ui()

func setup_logs():
	var stamp := Time.get_datetime_string_from_system(false, true).replace(":", "-").replace(" ", "_")
	var media := "/storage/emulated/0/Android/media/com.backrooms.levelzero/BackroomsLevelZeroLogs/" + stamp
	var ok := false
	if OS.get_name() == "Android":
		ok = DirAccess.make_dir_recursive_absolute(media) == OK
	if ok:
		log_file = FileAccess.open(media + "/session.log", FileAccess.WRITE)
	else:
		var d := DirAccess.open("user://")
		if d:
			d.make_dir_recursive("BackroomsLevelZeroLogs/" + stamp)
		log_file = FileAccess.open("user://BackroomsLevelZeroLogs/" + stamp + "/session.log", FileAccess.WRITE)
	log_event("log_open", "logging enabled")

func log_event(tag, text := ""):
	var line := "[%s] %s %s" % [Time.get_datetime_string_from_system(false, true), tag, text]
	print(line)
	if log_file:
		log_file.store_line(line)
		log_file.flush()

func mat(c, rough := 0.8, texture_path := ""):
	var m := StandardMaterial3D.new()
	m.albedo_color = c
	m.roughness = rough
	if texture_path != "" and ResourceLoader.exists(texture_path):
		m.albedo_texture = load(texture_path)
	return m

func make_materials():
	wall_m = mat(Color(0.72, 0.63, 0.25), 0.95, "res://assets/textures/wallpaper_yellow_color.png")
	floor_m = mat(Color(0.30, 0.24, 0.12), 0.98, "res://assets/textures/carpet_dirty_color.png")
	ceil_m = mat(Color(0.62, 0.60, 0.46), 0.90, "res://assets/textures/ceiling_tiles_color.png")
	light_m = mat(Color(1.0, 0.90, 0.55), 0.2, "res://assets/textures/plastic_panel_color.png")
	light_m.emission_enabled = true
	light_m.emission = Color(1.0, 0.88, 0.45)
	light_m.emission_energy_multiplier = 1.25
func make_level():
	var we := WorldEnvironment.new()
	var env := Environment.new()
	env.background_mode = Environment.BG_COLOR
	env.background_color = Color(0.04, 0.035, 0.02)
	env.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	env.ambient_light_color = Color(0.55, 0.50, 0.32)
	env.ambient_light_energy = 0.55
	env.fog_enabled = true
	env.fog_density = 0.025
	we.environment = env
	add_child(we)
	for x in range(-3, 4):
		for z in range(-3, 4):
			var p := Vector3(x * 4.0, 0, z * 4.0)
			box(p + Vector3(0, -0.05, 0), Vector3(4, 0.1, 4), floor_m, true)
			box(p + Vector3(0, 2.55, 0), Vector3(4, 0.1, 4), ceil_m, false)
			if abs(x) == 3:
				box(p + Vector3(sign(x) * 2.0, 1.2, 0), Vector3(0.15, 2.5, 4), wall_m, true)
			if abs(z) == 3:
				box(p + Vector3(0, 1.2, sign(z) * 2.0), Vector3(4, 2.5, 0.15), wall_m, true)
			if (x + z) % 2 == 0:
				box(p + Vector3(0, 2.45, 0), Vector3(1.6, 0.06, 0.35), light_m, false)
				var l := OmniLight3D.new()
				l.light_color = Color(1, 0.88, 0.48)
				l.light_energy = 0.75
				l.omni_range = 7
				add_child(l)
				l.global_position = p + Vector3(0, 2.2, 0)
	# internal confusing walls with gaps
	for z in [-2, -1, 1, 2]:
		box(Vector3(-4, 1.2, z * 4), Vector3(0.15, 2.5, 3.4), wall_m, true)
		box(Vector3(4, 1.2, z * 4), Vector3(0.15, 2.5, 3.4), wall_m, true)
	for x in [-2, 0, 2]:
		box(Vector3(x * 4, 1.2, -4), Vector3(3.4, 2.5, 0.15), wall_m, true)
		box(Vector3(x * 4, 1.2, 4), Vector3(3.4, 2.5, 0.15), wall_m, true)

func make_player():
	player = CharacterBody3D.new()
	add_child(player)
	player.global_position = Vector3(0, 0.35, 0)
	var cs := CollisionShape3D.new()
	var cap := CapsuleShape3D.new()
	cap.height = 1.5
	cap.radius = 0.32
	cs.shape = cap
	cs.position = Vector3(0, 0.85, 0)
	player.add_child(cs)
	cam = Camera3D.new()
	cam.position = Vector3(0, 1.58, 0)
	cam.fov = 72
	player.add_child(cam)
	log_event("player_spawn", str(player.global_position))

func make_ui():
	ui = CanvasLayer.new()
	add_child(ui)
	shade = ColorRect.new()
	shade.color = Color(0.01, 0.009, 0.004, 0.55)
	shade.set_anchors_preset(Control.PRESET_FULL_RECT)
	ui.add_child(shade)
	loading_panel = panel(Vector2(260, 190), Vector2(760, 315))
	label(loading_panel, "BACKROOMS\nLEVEL ZERO", Vector2(55, 45), 52)
	label(loading_panel, "loading fluorescent maze...", Vector2(130, 215), 28)
	menu_panel = panel(Vector2(380, 105), Vector2(520, 485))
	label(menu_panel, "BACKROOMS\nLEVEL ZERO", Vector2(45, 35), 46)
	label(menu_panel, "Find the exit. Stay quiet.\nUse the left side to move and right side to look.", Vector2(42, 172), 20)
	start_b = button(menu_panel, "ENTER LEVEL 0", Vector2(112, 286), Vector2(296, 66))
	start_b.pressed.connect(start_game)
	exit_b = button(menu_panel, "EXIT", Vector2(160, 372), Vector2(200, 54))
	exit_b.pressed.connect(quit_game)
	pause_panel = panel(Vector2(405, 122), Vector2(470, 420))
	label(pause_panel, "PAUSED", Vector2(135, 35), 46)
	resume_b = button(pause_panel, "RESUME", Vector2(95, 132), Vector2(280, 62))
	resume_b.pressed.connect(resume_game)
	restart_b = button(pause_panel, "RESTART", Vector2(95, 212), Vector2(280, 62))
	restart_b.pressed.connect(restart_game)
	quit_b = button(pause_panel, "QUIT", Vector2(95, 292), Vector2(280, 54))
	quit_b.pressed.connect(quit_game)
	hud = Label.new()
	hud.position = Vector2(28, 22)
	hud.add_theme_font_size_override("font_size", 26)
	hud.add_theme_color_override("font_color", Color(0.95, 0.86, 0.45))
	ui.add_child(hud)
	msg_label = Label.new()
	msg_label.position = Vector2(28, 62)
	msg_label.add_theme_font_size_override("font_size", 22)
	msg_label.add_theme_color_override("font_color", Color(0.90, 0.82, 0.55))
	ui.add_child(msg_label)
	bar = ProgressBar.new()
	bar.position = Vector2(28, 670)
	bar.size = Vector2(280, 18)
	bar.max_value = 100
	bar.show_percentage = false
	ui.add_child(bar)
	pause_b = button(ui, "II", Vector2(1180, 24), Vector2(70, 62))
	pause_b.pressed.connect(open_pause)
	sprint_b = button(ui, "SPRINT", Vector2(1050, 575), Vector2(170, 78))
	sprint_b.button_down.connect(func(): sprint = true)
	sprint_b.button_up.connect(func(): sprint = false)
	joy_ring = panel(Vector2(72, 510), Vector2(165, 165), Color(0.09, 0.08, 0.035, 0.42))
	joy_knob = panel(Vector2(125, 563), Vector2(58, 58), Color(0.70, 0.62, 0.25, 0.50))

func panel(pos, size, c := Color(0.045, 0.039, 0.018, 0.96)):
	var p := Panel.new()
	p.position = pos
	p.size = size
	var st := StyleBoxFlat.new()
	st.bg_color = c
	st.border_color = Color(0.68, 0.57, 0.23, 0.92)
	st.set_border_width_all(2)
	st.corner_radius_top_left = 10
	st.corner_radius_top_right = 10
	st.corner_radius_bottom_left = 10
	st.corner_radius_bottom_right = 10
	p.add_theme_stylebox_override("panel", st)
	ui.add_child(p)
	return p

func label(parent, t, pos, fs):
	var l := Label.new()
	l.text = t
	l.position = pos
	l.add_theme_font_size_override("font_size", fs)
	l.add_theme_color_override("font_color", Color(0.95, 0.84, 0.40))
	parent.add_child(l)
	return l

func button(parent, t, pos, size):
	var b := Button.new()
	b.text = t
	b.position = pos
	b.size = size
	b.focus_mode = Control.FOCUS_NONE
	b.add_theme_font_size_override("font_size", 22)
	b.add_theme_color_override("font_color", Color(0.96, 0.86, 0.48))
	parent.add_child(b)
	return b

func _physics_process(delta):
	if state == State.LOADING:
		loading_time -= delta
		if loading_time <= 0:
			state = State.TITLE
		update_ui()
		return
	if state == State.PLAY:
		move_player(delta)
		if player.global_position.y < -4:
			player.global_position = Vector3(0, 0.35, 0)
			player.velocity = Vector3.ZERO
			show_msg("The carpet drops away. Level 0 puts you back.")
	if msg_time > 0:
		msg_time -= delta
	update_ui()

func move_player(delta):
	player.rotate_y(-look.x * 0.0038)
	pitch = clamp(pitch + look.y * -0.0033, deg_to_rad(-80), deg_to_rad(80))
	cam.rotation.x = pitch
	look = Vector2.ZERO
	var wish := player.global_transform.basis * Vector3(move.x, 0, -move.y)
	wish.y = 0
	if wish.length() > 1:
		wish = wish.normalized()
	var running := sprint and wish.length() > 0.05 and stamina > 0.0
	if running:
		stamina = max(0.0, stamina - delta * 0.11)
		if stamina <= 0.0:
			sprint = false
	elif wish.length() < 0.05 and not sprint:
		stamina = min(1.0, stamina + delta * 0.20)
	var sp := 5.15 if running else 3.2
	player.velocity.x = wish.x * sp
	player.velocity.z = wish.z * sp
	player.velocity.y = -0.1 if player.is_on_floor() else player.velocity.y - 18.0 * delta
	player.move_and_slide()

func _input(e):
	if e is InputEventMouseMotion and OS.get_name() != "Android" and state == State.PLAY and Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
		look += e.relative
	if e is InputEventScreenTouch:
		var p := e.position
		if e.pressed:
			if is_ui_point(p) or state != State.PLAY:
				return
			var screen := get_viewport().get_visible_rect().size
			if p.x < screen.x * 0.45 and joy_id == -1:
				joy_id = e.index
				joy_base = p
				joy_ring.position = joy_base - joy_ring.size * 0.5
				joy_knob.position = joy_base - joy_knob.size * 0.5
			elif p.x >= screen.x * 0.50 and look_id == -1:
				look_id = e.index
		else:
			if e.index == joy_id:
				joy_id = -1
				move = Vector2.ZERO
				joy_ring.position = Vector2(72, 510)
				joy_knob.position = Vector2(125, 563)
			if e.index == look_id:
				look_id = -1
	if e is InputEventScreenDrag and state == State.PLAY:
		if e.index == joy_id:
			var r: float = min(get_viewport().get_visible_rect().size.y * 0.16, 150.0)
			var d := e.position - joy_base
			if d.length() > r:
				d = d.normalized() * r
			move = Vector2(d.x / r, -d.y / r)
			joy_knob.position = joy_base + d - joy_knob.size * 0.5
		elif e.index == look_id:
			look += e.relative

func is_ui_point(p: Vector2) -> bool:
	for b in [start_b, exit_b, pause_b, sprint_b, resume_b, restart_b, quit_b]:
		if b and b.visible and b.get_global_rect().has_point(p):
			return true
	return false

func update_ui():
	hud.text = "Objective: explore Level 0"
	msg_label.text = msg if msg_time > 0 else ""
	bar.value = stamina * 100.0
	var title := state == State.TITLE
	var play := state == State.PLAY
	var pause := state == State.PAUSE
	var loading := state == State.LOADING
	shade.visible = loading or title or pause
	loading_panel.visible = loading
	menu_panel.visible = title
	pause_panel.visible = pause
	hud.visible = play or pause
	msg_label.visible = play or pause
	bar.visible = play
	pause_b.visible = play or pause
	sprint_b.visible = play
	joy_ring.visible = play
	joy_knob.visible = play

func start_game():
	state = State.PLAY
	show_msg("Objective: explore the yellow maze.")
	log_event("start", "gameplay")

func open_pause():
	if state == State.PLAY:
		state = State.PAUSE
		sprint = false
		log_event("pause", "open")

func resume_game():
	if state == State.PAUSE:
		state = State.PLAY
		log_event("pause", "resume")

func restart_game():
	log_event("restart", "requested")
	get_tree().reload_current_scene()

func quit_game():
	log_event("quit", "requested")
	get_tree().quit()

func show_msg(t):
	msg = t
	msg_time = 4.0

func box(pos, size, material, collidable := true):
	var m := MeshInstance3D.new()
	var bm := BoxMesh.new()
	bm.size = size
	m.mesh = bm
	m.material_override = material
	add_child(m)
	m.position = pos
	if collidable:
		var body := StaticBody3D.new()
		var shape := CollisionShape3D.new()
		var bs := BoxShape3D.new()
		bs.size = size
		shape.shape = bs
		body.add_child(shape)
		add_child(body)
		body.position = pos
	return m
