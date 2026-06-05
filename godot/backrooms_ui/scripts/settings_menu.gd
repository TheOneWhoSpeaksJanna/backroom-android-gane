extends Panel

const CONFIG_PATH := "user://backrooms_settings.cfg"

@onready var sensitivity_slider: HSlider = $VBox/SensitivitySlider
@onready var brightness_slider: HSlider = $VBox/BrightnessSlider
@onready var volume_slider: HSlider = $VBox/VolumeSlider
@onready var frame_slider: HSlider = $VBox/FrameSlider
@onready var close_button: Button = $VBox/CloseButton

func _ready() -> void:
    _load_settings()
    close_button.pressed.connect(_on_close_pressed)
    sensitivity_slider.value_changed.connect(_save_float.bind("sensitivity"))
    brightness_slider.value_changed.connect(_save_float.bind("brightness"))
    volume_slider.value_changed.connect(_on_volume_changed)
    frame_slider.value_changed.connect(_on_frame_changed)

func _save_float(value: float, key: String) -> void:
    var cfg := ConfigFile.new()
    cfg.load(CONFIG_PATH)
    cfg.set_value("settings", key, value)
    cfg.save(CONFIG_PATH)

func _on_volume_changed(value: float) -> void:
    _save_float(value, "volume")
    AudioServer.set_bus_volume_db(0, linear_to_db(max(value, 0.001)))

func _on_frame_changed(value: float) -> void:
    _save_float(value, "frame_cap")
    Engine.max_fps = int(value)

func _load_settings() -> void:
    var cfg := ConfigFile.new()
    cfg.load(CONFIG_PATH)
    sensitivity_slider.value = float(cfg.get_value("settings", "sensitivity", 1.0))
    brightness_slider.value = float(cfg.get_value("settings", "brightness", 1.0))
    volume_slider.value = float(cfg.get_value("settings", "volume", 0.8))
    frame_slider.value = float(cfg.get_value("settings", "frame_cap", 60.0))
    Engine.max_fps = int(frame_slider.value)

func _on_close_pressed() -> void:
    queue_free()
