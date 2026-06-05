extends Node3D

enum State { LOADING, TITLE, PLAY, PAUSE, WIN, LOSE }

var state := State.LOADING
var player: CharacterBody3D
var cam: Camera3D
var ui: CanvasLayer
var shade: ColorRect
var loading_panel: Panel
var menu_panel: Panel
var pause_panel: Panel
var settings_panel: Panel
var end_panel: Panel
var hud: Label
var msg_label: Label
var end_label: Label
var bar: ProgressBar
var sanity_bar: ProgressBar
var start_b: Button
var settings_b: Button
var exit_b: Button
var pause_b: Button
var sprint_b: Button
var resume_b: Button
var restart_b: Button
var quit_b: Button
var settings_resume_b: Button
var settings_back_b: Button
var sens_slider: HSlider
var bright_slider: HSlider
var volume_slider: HSlider
var joy_ring: Panel
var joy_knob: Panel
var wall_m: StandardMaterial3D
var floor_m: StandardMaterial3D
var ceil_m: StandardMaterial3D
var light_m: StandardMaterial3D
var grime_m: StandardMaterial3D
var exit_m: StandardMaterial3D
var fuse_m: StandardMaterial3D
var threat_m: StandardMaterial3D
var exit_door: Node3D
var entity: CharacterBody3D
var save_timer := 0.0
var loading_time := 1.2
var move := Vector2.ZERO
var look := Vector2.ZERO
var joy_id := -1
var look_id := -1
var joy_base := Vector2.ZERO
var sprint := false
var stamina := 1.0
var sanity := 1.0
var pitch := 0.0
var sensitivity := 1.0
var brightness := 1.0
var volume := 0.8
var msg := "BACKROOMS LEVEL ZERO\nloading fluorescent maze..."
var msg_time := 3.0
var fuses_required := 3
var fuses_collected := 0
var exit_unlocked := false
var interaction_range := 1.9
var threat_pressure := 0.0
var threat_timer := 0.0
var ambience_timer := 5.0
var step_timer := 0.0
var log_file: FileAccess
var fuse_nodes: Array[Node3D] = []
var note_nodes: Array[Node3D] = []
var checkpoint_pos := Vector3(0, 0.35, 0)
var checkpoint_file := "user://checkpoint.cfg"

func _ready():
	setup_logs()
	log_event("ready", "app started")
	make_materials()
	make_level()
	make_player()
	load_checkpoint()
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

func mat(c: Color, rough := 0.8, texture_path := "") -> StandardMaterial3D:
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
	grime_m = mat(Color(0.09, 0.07, 0.04, 0.88), 0.99, "res://assets/textures/grime_stain_color.png")
	exit_m = mat(Color(0.18, 0.11, 0.045), 0.7)
	fuse_m = mat(Color(0.5, 0.9, 0.95), 0.35)
	fuse_m.emission_enabled = true
	fuse_m.emission = Color(0.20, 0.75, 1.0)
	fuse_m.emission_energy_multiplier = 0.8
	threat_m = mat(Color(0.015, 0.012, 0.010), 1.0)

func make_level():
	var we := WorldEnvironment.new()
	var env := Environment.new()
	env.background_mode = Environment.BG_COLOR
	env.background_color = Color(0.04, 0.035, 0.02)
	env.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	env.ambient_light_color = Color(0.55, 0.50, 0.32)
	env.ambient_light_energy = 0.48
	env.fog_enabled = true
	env.fog_density = 0.027
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
				make_light(p)

	for z in [-2, -1, 1, 2]:
		box(Vector3(-4, 1.2, z * 4), Vector3(0.15, 2.5, 3.4), wall_m, true)
		box(Vector3(4, 1.2, z * 4), Vector3(0.15, 2.5, 3.4), wall_m, true)
	for x in [-2, 0, 2]:
		box(Vector3(x * 4, 1.2, -4), Vector3(3.4, 2.5, 0.15), wall_m, true)
		box(Vector3(x * 4, 1.2, 4), Vector3(3.4, 2.5, 0.15), wall_m, true)

	# Extra Level 0 variety: pillars, maintenance room, grime, dead ends.
	for pos in [Vector3(-8, 1.2, 8), Vector3(8, 1.2, -8), Vector3(0, 1.2, 8)]:
		box(pos, Vector3(0.65, 2.5, 0.65), wall_m, true)
	for pos in [Vector3(-10, 0.55, -2), Vector3(6, 0.56, 10), Vector3(12, 0.57, -8), Vector3(-6, 0.56, 12)]:
		box(pos, Vector3(1.2, 0.03, 1.0), grime_m, false)
	box(Vector3(12, 1.2, 6), Vector3(0.12, 2.5, 3.5), wall_m, true)
	box(Vector3(10.8, 1.2, 7.7), Vector3(2.5, 2.5, 0.12), wall_m, true)

	make_fuse(Vector3(-9, 0.55, -9))
	make_fuse(Vector3(10, 0.55, -6))
	make_fuse(Vector3(-6, 0.55, 10))
	make_note(Vector3(2, 0.7, -11.5), "A damp note reads: THREE BAUE FUSES OPEN THE SERVICE EXIT.")
	make_note(Vector3(-11, 0.7, 2), "The wallpaper has arrows scratched under the grime.")
	make_exit(Vector3(12.0, 1.15, 12.0))
	make_entity()

func make_light(p: Vector3):
	box(p + Vector3(0, 2.45, 0), Vector3(1.6, 0.06, 0.35), light_m, false)
	var l := OmniLight3D.new()
	l.light_color = Color(1, 0.88 + randf() * 0.08, 0.48)
	l.light_energy = 0.55 + randf() * 0.32
	l.omni_range = 6.0 + randf() * 2.0
	add_child(l)
	l.global_position = p + Vector3(0, 2.2, 0)

func make_fuse(pos: Vector3):
	var fuse := box(pos, Vector3(0.35, 0.35, 0.35), fuse_m, false)
	fuse.name = "Fuse"
	fuse_nodes.append(fuse)
	var l := OmniLight3D.new()
	l.light_color = Color(0.2, 0.8, 1.0)
	l.light_energy = 0.35
	l.omni_range = 2.5
	fuse.add_child(l)

func make_note(pos: Vector3, text: String):
	var note := box(pos, Vector3(0.65, 0.05, 0.45), mat(Color(0.72, 0.65, 0.44), 0.85), false)
	note.name = text
	note_nodes.append(note)

func make_exit(pos: Vector3):
	exit_door = box(pos, Vector3(1.8, 2.2, 0.22), exit_m, true)
	exit_door.name = "ExitDoor"
	var l := OmniLight3D.new()
	l.light_color = Color(1.0, 0.72, 0.32)
	l.light_energy = 0.25
	l.omni_range = 4.0
	exit_door.add_child(l)

func make_entity():
	entity = CharacterBody3D.new()
	entity.name = "PressureEntity"
	add_child(entity)
	entity.global_position = Vector3(-11, 0.35, 11)
	var cs := CollisionShape3D.new()
	var cap := CapsuleShape3D.new()
	cap.height = 1.7
	cap.radius = 0.26
	cs.shape = cap
	cs.position = Vector3(0, 0.85, 0)
	entity.add_child(cs)
	box_child(entity, Vector3(0, 0.9, 0), Vector3(0.45, 1.8, 0.45), threat_m)

func make_player():
	player = CharacterBody3D.new()
	add_child(player)
	player.global_position = checkpoint_pos
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

	menu_panel = panel(Vector2(380, 92), Vector2(540, 550))
	label(menu_panel, "BACKROOMS\nLEVEL ZERO", Vector2(45, 35), 46)
	label(menu_panel, "Find three fuses, restore the exit,\nand avoid what follows you.", Vector2(42, 162), 20)
	start_b = button(menu_panel, "ENTER LEVEL 0", Vector2(110, 258), Vector2(310, 62))
	start_b.pressed.connect(start_game)
	settings_b = button(menu_panel, "SETTINGS", Vector2(143, 334), Vector2(242, 54))
	settings_b.pressed.connect(open_settings)
	exit_b = button(menu_panel, "EXIT", Vector2(168, 402), Vector2(190, 54))
	exit_b.pressed.connect(quit_game)

	pause_panel = panel(Vector2(405, 122), Vector2(470, 435))
	label(pause_panel, "PAUSED", Vector2(135, 35), 46)
	resume_b = button(pause_panel, "RESUME", Vector2(95, 128), Vector2(280, 58))
	resume_b.pressed.connect(resume_game)
	restart_b = button(pause_panel, "RESTART", Vector2(95, 200), Vector2(280, 58))
	restart_b.pressed.connect(restart_game)
	settings_resume_b = button(pause_panel, "SETTINGS", Vector2(95, 272), Vector2(280, 58))
	settings_resume_b.pressed.connect(open_settings)
	quit_b = button(pause_panel, "QUIT", Vector2(95, 344), Vector2(280, 54))
	quit_b.pressed.connect(quit_game)

	settings_panel = panel(Vector2(345, 110), Vector2(590, 470))
	label(settings_panel, "SETTINGS", Vector2(170, 32), 42)
	label(settings_panel, "Look sensitivity", Vector2(58, 115), 22)
	sens_slider = slider(settings_panel, Vector2(260, 112), 0.5, 1.8, sensitivity)
	label(settings_panel, "Brightness", Vector2(58, 180), 22)
	bright_slider = slider(settings_panel, Vector2(260, 177), 0.6, 1.4, brightness)
	label(settings_panel, "Volume", Vector2(58, 245), 22)
	volume_slider = slider(settings_panel, Vector2(260, 242), 0.0, 1.0, volume)
	settings_back_b = button(settings_panel, "BACK", Vector2(170, 350), Vector2(250, 58))
	settings_back_b.pressed.connect(close_settings)

	end_panel = panel(Vector2(350, 160), Vector2(580, 360))
	end_label = label(end_panel, "", Vector2(46, 44), 32)
	var end_restart := button(end_panel, "PLAY AGAIN", Vector2(160, 220), Vector2(260, 58))
	end_restart.pressed.connect(restart_game)

	hud = Label.new()
	hud.position = Vector2(28, 22)
	hud.add_theme_font_size_override("font_size", 24)
	hud.add_theme_color_override("font_color", Color(0.95, 0.86, 0.45))
	ui.add_child(hud)
	msg_label = Label.new()
	msg_label.position = Vector2(28, 76)
	msg_label.add_theme_font_size_override("font_size", 21)
	msg_label.add_theme_color_override("font_color", Color(0.90, 0.82, 0.55))
	ui.add_child(msg_label)
	bar = ProgressBar.new()
	bar.position = Vector2(28, 670)
	bar.size = Vector2(280, 18)
	bar.max_value = 100
	bar.show_percentage = false
	ui.add_child(bar)
	sanity_bar = ProgressBar.new()
	sanity_bar.position = Vector2(28, 696)
	sanity_bar.size = Vector2(280, 18)
	sanity_bar.max_value = 100
	sanity_bar.show_percentage = false
	ui.add_child(sanity_bar)
	pause_b = button(ui, "II", Vector2(1180, 24), Vector2(70, 62))
	pause_b.pressed.connect(open_pause)
	sprint_b = button(ui, "SPRINT", Vector2(1050, 575), Vector2(170, 78))
	sprint_b.button_down.connect(func(): sprint = true)
	sprint_b.button_up.connect(func(): sprint = false)
	joy_ring = panel(Vector2(72, 510), Vector2(165, 165), Color(0.09, 0.08, 0.035, 0.42))
	joy_knob = panel(Vector2(125, 563), Vector2(58, 58), Color(0.70, 0.62, 0.25, 0.50))

func slider(parent: Control, pos: Vector2, min_v: float, max_v: float, val: float) -> HSlider:
	var s := HSlider.new()
	s.position = pos
	s.size = Vector2(250, 42)
	s.min_value = min_v
	s.max_value = max_v
	s.step = 0.05
	s.value = val
	s.focus_mode = Control.FOCUS_NONE
	s.value_changed.connect(apply_settings)
	parent.add_child(s)
	return s

func panel(pos: Vector2, size: Vector2, c := Color(0.045, 0.039, 0.018, 0.96)) -> Panel:
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

func label(parent: Control, t: String, pos: Vector2, fs: int) -> Label:
	var l := Label.new()
	l.text = t
	l.position = pos
	l.add_theme_font_size_override("font_size", fs)
	l.add_theme_color_override("font_color", Color(0.95, 0.84, 0.40))
	parent.add_child(l)
	return l

func button(parent: Node, t: String, pos: Vector2, size: Vector2) -> Button:
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
		update_objectives(delta)
		update_entity(delta)
		update_ambience(delta)
		save_timer += delta
		if save_timer > 8.0:
			save_checkpoint()
			save_timer = 0.0
		if player.global_position.y < -4:
			player.global_position = checkpoint_pos
			player.velocity = Vector3.ZERO
			show_msg("The carpet drops away. Level 0 puts you back.")
	if msg_time > 0:
		msg_time -= delta
	update_ui()

func move_player(delta: float):
	player.rotate_y(-look.x * 0.0038 * sensitivity)
	pitch = clamp(pitch + look.y * -0.0033 * sensitivity, deg_to_rad(-80), deg_to_rad(80))
	cam.rotation.x = pitch
	look = Vector2.ZERO
	var wish := player.global_transform.basis * Vector3(move.x, 0, -move.y)
	wish.y = 0
	if wish.length() > 1:
		wish = wish.normalized()
	var running := sprint and wish.length() > 0.05 and stamina > 0.0
	if running:
		stamina = max(0.0, stamina - delta * 0.13)
		step_timer -= delta * 1.65
		if stamina <= 0.0:
			sprint = false
	elif wish.length() < 0.05 or not sprint:
		stamina = min(1.0, stamina + delta * 0.22)
	var sp := 5.15 if running else 3.2
	player.velocity.x = wish.x * sp
	player.velocity.z = wish.z * sp
	player.velocity.y = -0.1 if player.is_on_floor() else player.velocity.y - 18.0 * delta
	player.move_and_slide()
	if wish.length() > 0.05:
		step_timer -= delta
		if step_timer <= 0.0:
			step_timer = 0.42 if running else 0.62
			show_msg("soft carpet steps", 0.35)

func update_objectives(delta: float):
	for fuse in fuse_nodes.duplicate():
		if player.global_position.distance_to(fuse.global_position) <= interaction_range:
			fuses_collected += 1
			fuse_nodes.erase(fuse)
			fuse.queue_free()
			show_msg("Fuse collected: %d/%d" % [fuses_collected, fuses_required])
			log_event("fuse", str(fuses_collected))
	if not exit_unlocked and fuses_collected >= fuses_required:
		exit_unlocked = true
		exit_m.albedo_color = Color(0.8, 0.55, 0.22)
		show_msg("The service exit buzzes open somewhere in the maze.")
		log_event("exit", "unlocked")
	for note in note_nodes:
		if is_instance_valid(note) and player.global_position.distance_to(note.global_position) <= interaction_range:
			show_msg(note.name, 2.5)
	if exit_door and player.global_position.distance_to(exit_door.global_position) < 2.0:
		if exit_unlocked:
			win_game()
		else:
			show_msg("The exit door has no power. Find the fuses.", 1.6)

func update_entity(delta: float):
	if not entity:
		return
	threat_timer += delta
	var dist := entity.global_position.distance_to(player.global_position)
	if threat_timer > 6.0 and dist > 11.0:
		threat_timer = 0.0
		var behind := -player.global_transform.basis.z * -8.0
		entity.global_position = player.global_position + behind + Vector3(randf_range(-4, 4), 0, randf_range(-4, 4))
	if dist > 1.3:
		var dir := (player.global_position - entity.global_position)
		dir.y = 0
		if dir.length() > 0:
			entity.velocity.x = dir.normalized().x * 1.45
			entity.velocity.z = dir.normalized().z * 1.45
	else:
		entity.velocity = Vector3.ZERO
	entity.velocity.y = -0.1 if entity.is_on_floor() else entity.velocity.y - 18.0 * delta
	entity.move_and_slide()
	threat_pressure = clamp(1.0 - (dist / 10.0), 0.0, 1.0)
	if threat_pressure > 0.35:
		sanity = max(0.0, sanity - delta * threat_pressure * 0.10)
	else:
		sanity = min(1.0, sanity + delta * 0.025)
	if threat_pressure > 0.55:
		show_msg("The hum bends behind you.", 0.75)
	if sanity <= 0.0:
		lose_game("You lost your bearings in the endless yellow rooms.")

func update_ambience(delta: float):
	ambience_timer -= delta
	if ambience_timer <= 0.0:
		ambience_timer = randf_range(6.0, 13.0)
		var choices := [
			"fluorescent hum rises and fades",
			"a distant thud echoes through the walls",
			"somewhere, wet carpet squelches",
			"the lights flicker out of rhythm"
		]
		show_msg(choices.pick_random(), 1.2)

func _input(e):
	if e is InputEventKey and e.pressed and e.keycode == KEY_ESCAPE:
		if state == State.PLAY:
			open_pause()
		elif state == State.PAUSE:
			resume_game()
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
	for b in [start_b, settings_b, exit_b, pause_b, sprint_b, resume_b, restart_b, quit_b, settings_resume_b, settings_back_b]:
		if b and b.visible and b.get_global_rect().has_point(p):
			return true
	for s in [sens_slider, bright_slider, volume_slider]:
		if s and s.visible and s.get_global_rect().has_point(p):
			return true
	return false

func update_ui():
	apply_settings(0.0)
	hud.text = "Objective: collect fuses %d/%d  |  Exit: %s" % [fuses_collected, fuses_required, "OPEN" if exit_unlocked else "LOCKED"]
	msg_label.text = msg if msg_time > 0 else ""
	bar.value = stamina * 100.0
	sanity_bar.value = sanity * 100.0
	var title := state == State.TITLE
	var play := state == State.PLAY
	var pause := state == State.PAUSE
	var loading := state == State.LOADING
	var settings := settings_panel and settings_panel.visible
	var ended := state == State.WIN or state == State.LOSE
	shade.visible = loading or title or pause or settings or ended or threat_pressure > 0.55
	if threat_pressure > 0.55 and play:
		shade.color = Color(0.01, 0.0, 0.0, 0.30 + threat_pressure * 0.28)
	else:
		shade.color = Color(0.01, 0.009, 0.004, 0.55)
	loading_panel.visible = loading
	menu_panel.visible = title
	pause_panel.visible = pause and not settings
	end_panel.visible = ended
	hud.visible = play or pause
	msg_label.visible = play or pause
	bar.visible = play
	sanity_bar.visible = play
	pause_b.visible = play or pause
	sprint_b.visible = play
	joy_ring.visible = play
	joy_knob.visible = play

func apply_settings(_value: float):
	if sens_slider:
		sensitivity = float(sens_slider.value)
	if bright_slider:
		brightness = float(bright_slider.value)
	if volume_slider:
		volume = float(volume_slider.value)
	AudioServer.set_bus_volume_db(0, linear_to_db(max(volume, 0.001)))
	if cam:
		cam.attributes = CameraAttributesPractical.new()
		cam.attributes.exposure_multiplier = brightness

func start_game():
	state = State.PLAY
	show_msg("Objective: collect three fuses and reach the service exit.")
	log_event("start", "gameplay")
	save_checkpoint()
	update_ui()

func open_pause():
	if state == State.PLAY:
		state = State.PAUSE
		sprint = false
		save_checkpoint()
		log_event("pause", "open")
		update_ui()

func resume_game():
	if state == State.PAUSE:
		state = State.PLAY
		log_event("pause", "resume")
		update_ui()

func open_settings():
	settings_panel.visible = true
	update_ui()

func close_settings():
	settings_panel.visible = false
	update_ui()

func restart_game():
	log_event("restart", "requested")
	if FileAccess.file_exists(checkpoint_file):
		DirAccess.remove_absolute(ProjectSettings.globalize_path(checkpoint_file))
	get_tree().reload_current_scene()

func quit_game():
	log_event("quit", "requested")
	get_tree().quit()

func win_game():
	state = State.WIN
	end_label.text = "YOU FOUND THE EXIT\n\nThe buzzing fades behind the door.\nLevel 0 lets you leave."
	save_checkpoint()
	log_event("win", "exit reached")
	update_ui()

func lose_game(reason: String):
	state = State.LOSE
	end_label.text = "LOST IN LEVEL 0\n\n" + reason
	log_event("lose", reason)
	update_ui()

func show_msg(t: String, time := 4.0):
	msg = t
	msg_time = time

func save_checkpoint():
	var cfg := ConfigFile.new()
	cfg.set_value("player", "x", player.global_position.x)
	cfg.set_value("player", "y", player.global_position.y)
	cfg.set_value("player", "z", player.global_position.z)
	cfg.set_value("progress", "fuses", fuses_collected)
	cfg.set_value("progress", "exit_unlocked", exit_unlocked)
	cfg.set_value("progress", "sanity", sanity)
	cfg.save(checkpoint_file)

func load_checkpoint():
	if not FileAccess.file_exists(checkpoint_file):
		return
	var cfg := ConfigFile.new()
	if cfg.load(checkpoint_file) != OK:
		return
	checkpoint_pos = Vector3(float(cfg.get_value("player", "x", 0.0)), float(cfg.get_value("player", "y", 0.35)), float(cfg.get_value("player", "z", 0.0)))
	fuses_collected = int(cfg.get_value("progress", "fuses", 0))
	exit_unlocked = bool(cfg.get_value("progress", "exit_unlocked", false))
	sanity = float(cfg.get_value("progress", "sanity", 1.0))
	if player:
		player.global_position = checkpoint_pos
	log_event("checkpoint", "loaded")

func box(pos: Vector3, size: Vector3, material: Material, collidable := true) -> MeshInstance3D:
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

func box_child(parent: Node3D, pos: Vector3, size: Vector3, material: Material):
	var m := MeshInstance3D.new()
	var bm := BoxMesh.new()
	bm.size = size
	m.mesh = bm
	m.material_override = material
	parent.add_child(m)
	m.position = pos
	return m
