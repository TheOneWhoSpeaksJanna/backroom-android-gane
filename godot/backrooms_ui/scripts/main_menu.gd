extends Control

const LOADING_SCENE := "res://backrooms_ui/scenes/loading_screen.tscn"
const SETTINGS_SCENE := preload("res://backrooms_ui/scenes/settings_menu.tscn")
const SAVE_PREFIX := "user://checkpoint_slot_"

@export_file("*.tscn") var game_scene_path := "res://Main.tscn"

@onready var buttons: VBoxContainer = $Center/Buttons
@onready var enter_button: Button = $Center/Buttons/EnterButton
@onready var settings_button: Button = $Center/Buttons/SettingsButton
@onready var exit_button: Button = $Center/Buttons/ExitButton
@onready var status_label: Label = $Center/Status

func _ready() -> void:
	Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
	enter_button.text = "NEW GAME SLOT 1"
	enter_button.pressed.connect(_start_new.bind(0))
	settings_button.pressed.connect(_open_settings)
	exit_button.pressed.connect(_exit)
	_add_slot_buttons()
	status_label.text = "Desolation is ready. Save slots persist on this device."

func _add_slot_buttons() -> void:
	for i in range(3):
		if FileAccess.file_exists(_save_path(i)):
			var resume := _make_button("CONTINUE SLOT %d" % (i + 1))
			resume.pressed.connect(_resume.bind(i))
		if i > 0:
			var fresh := _make_button("NEW GAME SLOT %d" % (i + 1))
			fresh.pressed.connect(_start_new.bind(i))
	var help := _make_button("APK DOWNLOAD HELP")
	help.pressed.connect(_show_apk_help)
	buttons.move_child(settings_button, buttons.get_child_count() - 2)
	buttons.move_child(exit_button, buttons.get_child_count() - 1)

func _make_button(text: String) -> Button:
	var b := Button.new()
	b.text = text
	b.custom_minimum_size = Vector2(420, 56)
	b.focus_mode = Control.FOCUS_NONE
	b.add_theme_font_size_override("font_size", 19)
	buttons.add_child(b)
	return b

func _start_new(slot: int) -> void:
	var path := _save_path(slot)
	if FileAccess.file_exists(path):
		DirAccess.remove_absolute(path)
	_load_slot(slot)

func _resume(slot: int) -> void:
	_load_slot(slot)

func _load_slot(slot: int) -> void:
	var target := game_scene_path
	if target.is_empty():
		target = str(ProjectSettings.get_setting("backrooms/game_scene", "res://Main.tscn"))
	if not ResourceLoader.exists(target):
		status_label.text = "Game scene not found: %s" % target
		return
	get_tree().set_meta("desolation_slot", slot)
	get_tree().set_meta("backrooms_next_scene", target)
	get_tree().change_scene_to_file(LOADING_SCENE)

func _open_settings() -> void:
	var settings := SETTINGS_SCENE.instantiate()
	add_child(settings)

func _show_apk_help() -> void:
	status_label.text = ".7z files are compressed. On Android, extract DesolationTheBackrooms.apk.7z first, then install the .apk."

func _exit() -> void:
	get_tree().quit()

func _input(event: InputEvent) -> void:
	if event is InputEventKey and event.pressed and event.keycode == KEY_BACK:
		get_tree().quit()

func _save_path(slot: int) -> String:
	return SAVE_PREFIX + str(clamp(slot, 0, 2)) + ".cfg"
