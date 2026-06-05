extends Node

var main: Node
var booted := false
var interact_label: Label
var interact_button: Button
var target: Node3D
var target_kind := ""
var hide_spots: Array[Node3D] = []
var locked_doors: Array[Node3D] = []
var hiding_until := 0.0
var frame_slider: HSlider
var quality_slider: HSlider
var hum_player: AudioStreamPlayer
var step_player: AudioStreamPlayer
var thud_player: AudioStreamPlayer
var buzz_player: AudioStreamPlayer
var ui_player: AudioStreamPlayer
var last_step_pos := Vector3.ZERO
var step_distance := 0.0
var ambience_timer := 4.0

func _ready():
	set_process(true)

func _process(delta: float):
	if not booted:
		try_boot()
		return
	if not is_instance_valid(main):
		booted = false
		return
	update_settings()
	update_interaction_target()
	update_hiding(delta)
	update_audio_cues(delta)

func try_boot():
	var scene := get_tree().current_scene
	if not scene or not scene.has_method("show_msg"):
		return
	main = scene
	booted = true
	last_step_pos = get_player_pos()
	add_extra_world_features()
	add_extra_ui()
	add_extra_settings()
	make_audio()
	log_feature("feature_completer", "autoload active")

func add_extra_world_features():
	if not main.has_method("box"):
		return
	var wall_m = main.get("wall_m")
	var grime_m = main.get("grime_m")
	var exit_m = main.get("exit_m")
	var light_m = main.get("light_m")
	var door := main.box(Vector3(-12.0, 1.15, -12.0), Vector3(1.4, 2.0, 0.18), exit_m, true)
	door.name = "Locked service door"
	locked_doors.append(door)
	make_hide_spot(Vector3(-10.5, 0.7, 6.0), wall_m, grime_m)
	make_hide_spot(Vector3(7.5, 0.7, -10.0), wall_m, grime_m)
	main.box(Vector3(-1.4, 0.7, -12.0), Vector3(0.15, 1.4, 2.8), grime_m, false)
	main.box(Vector3(1.4, 0.7, -12.0), Vector3(0.15, 1.4, 2.8), grime_m, false)
	main.box(Vector3(0, 1.9, -13.4), Vector3(2.8, 0.15, 0.15), light_m, false)
	if main.has_method("make_note"):
		main.make_note(Vector3(0, 0.7, -10.6), "A maintenance tag says: SOME DOORS ONLY OPEN AFTER THE HUM STOPS.")
		main.make_note(Vector3(9.5, 0.7, 9.5), "Fresh marker arrows point toward the powered service exit.")
	# Fix a small typo from the earlier objective note.
	var notes = main.get("note_nodes")
	if notes is Array:
		for note in notes:
			if is_instance_valid(note) and String(note.name).find("BAUE") >= 0:
				note.name = String(note.name).replace("BAUE", "BLUE")

func make_hide_spot(pos: Vector3, wall_m: Material, grime_m: Material):
	var spot := main.box(pos, Vector3(1.1, 1.4, 0.8), wall_m, true)
	spot.name = "Hide spot"
	hide_spots.append(spot)
	var curtain := main.box(pos + Vector3(0, 0.4, 0.45), Vector3(1.25, 1.2, 0.05), grime_m, false)
	curtain.name = "Stained divider"

func add_extra_ui():
	var ui = main.get("ui")
	if not ui:
		return
	interact_label = Label.new()
	interact_label.position = Vector2(400, 606)
	interact_label.size = Vector2(520, 40)
	interact_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	interact_label.add_theme_font_size_override("font_size", 24)
	interact_label.add_theme_color_override("font_color", Color(0.96, 0.86, 0.48))
	ui.add_child(interact_label)
	interact_button = Button.new()
	interact_button.text = "USE"
	interact_button.position = Vector2(565, 650)
	interact_button.size = Vector2(150, 68)
	interact_button.focus_mode = Control.FOCUS_NONE
	interact_button.pressed.connect(perform_interaction)
	ui.add_child(interact_button)

func add_extra_settings():
	var panel = main.get("settings_panel")
	if not panel:
		return
	var frame_label := Label.new()
	frame_label.text = "Frame cap"
	frame_label.position = Vector2(58, 296)
	frame_label.add_theme_font_size_override("font_size", 20)
	frame_label.add_theme_color_override("font_color", Color(0.95, 0.84, 0.40))
	panel.add_child(frame_label)
	frame_slider = HSlider.new()
	frame_slider.position = Vector2(260, 292)
	frame_slider.size = Vector2(250, 34)
	frame_slider.min_value = 30
	frame_slider.max_value = 120
	frame_slider.step = 15
	frame_slider.value = 60
	frame_slider.focus_mode = Control.FOCUS_NONE
	panel.add_child(frame_slider)
	var quality_label := Label.new()
	quality_label.text = "Graphics scale"
	quality_label.position = Vector2(58, 336)
	quality_label.add_theme_font_size_override("font_size", 20)
	quality_label.add_theme_color_override("font_color", Color(0.95, 0.84, 0.40))
	panel.add_child(quality_label)
	quality_slider = HSlider.new()
	quality_slider.position = Vector2(260, 332)
	quality_slider.size = Vector2(250, 34)
	quality_slider.min_value = 0.6
	quality_slider.max_value = 1.2
	quality_slider.step = 0.1
	quality_slider.value = 1.0
	quality_slider.focus_mode = Control.FOCUS_NONE
	panel.add_child(quality_slider)

func update_settings():
	if frame_slider:
		Engine.max_fps = int(frame_slider.value)
	if quality_slider:
		get_viewport().scaling_3d_scale = float(quality_slider.value)

func update_interaction_target():
	target = null
	target_kind = ""
	if int(main.get("state")) != 2:
		set_interaction_visible(false)
		return
	var pos := get_player_pos()
	var max_dist := float(main.get("interaction_range")) if main.get("interaction_range") != null else 1.9
	for spot in hide_spots:
		if is_instance_valid(spot) and pos.distance_to(spot.global_position) <= max_dist:
			target = spot
			target_kind = "hide"
			break
	if not target:
		for door in locked_doors:
			if is_instance_valid(door) and pos.distance_to(door.global_position) <= max_dist:
				target = door
				target_kind = "door"
				break
	if not target:
		var notes = main.get("note_nodes")
		if notes is Array:
			for note in notes:
				if is_instance_valid(note) and pos.distance_to(note.global_position) <= max_dist:
					target = note
					target_kind = "note"
					break
	if not target:
		var exit_door = main.get("exit_door")
		if exit_door and is_instance_valid(exit_door) and pos.distance_to(exit_door.global_position) <= 2.0:
			target = exit_door
			target_kind = "exit"
	set_interaction_visible(target != null)

func set_interaction_visible(v: bool):
	if interact_button:
		interact_button.visible = v
	if interact_label:
		interact_label.visible = v
		if v:
			var verb := "Hide" if target_kind == "hide" else "Read" if target_kind == "note" else "Open" if target_kind == "exit" else "Inspect"
			interact_label.text = "%s  |  tap USE or press E" % verb
		else:
			interact_label.text = ""

func perform_interaction():
	if not target or not is_instance_valid(target):
		return
	if target_kind == "hide":
		hiding_until = Time.get_ticks_msec() / 1000.0 + 2.4
		main.set("sanity", min(1.0, float(main.get("sanity")) + 0.08))
		main.show_msg("You hold still behind the stained divider.", 1.6)
		play_sound(ui_player)
	elif target_kind == "door":
		main.show_msg("The locked service door is swollen shut. Something hums behind it.", 2.4)
		play_sound(thud_player)
	elif target_kind == "note":
		main.show_msg(String(target.name), 3.2)
		play_sound(ui_player)
	elif target_kind == "exit":
		if bool(main.get("exit_unlocked")) and main.has_method("win_game"):
			main.win_game()
		else:
			main.show_msg("The exit door has no power. Find the fuses.", 1.8)
			play_sound(thud_player)

func update_hiding(delta: float):
	var is_hiding := Time.get_ticks_msec() / 1000.0 < hiding_until
	if is_hiding:
		var threat = main.get("entity")
		if threat and is_instance_valid(threat):
			threat.velocity.x *= 0.35
			threat.velocity.z *= 0.35
		main.set("threat_pressure", min(float(main.get("threat_pressure")), 0.25))
		main.set("sanity", min(1.0, float(main.get("sanity")) + delta * 0.08))

func update_audio_cues(delta: float):
	if int(main.get("state")) != 2:
		return
	var pos := get_player_pos()
	step_distance += pos.distance_to(last_step_pos)
	last_step_pos = pos
	if step_distance > 1.8:
		step_distance = 0.0
		play_sound(step_player)
	ambience_timer -= delta
	if ambience_timer <= 0:
		ambience_timer = randf_range(6.0, 12.0)
		play_sound(thud_player if randf() < 0.45 else buzz_player)

func _input(event):
	if event is InputEventKey and event.pressed and event.keycode == KEY_E:
		perform_interaction()
	if event is InputEventKey and event.pressed and (event.keycode == KEY_ESCAPE or event.keycode == KEY_BACK):
		if not booted:
			return
		var settings_panel = main.get("settings_panel")
		if settings_panel and settings_panel.visible and main.has_method("close_settings"):
			main.close_settings()
		elif int(main.get("state")) == 2 and main.has_method("open_pause"):
			main.open_pause()
		elif int(main.get("state")) == 3 and main.has_method("resume_game"):
			main.resume_game()
		elif int(main.get("state")) == 1 and main.has_method("quit_game"):
			main.quit_game()

func _notification(what):
	if what == NOTIFICATION_WM_GO_BACK_REQUEST and booted:
		var fake := InputEventKey.new()
		fake.keycode = KEY_BACK
		fake.pressed = true
		_input(fake)

func make_audio():
	hum_player = make_player_audio(make_tone(74.0, 0.9, 0.055))
	step_player = make_player_audio(make_tone(115.0, 0.08, 0.20))
	thud_player = make_player_audio(make_tone(42.0, 0.32, 0.28))
	buzz_player = make_player_audio(make_tone(155.0, 0.18, 0.16))
	ui_player = make_player_audio(make_tone(520.0, 0.05, 0.12))
	hum_player.finished.connect(func(): hum_player.play())
	hum_player.play()

func make_player_audio(stream: AudioStreamWAV) -> AudioStreamPlayer:
	var p := AudioStreamPlayer.new()
	p.stream = stream
	p.volume_db = linear_to_db(0.35)
	add_child(p)
	return p

func make_tone(freq: float, duration: float, amp: float) -> AudioStreamWAV:
	var stream := AudioStreamWAV.new()
	var rate := 22050
	var data := PackedByteArray()
	var total := int(duration * rate)
	for i in range(total):
		var fade := min(1.0, float(i) / 400.0) * min(1.0, float(total - i) / 400.0)
		var s := sin(TAU * freq * float(i) / float(rate)) * amp * fade
		var v := int(clamp(s, -1.0, 1.0) * 32767.0)
		if v < 0:
			v += 65536
		data.append(v & 255)
		data.append((v >> 8) & 255)
	stream.format = AudioStreamWAV.FORMAT_16_BITS
	stream.mix_rate = rate
	stream.stereo = false
	stream.data = data
	return stream

func play_sound(player_ref: AudioStreamPlayer):
	if not player_ref:
		return
	var volume := float(main.get("volume")) if booted and main.get("volume") != null else 0.8
	if volume <= 0.01:
		return
	player_ref.stop()
	player_ref.volume_db = linear_to_db(max(volume, 0.001))
	player_ref.play()

func get_player_pos() -> Vector3:
	var player = main.get("player")
	if player and is_instance_valid(player):
		return player.global_position
	return Vector3.ZERO

func log_feature(tag: String, text: String):
	if main and main.has_method("log_event"):
		main.log_event(tag, text)
