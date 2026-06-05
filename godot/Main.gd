extends Node3D

const GRAVITY := 18.0
const WALK_SPEED := 3.4
const SPRINT_SPEED := 5.2
const INTERACT_RANGE := 2.1

var player: CharacterBody3D
var camera: Camera3D
var ui: CanvasLayer
var hud: Label
var message_label: Label
var use_button: Button
var pause_button: Button

var wall_mat: StandardMaterial3D
var carpet_mat: StandardMaterial3D
var ceiling_mat: StandardMaterial3D
var light_mat: StandardMaterial3D
var leak_mat: StandardMaterial3D
var metal_mat: StandardMaterial3D
var raw_metal_mat: StandardMaterial3D
var plastic_mat: StandardMaterial3D
var wood_mat: StandardMaterial3D
var fuse_mat: StandardMaterial3D
var exit_mat: StandardMaterial3D
var dark_mat: StandardMaterial3D

var fuse_nodes: Array[Node3D] = []
var note_nodes: Array[Node3D] = []
var hide_nodes: Array[Node3D] = []
var exit_door: Node3D

var touch_move := Vector2.ZERO
var look_delta := Vector2.ZERO
var pitch := 0.0
var sprinting := false
var stamina := 1.0
var sanity := 1.0
var fuses_collected := 0
var fuses_required := 3
var exit_unlocked := false
var hiding_until := 0.0
var message_time := 0.0
var game_over := false

func _ready() -> void:
	Engine.max_fps = 60
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	_make_materials()
	_make_world()
	_make_player()
	_make_ui()
	show_msg("Level 0 loaded with uploaded carpet, ceiling, metal, plastic, wood, and leak textures.", 3.5)

func _make_materials() -> void:
	wall_mat = make_mat(Color(0.72, 0.63, 0.25), "res://assets/textures/wallpaper_yellow_color.png", "", "", 0.0, 0.96, Vector3(8, 4, 1))
	carpet_mat = make_mat(Color(0.35, 0.27, 0.13), "res://assets/textures/uploaded/carpet_fabric_color.jpg", "res://assets/textures/uploaded/carpet_fabric_normal_gl.jpg", "res://assets/textures/uploaded/carpet_fabric_roughness.jpg", 0.0, 0.98, Vector3(10, 10, 1))
	ceiling_mat = make_mat(Color(0.62, 0.60, 0.46), "res://assets/textures/uploaded/office_ceiling_color.jpg", "res://assets/textures/uploaded/office_ceiling_normal_gl.jpg", "res://assets/textures/uploaded/office_ceiling_roughness.jpg", 0.0, 0.9, Vector3(8, 8, 1)
	light_mat = make_mat(Color(1.0, 0.90, 0.55), "res://assets/textures/uploaded/plastic_panel_color.jpg", "res://assets/textures/uploaded/plastic_panel_normal_gl.jpg", "res://assets/textures/uploaded/plastic_panel_roughness.jpg", 0.0, 0.35, Vector3(2, 1, 1))
	light_mat.emission_enabled = true
	light_mat.emission = Color(1.0, 0.86, 0.45)
	light_mat.emission_energy_multiplier = 1.3
	leak_mat = make_mat(Color(0.28, 0.22, 0.12, 0.88), "res://assets/textures/uploaded/wall_leak_color.jpg", "res://assets/textures/uploaded/wall_leak_normal_gl.jpg", "res://assets/textures/uploaded/wall_leak_roughness.jpg", 0.0, 0.95, Vector3(1, 1, 1))
leak_mat.transparency = BaseMaterial3D.TRANSPARENCY_ALPHA
	leak_mat.cull_mode = BaseMaterial3D.CULL_DISABLED
	metal_mat = make_mat(Color(0.25, 0.23, 0.20), "res://assets/textures/uploaded/painted_metal_color.jpg", "res://assets/textures/uploaded/painted_metal_normal_gl.jpg", "res://assets/textures/uploaded/painted_metal_roughness.jpg", 0.2, 0.76, Vector3(2, 2, 1))
	raw_metal_mat = make_mat(Color(0.18, 0.18, 0.17), "res://assets/textures/uploaded/raw_metal_color.jpg", "res://assets/textures/uploaded/raw_metal_normal_gl.jpg", "res://assets/textures/uploaded/raw_metal_roughness.jpg", 0.7, 0.68, Vector3(3, 1, 1))
	plastic_mat = make_mat(Color(0.75, 0.70, 0.50), "res://assets/textures/uploaded/plastic_panel_color.jpg", "res://assets/textures/uploaded/plastic_panel_normal_gl.jpg", "res://assets/textures/uploaded/plastic_panel_roughness.jpg", 0.0, 0.45, Vector3(2, 2, 1))
	wood_mat = make_mat(Color(0.32, 0.22, 0.12), "res://assets/textures/uploaded/wood_trim_color.jpg", "res://assets/textures/uploaded/wood_trim_normal_gl.jpg", "res://assets/textures/uploaded/wood_trim_roughness.jpg", 0.0, 0.72, Vector3(5, 1, 1))
	fuse_mat = make_mat(Color(0.30, 0.90, 1.0), "", "", "", 0.0, 0.3)
	fuse_mat.emission_enabled = true
	fuse_mat.emission = Color(0.10, 0.65, 1.0)
	fuse_mat.emission_energy_multiplier = 1.2
	exit_mat = metal_mat.duplicate()
	dark_mat = make_mat(Color(0.012, 0.010, 0.008), "", "", "", 0.0, 1.0)

func make_mat(color: Color, albedo_path := "", normal_path := "", rough_path := "", metallic := 0.0, roughness := 0.9, uv_scale := Vector3.ONE) = StandardMaterial3D:
	var mat := StandardMaterial3D.new()
	mat.albedo_color = color
	mat.roughness = roughness
	mat.metallic = metallic
	mat.uv1_scale = uv_scale
	if albedo_path != "" and ResourceLoader.exists(albedo_path):
		mat.albedo_texture = load(albedo_path)
	if normal_path != "" and ResourceLoader.exists(normal_path):
		mat.normal_enabled = true
		mat.normal_texture = load(normal_path)
	if rough_path != "" and ResourceLoader.exists(rough_path):
		mat.roughness_texture = load(rough_path)
	return mat

func _make_world() -> void:
	var world := WorldEnvironment.new()
	var env := Environment.new()
	env.background_mode = Environment.BG_COLOR
	env.background_color = Color(0.045, 0.040, 0.024)
	env.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	env.ambient_light_color = Color(0.55, 0.50, 0.34)
	env.ambient_light_energy = 0.62
	world.environment = env
	add_child(world)

	box(Vector3(0, -0.05, 0), Vector3(30, 0.1, 30), carpet_mat, true)
	box(Vector3(0, 2.55, 0), Vector3(30, 0.1, 30), ceiling_mat, false)
	box(Vector3(-15, 1.2, 0), Vector3(0.2, 2.5, 30), wall_mat, true)
	box(Vector3(15, 1.2, 0), Vector3(0.2, 2.5, 30), wall_mat, true)
	box(Vector3(0, 1.2, -15), Vector3(30, 2.5, 0.2), wall_mat, true)
	box(Vector3(0, 1.2, 15), Vector3(30, 2.5, 0.2), wall_mat, true)

	for z in [-8, -4, 4, 8]:
		box(Vector3(-5, 1.2, z), Vector3(0.18, 2.5, 4.8), wall_mat, true)
		box(Vector3(5, 1.2, z), Vector3(0.18, 2.5, 4.8), wall_mat, true)
	for x in [-8, 0, 8]:
		box(Vector3(x, 1.2, -5), Vector3(4.8, 2.5, 0.18), wall_mat, true)
		box(Vector3(x, 1.2, 5), Vector3(4.8, 2.5, 0.18), wall_mat, true)

	for p in [Vector3(-10, 2.45, -10), Vector3(0, 2.45, -8), Vector3(10, 2.45, -4), Vector3(-6, 2.45, 4), Vector3(8, 2.45, 9)]:
		box(p, Vector3(2.0, 0.06, 0.38), light_mat, false)
		var lamp := OmniLight3D.new()
		lamp.light_color = Color(1.0, 0.86, 0.48)
		lamp.light_energy = 0.55
		lamp.omni_range = 7.5
		add_child(lamp)
		lamp.global_position = p - Vector3(0, 0.25, 0)

	for p in [Vector3(-9, 1.25, -14.88), Vector3(14.88, 1.25, -6), Vector3(-14.88, 1.25, 7), Vector3(7, 1.25, 14.88)]:
		make_leak(p)

	for p in [Vector3(-10, 0.55, -9), Vector3(10, 0.55, -6), Vector3(-6, 0.55, 10)]:
		make_fuse(p)
	make_note(Vector3(2, 0.75, -13.8), "THREE BLUE FUSES OPEN THE SERVICE EXIT.")
	make_note(Vector3(-13.8, 0.75, 2), "The leaks mark the path back from dead rooms.")
	make_hide_spot(Vector3(-11, 0.8, 7))
	make_hide_spot(Vector3(8, 0.8, -11))

	exit_door = box(Vector3(13.9, 1.15, 13.0), Vector3(0.28, 2.2, 2.2), exit_mat, true)
	box(Vector3(13.55, 1.15, 13.0), Vector3(0.10, 1.3, 0.25), raw_metal_mat, false)
	box(Vector3(-14.7, 0.9, -10), Vector3(0.22, 1.8, 1.4), metal_mat, true)

func make_leak(pos: Vector3) -> void:
	var mesh := MeshInstance3D.new()
	var quad := QuadMesh.new()
	quad.size = Vector2(1.8, 1.8)
	mesh.mesh = quad
	mesh.material_override = leak_mat
	add_child(mesh)
	mesh.global_position = pos
	if abs(pos.x) > abs(pos.z):
		mesh.rotation_degrees.y = 90


func make_fuse(pos: Vector3) -> void:
	var fuse := box(pos, Vector3(0.45, 0.45, 0.45), fuse_mat, false)
	fuse.name = "Blue fuse"
	fuse_nodes.append(fuse)
	var light := OmniLight3D.new()
	light.light_color = Color(0.25, 0.80, 1.0)
	light.light_energy = 0.35
	light.omni_range = 2.3
	fuse.add_child(light)

func make_note(pos: Vector3, text: String) -> void:
	var note := box(pos, Vector3(0.75, 0.04, 0.45), wood_mat, false)
	note.name = text
	note_nodes.append(note)

func make_hide_spot(pos: Vector3) -> void:
	var h := box(pos, Vector3(1.15, 1.35, 0.75), wall_mat, true)
	h.name = "Hide spot"
	hide_nodes.append(h)
	box(pos + Vector3(0, 0.38, 0.45), Vector3(1.25, 1.15, 0.05), leak_mat, false)

func _make_player() -> void:
	player = CharacterBody3D.new()
	add_child(player)
	player.global_position = Vector3(0, 0.35, 0)
	var shape := CollisionShape3D.new()
	var capsule := CapsuleShape3D.new()
	capsule.height = 1.55
	capsule.radius = 0.32
	shape.shape = capsule
	shape.position = Vector3(0, 0.85, 0)
	player.add_child(shape)
	camera = Camera3D.new()
	camera.current = true
	camera.fov = 72
	camera.position = Vector3(0, 1.58, 0)
	player.add_child(camera)

func _make_ui() -> void:
	ui = CanvasLayer.new()
	add_child(ui)
	hud = Label.new()
	hud.position = Vector2(24, 18)
	hud.add_theme_font_size_override("font_size", 24)
	hud.add_theme_color_override("font_color", Color(0.96, 0.86, 0.46))
	ui.add_child(hud)
	message_label = Label.new()
	message_label.position = Vector2(24, 58)
	message_label.add_theme_font_size_override("font_size", 21)
	message_label.add_theme_color_override("font_color", Color(0.92, 0.82, 0.55))
	message_label.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	message_label.size = Vector2(720, 90)
	ui.add_child(message_label)

	make_button("↑", Vector2(120, 520), Vector2(72, 72), func(): touch_move.y = 1.0, func(): touch_move.y = 0.0)
	make_button("↓", Vector2(120, 610), Vector2(72, 72), func(): touch_move.y = -1.0, func(): touch_move.y = 0.0)
	make_button("←", Vector2(35, 610), Vector2(72, 72), func(): touch_move.x = -1.0, func(): touch_move.x = 0.0)
	make_button("→", Vector2(205, 610), Vector2(72, 72), func(): touch_move.x = 1.0, func(): touch_move.x = 0.0)
	make_button("SPRINT", Vector2(1010, 590), Vector2(180, 70), func(): sprinting = true, func(): sprinting = false)
	use_button = make_button("USE", Vector2(560, 630), Vector2(170, 68), use_action, null)
	pause_button = make_button("MENU", Vector2(1140, 22), Vector2(100, 58), return_to_menu, null)

func make_button(text: String, pos: Vector2, size: Vector2, down: Callable, up) -> Button:
	var b := Button.new()
	b.text = text
	b.position = pos
	b.size = size
	b.focus_mode = Control.FOCUS_NONE
	b.mouse_filter = Control.MOUSE_FILTER_STOP
	b.add_theme_font_size_override("font_size", 22)
	if up == null:
		b.pressed.connect(down)
	else:
		b.button_down.connect(down)
		b.button_up.connect(up)
	ui.add_child(b)
	return b

func _physics_process(delta: float) -> void:
	if game_over:
		return
	handle_movement(delta)
	handle_objectives()
	update_hud(delta)

func handle_movement(delta: float) -> void:
	var key := Vector2.ZERO
	if Input.is_key_pressed(KEY_A):
		key.x -= 1.0
	if Input.is_key_pressed(KEY_D):
		key.x += 1.0
	if Input.is_key_pressed(KEY_W):
		key.y += 1.0
	if Input.is_key_pressed(KEY_S):
		key.y -= 1.0
	var motion := key + touch_move
	if motion.length() > 1.0:
		motion = motion.normalized()

	player.rotate_y(-look_delta.x * 0.0038)
	pitch = clamp(pitch + look_delta.y * -0.0033, deg_to_rad(-80), deg_to_rad(80))
	camera.rotation.x = pitch
	look_delta = Vector2.ZERO

	var direction := player.global_transform.basis * Vector3(motion.x, 0, -motion.y)
	direction.y = 0
	if direction.length() > 1.0:
		direction = direction.normalized()
	var speed := SPRINT_SPEED if sprinting and stamina > 0.05 else WALK_SPEED
	player.velocity.x = direction.x * speed
	player.velocity.z = direction.z * speed
	if not player.is_on_floor():
		player.velocity.y -= GRAVITY * delta
	else:
		player.velocity.y = -0.1
	player.move_and_slide()

	if sprinting and direction.length() > 0.1:
		stamina = max(0.0, stamina - delta * 0.15)
	else:
		stamina = min(1.0, stamina + delta * 0.18)
	if Time.get_ticks_msec() / 1000.0 < hiding_until:
		sanity = min(1.0, sanity + delta * 0.06)
	else:
		sanity = max(0.0, sanity - delta * 0.004)

func handle_objectives() -> void:
	for fuse in fuse_nodes.duplicate():
		if is_instance_valid(fuse) and player.global_position.distance_to(fuse.global_position) <= INTERACT_RANGE:
			fuse_nodes.erase(fuse)
			fuse.queue_free()
			fuses_collected += 1
			show_msg("Fuse collected: %d/%d" % [fuses_collected, fuses_required], 1.8)
	if not exit_unlocked and fuses_collected >= fuses_required:
		exit_unlocked = true
		exit_mat.albedo_color = Color(0.78, 0.54, 0.22)
		show_msg("The service exit buzzes open. Follow the metal door.", 2.4)
	if exit_door and player.global_position.distance_to(exit_door.global_position) <= INTERACT_RANGE and exit_unlocked:
		show_msg("YOU FOUND THE EXIT. Level 0 lets you leave.", 999.0)
		game_over = true

func use_action() -> void:
	var pos := player.global_position
	for note in note_nodes:
		if is_instance_valid(note) and pos.distance_to(note.global_position) <= INTERACT_RANGE:
			show_msg(String(note.name), 3.0)
			return
	for hide in hide_nodes:
		if is_instance_valid(hide) and pos.distance_to(hide.global_position) <= INTERACT_RANGE:
			hiding_until = Time.get_ticks_msec() / 1000.0 + 2.5
			show_msg("You hold still behind stained wallpaper.", 1.8)
			return
	if exit_door and pos.distance_to(exit_door.global_position) <= INTERACT_RANGE:
		if exit_unlocked:
			show_msg("YOU FOUND THE EXIT. Level 0 lets you leave.", 999.0)
			game_over = true
		else:
			show_msg("The exit has no power. Find the blue fuses.", 1.8)


	eleif true:
		show_msg("Nothing close enough to use.", 1.1)

func update_hud(delta: float) -> void:
	hud.text = "Fuses %d/%d  |  Exit %s  |  Stamina %d%%  |  Sanity %d%%" % [fuses_collected, fuses_required, "OPEN" if exit_unlocked else "LOCKED", int(stamina * 100.0), int(sanity * 100.0)]
	if message_time > 0.0:
		message_time -= delta
	else:
		message_label.text = ""

func show_msg(text: String, duration := 2.0) -> void:
	message_label.text = text
	message_time = duration

func return_to_menu() -> void:
	get_tree().change_scene_to_file("res://backrooms_ui/scenes/main_menu.tscn")

func _input(event: InputEvent) -> void:
	if event is InputEventMouseMotion and Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
		look_delta += event.relative
	if event is InputEventScreenDrag:
		var width := get_viewport().get_visible_rect().size.x
		if event.position.x > width * 0.45:
			look_delta += event.relative
	if event is InputEventKey and event.pressed:
		if event.keycode == KEY_E:
			use_action()
		elif event.keycode == KEY_ESCAPE or event.keycode == KEY_BACK:
			return_to_menu()

func box(pos: Vector3, size: Vector3, material: Material, collide := true) -> MeshInstance3D:
	var mesh := MeshInstance3D.new()
	var box_mesh := BoxMesh.new()
	box_mesh.size = size
	mesh.mesh = box_mesh
	mesh.material_override = material
	add_child(mesh)
	mesh.global_position = pos
	if collide:
		var body := StaticBody3D.new()
		var col := CollisionShape3D.new()
		var shape := BoxShape3D.new()
		shape.size = size
		col.shape = shape
		body.add_child(col)
		add_child(body)
		body.global_position = pos
	return mesh
