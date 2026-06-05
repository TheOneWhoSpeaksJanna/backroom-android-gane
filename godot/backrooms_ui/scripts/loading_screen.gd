extends Control

@export var minimum_time := 0.75
@onready var progress_bar: ProgressBar = $Center/ProgressBar
@onready var status_label: Label = $Center/Status

var elapsed := 0.0
var target_scene := "res://Main.tscn"

func _ready() -> void:
    if get_tree().has_meta("backrooms_next_scene"):
        target_scene = str(get_tree().get_meta("backrooms_next_scene"))
    status_label.text = "Loading Level 0..."
    progress_bar.value = 0.0

func _process(delta: float) -> void:
    elapsed += delta
    progress_bar.value = min(100.0, elapsed / minimum_time * 100.0)
    if elapsed >= minimum_time:
        if ResourceLoader.exists(target_scene):
            get_tree().change_scene_to_file(target_scene)
        else:
            status_label.text = "Missing scene: %s" % target_scene
