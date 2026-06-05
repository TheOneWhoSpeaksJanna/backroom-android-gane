extends Control

const LOADING_SCENE := "res://backrooms_ui/scenes/loading_screen.tscn"
const SETTINGS_SCENE := preload("res://backrooms_ui/scenes/settings_menu.tscn")

@export_file("*.tscn") var game_scene_path := "res://Main.tscn"

@onready var enter_button: Button = $Center/Buttons/EnterButton
@onready var settings_button: Button = $Center/Buttons/SettingsButton
@onready var exit_button: Button = $Center/Buttons/ExitButton
@onready var status_label: Label = $Center/Status

func _ready() -> void:
    Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
    enter_button.pressed.connect(_on_enter_pressed)
    settings_button.pressed.connect(_on_settings_pressed)
    exit_button.pressed.connect(_on_exit_pressed)
    status_label.text = "Ready. Tap ENTER LEVEL 0."

func _on_enter_pressed() -> void:
    var target := game_scene_path
    if target.is_empty():
        target = str(ProjectSettings.get_setting("backrooms/game_scene", "res://Main.tscn"))
    if not ResourceLoader.exists(target):
        status_label.text = "Game scene not found: %s" % target
        return
    get_tree().set_meta("backrooms_next_scene", target)
    get_tree().change_scene_to_file(LOADING_SCENE)

func _on_settings_pressed() -> void:
    var settings := SETTINGS_SCENE.instantiate()
    add_child(settings)

func _on_exit_pressed() -> void:
    get_tree().quit()

func _input(event: InputEvent) -> void:
    if event is InputEventKey and event.pressed and event.keycode == KEY_BACK:
        get_tree().quit()
